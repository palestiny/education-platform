using EducationPlatform.Application.FirstSlice;
using EducationPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EducationPlatform.IntegrationTests;

[CollectionDefinition("PostgresPersistence", DisableParallelization = true)]
public sealed class PostgresPersistenceCollection { }

[Collection("PostgresPersistence")]
public sealed class PostgresFirstSliceStoreTests
{
    [Fact]
    public void Atomic_assignment_creation_persists_idempotency_audit_and_outbox()
    {
        using var db = CreateFreshDatabase();

        var service = new FirstSliceService(new PostgresFirstSliceStore(db));

        var first = service.CreateAssignment(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{}", "postgres-key", "correlation-a");

        var second = service.CreateAssignment(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{}", "postgres-key", "correlation-b");

        Assert.False(first.Replayed);
        Assert.True(second.Replayed);
        Assert.Equal(first.Value.Id, second.Value.Id);
        Assert.Equal(1, db.Assignments.Count());
        Assert.Equal(1, db.IdempotencyRecords.Count());
        Assert.Equal(1, db.AuditRecords.Count());
        Assert.Equal(1, db.OutboxMessages.Count());
    }

    [Fact]
    public void Reusing_idempotency_key_with_different_request_is_rejected()
    {
        using var db = CreateFreshDatabase();
        var service = new FirstSliceService(new PostgresFirstSliceStore(db));

        _ = service.CreateAssignment(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{}", "conflict-key", "correlation-a");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            service.CreateAssignment(
                "tenant-a", "authorized-teacher", "context-a", "goal-a",
                "learner-b", "{}", "conflict-key", "correlation-b"));

        Assert.Equal("IDEMPOTENCY_CONFLICT", ex.Message);
    }

    [Fact]
    public void Stale_expected_version_is_rejected_and_only_one_close_commits()
    {
        using var seedDb = CreateFreshDatabase();
        var seedService = new FirstSliceService(new PostgresFirstSliceStore(seedDb));

        var created = seedService.CreateAssignment(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{}", "concurrency-create", "correlation-create");

        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));

        using var firstDb = CreateContext(connection!);
        using var secondDb = CreateContext(connection!);

        var firstService = new FirstSliceService(new PostgresFirstSliceStore(firstDb));
        var secondService = new FirstSliceService(new PostgresFirstSliceStore(secondDb));

        _ = firstService.CloseAssignment(
            "tenant-a", "authorized-teacher", created.Value.Id, 1,
            "close-first", "correlation-close-first");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            secondService.CloseAssignment(
                "tenant-a", "authorized-teacher", created.Value.Id, 1,
                "close-second", "correlation-close-second"));

        Assert.Equal("CONCURRENCY_CONFLICT", ex.Message);

        using var verifyDb = CreateContext(connection!);
        var assignment = verifyDb.Assignments.Single(x => x.Id == created.Value.Id);

        Assert.Equal("CLOSED", assignment.Status);
        Assert.Equal(2, assignment.Version);
        Assert.Equal(2, verifyDb.AuditRecords.Count(x => x.ResourceId == created.Value.Id));
        Assert.Equal(2, verifyDb.OutboxMessages.Count(x => x.AggregateId == created.Value.Id));
    }

    [Fact]
    public void Audit_failure_rolls_back_authoritative_mutation_and_outbox()
    {
        using var db = CreateFreshDatabase();

        db.Database.ExecuteSqlRaw("""
            CREATE OR REPLACE FUNCTION fail_audit_insert()
            RETURNS trigger
            LANGUAGE plpgsql
            AS $
            BEGIN
                RAISE EXCEPTION 'forced_audit_failure';
            END;
            $;

            CREATE TRIGGER audit_failure_trigger
            BEFORE INSERT ON audit_records
            FOR EACH ROW EXECUTE FUNCTION fail_audit_insert();
            """);

        try
        {
            var service = new FirstSliceService(new PostgresFirstSliceStore(db));

            Assert.ThrowsAny<Exception>(() =>
                service.CreateAssignment(
                    "tenant-a", "authorized-teacher", "context-a", "goal-a",
                    "learner-a", "{}", "audit-failure-key", "correlation-audit-failure"));

            db.ChangeTracker.Clear();
            Assert.Empty(db.Assignments);
            Assert.Empty(db.IdempotencyRecords);
            Assert.Empty(db.AuditRecords);
            Assert.Empty(db.OutboxMessages);
        }
        finally
        {
            db.Database.ExecuteSqlRaw("""
                DROP TRIGGER IF EXISTS audit_failure_trigger ON audit_records;
                DROP FUNCTION IF EXISTS fail_audit_insert();
                """);
        }
    }

    [Fact]
    public void Outbox_failure_rolls_back_authoritative_mutation_and_audit()
    {
        using var db = CreateFreshDatabase();

        db.Database.ExecuteSqlRaw("""
            CREATE OR REPLACE FUNCTION fail_outbox_insert()
            RETURNS trigger
            LANGUAGE plpgsql
            AS $
            BEGIN
                RAISE EXCEPTION 'forced_outbox_failure';
            END;
            $;

            CREATE TRIGGER outbox_failure_trigger
            BEFORE INSERT ON outbox_messages
            FOR EACH ROW EXECUTE FUNCTION fail_outbox_insert();
            """);

        try
        {
            var service = new FirstSliceService(new PostgresFirstSliceStore(db));

            Assert.ThrowsAny<Exception>(() =>
                service.CreateAssignment(
                    "tenant-a", "authorized-teacher", "context-a", "goal-a",
                    "learner-a", "{}", "outbox-failure-key", "correlation-outbox-failure"));

            db.ChangeTracker.Clear();
            Assert.Empty(db.Assignments);
            Assert.Empty(db.IdempotencyRecords);
            Assert.Empty(db.AuditRecords);
            Assert.Empty(db.OutboxMessages);
        }
        finally
        {
            db.Database.ExecuteSqlRaw("""
                DROP TRIGGER IF EXISTS outbox_failure_trigger ON outbox_messages;
                DROP FUNCTION IF EXISTS fail_outbox_insert();
                """);
        }
    }

    private static EducationPlatformDbContext CreateContext(string connection)
    {
        var options = new DbContextOptionsBuilder<EducationPlatformDbContext>()
            .UseNpgsql(connection, npgsql =>
                npgsql.MigrationsAssembly(typeof(EducationPlatformDbContext).Assembly.FullName))
            .Options;
        return new EducationPlatformDbContext(options);
    }

    private static EducationPlatformDbContext CreateFreshDatabase()
    {
        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));

        var options = new DbContextOptionsBuilder<EducationPlatformDbContext>()
            .UseNpgsql(connection, npgsql =>
                npgsql.MigrationsAssembly(typeof(EducationPlatformDbContext).Assembly.FullName))
            .Options;

        var db = new EducationPlatformDbContext(options);
        db.Database.Migrate();

        db.Assignments.RemoveRange(db.Assignments);
        db.Submissions.RemoveRange(db.Submissions);
        db.IdempotencyRecords.RemoveRange(db.IdempotencyRecords);
        db.AuditRecords.RemoveRange(db.AuditRecords);
        db.OutboxMessages.RemoveRange(db.OutboxMessages);
        db.SaveChanges();

        return db;
    }
}
