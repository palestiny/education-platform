using EducationPlatform.Application.FirstSlice;
using EducationPlatform.Domain.FirstSlice;
using EducationPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EducationPlatform.IntegrationTests;

[CollectionDefinition("PostgresPersistence", DisableParallelization = true)]
public sealed class PostgresPersistenceCollection;

[Collection("PostgresPersistence")]
public sealed class PostgresFirstSliceStoreTests
{
    [Fact]
    public void Atomic_assignment_creation_persists_idempotency_audit_and_outbox()
    {
        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));

        var options = new DbContextOptionsBuilder<EducationPlatformDbContext>()
            .UseNpgsql(connection)
            .Options;

        using (var setup = new EducationPlatformDbContext(options))
        {
            setup.Database.EnsureDeleted();
            setup.Database.EnsureCreated();
        }

        using (var db = new EducationPlatformDbContext(options))
        {
            var store = new PostgresFirstSliceStore(db);
            var service = new FirstSliceService(store);

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
    }

    [Fact]
    public void Reusing_idempotency_key_with_different_request_is_rejected()
    {
        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));

        var options = new DbContextOptionsBuilder<EducationPlatformDbContext>()
            .UseNpgsql(connection)
            .Options;

        using (var setup = new EducationPlatformDbContext(options))
        {
            setup.Database.EnsureDeleted();
            setup.Database.EnsureCreated();
        }

        using var db = new EducationPlatformDbContext(options);
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
}
