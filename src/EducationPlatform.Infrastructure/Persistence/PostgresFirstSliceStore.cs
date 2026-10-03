using System.Text.Json;
using EducationPlatform.Application.FirstSlice;
using EducationPlatform.Domain.FirstSlice;
using Microsoft.EntityFrameworkCore;

namespace EducationPlatform.Infrastructure.Persistence;

public sealed class PostgresFirstSliceStore(EducationPlatformDbContext db) : IFirstSliceStore
{
    public Assignment? GetAssignment(string id) =>
        db.Assignments.SingleOrDefault(x => x.Id == id);

    public FirstSliceMutation<Assignment> CreateAssignment(
        string tenantId, string actorId, string contextId, string goalId,
        string learnerId, string work, string? idempotencyKey,
        string requestFingerprint, string correlationId)
    {
        return ExecuteAtomic(
            tenantId, actorId, "assignment.create", contextId, idempotencyKey, requestFingerprint,
            () =>
            {
                var assignment = new Assignment
                {
                    Id = Guid.NewGuid().ToString("N"),
                    TenantId = tenantId,
                    ContextId = contextId,
                    GoalId = goalId,
                    LearnerId = learnerId,
                    Work = work
                };
                db.Assignments.Add(assignment);
                return (assignment, "AssignmentCreated");
            },
            correlationId);
    }

    public FirstSliceMutation<Submission> CreateSubmission(
        string tenantId, string actorId, Assignment assignment, string learnerId,
        string payload, string? idempotencyKey, string requestFingerprint, string correlationId)
    {
        return ExecuteAtomic(
            tenantId, actorId, "submission.create", assignment.ContextId, idempotencyKey, requestFingerprint,
            () =>
            {
                var submission = new Submission
                {
                    Id = Guid.NewGuid().ToString("N"),
                    AssignmentId = assignment.Id,
                    TenantId = tenantId,
                    LearnerId = learnerId,
                    Payload = payload
                };
                db.Submissions.Add(submission);
                return (submission, "SubmissionCreated");
            },
            correlationId);
    }

    private FirstSliceMutation<T> ExecuteAtomic<T>(
        string tenantId, string actorId, string operation, string contextId, string? idempotencyKey,
        string requestFingerprint, Func<(T Value, string Status)> mutation,
        string correlationId)
    {
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            var existing = db.IdempotencyRecords.SingleOrDefault(x =>
                x.TenantId == tenantId &&
                x.ActorId == actorId &&
                x.OperationScope == operation &&
                x.IdempotencyKey == idempotencyKey);

            if (existing is not null)
            {
                if (existing.RequestFingerprint != requestFingerprint)
                    throw new InvalidOperationException("IDEMPOTENCY_CONFLICT");

                return new FirstSliceMutation<T>(
                    JsonSerializer.Deserialize<T>(existing.ResponseJson)!,
                    true);
            }
        }

        using var transaction = db.Database.BeginTransaction();
        try
        {
            var (value, status) = mutation();

            if (!string.IsNullOrWhiteSpace(idempotencyKey))
            {
                db.IdempotencyRecords.Add(new IdempotencyRecord
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ActorId = actorId,
                    OperationScope = operation,
                    IdempotencyKey = idempotencyKey,
                    RequestFingerprint = requestFingerprint,
                    ResourceType = typeof(T).Name,
                    ResourceId = GetId(value),
                    ResponseJson = JsonSerializer.Serialize(value),
                    CreatedAt = DateTimeOffset.UtcNow
                });
            }

            db.AuditRecords.Add(new AuditRecord
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ActorId = actorId,
                ContextId = contextId,
                Operation = operation,
                CorrelationId = correlationId,
                ResourceType = typeof(T).Name,
                ResourceId = GetId(value),
                ResultingStatus = status,
                OccurredAt = DateTimeOffset.UtcNow
            });

            db.OutboxMessages.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                MessageType = $"{typeof(T).Name}.{status}",
                AggregateType = typeof(T).Name,
                AggregateId = GetId(value),
                Payload = JsonSerializer.Serialize(value),
                OccurredAt = DateTimeOffset.UtcNow,
                Status = "PENDING"
            });

            db.SaveChanges();
            transaction.Commit();
            return new FirstSliceMutation<T>(value, false);
        }
        catch (DbUpdateException ex) when (IsIdempotencyUniqueViolation(ex) && !string.IsNullOrWhiteSpace(idempotencyKey))
        {
            transaction.Rollback();
            var existing = db.IdempotencyRecords.Single(x =>
                x.TenantId == tenantId &&
                x.ActorId == actorId &&
                x.OperationScope == operation &&
                x.IdempotencyKey == idempotencyKey);

            if (existing.RequestFingerprint != requestFingerprint)
                throw new InvalidOperationException("IDEMPOTENCY_CONFLICT");

            return new FirstSliceMutation<T>(
                JsonSerializer.Deserialize<T>(existing.ResponseJson)!,
                true);
        }
    }

    public Assignment CloseAssignment(
        string tenantId, string actorId, string assignmentId, int expectedVersion,
        string? idempotencyKey, string requestFingerprint, string correlationId)
    {
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            var existing = db.IdempotencyRecords.SingleOrDefault(x =>
                x.TenantId == tenantId &&
                x.ActorId == actorId &&
                x.OperationScope == "assignment.close" &&
                x.IdempotencyKey == idempotencyKey);

            if (existing is not null)
            {
                if (existing.RequestFingerprint != requestFingerprint)
                    throw new InvalidOperationException("IDEMPOTENCY_CONFLICT");

                return JsonSerializer.Deserialize<Assignment>(existing.ResponseJson)!;
            }
        }

        using var transaction = db.Database.BeginTransaction();
        try
        {
            var assignment = db.Assignments.SingleOrDefault(x =>
                x.Id == assignmentId && x.TenantId == tenantId);

            if (assignment is null)
                throw new InvalidOperationException("RESOURCE_NOT_FOUND");

            if (assignment.Version != expectedVersion)
                throw new InvalidOperationException("CONCURRENCY_CONFLICT");

            assignment.Close();

            if (!string.IsNullOrWhiteSpace(idempotencyKey))
            {
                db.IdempotencyRecords.Add(new IdempotencyRecord
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ActorId = actorId,
                    OperationScope = "assignment.close",
                    IdempotencyKey = idempotencyKey,
                    RequestFingerprint = requestFingerprint,
                    ResourceType = nameof(Assignment),
                    ResourceId = assignment.Id,
                    ResponseJson = JsonSerializer.Serialize(assignment),
                    CreatedAt = DateTimeOffset.UtcNow
                });
            }

            db.AuditRecords.Add(new AuditRecord
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ActorId = actorId,
                ContextId = assignment.ContextId,
                Operation = "assignment.close",
                CorrelationId = correlationId,
                ResourceType = nameof(Assignment),
                ResourceId = assignment.Id,
                ResultingStatus = "AssignmentClosed",
                OccurredAt = DateTimeOffset.UtcNow
            });

            db.OutboxMessages.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                MessageType = "Assignment.AssignmentClosed",
                AggregateType = nameof(Assignment),
                AggregateId = assignment.Id,
                Payload = JsonSerializer.Serialize(assignment),
                OccurredAt = DateTimeOffset.UtcNow,
                Status = "PENDING"
            });

            db.SaveChanges();
            transaction.Commit();
            return assignment;
        }
        catch (DbUpdateConcurrencyException)
        {
            transaction.Rollback();
            throw new InvalidOperationException("CONCURRENCY_CONFLICT");
        }
        catch (DbUpdateException ex) when (IsIdempotencyUniqueViolation(ex) && !string.IsNullOrWhiteSpace(idempotencyKey))
        {
            transaction.Rollback();
            var existing = db.IdempotencyRecords.Single(x =>
                x.TenantId == tenantId &&
                x.ActorId == actorId &&
                x.OperationScope == "assignment.close" &&
                x.IdempotencyKey == idempotencyKey);

            if (existing.RequestFingerprint != requestFingerprint)
                throw new InvalidOperationException("IDEMPOTENCY_CONFLICT");

            return JsonSerializer.Deserialize<Assignment>(existing.ResponseJson)!;
        }
    }

    private static string GetId<T>(T value) =>
        value switch
        {
            Assignment a => a.Id,
            Submission s => s.Id,
            _ => throw new InvalidOperationException("UNSUPPORTED_RESOURCE")
        };

    private static bool IsIdempotencyUniqueViolation(DbUpdateException ex) =>
        ex.InnerException is Npgsql.PostgresException { SqlState: Npgsql.PostgresErrorCodes.UniqueViolation };
}
