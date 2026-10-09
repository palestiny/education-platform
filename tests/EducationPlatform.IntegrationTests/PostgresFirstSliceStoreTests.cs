using EducationPlatform.Application.FirstSlice;
using EducationPlatform.Application.Security;
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

        var service = new FirstSliceService(new PostgresFirstSliceStore(db), new AllowCloseAuthorizer());

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
    public async Task Submission_retry_after_assignment_close_replays_without_duplicate_reliability_records()
    {
        using var db = CreateFreshDatabase();
        var service = new FirstSliceService(new PostgresFirstSliceStore(db), new AllowCloseAuthorizer());

        var created = service.CreateAssignment(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "authorized-learner", "{}", "submission-replay-create", "correlation-create");

        var assignment = db.Assignments.Single(x => x.Id == created.Value.Id);
        var first = service.CreateSubmission(
            "tenant-a", "authorized-learner", assignment, "authorized-learner",
            "{\"answer\":\"test\"}", "submission-replay-key", "correlation-submit");

        Assert.False(first.Replayed);

        _ = await service.CloseAssignmentAsync(
            "tenant-a", "authorized-teacher", assignment.Id, 1,
            "submission-replay-close", "correlation-close", TestContext.Current.CancellationToken);

        var retry = service.CreateSubmission(
            "tenant-a", "authorized-learner", assignment, "authorized-learner",
            "{\"answer\":\"test\"}", "submission-replay-key", "correlation-retry");

        Assert.True(retry.Replayed);
        Assert.Equal(first.Value.Id, retry.Value.Id);
        Assert.Equal(1, db.Submissions.Count());
        Assert.Equal(1, db.IdempotencyRecords.Count(x => x.OperationScope == "submission.create"));
        Assert.Equal(1, db.AuditRecords.Count(x => x.Operation == "submission.create"));
        Assert.Equal(1, db.OutboxMessages.Count(x =>
            x.AggregateType == nameof(EducationPlatform.Domain.FirstSlice.Submission)));
    }

    [Fact]
    public async Task Authorization_denial_does_not_write_close_idempotency_audit_or_outbox_records()
    {
        using var db = CreateFreshDatabase();
        var authorizer = new MutableCloseAuthorizer(AuthorizationDecision.Denied);
        var service = new FirstSliceService(new PostgresFirstSliceStore(db), authorizer);

        var created = service.CreateAssignment(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{}", "denied-close-create", "correlation-create");

        var auditCountBefore = db.AuditRecords.Count();
        var outboxCountBefore = db.OutboxMessages.Count();
        var idempotencyCountBefore = db.IdempotencyRecords.Count();

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CloseAssignmentAsync(
                "tenant-a", "authorized-teacher", created.Value.Id, 1,
                "denied-close-key", "correlation-denied-close",
                TestContext.Current.CancellationToken));

        Assert.Equal("FORBIDDEN", error.Message);
        Assert.Equal(1, db.Assignments.Single(x => x.Id == created.Value.Id).Version);
        Assert.Equal(auditCountBefore, db.AuditRecords.Count());
        Assert.Equal(outboxCountBefore, db.OutboxMessages.Count());
        Assert.Equal(idempotencyCountBefore, db.IdempotencyRecords.Count());
        Assert.Equal(1, authorizer.CallCount);
    }

    [Fact]
    public async Task Revoked_authorization_cannot_replay_durable_close_response()
    {
        using var db = CreateFreshDatabase();
        var authorizer = new MutableCloseAuthorizer(AuthorizationDecision.Allowed);
        var service = new FirstSliceService(new PostgresFirstSliceStore(db), authorizer);

        var created = service.CreateAssignment(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{}", "durable-replay-create", "correlation-create");

        var first = await service.CloseAssignmentAsync(
            "tenant-a", "authorized-teacher", created.Value.Id, 1,
            "durable-close-replay-key", "correlation-close",
            TestContext.Current.CancellationToken);

        Assert.Equal("CLOSED", first.Status);
        var auditCountAfterClose = db.AuditRecords.Count();
        var outboxCountAfterClose = db.OutboxMessages.Count();
        var idempotencyCountAfterClose = db.IdempotencyRecords.Count();

        authorizer.Decision = AuthorizationDecision.Denied;
        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CloseAssignmentAsync(
                "tenant-a", "authorized-teacher", created.Value.Id, 1,
                "durable-close-replay-key", "correlation-retry",
                TestContext.Current.CancellationToken));

        Assert.Equal("FORBIDDEN", error.Message);
        Assert.Equal(auditCountAfterClose, db.AuditRecords.Count());
        Assert.Equal(outboxCountAfterClose, db.OutboxMessages.Count());
        Assert.Equal(idempotencyCountAfterClose, db.IdempotencyRecords.Count());
        Assert.Equal(2, authorizer.CallCount);
    }

    [Fact]
    public void Reusing_idempotency_key_with_different_request_is_rejected()
    {
        using var db = CreateFreshDatabase();
        var service = new FirstSliceService(new PostgresFirstSliceStore(db), new AllowCloseAuthorizer());

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
    public async Task Stale_expected_version_is_rejected_and_only_one_close_commits()
    {
        using var seedDb = CreateFreshDatabase();
        var seedService = new FirstSliceService(new PostgresFirstSliceStore(seedDb), new AllowCloseAuthorizer());

        var created = seedService.CreateAssignment(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{}", "concurrency-create", "correlation-create");

        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));

        using var firstDb = CreateContext(connection!);
        using var secondDb = CreateContext(connection!);

        var firstService = new FirstSliceService(new PostgresFirstSliceStore(firstDb), new AllowCloseAuthorizer());
        var secondService = new FirstSliceService(new PostgresFirstSliceStore(secondDb), new AllowCloseAuthorizer());

        _ = await firstService.CloseAssignmentAsync(
            "tenant-a", "authorized-teacher", created.Value.Id, 1,
            "close-first", "correlation-close-first", TestContext.Current.CancellationToken);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await secondService.CloseAssignmentAsync(
                "tenant-a", "authorized-teacher", created.Value.Id, 1,
                "close-second", "correlation-close-second", TestContext.Current.CancellationToken));

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
        db.Database.ExecuteSqlRaw(
            """ALTER TABLE audit_records ADD CONSTRAINT forced_audit_failure CHECK (false) NOT VALID;""");

        try
        {
            var service = new FirstSliceService(new PostgresFirstSliceStore(db), new AllowCloseAuthorizer());

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
            db.Database.ExecuteSqlRaw(
                """ALTER TABLE audit_records DROP CONSTRAINT IF EXISTS forced_audit_failure;""");
        }
    }

    [Fact]
    public void Outbox_failure_rolls_back_authoritative_mutation_and_audit()
    {
        using var db = CreateFreshDatabase();
        db.Database.ExecuteSqlRaw(
            """ALTER TABLE outbox_messages ADD CONSTRAINT forced_outbox_failure CHECK (false) NOT VALID;""");

        try
        {
            var service = new FirstSliceService(new PostgresFirstSliceStore(db), new AllowCloseAuthorizer());

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
            db.Database.ExecuteSqlRaw(
                """ALTER TABLE outbox_messages DROP CONSTRAINT IF EXISTS forced_outbox_failure;""");
        }
    }

    private sealed class MutableCloseAuthorizer(AuthorizationDecision initialDecision)
        : IAssignmentCloseAuthorizer
    {
        public AuthorizationDecision Decision { get; set; } = initialDecision;
        public int CallCount { get; private set; }

        public ValueTask<AuthorizationDecision> AuthorizeAsync(
            AssignmentCloseAuthorizationRequest request,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            return ValueTask.FromResult(Decision);
        }
    }

    private sealed class AllowCloseAuthorizer : IAssignmentCloseAuthorizer
    {
        public ValueTask<AuthorizationDecision> AuthorizeAsync(
            AssignmentCloseAuthorizationRequest request,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(AuthorizationDecision.Allowed);
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
