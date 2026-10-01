using System.Collections.Concurrent;
using System.Text.Json;
using EducationPlatform.Domain.FirstSlice;

namespace EducationPlatform.Application.FirstSlice;

public sealed class InMemoryFirstSliceStore : IFirstSliceStore
{
    private readonly ConcurrentDictionary<string, Assignment> _assignments = new();
    private readonly ConcurrentDictionary<string, Submission> _submissions = new();
    private readonly ConcurrentDictionary<string, (string Fingerprint, object Response)> _idempotency = new();
    private readonly object _gate = new();

    public InMemoryFirstSliceStore()
    {
        _assignments["assignment-a"] = new Assignment
        {
            Id = "assignment-a", TenantId = "tenant-a", ContextId = "context-a",
            GoalId = "goal-a", LearnerId = "authorized-learner", Work = "seed"
        };
        _assignments["closed-assignment"] = new Assignment
        {
            Id = "closed-assignment", TenantId = "tenant-a", ContextId = "context-a",
            GoalId = "goal-a", LearnerId = "authorized-learner", Work = "seed"
        };
        _assignments["closed-assignment"].Close();
    }

    public Assignment? GetAssignment(string id) =>
        _assignments.TryGetValue(id, out var assignment) ? assignment : null;

    public FirstSliceMutation<Assignment> CreateAssignment(
        string tenantId, string actorId, string contextId, string goalId,
        string learnerId, string work, string? idempotencyKey,
        string requestFingerprint, string correlationId)
    {
        lock (_gate)
        {
            var existing = TryGetIdempotency(tenantId, actorId, "assignment.create", idempotencyKey);
            if (existing is not null)
                return new FirstSliceMutation<Assignment>(ReadAssignment(existing.Value.Response), true);

            var assignment = new Assignment
            {
                Id = Guid.NewGuid().ToString("N"),
                TenantId = tenantId, ContextId = contextId, GoalId = goalId,
                LearnerId = learnerId, Work = work
            };
            _assignments[assignment.Id] = assignment;
            SaveIdempotency(tenantId, actorId, "assignment.create", idempotencyKey, requestFingerprint, assignment);
            return new FirstSliceMutation<Assignment>(assignment, false);
        }
    }

    public FirstSliceMutation<Submission> CreateSubmission(
        string tenantId, string actorId, Assignment assignment, string learnerId,
        string payload, string? idempotencyKey, string requestFingerprint, string correlationId)
    {
        lock (_gate)
        {
            var existing = TryGetIdempotency(tenantId, actorId, "submission.create", idempotencyKey);
            if (existing is not null)
                return new FirstSliceMutation<Submission>(ReadSubmission(existing.Value.Response), true);

            var submission = new Submission
            {
                Id = Guid.NewGuid().ToString("N"),
                AssignmentId = assignment.Id, TenantId = tenantId,
                LearnerId = learnerId, Payload = payload
            };
            _submissions[submission.Id] = submission;
            SaveIdempotency(tenantId, actorId, "submission.create", idempotencyKey, requestFingerprint, submission);
            return new FirstSliceMutation<Submission>(submission, false);
        }
    }

    private (string Fingerprint, object Response)? TryGetIdempotency(
        string tenantId, string actorId, string operation, string? key)
    {
        if (string.IsNullOrWhiteSpace(key)) return null;
        return _idempotency.TryGetValue(Key(tenantId, actorId, operation, key), out var value)
            ? value
            : null;
    }

    private void SaveIdempotency(
        string tenantId, string actorId, string operation, string? key,
        string fingerprint, object response)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        var storageKey = Key(tenantId, actorId, operation, key);
        if (_idempotency.TryGetValue(storageKey, out var existing))
        {
            if (existing.Fingerprint != fingerprint)
                throw new InvalidOperationException("IDEMPOTENCY_CONFLICT");
            return;
        }
        _idempotency[storageKey] = (fingerprint, JsonSerializer.Serialize(response));
    }

    private static string Key(string tenantId, string actorId, string operation, string key) =>
        $"{tenantId}:{actorId}:{operation}:{key}";

    private static Assignment ReadAssignment(object response) =>
        JsonSerializer.Deserialize<Assignment>((string)response)!;

    private static Submission ReadSubmission(object response) =>
        JsonSerializer.Deserialize<Submission>((string)response)!;
}
