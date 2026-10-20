using System.Text.Json;
using EducationPlatform.Application.FirstSlice;
using EducationPlatform.Domain.FirstSlice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EducationPlatform.Infrastructure.Persistence;

/// <summary>
/// PostgreSQL implementation of the approved A1 boundary. Each protected command locks
/// its tenant-scoped learning context before evaluating current authority and before
/// reading an idempotency response. Authority writers must use the same context lock.
/// </summary>
public sealed class PostgresFirstSliceTransactionalCommand(EducationPlatformDbContext db)
    : IFirstSliceTransactionalCommand
{
    public async ValueTask<FirstSliceMutation<Assignment>> CreateAssignmentAsync(
        CreateAssignmentCommand command, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await LockAndValidateContextAsync(command.TenantId, command.ContextId, cancellationToken);
            await RequirePersonAsync(command.ActorId, cancellationToken);
            await RequirePersonAsync(command.LearnerId, cancellationToken);
            await RequireMembershipAsync(command.TenantId, command.ContextId, command.ActorId, "TEACHER", cancellationToken);
            await RequireMembershipAsync(command.TenantId, command.ContextId, command.LearnerId, "LEARNER", cancellationToken);
            await RequireGoalAsync(command.TenantId, command.ContextId, command.GoalId, cancellationToken);
            await RequireGrantAsync(command.TenantId, command.ContextId, command.ActorId, "assignment.create", cancellationToken);

            var replay = await FindReplayAsync<Assignment>(
                command.TenantId, command.ActorId, "assignment.create", command.IdempotencyKey,
                command.RequestFingerprint, cancellationToken);
            if (replay is not null)
            {
                await transaction.CommitAsync(cancellationToken);
                return new FirstSliceMutation<Assignment>(replay, true);
            }

            var assignment = new Assignment
            {
                Id = Guid.NewGuid().ToString("N"),
                TenantId = command.TenantId,
                ContextId = command.ContextId,
                GoalId = command.GoalId,
                LearnerId = command.LearnerId,
                Work = command.Work
            };
            db.Assignments.Add(assignment);
            AddIdempotency(command.TenantId, command.ActorId, "assignment.create", command.IdempotencyKey,
                command.RequestFingerprint, assignment);
            AddAuditAndOutbox(command.TenantId, command.ActorId, command.ContextId, "assignment.create",
                command.CorrelationId, assignment, "AssignmentCreated");
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return new FirstSliceMutation<Assignment>(assignment, false);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            db.ChangeTracker.Clear();
            throw;
        }
    }

    public async ValueTask<FirstSliceMutation<Submission>> CreateSubmissionAsync(
        CreateSubmissionCommand command, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var initial = await db.Assignments.AsNoTracking().SingleOrDefaultAsync(
                x => x.Id == command.AssignmentId && x.TenantId == command.TenantId, cancellationToken);
            if (initial is null)
                throw new InvalidOperationException("RESOURCE_NOT_FOUND");

            await LockAndValidateContextAsync(command.TenantId, initial.ContextId, cancellationToken);

            var assignment = await db.Assignments.FromSqlInterpolated(
                    $"SELECT * FROM assignments WHERE \"Id\" = {command.AssignmentId} AND \"TenantId\" = {command.TenantId} FOR UPDATE")
                .AsTracking()
                .SingleOrDefaultAsync(cancellationToken);
            if (assignment is null)
                throw new InvalidOperationException("RESOURCE_NOT_FOUND");
            if (!string.Equals(command.ActorId, assignment.LearnerId, StringComparison.Ordinal))
                throw new InvalidOperationException("RESOURCE_NOT_FOUND");

            await RequirePersonAsync(command.ActorId, cancellationToken);
            await RequireMembershipAsync(command.TenantId, assignment.ContextId, command.ActorId, "LEARNER", cancellationToken);
            await RequireGrantAsync(command.TenantId, assignment.ContextId, command.ActorId, "submission.create", cancellationToken);

            var replay = await FindReplayAsync<Submission>(
                command.TenantId, command.ActorId, "submission.create", command.IdempotencyKey,
                command.RequestFingerprint, cancellationToken);
            if (replay is not null)
            {
                await transaction.CommitAsync(cancellationToken);
                return new FirstSliceMutation<Submission>(replay, true);
            }

            if (assignment.IsClosed)
                throw new InvalidOperationException("BUSINESS_RULE_VIOLATION");

            var submission = new Submission
            {
                Id = Guid.NewGuid().ToString("N"),
                AssignmentId = assignment.Id,
                TenantId = command.TenantId,
                LearnerId = assignment.LearnerId,
                Payload = command.Payload
            };
            db.Submissions.Add(submission);
            AddIdempotency(command.TenantId, command.ActorId, "submission.create", command.IdempotencyKey,
                command.RequestFingerprint, submission);
            AddAuditAndOutbox(command.TenantId, command.ActorId, assignment.ContextId, "submission.create",
                command.CorrelationId, submission, "SubmissionCreated");
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return new FirstSliceMutation<Submission>(submission, false);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            db.ChangeTracker.Clear();
            throw;
        }
    }

    public async ValueTask<Assignment> CloseAssignmentAsync(
        CloseAssignmentCommand command, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var initial = await db.Assignments.AsNoTracking().SingleOrDefaultAsync(
                x => x.Id == command.AssignmentId && x.TenantId == command.TenantId, cancellationToken);
            if (initial is null)
                throw new InvalidOperationException("RESOURCE_NOT_FOUND");

            await LockAndValidateContextAsync(command.TenantId, initial.ContextId, cancellationToken);
            var assignment = await db.Assignments.FromSqlInterpolated(
                    $"SELECT * FROM assignments WHERE \"Id\" = {command.AssignmentId} AND \"TenantId\" = {command.TenantId} FOR UPDATE")
                .AsTracking()
                .SingleOrDefaultAsync(cancellationToken);
            if (assignment is null)
                throw new InvalidOperationException("RESOURCE_NOT_FOUND");

            await RequirePersonAsync(command.ActorId, cancellationToken);
            await RequireMembershipAsync(command.TenantId, assignment.ContextId, command.ActorId, "TEACHER", cancellationToken);
            await RequireGrantAsync(command.TenantId, assignment.ContextId, command.ActorId, "assignment.close", cancellationToken);

            var replay = await FindReplayAsync<Assignment>(
                command.TenantId, command.ActorId, "assignment.close", command.IdempotencyKey,
                command.RequestFingerprint, cancellationToken);
            if (replay is not null)
            {
                await transaction.CommitAsync(cancellationToken);
                return replay;
            }

            if (assignment.Version != command.ExpectedVersion)
                throw new InvalidOperationException("CONCURRENCY_CONFLICT");

            assignment.Close();
            AddIdempotency(command.TenantId, command.ActorId, "assignment.close", command.IdempotencyKey,
                command.RequestFingerprint, assignment);
            AddAuditAndOutbox(command.TenantId, command.ActorId, assignment.ContextId, "assignment.close",
                command.CorrelationId, assignment, "AssignmentClosed");
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return assignment;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            db.ChangeTracker.Clear();
            throw;
        }
    }

    private async Task LockAndValidateContextAsync(
        string tenantId, string contextId, CancellationToken cancellationToken)
    {
        // Locking the context serializes protected commands with each other. Every
        // authority revocation/update path must take this same lock before changing grants
        // or memberships; until those writers exist, revocation serialization is not proven.
        var context = await db.LearningContexts.FromSqlInterpolated(
                $"SELECT * FROM learning_contexts WHERE \"TenantId\" = {tenantId} AND \"Id\" = {contextId} FOR UPDATE")
            .AsTracking()
            .SingleOrDefaultAsync(cancellationToken);
        if (context is null || context.Status != "ACTIVE")
            throw new InvalidOperationException("FORBIDDEN");
    }

    private async Task RequirePersonAsync(string personId, CancellationToken cancellationToken)
    {
        var person = await db.Persons.AsNoTracking().SingleOrDefaultAsync(x => x.Id == personId, cancellationToken);
        if (person is null || person.Status != "ACTIVE")
            throw new InvalidOperationException("FORBIDDEN");
    }

    private async Task RequireGoalAsync(
        string tenantId, string contextId, string goalId, CancellationToken cancellationToken)
    {
        var goal = await db.LearningGoals.AsNoTracking().SingleOrDefaultAsync(
            x => x.TenantId == tenantId && x.ContextId == contextId && x.Id == goalId, cancellationToken);
        if (goal is null || goal.Status != "ACTIVE")
            throw new InvalidOperationException("FORBIDDEN");
    }

    private async Task RequireMembershipAsync(
        string tenantId, string contextId, string personId, string requiredMembershipType, CancellationToken cancellationToken)
    {
        var membership = await db.ContextualMemberships.AsNoTracking().AnyAsync(
            x => x.TenantId == tenantId && x.ContextId == contextId &&
                 x.PersonId == personId && x.MembershipType == requiredMembershipType &&
                 x.Status == "ACTIVE", cancellationToken);
        if (!membership)
            throw new InvalidOperationException("FORBIDDEN");
    }

    private async Task RequireGrantAsync(
        string tenantId, string contextId, string personId, string action, CancellationToken cancellationToken)
    {
        var grant = await db.ActionPolicyGrants.AsNoTracking().AnyAsync(
            x => x.TenantId == tenantId && x.ContextId == contextId &&
                 x.SubjectType == "PERSON" && x.SubjectId == personId &&
                 x.Action == action && x.Status == "ACTIVE", cancellationToken);
        if (!grant)
            throw new InvalidOperationException("FORBIDDEN");
    }

    private async Task<T?> FindReplayAsync<T>(
        string tenantId, string actorId, string operation, string? key, string fingerprint,
        CancellationToken cancellationToken) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        var existing = await db.IdempotencyRecords.AsNoTracking().SingleOrDefaultAsync(x =>
            x.TenantId == tenantId && x.ActorId == actorId &&
            x.OperationScope == operation && x.IdempotencyKey == key, cancellationToken);
        if (existing is null)
            return null;
        if (!string.Equals(existing.RequestFingerprint, fingerprint, StringComparison.Ordinal))
            throw new InvalidOperationException("IDEMPOTENCY_CONFLICT");
        return JsonSerializer.Deserialize<T>(existing.ResponseJson)
            ?? throw new InvalidOperationException("IDEMPOTENCY_RESPONSE_INVALID");
    }

    private void AddIdempotency<T>(
        string tenantId, string actorId, string operation, string? key, string fingerprint, T value)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
            return;
        db.IdempotencyRecords.Add(new IdempotencyRecord
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ActorId = actorId,
            OperationScope = operation,
            IdempotencyKey = key,
            RequestFingerprint = fingerprint,
            ResourceType = typeof(T).Name,
            ResourceId = GetId(value),
            ResponseJson = JsonSerializer.Serialize(value),
            CreatedAt = DateTimeOffset.UtcNow
        });
    }

    private void AddAuditAndOutbox<T>(
        string tenantId, string actorId, string contextId, string operation,
        string correlationId, T value, string status) where T : class
    {
        var id = GetId(value);
        var now = DateTimeOffset.UtcNow;
        db.AuditRecords.Add(new AuditRecord
        {
            Id = Guid.NewGuid(), TenantId = tenantId, ActorId = actorId, ContextId = contextId,
            Operation = operation, CorrelationId = correlationId, ResourceType = typeof(T).Name,
            ResourceId = id, ResultingStatus = status, OccurredAt = now
        });
        db.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(), MessageType = $"{typeof(T).Name}.{status}",
            AggregateType = typeof(T).Name, AggregateId = id, Payload = JsonSerializer.Serialize(value),
            OccurredAt = now, Status = "PENDING"
        });
    }

    private static string GetId<T>(T value) where T : class => value switch
    {
        Assignment assignment => assignment.Id,
        Submission submission => submission.Id,
        _ => throw new InvalidOperationException("UNSUPPORTED_RESOURCE")
    };
}
