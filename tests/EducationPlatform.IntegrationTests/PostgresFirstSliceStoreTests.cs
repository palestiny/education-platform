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
    public async Task Authority_goal_foreign_key_rejects_cross_tenant_context_reference()
    {
        using var db = CreateFreshDatabase();
        db.LearningContexts.Add(new LearningContextAuthorityRecord
        {
            Id = "context-authority-a",
            TenantId = "tenant-a",
            Status = "ACTIVE"
        });
        db.LearningGoals.Add(new LearningGoalAuthorityRecord
        {
            Id = "goal-authority-a",
            TenantId = "tenant-a",
            ContextId = "context-authority-a",
            Status = "ACTIVE"
        });
        db.SaveChanges();

        db.LearningGoals.Add(new LearningGoalAuthorityRecord
        {
            Id = "goal-authority-b",
            TenantId = "tenant-b",
            ContextId = "context-authority-a",
            Status = "ACTIVE"
        });

        await Assert.ThrowsAsync<DbUpdateException>(async () =>
            await db.SaveChangesAsync(TestContext.Current.CancellationToken));

        db.ChangeTracker.Clear();
        Assert.Single(db.LearningContexts);
        Assert.Single(db.LearningGoals);
        Assert.Equal("tenant-a", db.LearningGoals.Single().TenantId);
    }

    [Fact]
    public async Task Authority_policy_grant_foreign_key_rejects_cross_tenant_context_reference()
    {
        using var db = CreateFreshDatabase();
        db.LearningContexts.Add(new LearningContextAuthorityRecord
        {
            Id = "policy-context-a",
            TenantId = "tenant-a",
            Status = "ACTIVE"
        });
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        db.ActionPolicyGrants.Add(new ActionPolicyGrantAuthorityRecord
        {
            TenantId = "tenant-b",
            ContextId = "policy-context-a",
            SubjectType = "PERSON",
            SubjectId = "teacher-b",
            Action = "assignment.create",
            Status = "ACTIVE"
        });

        await Assert.ThrowsAsync<DbUpdateException>(async () =>
            await db.SaveChangesAsync(TestContext.Current.CancellationToken));

        db.ChangeTracker.Clear();
        Assert.Empty(db.ActionPolicyGrants);
        Assert.Single(db.LearningContexts);
    }

    [Fact]
    public async Task Authority_membership_foreign_keys_reject_unknown_person()
    {
        using var db = CreateFreshDatabase();
        db.LearningContexts.Add(new LearningContextAuthorityRecord
        {
            Id = "membership-context",
            TenantId = "tenant-a",
            Status = "ACTIVE"
        });
        db.SaveChanges();

        db.ContextualMemberships.Add(new ContextualMembershipAuthorityRecord
        {
            TenantId = "tenant-a",
            ContextId = "membership-context",
            PersonId = "person-does-not-exist",
            MembershipType = "LEARNER",
            Status = "ACTIVE"
        });

        await Assert.ThrowsAsync<DbUpdateException>(async () =>
            await db.SaveChangesAsync(TestContext.Current.CancellationToken));

        db.ChangeTracker.Clear();
        Assert.Empty(db.ContextualMemberships);
    }

    [Fact]
    public async Task Atomic_assignment_creation_persists_idempotency_audit_and_outbox()
    {
        using var db = CreateFreshDatabase();

        var service = new FirstSliceService(new PostgresFirstSliceStore(db), new AllowCloseAuthorizer(), new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer());

        var first = await service.CreateAssignmentAsync(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{}", "postgres-key", "correlation-a", TestContext.Current.CancellationToken);

        var second = await service.CreateAssignmentAsync(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{}", "postgres-key", "correlation-b", TestContext.Current.CancellationToken);

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
        var service = new FirstSliceService(new PostgresFirstSliceStore(db), new AllowCloseAuthorizer(), new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer());

        var created = await service.CreateAssignmentAsync(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "authorized-learner", "{}", "submission-replay-create", "correlation-create", TestContext.Current.CancellationToken);

        var assignment = db.Assignments.Single(x => x.Id == created.Value.Id);
        var first = await service.CreateSubmissionAsync(
            "tenant-a", "authorized-learner", assignment.Id, "authorized-learner",
            "{\"answer\":\"test\"}", "submission-replay-key", "correlation-submit", TestContext.Current.CancellationToken);

        Assert.False(first.Replayed);

        _ = await service.CloseAssignmentAsync(
            "tenant-a", "authorized-teacher", assignment.Id, 1,
            "submission-replay-close", "correlation-close", TestContext.Current.CancellationToken);

        var retry = await service.CreateSubmissionAsync(
            "tenant-a", "authorized-learner", assignment.Id, "authorized-learner",
            "{\"answer\":\"test\"}", "submission-replay-key", "correlation-retry", TestContext.Current.CancellationToken);

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
        var service = new FirstSliceService(new PostgresFirstSliceStore(db), authorizer, new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer());

        var created = await service.CreateAssignmentAsync(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{}", "denied-close-create", "correlation-create", TestContext.Current.CancellationToken);

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
        var service = new FirstSliceService(new PostgresFirstSliceStore(db), authorizer, new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer());

        var created = await service.CreateAssignmentAsync(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{}", "durable-replay-create", "correlation-create", TestContext.Current.CancellationToken);

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
    public async Task Reusing_idempotency_key_with_different_request_is_rejected()
    {
        using var db = CreateFreshDatabase();
        var service = new FirstSliceService(new PostgresFirstSliceStore(db), new AllowCloseAuthorizer(), new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer());

        _ = await service.CreateAssignmentAsync(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{}", "conflict-key", "correlation-a", TestContext.Current.CancellationToken);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CreateAssignmentAsync(
                "tenant-a", "authorized-teacher", "context-a", "goal-a",
                "learner-b", "{}", "conflict-key", "correlation-b", TestContext.Current.CancellationToken));

        Assert.Equal("IDEMPOTENCY_CONFLICT", ex.Message);
    }

    [Fact]
    public async Task Stale_expected_version_is_rejected_and_only_one_close_commits()
    {
        using var seedDb = CreateFreshDatabase();
        var seedService = new FirstSliceService(new PostgresFirstSliceStore(seedDb), new AllowCloseAuthorizer(), new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer());

        var created = await seedService.CreateAssignmentAsync(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{}", "concurrency-create", "correlation-create", TestContext.Current.CancellationToken);

        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));

        using var firstDb = CreateContext(connection!);
        using var secondDb = CreateContext(connection!);

        var firstService = new FirstSliceService(new PostgresFirstSliceStore(firstDb), new AllowCloseAuthorizer(), new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer());
        var secondService = new FirstSliceService(new PostgresFirstSliceStore(secondDb), new AllowCloseAuthorizer(), new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer());

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
    public async Task Audit_failure_rolls_back_authoritative_mutation_and_outbox()
    {
        using var db = CreateFreshDatabase();
        db.Database.ExecuteSqlRaw(
            """ALTER TABLE audit_records ADD CONSTRAINT forced_audit_failure CHECK (false) NOT VALID;""");

        try
        {
            var service = new FirstSliceService(new PostgresFirstSliceStore(db), new AllowCloseAuthorizer(), new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer());

            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await service.CreateAssignmentAsync(
                    "tenant-a", "authorized-teacher", "context-a", "goal-a",
                    "learner-a", "{}", "audit-failure-key", "correlation-audit-failure", TestContext.Current.CancellationToken));

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
    public async Task Outbox_failure_rolls_back_authoritative_mutation_and_audit()
    {
        using var db = CreateFreshDatabase();
        db.Database.ExecuteSqlRaw(
            """ALTER TABLE outbox_messages ADD CONSTRAINT forced_outbox_failure CHECK (false) NOT VALID;""");

        try
        {
            var service = new FirstSliceService(new PostgresFirstSliceStore(db), new AllowCloseAuthorizer(), new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer());

            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await service.CreateAssignmentAsync(
                    "tenant-a", "authorized-teacher", "context-a", "goal-a",
                    "learner-a", "{}", "outbox-failure-key", "correlation-outbox-failure", TestContext.Current.CancellationToken));

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


    [Fact]
    public async Task Assignment_creation_authorization_denial_writes_no_durable_records()
    {
        using var db = CreateFreshDatabase();
        var creationAuthorizer = new MutableCreationAuthorizer(AuthorizationDecision.Denied);
        var service = new FirstSliceService(
            new PostgresFirstSliceStore(db), new AllowCloseAuthorizer(),
            creationAuthorizer, new AllowSubmissionAuthorizer());

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CreateAssignmentAsync(
                "tenant-a", "authorized-teacher", "context-a", "goal-a",
                "learner-a", "{}", "postgres-create-denied", "correlation-create-denied",
                TestContext.Current.CancellationToken));

        Assert.Equal("FORBIDDEN", error.Message);
        Assert.Empty(db.Assignments);
        Assert.Empty(db.IdempotencyRecords);
        Assert.Empty(db.AuditRecords);
        Assert.Empty(db.OutboxMessages);
        Assert.Equal(1, creationAuthorizer.CallCount);
    }

    [Fact]
    public async Task Submission_authorization_denial_writes_no_durable_records()
    {
        using var db = CreateFreshDatabase();
        var submissionAuthorizer = new MutableSubmissionAuthorizer(AuthorizationDecision.Denied);
        var service = new FirstSliceService(
            new PostgresFirstSliceStore(db), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), submissionAuthorizer);

        var created = await service.CreateAssignmentAsync(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "authorized-learner", "{}", "postgres-submit-denied-create", "correlation-create",
            TestContext.Current.CancellationToken);

        var auditCountBefore = db.AuditRecords.Count();
        var outboxCountBefore = db.OutboxMessages.Count();
        var idempotencyCountBefore = db.IdempotencyRecords.Count();

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CreateSubmissionAsync(
                "tenant-a", "authorized-learner", created.Value.Id, "authorized-learner",
                "{\"answer\":\"test\"}", "postgres-submit-denied", "correlation-submit-denied",
                TestContext.Current.CancellationToken));

        Assert.Equal("FORBIDDEN", error.Message);
        Assert.Empty(db.Submissions);
        Assert.Equal(auditCountBefore, db.AuditRecords.Count());
        Assert.Equal(outboxCountBefore, db.OutboxMessages.Count());
        Assert.Equal(idempotencyCountBefore, db.IdempotencyRecords.Count());
        Assert.Equal(1, submissionAuthorizer.CallCount);
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



    private sealed class MutableCreationAuthorizer(AuthorizationDecision initialDecision)
        : IAssignmentCreationAuthorizer
    {
        public AuthorizationDecision Decision { get; set; } = initialDecision;
        public int CallCount { get; private set; }

        public ValueTask<AuthorizationDecision> AuthorizeAsync(
            AssignmentCreationAuthorizationRequest request,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount++;
            return ValueTask.FromResult(Decision);
        }
    }

    private sealed class MutableSubmissionAuthorizer(AuthorizationDecision initialDecision)
        : ILearnerSubmissionAuthorizer
    {
        public AuthorizationDecision Decision { get; set; } = initialDecision;
        public int CallCount { get; private set; }

        public ValueTask<AuthorizationDecision> AuthorizeAsync(
            LearnerSubmissionAuthorizationRequest request,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount++;
            return ValueTask.FromResult(Decision);
        }
    }

    private sealed class AllowCreationAuthorizer : IAssignmentCreationAuthorizer
    {
        public ValueTask<AuthorizationDecision> AuthorizeAsync(
            AssignmentCreationAuthorizationRequest request,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(AuthorizationDecision.Allowed);
        }
    }

    private sealed class AllowSubmissionAuthorizer : ILearnerSubmissionAuthorizer
    {
        public ValueTask<AuthorizationDecision> AuthorizeAsync(
            LearnerSubmissionAuthorizationRequest request,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(AuthorizationDecision.Allowed);
        }
    }

    private sealed class AllowCloseAuthorizer : IAssignmentCloseAuthorizer
    {
        public ValueTask<AuthorizationDecision> AuthorizeAsync(
            AssignmentCloseAuthorizationRequest request,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(AuthorizationDecision.Allowed);
    }

    [Fact]
    public async Task Concurrent_same_key_assignment_creation_commits_one_result_and_replays_the_winner()
    {
        using var seedDb = CreateFreshDatabase();
        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));

        using var firstDb = CreateContext(connection!);
        using var secondDb = CreateContext(connection!);
        var firstService = new FirstSliceService(
            new PostgresFirstSliceStore(firstDb), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer());
        var secondService = new FirstSliceService(
            new PostgresFirstSliceStore(secondDb), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer());

        var firstTask = firstService.CreateAssignmentAsync(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{\"work\":\"same\"}", "concurrent-same-key",
            "correlation-concurrent-first", TestContext.Current.CancellationToken).AsTask();
        var secondTask = secondService.CreateAssignmentAsync(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{\"work\":\"same\"}", "concurrent-same-key",
            "correlation-concurrent-second", TestContext.Current.CancellationToken).AsTask();

        var results = await Task.WhenAll(firstTask, secondTask);

        Assert.Single(results, x => !x.Replayed);
        Assert.Single(results, x => x.Replayed);
        Assert.Equal(results[0].Value.Id, results[1].Value.Id);

        using var verifyDb = CreateContext(connection!);
        Assert.Single(verifyDb.Assignments);
        Assert.Single(verifyDb.IdempotencyRecords, x => x.IdempotencyKey == "concurrent-same-key");
        Assert.Single(verifyDb.AuditRecords);
        Assert.Single(verifyDb.OutboxMessages);
    }

    [Fact]
    public async Task Concurrent_same_key_assignment_close_replays_committed_winner()
    {
        using var seedDb = CreateFreshDatabase();
        var seedService = new FirstSliceService(
            new PostgresFirstSliceStore(seedDb), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer());
        var created = await seedService.CreateAssignmentAsync(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{}", "concurrent-close-create", "correlation-create",
            TestContext.Current.CancellationToken);

        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));
        using var firstDb = CreateContext(connection!);
        using var secondDb = CreateContext(connection!);
        var firstService = new FirstSliceService(
            new PostgresFirstSliceStore(firstDb), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer());
        var secondService = new FirstSliceService(
            new PostgresFirstSliceStore(secondDb), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer());

        var firstTask = firstService.CloseAssignmentAsync(
            "tenant-a", "authorized-teacher", created.Value.Id, 1,
            "concurrent-close-same-key", "correlation-close-first",
            TestContext.Current.CancellationToken).AsTask();
        var secondTask = secondService.CloseAssignmentAsync(
            "tenant-a", "authorized-teacher", created.Value.Id, 1,
            "concurrent-close-same-key", "correlation-close-second",
            TestContext.Current.CancellationToken).AsTask();

        var results = await Task.WhenAll(firstTask, secondTask);

        Assert.Equal(results[0].Id, results[1].Id);
        using var verifyDb = CreateContext(connection!);
        Assert.Equal("CLOSED", verifyDb.Assignments.Single(x => x.Id == created.Value.Id).Status);
        Assert.Equal(2, verifyDb.IdempotencyRecords.Count());
        Assert.Equal(2, verifyDb.AuditRecords.Count());
        Assert.Equal(2, verifyDb.OutboxMessages.Count());
    }

    [Fact]
    public async Task Revoked_creation_authorization_cannot_replay_durable_assignment_response()
    {
        using var db = CreateFreshDatabase();
        var authorizer = new MutableCreationAuthorizer(AuthorizationDecision.Allowed);
        var service = new FirstSliceService(
            new PostgresFirstSliceStore(db), new AllowCloseAuthorizer(),
            authorizer, new AllowSubmissionAuthorizer());

        var first = await service.CreateAssignmentAsync(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "learner-a", "{}", "create-revocation-replay-key", "correlation-create",
            TestContext.Current.CancellationToken);
        var counts = (db.Assignments.Count(), db.IdempotencyRecords.Count(),
            db.AuditRecords.Count(), db.OutboxMessages.Count());

        authorizer.Decision = AuthorizationDecision.Denied;
        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CreateAssignmentAsync(
                "tenant-a", "authorized-teacher", "context-a", "goal-a",
                "learner-a", "{}", "create-revocation-replay-key", "correlation-retry",
                TestContext.Current.CancellationToken));

        Assert.Equal("FORBIDDEN", error.Message);
        Assert.Equal(counts, (db.Assignments.Count(), db.IdempotencyRecords.Count(),
            db.AuditRecords.Count(), db.OutboxMessages.Count()));
        Assert.Equal(2, authorizer.CallCount);
        Assert.NotNull(first.Value.Id);
    }

    [Fact]
    public async Task Revoked_submission_authorization_cannot_replay_durable_submission_response()
    {
        using var db = CreateFreshDatabase();
        var authorizer = new MutableSubmissionAuthorizer(AuthorizationDecision.Allowed);
        var service = new FirstSliceService(
            new PostgresFirstSliceStore(db), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), authorizer);

        var created = await service.CreateAssignmentAsync(
            "tenant-a", "authorized-teacher", "context-a", "goal-a",
            "authorized-learner", "{}", "submission-revocation-create", "correlation-create",
            TestContext.Current.CancellationToken);
        var first = await service.CreateSubmissionAsync(
            "tenant-a", "authorized-learner", created.Value.Id, "authorized-learner",
            "{\"answer\":\"test\"}", "submission-revocation-replay-key", "correlation-submit",
            TestContext.Current.CancellationToken);
        var counts = (db.Submissions.Count(), db.IdempotencyRecords.Count(),
            db.AuditRecords.Count(), db.OutboxMessages.Count());

        authorizer.Decision = AuthorizationDecision.Denied;
        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CreateSubmissionAsync(
                "tenant-a", "authorized-learner", created.Value.Id, "authorized-learner",
                "{\"answer\":\"test\"}", "submission-revocation-replay-key", "correlation-retry",
                TestContext.Current.CancellationToken));

        Assert.Equal("FORBIDDEN", error.Message);
        Assert.Equal(counts, (db.Submissions.Count(), db.IdempotencyRecords.Count(),
            db.AuditRecords.Count(), db.OutboxMessages.Count()));
        Assert.Equal(2, authorizer.CallCount);
        Assert.NotNull(first.Value.Id);
    }

    [Fact]
    public async Task Transactional_command_checks_authority_before_replaying_assignment_creation()
    {
        using var db = CreateFreshDatabase();
        db.Persons.AddRange(
            new PersonAuthorityRecord { Id = "authority-teacher", Status = "ACTIVE" },
            new PersonAuthorityRecord { Id = "authority-learner", Status = "ACTIVE" });
        db.LearningContexts.Add(new LearningContextAuthorityRecord
        {
            Id = "authority-context", TenantId = "tenant-a", Status = "ACTIVE"
        });
        db.LearningGoals.Add(new LearningGoalAuthorityRecord
        {
            Id = "authority-goal", TenantId = "tenant-a",
            ContextId = "authority-context", Status = "ACTIVE"
        });
        db.ContextualMemberships.AddRange(
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "authority-context",
                PersonId = "authority-teacher", MembershipType = "TEACHER", Status = "ACTIVE"
            },
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "authority-context",
                PersonId = "authority-learner", MembershipType = "LEARNER", Status = "ACTIVE"
            });
        db.ActionPolicyGrants.Add(new ActionPolicyGrantAuthorityRecord
        {
            TenantId = "tenant-a", ContextId = "authority-context",
            SubjectType = "PERSON", SubjectId = "authority-teacher",
            Action = "assignment.create", Status = "ACTIVE"
        });
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = new FirstSliceService(
            new PostgresFirstSliceStore(db), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer(),
            new PostgresFirstSliceTransactionalCommand(db));

        var first = await service.CreateAssignmentAsync(
            "tenant-a", "authority-teacher", "authority-context", "authority-goal",
            "authority-learner", "{\"work\":\"authority\"}", "authority-replay-key",
            "authority-correlation-first", TestContext.Current.CancellationToken);
        Assert.False(first.Replayed);

        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));
        using var revokeDb = CreateContext(connection!);
        await new PostgresAuthorityRevocationWriter(revokeDb).RevokeGrantAsync(
            "tenant-a", "authority-context", "PERSON", "authority-teacher",
            "assignment.create", TestContext.Current.CancellationToken);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CreateAssignmentAsync(
                "tenant-a", "authority-teacher", "authority-context", "authority-goal",
                "authority-learner", "{\"work\":\"authority\"}", "authority-replay-key",
                "authority-correlation-retry", TestContext.Current.CancellationToken));

        Assert.Equal("FORBIDDEN", error.Message);
        Assert.Single(db.Assignments);
        Assert.Single(db.IdempotencyRecords);
        Assert.Single(db.AuditRecords);
        Assert.Single(db.OutboxMessages);
    }

    [Fact]
    public async Task Context_lock_serializes_creation_after_concurrent_authority_revocation()
    {
        using var seedDb = CreateFreshDatabase();
        seedDb.Persons.AddRange(
            new PersonAuthorityRecord { Id = "race-teacher", Status = "ACTIVE" },
            new PersonAuthorityRecord { Id = "race-learner", Status = "ACTIVE" });
        seedDb.LearningContexts.Add(new LearningContextAuthorityRecord
        {
            Id = "race-context", TenantId = "tenant-a", Status = "ACTIVE"
        });
        seedDb.LearningGoals.Add(new LearningGoalAuthorityRecord
        {
            Id = "race-goal", TenantId = "tenant-a", ContextId = "race-context", Status = "ACTIVE"
        });
        seedDb.ContextualMemberships.AddRange(
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "race-context", PersonId = "race-teacher",
                MembershipType = "TEACHER", Status = "ACTIVE"
            },
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "race-context", PersonId = "race-learner",
                MembershipType = "LEARNER", Status = "ACTIVE"
            });
        seedDb.ActionPolicyGrants.Add(new ActionPolicyGrantAuthorityRecord
        {
            TenantId = "tenant-a", ContextId = "race-context", SubjectType = "PERSON",
            SubjectId = "race-teacher", Action = "assignment.create", Status = "ACTIVE"
        });
        await seedDb.SaveChangesAsync(TestContext.Current.CancellationToken);

        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));
        using var commandDb = CreateContext(connection!);
        using var revokeDb = CreateContext(connection!);
        using var observerDb = CreateContext(connection!);
        await commandDb.Database.OpenConnectionAsync(TestContext.Current.CancellationToken);
        var commandBackendPid = await commandDb.Database
            .SqlQueryRaw<int>("SELECT pg_backend_pid() AS \"Value\"")
            .SingleAsync(TestContext.Current.CancellationToken);
        var service = new FirstSliceService(
            new PostgresFirstSliceStore(commandDb), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer(),
            new PostgresFirstSliceTransactionalCommand(commandDb));

        await using var revokeTransaction =
            await revokeDb.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
        var tenantId = "tenant-a";
        var contextId = "race-context";
        _ = await revokeDb.LearningContexts.FromSqlInterpolated(
                $"SELECT * FROM learning_contexts WHERE \"TenantId\" = {tenantId} AND \"Id\" = {contextId} FOR UPDATE")
            .AsTracking()
            .SingleAsync(TestContext.Current.CancellationToken);
        var grant = await revokeDb.ActionPolicyGrants.SingleAsync(
            x => x.TenantId == "tenant-a" && x.ContextId == "race-context" &&
                 x.SubjectId == "race-teacher" && x.Action == "assignment.create",
            TestContext.Current.CancellationToken);
        grant.Status = "REVOKED";
        grant.Version++;
        await revokeDb.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Start the protected command while the independent connection still owns the
        // context lock. After revocation commits, the command must observe the revoked grant.
        var createTask = service.CreateAssignmentAsync(
            "tenant-a", "race-teacher", "race-context", "race-goal", "race-learner",
            "{\"work\":\"race\"}", "race-revocation-key", "race-correlation",
            TestContext.Current.CancellationToken).AsTask();
        await AssertBackendWaitingForLockAsync(
            observerDb, commandBackendPid, TestContext.Current.CancellationToken);
        await revokeTransaction.CommitAsync(TestContext.Current.CancellationToken);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () => await createTask);
        Assert.Equal("FORBIDDEN", error.Message);
        using var verifyDb = CreateContext(connection!);
        Assert.Empty(verifyDb.Assignments);
        Assert.Empty(verifyDb.IdempotencyRecords);
        Assert.Empty(verifyDb.AuditRecords);
        Assert.Empty(verifyDb.OutboxMessages);
    }

    [Fact]
    public async Task Transactional_command_lock_wins_before_grant_revocation()
    {
        using var seedDb = CreateFreshDatabase();
        seedDb.Persons.AddRange(
            new PersonAuthorityRecord { Id = "reverse-teacher", Status = "ACTIVE" },
            new PersonAuthorityRecord { Id = "reverse-learner", Status = "ACTIVE" });
        seedDb.LearningContexts.Add(new LearningContextAuthorityRecord
        {
            Id = "reverse-context", TenantId = "tenant-a", Status = "ACTIVE"
        });
        seedDb.LearningGoals.Add(new LearningGoalAuthorityRecord
        {
            Id = "reverse-goal", TenantId = "tenant-a",
            ContextId = "reverse-context", Status = "ACTIVE"
        });
        seedDb.ContextualMemberships.AddRange(
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "reverse-context",
                PersonId = "reverse-teacher", MembershipType = "TEACHER", Status = "ACTIVE"
            },
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "reverse-context",
                PersonId = "reverse-learner", MembershipType = "LEARNER", Status = "ACTIVE"
            });
        seedDb.ActionPolicyGrants.Add(new ActionPolicyGrantAuthorityRecord
        {
            TenantId = "tenant-a", ContextId = "reverse-context", SubjectType = "PERSON",
            SubjectId = "reverse-teacher", Action = "assignment.create", Status = "ACTIVE"
        });
        await seedDb.SaveChangesAsync(TestContext.Current.CancellationToken);

        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));
        using var commandDb = CreateContext(connection!);
        using var revokeDb = CreateContext(connection!);
        using var blockerDb = CreateContext(connection!);
        using var observerDb = CreateContext(connection!);
        await commandDb.Database.OpenConnectionAsync(TestContext.Current.CancellationToken);
        await revokeDb.Database.OpenConnectionAsync(TestContext.Current.CancellationToken);
        var commandBackendPid = await commandDb.Database
            .SqlQueryRaw<int>("SELECT pg_backend_pid() AS \"Value\"")
            .SingleAsync(TestContext.Current.CancellationToken);
        var revokeBackendPid = await revokeDb.Database
            .SqlQueryRaw<int>("SELECT pg_backend_pid() AS \"Value\"")
            .SingleAsync(TestContext.Current.CancellationToken);

        var service = new FirstSliceService(
            new PostgresFirstSliceStore(commandDb), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer(),
            new PostgresFirstSliceTransactionalCommand(commandDb));
        var revocationWriter = new PostgresAuthorityRevocationWriter(revokeDb);

        // Hold the target table lock so the real protected command pauses after it has
        // acquired the context lock and passed its transaction-local authority checks.
        await using var blockerTransaction =
            await blockerDb.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
        await blockerDb.Database.ExecuteSqlRawAsync(
            "LOCK TABLE assignments IN ACCESS EXCLUSIVE MODE",
            TestContext.Current.CancellationToken);

        var createTask = service.CreateAssignmentAsync(
            "tenant-a", "reverse-teacher", "reverse-context", "reverse-goal", "reverse-learner",
            "{\"work\":\"reverse-order\"}", "reverse-order-create-key", "reverse-order-correlation",
            TestContext.Current.CancellationToken).AsTask();
        await AssertBackendWaitingForLockAsync(
            observerDb, commandBackendPid, TestContext.Current.CancellationToken);

        // Revocation now attempts the same context lock. Observing its actual PostgreSQL
        // lock wait proves the command owns the lock first; no timing-only sleep is used.
        var revokeTask = revocationWriter.RevokeGrantAsync(
            "tenant-a", "reverse-context", "PERSON", "reverse-teacher", "assignment.create",
            TestContext.Current.CancellationToken);
        await AssertBackendWaitingForLockAsync(
            observerDb, revokeBackendPid, TestContext.Current.CancellationToken);

        await blockerTransaction.CommitAsync(TestContext.Current.CancellationToken);
        var created = await createTask;
        await revokeTask;

        Assert.False(created.Replayed);
        using var verifyDb = CreateContext(connection!);
        Assert.Single(verifyDb.Assignments.Where(x =>
            x.TenantId == "tenant-a" && x.ContextId == "reverse-context"));
        Assert.Equal("REVOKED", verifyDb.ActionPolicyGrants.Single(x =>
            x.TenantId == "tenant-a" && x.ContextId == "reverse-context" &&
            x.SubjectId == "reverse-teacher" && x.Action == "assignment.create").Status);
        Assert.Single(verifyDb.IdempotencyRecords.Where(x => x.IdempotencyKey == "reverse-order-create-key"));
        Assert.Single(verifyDb.AuditRecords.Where(x => x.Operation == "assignment.create"));
        Assert.Single(verifyDb.OutboxMessages.Where(x => x.MessageType == "Assignment.AssignmentCreated"));
    }

    [Fact]
    public async Task Concurrent_same_key_creation_commits_one_mutation_and_returns_one_replay()
    {
        using var seedDb = CreateFreshDatabase();
        seedDb.Persons.AddRange(
            new PersonAuthorityRecord { Id = "same-key-teacher", Status = "ACTIVE" },
            new PersonAuthorityRecord { Id = "same-key-learner", Status = "ACTIVE" });
        seedDb.LearningContexts.Add(new LearningContextAuthorityRecord
        {
            Id = "same-key-context", TenantId = "tenant-a", Status = "ACTIVE"
        });
        seedDb.LearningGoals.Add(new LearningGoalAuthorityRecord
        {
            Id = "same-key-goal", TenantId = "tenant-a",
            ContextId = "same-key-context", Status = "ACTIVE"
        });
        seedDb.ContextualMemberships.AddRange(
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "same-key-context",
                PersonId = "same-key-teacher", MembershipType = "TEACHER", Status = "ACTIVE"
            },
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "same-key-context",
                PersonId = "same-key-learner", MembershipType = "LEARNER", Status = "ACTIVE"
            });
        seedDb.ActionPolicyGrants.Add(new ActionPolicyGrantAuthorityRecord
        {
            TenantId = "tenant-a", ContextId = "same-key-context", SubjectType = "PERSON",
            SubjectId = "same-key-teacher", Action = "assignment.create", Status = "ACTIVE"
        });
        await seedDb.SaveChangesAsync(TestContext.Current.CancellationToken);

        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));
        using var firstDb = CreateContext(connection!);
        using var secondDb = CreateContext(connection!);
        using var blockerDb = CreateContext(connection!);
        using var observerDb = CreateContext(connection!);
        await firstDb.Database.OpenConnectionAsync(TestContext.Current.CancellationToken);
        await secondDb.Database.OpenConnectionAsync(TestContext.Current.CancellationToken);
        var firstPid = await firstDb.Database
            .SqlQueryRaw<int>("SELECT pg_backend_pid() AS \"Value\"")
            .SingleAsync(TestContext.Current.CancellationToken);
        var secondPid = await secondDb.Database
            .SqlQueryRaw<int>("SELECT pg_backend_pid() AS \"Value\"")
            .SingleAsync(TestContext.Current.CancellationToken);

        FirstSliceService BuildService(EducationPlatformDbContext db) => new(
            new PostgresFirstSliceStore(db), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer(),
            new PostgresFirstSliceTransactionalCommand(db));
        var firstService = BuildService(firstDb);
        var secondService = BuildService(secondDb);

        // Force the first request to hold the context lock while waiting to insert its
        // assignment. The second request must wait for that same context lock.
        await using var blockerTransaction =
            await blockerDb.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
        await blockerDb.Database.ExecuteSqlRawAsync(
            "LOCK TABLE assignments IN ACCESS EXCLUSIVE MODE",
            TestContext.Current.CancellationToken);

        const string key = "concurrent-same-key";
        const string payload = "{\"work\":\"same-request\"}";
        var firstTask = firstService.CreateAssignmentAsync(
            "tenant-a", "same-key-teacher", "same-key-context", "same-key-goal", "same-key-learner",
            payload, key, "same-key-correlation-1", TestContext.Current.CancellationToken).AsTask();
        await AssertBackendWaitingForLockAsync(
            observerDb, firstPid, TestContext.Current.CancellationToken);

        var secondTask = secondService.CreateAssignmentAsync(
            "tenant-a", "same-key-teacher", "same-key-context", "same-key-goal", "same-key-learner",
            payload, key, "same-key-correlation-2", TestContext.Current.CancellationToken).AsTask();
        await AssertBackendWaitingForLockAsync(
            observerDb, secondPid, TestContext.Current.CancellationToken);

        await blockerTransaction.CommitAsync(TestContext.Current.CancellationToken);
        var results = await Task.WhenAll(firstTask, secondTask);

        Assert.Single(results.Where(x => !x.Replayed));
        Assert.Single(results.Where(x => x.Replayed));
        Assert.Single(results.Select(x => x.Value.Id).Distinct());

        using var verifyDb = CreateContext(connection!);
        Assert.Single(verifyDb.Assignments.Where(x =>
            x.TenantId == "tenant-a" && x.ContextId == "same-key-context"));
        Assert.Single(verifyDb.IdempotencyRecords.Where(x =>
            x.TenantId == "tenant-a" && x.ActorId == "same-key-teacher" &&
            x.OperationScope == "assignment.create" && x.IdempotencyKey == key));
        Assert.Single(verifyDb.AuditRecords.Where(x => x.Operation == "assignment.create"));
        Assert.Single(verifyDb.OutboxMessages.Where(x => x.MessageType == "Assignment.AssignmentCreated"));
    }

    private static async Task AssertBackendWaitingForLockAsync(
        EducationPlatformDbContext observerDb, int backendPid, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            var waitEvent = await observerDb.Database
                .SqlQueryRaw<string?>(
                    "SELECT wait_event_type AS \"Value\" FROM pg_stat_activity WHERE pid = {0}",
                    backendPid)
                .SingleOrDefaultAsync(cancellationToken);
            if (waitEvent == "Lock")
            {
                return;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken);
        }

        Assert.Fail($"PostgreSQL backend {backendPid} did not enter a lock wait within one second.");
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

        db.ActionPolicyGrants.RemoveRange(db.ActionPolicyGrants);
        db.ContextualMemberships.RemoveRange(db.ContextualMemberships);
        db.LearningGoals.RemoveRange(db.LearningGoals);
        db.Persons.RemoveRange(db.Persons);
        db.LearningContexts.RemoveRange(db.LearningContexts);
        db.Assignments.RemoveRange(db.Assignments);
        db.Submissions.RemoveRange(db.Submissions);
        db.IdempotencyRecords.RemoveRange(db.IdempotencyRecords);
        db.AuditRecords.RemoveRange(db.AuditRecords);
        db.OutboxMessages.RemoveRange(db.OutboxMessages);
        db.SaveChanges();

        return db;
    }

    [Fact]
    public async Task Transactional_submission_replay_is_denied_after_authoritative_membership_revocation()
    {
        using var db = CreateFreshDatabase();
        db.Persons.AddRange(
            new PersonAuthorityRecord { Id = "submit-teacher", Status = "ACTIVE" },
            new PersonAuthorityRecord { Id = "submit-learner", Status = "ACTIVE" });
        db.LearningContexts.Add(new LearningContextAuthorityRecord
        {
            Id = "submit-context", TenantId = "tenant-a", Status = "ACTIVE"
        });
        db.LearningGoals.Add(new LearningGoalAuthorityRecord
        {
            Id = "submit-goal", TenantId = "tenant-a", ContextId = "submit-context", Status = "ACTIVE"
        });
        db.ContextualMemberships.AddRange(
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "submit-context", PersonId = "submit-teacher",
                MembershipType = "TEACHER", Status = "ACTIVE"
            },
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "submit-context", PersonId = "submit-learner",
                MembershipType = "LEARNER", Status = "ACTIVE"
            });
        db.ActionPolicyGrants.AddRange(
            new ActionPolicyGrantAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "submit-context", SubjectType = "PERSON",
                SubjectId = "submit-teacher", Action = "assignment.create", Status = "ACTIVE"
            },
            new ActionPolicyGrantAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "submit-context", SubjectType = "PERSON",
                SubjectId = "submit-learner", Action = "submission.create", Status = "ACTIVE"
            });
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = new FirstSliceService(
            new PostgresFirstSliceStore(db), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer(),
            new PostgresFirstSliceTransactionalCommand(db));
        var assignment = await service.CreateAssignmentAsync(
            "tenant-a", "submit-teacher", "submit-context", "submit-goal", "submit-learner",
            "{\"work\":\"submit-revocation\"}", "submit-revocation-create", "submit-correlation-create",
            TestContext.Current.CancellationToken);
        var first = await service.CreateSubmissionAsync(
            "tenant-a", "submit-learner", assignment.Value.Id, "submit-learner",
            "{\"answer\":\"ok\"}", "submit-revocation-key", "submit-correlation-first",
            TestContext.Current.CancellationToken);
        Assert.False(first.Replayed);

        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));
        using var revokeDb = CreateContext(connection!);
        await new PostgresAuthorityRevocationWriter(revokeDb).RevokeMembershipAsync(
            "tenant-a", "submit-context", "submit-learner", "LEARNER",
            TestContext.Current.CancellationToken);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CreateSubmissionAsync(
                "tenant-a", "submit-learner", assignment.Value.Id, "submit-learner",
                "{\"answer\":\"ok\"}", "submit-revocation-key", "submit-correlation-retry",
                TestContext.Current.CancellationToken));

        Assert.Equal("FORBIDDEN", error.Message);
        using var verifyDb = CreateContext(connection!);
        Assert.Single(verifyDb.Submissions);
        Assert.Equal(1, verifyDb.IdempotencyRecords.Count(x => x.OperationScope == "submission.create"));
        Assert.Equal(1, verifyDb.AuditRecords.Count(x => x.Operation == "submission.create"));
        Assert.Equal(1, verifyDb.OutboxMessages.Count(x => x.MessageType == "Submission.SubmissionCreated"));
    }

    [Fact]
    public async Task Transactional_close_replay_is_denied_after_authoritative_grant_revocation()
    {
        using var db = CreateFreshDatabase();
        db.Persons.AddRange(
            new PersonAuthorityRecord { Id = "close-teacher", Status = "ACTIVE" },
            new PersonAuthorityRecord { Id = "close-learner", Status = "ACTIVE" });
        db.LearningContexts.Add(new LearningContextAuthorityRecord
        {
            Id = "close-context", TenantId = "tenant-a", Status = "ACTIVE"
        });
        db.LearningGoals.Add(new LearningGoalAuthorityRecord
        {
            Id = "close-goal", TenantId = "tenant-a", ContextId = "close-context", Status = "ACTIVE"
        });
        db.ContextualMemberships.AddRange(
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "close-context", PersonId = "close-teacher",
                MembershipType = "TEACHER", Status = "ACTIVE"
            },
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "close-context", PersonId = "close-learner",
                MembershipType = "LEARNER", Status = "ACTIVE"
            });
        db.ActionPolicyGrants.AddRange(
            new ActionPolicyGrantAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "close-context", SubjectType = "PERSON",
                SubjectId = "close-teacher", Action = "assignment.create", Status = "ACTIVE"
            },
            new ActionPolicyGrantAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "close-context", SubjectType = "PERSON",
                SubjectId = "close-teacher", Action = "assignment.close", Status = "ACTIVE"
            });
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = new FirstSliceService(
            new PostgresFirstSliceStore(db), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer(),
            new PostgresFirstSliceTransactionalCommand(db));
        var assignment = await service.CreateAssignmentAsync(
            "tenant-a", "close-teacher", "close-context", "close-goal", "close-learner",
            "{\"work\":\"close-revocation\"}", "close-revocation-create", "close-correlation-create",
            TestContext.Current.CancellationToken);
        var first = await service.CloseAssignmentAsync(
            "tenant-a", "close-teacher", assignment.Value.Id, 1,
            "close-revocation-key", "close-correlation-first", TestContext.Current.CancellationToken);
        Assert.Equal("CLOSED", first.Status);

        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));
        using var revokeDb = CreateContext(connection!);
        await new PostgresAuthorityRevocationWriter(revokeDb).RevokeGrantAsync(
            "tenant-a", "close-context", "PERSON", "close-teacher", "assignment.close",
            TestContext.Current.CancellationToken);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CloseAssignmentAsync(
                "tenant-a", "close-teacher", assignment.Value.Id, 1,
                "close-revocation-key", "close-correlation-retry", TestContext.Current.CancellationToken));

        Assert.Equal("FORBIDDEN", error.Message);
        using var verifyDb = CreateContext(connection!);
        Assert.Equal("CLOSED", verifyDb.Assignments.Single(x => x.Id == assignment.Value.Id).Status);
        Assert.Equal(1, verifyDb.IdempotencyRecords.Count(x => x.OperationScope == "assignment.close"));
        Assert.Equal(1, verifyDb.AuditRecords.Count(x => x.Operation == "assignment.close"));
        Assert.Equal(1, verifyDb.OutboxMessages.Count(x => x.MessageType == "Assignment.AssignmentClosed"));
    }

    [Fact]
    public async Task Transactional_submission_started_during_membership_revocation_fails_closed()
    {
        using var seedDb = CreateFreshDatabase();
        seedDb.Persons.AddRange(
            new PersonAuthorityRecord { Id = "race-submit-teacher", Status = "ACTIVE" },
            new PersonAuthorityRecord { Id = "race-submit-learner", Status = "ACTIVE" });
        seedDb.LearningContexts.Add(new LearningContextAuthorityRecord
        {
            Id = "race-submit-context", TenantId = "tenant-a", Status = "ACTIVE"
        });
        seedDb.LearningGoals.Add(new LearningGoalAuthorityRecord
        {
            Id = "race-submit-goal", TenantId = "tenant-a",
            ContextId = "race-submit-context", Status = "ACTIVE"
        });
        seedDb.ContextualMemberships.AddRange(
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "race-submit-context",
                PersonId = "race-submit-teacher", MembershipType = "TEACHER", Status = "ACTIVE"
            },
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "race-submit-context",
                PersonId = "race-submit-learner", MembershipType = "LEARNER", Status = "ACTIVE"
            });
        seedDb.ActionPolicyGrants.AddRange(
            new ActionPolicyGrantAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "race-submit-context", SubjectType = "PERSON",
                SubjectId = "race-submit-teacher", Action = "assignment.create", Status = "ACTIVE"
            },
            new ActionPolicyGrantAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "race-submit-context", SubjectType = "PERSON",
                SubjectId = "race-submit-learner", Action = "submission.create", Status = "ACTIVE"
            });
        await seedDb.SaveChangesAsync(TestContext.Current.CancellationToken);

        var seedService = new FirstSliceService(
            new PostgresFirstSliceStore(seedDb), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer(),
            new PostgresFirstSliceTransactionalCommand(seedDb));
        var assignment = await seedService.CreateAssignmentAsync(
            "tenant-a", "race-submit-teacher", "race-submit-context", "race-submit-goal",
            "race-submit-learner", "{\"work\":\"race\"}", "race-submit-create",
            "race-submit-correlation-create", TestContext.Current.CancellationToken);

        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));
        using var commandDb = CreateContext(connection!);
        using var revokeDb = CreateContext(connection!);
        using var observerDb = CreateContext(connection!);
        await commandDb.Database.OpenConnectionAsync(TestContext.Current.CancellationToken);
        var commandBackendPid = await commandDb.Database
            .SqlQueryRaw<int>("SELECT pg_backend_pid() AS \"Value\"")
            .SingleAsync(TestContext.Current.CancellationToken);
        var submitService = new FirstSliceService(
            new PostgresFirstSliceStore(commandDb), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer(),
            new PostgresFirstSliceTransactionalCommand(commandDb));

        await using var revokeTransaction =
            await revokeDb.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
        _ = await revokeDb.LearningContexts.FromSqlInterpolated(
                $"SELECT * FROM learning_contexts WHERE \"TenantId\" = {"tenant-a"} AND \"Id\" = {"race-submit-context"} FOR UPDATE")
            .AsTracking().SingleAsync(TestContext.Current.CancellationToken);
        var membership = await revokeDb.ContextualMemberships.SingleAsync(
            x => x.TenantId == "tenant-a" && x.ContextId == "race-submit-context" &&
                 x.PersonId == "race-submit-learner" && x.MembershipType == "LEARNER",
            TestContext.Current.CancellationToken);
        membership.Status = "REVOKED";
        membership.Version++;
        await revokeDb.SaveChangesAsync(TestContext.Current.CancellationToken);

        var submitTask = submitService.CreateSubmissionAsync(
            "tenant-a", "race-submit-learner", assignment.Value.Id, "race-submit-learner",
            "{\"answer\":\"race\"}", "race-submit-key", "race-submit-correlation",
            TestContext.Current.CancellationToken).AsTask();
        await AssertBackendWaitingForLockAsync(
            observerDb, commandBackendPid, TestContext.Current.CancellationToken);
        await revokeTransaction.CommitAsync(TestContext.Current.CancellationToken);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () => await submitTask);
        Assert.Equal("FORBIDDEN", error.Message);
        using var verifyDb = CreateContext(connection!);
        Assert.Empty(verifyDb.Submissions);
        Assert.Empty(verifyDb.IdempotencyRecords.Where(x => x.OperationScope == "submission.create"));
        Assert.Empty(verifyDb.AuditRecords.Where(x => x.Operation == "submission.create"));
        Assert.Empty(verifyDb.OutboxMessages.Where(x => x.MessageType == "Submission.SubmissionCreated"));
    }

    [Fact]
    public async Task Transactional_close_started_during_grant_revocation_fails_closed()
    {
        using var seedDb = CreateFreshDatabase();
        seedDb.Persons.AddRange(
            new PersonAuthorityRecord { Id = "race-close-teacher", Status = "ACTIVE" },
            new PersonAuthorityRecord { Id = "race-close-learner", Status = "ACTIVE" });
        seedDb.LearningContexts.Add(new LearningContextAuthorityRecord
        {
            Id = "race-close-context", TenantId = "tenant-a", Status = "ACTIVE"
        });
        seedDb.LearningGoals.Add(new LearningGoalAuthorityRecord
        {
            Id = "race-close-goal", TenantId = "tenant-a",
            ContextId = "race-close-context", Status = "ACTIVE"
        });
        seedDb.ContextualMemberships.AddRange(
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "race-close-context",
                PersonId = "race-close-teacher", MembershipType = "TEACHER", Status = "ACTIVE"
            },
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "race-close-context",
                PersonId = "race-close-learner", MembershipType = "LEARNER", Status = "ACTIVE"
            });
        seedDb.ActionPolicyGrants.AddRange(
            new ActionPolicyGrantAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "race-close-context", SubjectType = "PERSON",
                SubjectId = "race-close-teacher", Action = "assignment.create", Status = "ACTIVE"
            },
            new ActionPolicyGrantAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "race-close-context", SubjectType = "PERSON",
                SubjectId = "race-close-teacher", Action = "assignment.close", Status = "ACTIVE"
            });
        await seedDb.SaveChangesAsync(TestContext.Current.CancellationToken);

        var seedService = new FirstSliceService(
            new PostgresFirstSliceStore(seedDb), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer(),
            new PostgresFirstSliceTransactionalCommand(seedDb));
        var assignment = await seedService.CreateAssignmentAsync(
            "tenant-a", "race-close-teacher", "race-close-context", "race-close-goal",
            "race-close-learner", "{\"work\":\"race\"}", "race-close-create",
            "race-close-correlation-create", TestContext.Current.CancellationToken);

        var connection = Environment.GetEnvironmentVariable("EDUCATION_PLATFORM_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connection));
        using var commandDb = CreateContext(connection!);
        using var revokeDb = CreateContext(connection!);
        using var observerDb = CreateContext(connection!);
        await commandDb.Database.OpenConnectionAsync(TestContext.Current.CancellationToken);
        var commandBackendPid = await commandDb.Database
            .SqlQueryRaw<int>("SELECT pg_backend_pid() AS \"Value\"")
            .SingleAsync(TestContext.Current.CancellationToken);
        var closeService = new FirstSliceService(
            new PostgresFirstSliceStore(commandDb), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer(),
            new PostgresFirstSliceTransactionalCommand(commandDb));

        await using var revokeTransaction =
            await revokeDb.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
        _ = await revokeDb.LearningContexts.FromSqlInterpolated(
                $"SELECT * FROM learning_contexts WHERE \"TenantId\" = {"tenant-a"} AND \"Id\" = {"race-close-context"} FOR UPDATE")
            .AsTracking().SingleAsync(TestContext.Current.CancellationToken);
        var grant = await revokeDb.ActionPolicyGrants.SingleAsync(
            x => x.TenantId == "tenant-a" && x.ContextId == "race-close-context" &&
                 x.SubjectId == "race-close-teacher" && x.Action == "assignment.close",
            TestContext.Current.CancellationToken);
        grant.Status = "REVOKED";
        grant.Version++;
        await revokeDb.SaveChangesAsync(TestContext.Current.CancellationToken);

        var closeTask = closeService.CloseAssignmentAsync(
            "tenant-a", "race-close-teacher", assignment.Value.Id, 1,
            "race-close-key", "race-close-correlation", TestContext.Current.CancellationToken);
        await AssertBackendWaitingForLockAsync(
            observerDb, commandBackendPid, TestContext.Current.CancellationToken);
        await revokeTransaction.CommitAsync(TestContext.Current.CancellationToken);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () => await closeTask);
        Assert.Equal("FORBIDDEN", error.Message);
        using var verifyDb = CreateContext(connection!);
        Assert.Equal("ACTIVE", verifyDb.Assignments.Single(x => x.Id == assignment.Value.Id).Status);
        Assert.Empty(verifyDb.IdempotencyRecords.Where(x => x.OperationScope == "assignment.close"));
        Assert.Empty(verifyDb.AuditRecords.Where(x => x.Operation == "assignment.close"));
        Assert.Empty(verifyDb.OutboxMessages.Where(x => x.MessageType == "Assignment.AssignmentClosed"));
    }

    [Fact]
    public async Task Same_idempotency_key_is_conflict_across_contexts_for_same_actor_and_operation()
    {
        using var db = CreateFreshDatabase();
        db.Persons.AddRange(
            new PersonAuthorityRecord { Id = "scope-teacher", Status = "ACTIVE" },
            new PersonAuthorityRecord { Id = "scope-learner-a", Status = "ACTIVE" },
            new PersonAuthorityRecord { Id = "scope-learner-b", Status = "ACTIVE" });
        db.LearningContexts.AddRange(
            new LearningContextAuthorityRecord { Id = "scope-context-a", TenantId = "tenant-a", Status = "ACTIVE" },
            new LearningContextAuthorityRecord { Id = "scope-context-b", TenantId = "tenant-a", Status = "ACTIVE" });
        db.LearningGoals.AddRange(
            new LearningGoalAuthorityRecord { Id = "scope-goal-a", TenantId = "tenant-a", ContextId = "scope-context-a", Status = "ACTIVE" },
            new LearningGoalAuthorityRecord { Id = "scope-goal-b", TenantId = "tenant-a", ContextId = "scope-context-b", Status = "ACTIVE" });
        db.ContextualMemberships.AddRange(
            new ContextualMembershipAuthorityRecord { TenantId = "tenant-a", ContextId = "scope-context-a", PersonId = "scope-teacher", MembershipType = "TEACHER", Status = "ACTIVE" },
            new ContextualMembershipAuthorityRecord { TenantId = "tenant-a", ContextId = "scope-context-a", PersonId = "scope-learner-a", MembershipType = "LEARNER", Status = "ACTIVE" },
            new ContextualMembershipAuthorityRecord { TenantId = "tenant-a", ContextId = "scope-context-b", PersonId = "scope-teacher", MembershipType = "TEACHER", Status = "ACTIVE" },
            new ContextualMembershipAuthorityRecord { TenantId = "tenant-a", ContextId = "scope-context-b", PersonId = "scope-learner-b", MembershipType = "LEARNER", Status = "ACTIVE" });
        db.ActionPolicyGrants.AddRange(
            new ActionPolicyGrantAuthorityRecord { TenantId = "tenant-a", ContextId = "scope-context-a", SubjectType = "PERSON", SubjectId = "scope-teacher", Action = "assignment.create", Status = "ACTIVE" },
            new ActionPolicyGrantAuthorityRecord { TenantId = "tenant-a", ContextId = "scope-context-b", SubjectType = "PERSON", SubjectId = "scope-teacher", Action = "assignment.create", Status = "ACTIVE" });
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = new FirstSliceService(
            new PostgresFirstSliceStore(db), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer(),
            new PostgresFirstSliceTransactionalCommand(db));

        var first = await service.CreateAssignmentAsync(
            "tenant-a", "scope-teacher", "scope-context-a", "scope-goal-a",
            "scope-learner-a", "{\"work\":\"context-a\"}", "shared-key",
            "scope-correlation-a", TestContext.Current.CancellationToken);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CreateAssignmentAsync(
                "tenant-a", "scope-teacher", "scope-context-b", "scope-goal-b",
                "scope-learner-b", "{\"work\":\"context-b\"}", "shared-key",
                "scope-correlation-b", TestContext.Current.CancellationToken));

        Assert.Equal("IDEMPOTENCY_CONFLICT", error.Message);
        Assert.Single(db.Assignments);
        Assert.Equal(first.Value.Id, db.Assignments.Single().Id);
        Assert.Single(db.IdempotencyRecords.Where(x => x.OperationScope == "assignment.create"));
        Assert.Equal(1, db.AuditRecords.Count(x => x.Operation == "assignment.create"));
        Assert.Equal(1, db.OutboxMessages.Count(x => x.MessageType == "Assignment.AssignmentCreated"));
    }

    [Fact]
    public async Task Transactional_creation_rejects_active_membership_of_ineligible_type()
    {
        using var db = CreateFreshDatabase();
        db.Persons.AddRange(
            new PersonAuthorityRecord { Id = "role-teacher", Status = "ACTIVE" },
            new PersonAuthorityRecord { Id = "role-learner", Status = "ACTIVE" });
        db.LearningContexts.Add(new LearningContextAuthorityRecord
        {
            Id = "role-context", TenantId = "tenant-a", Status = "ACTIVE"
        });
        db.LearningGoals.Add(new LearningGoalAuthorityRecord
        {
            Id = "role-goal", TenantId = "tenant-a", ContextId = "role-context", Status = "ACTIVE"
        });
        // Both memberships are active, but their types are deliberately reversed.
        db.ContextualMemberships.AddRange(
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "role-context",
                PersonId = "role-teacher", MembershipType = "LEARNER", Status = "ACTIVE"
            },
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "role-context",
                PersonId = "role-learner", MembershipType = "TEACHER", Status = "ACTIVE"
            });
        db.ActionPolicyGrants.Add(new ActionPolicyGrantAuthorityRecord
        {
            TenantId = "tenant-a", ContextId = "role-context",
            SubjectType = "PERSON", SubjectId = "role-teacher",
            Action = "assignment.create", Status = "ACTIVE"
        });
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = new FirstSliceService(
            new PostgresFirstSliceStore(db), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer(),
            new PostgresFirstSliceTransactionalCommand(db));

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CreateAssignmentAsync(
                "tenant-a", "role-teacher", "role-context", "role-goal",
                "role-learner", "{\"work\":\"role-test\"}", "role-test-key",
                "role-test-correlation", TestContext.Current.CancellationToken));

        Assert.Equal("FORBIDDEN", error.Message);
        Assert.Empty(db.Assignments);
        Assert.Empty(db.IdempotencyRecords);
        Assert.Empty(db.AuditRecords);
        Assert.Empty(db.OutboxMessages);
    }


    [Fact]
    public async Task Transactional_submission_and_close_reject_active_memberships_of_wrong_type()
    {
        using var db = CreateFreshDatabase();
        db.Persons.AddRange(
            new PersonAuthorityRecord { Id = "wrong-role-teacher", Status = "ACTIVE" },
            new PersonAuthorityRecord { Id = "wrong-role-learner", Status = "ACTIVE" });
        db.LearningContexts.Add(new LearningContextAuthorityRecord
        {
            Id = "wrong-role-context", TenantId = "tenant-a", Status = "ACTIVE"
        });
        db.LearningGoals.Add(new LearningGoalAuthorityRecord
        {
            Id = "wrong-role-goal", TenantId = "tenant-a",
            ContextId = "wrong-role-context", Status = "ACTIVE"
        });
        db.ContextualMemberships.AddRange(
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "wrong-role-context",
                PersonId = "wrong-role-teacher", MembershipType = "TEACHER", Status = "ACTIVE"
            },
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "wrong-role-context",
                PersonId = "wrong-role-learner", MembershipType = "LEARNER", Status = "ACTIVE"
            });
        db.ActionPolicyGrants.AddRange(
            new ActionPolicyGrantAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "wrong-role-context",
                SubjectType = "PERSON", SubjectId = "wrong-role-teacher",
                Action = "assignment.create", Status = "ACTIVE"
            },
            new ActionPolicyGrantAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "wrong-role-context",
                SubjectType = "PERSON", SubjectId = "wrong-role-learner",
                Action = "submission.create", Status = "ACTIVE"
            },
            new ActionPolicyGrantAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "wrong-role-context",
                SubjectType = "PERSON", SubjectId = "wrong-role-teacher",
                Action = "assignment.close", Status = "ACTIVE"
            });
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = new FirstSliceService(
            new PostgresFirstSliceStore(db), new AllowCloseAuthorizer(),
            new AllowCreationAuthorizer(), new AllowSubmissionAuthorizer(),
            new PostgresFirstSliceTransactionalCommand(db));
        var assignment = await service.CreateAssignmentAsync(
            "tenant-a", "wrong-role-teacher", "wrong-role-context", "wrong-role-goal",
            "wrong-role-learner", "{\"work\":\"role-test\"}", "wrong-role-create",
            "wrong-role-create-correlation", TestContext.Current.CancellationToken);

        // MembershipType participates in the membership key, so replace the
        // authority rows rather than mutating their key in place.
        var existingMemberships = await db.ContextualMemberships
            .Where(x => x.ContextId == "wrong-role-context")
            .ToListAsync(TestContext.Current.CancellationToken);
        db.ContextualMemberships.RemoveRange(existingMemberships);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        db.ContextualMemberships.AddRange(
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "wrong-role-context",
                PersonId = "wrong-role-teacher", MembershipType = "LEARNER", Status = "ACTIVE"
            },
            new ContextualMembershipAuthorityRecord
            {
                TenantId = "tenant-a", ContextId = "wrong-role-context",
                PersonId = "wrong-role-learner", MembershipType = "TEACHER", Status = "ACTIVE"
            });
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var submissionError = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CreateSubmissionAsync(
                "tenant-a", "wrong-role-learner", assignment.Value.Id, "wrong-role-learner",
                "{\"answer\":\"should-deny\"}", "wrong-role-submit",
                "wrong-role-submit-correlation", TestContext.Current.CancellationToken));
        Assert.Equal("FORBIDDEN", submissionError.Message);

        var closeError = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CloseAssignmentAsync(
                "tenant-a", "wrong-role-teacher", assignment.Value.Id, 1,
                "wrong-role-close", "wrong-role-close-correlation",
                TestContext.Current.CancellationToken));
        Assert.Equal("FORBIDDEN", closeError.Message);

        db.ChangeTracker.Clear();
        Assert.Equal("ACTIVE", (await db.Assignments.SingleAsync(
            x => x.Id == assignment.Value.Id, TestContext.Current.CancellationToken)).Status);
        Assert.Empty(db.Submissions);
        Assert.Equal(1, await db.IdempotencyRecords.CountAsync(
            x => x.OperationScope == "assignment.create", TestContext.Current.CancellationToken));
        Assert.Empty(await db.IdempotencyRecords.Where(
            x => x.OperationScope == "submission.create" || x.OperationScope == "assignment.close")
            .ToListAsync(TestContext.Current.CancellationToken));
        Assert.Equal(1, await db.AuditRecords.CountAsync(
            x => x.Operation == "assignment.create", TestContext.Current.CancellationToken));
        Assert.Empty(await db.AuditRecords.Where(
            x => x.Operation == "submission.create" || x.Operation == "assignment.close")
            .ToListAsync(TestContext.Current.CancellationToken));
        Assert.Equal(1, await db.OutboxMessages.CountAsync(
            x => x.MessageType == "Assignment.AssignmentCreated", TestContext.Current.CancellationToken));
        Assert.Empty(await db.OutboxMessages.Where(
            x => x.MessageType == "Submission.SubmissionCreated" ||
                 x.MessageType == "Assignment.AssignmentClosed")
            .ToListAsync(TestContext.Current.CancellationToken));
    }

}
