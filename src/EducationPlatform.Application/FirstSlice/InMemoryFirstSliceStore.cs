using System.Collections.Concurrent;
using EducationPlatform.Domain.FirstSlice;

namespace EducationPlatform.Application.FirstSlice;

// Development/test adapter. The durable PostgreSQL adapter is the next GREEN slice.
public sealed class InMemoryFirstSliceStore : IFirstSliceStore
{
    private readonly ConcurrentDictionary<string, Assignment> _assignments = new();
    private readonly ConcurrentDictionary<string, Submission> _submissions = new();
    private readonly ConcurrentDictionary<string, (string Fingerprint, object Response)> _idempotency = new();

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

    public Assignment CreateAssignment(string tenantId, string contextId, string goalId, string learnerId, string work)
    {
        var assignment = new Assignment
        {
            Id = Guid.NewGuid().ToString("N"),
            TenantId = tenantId, ContextId = contextId, GoalId = goalId,
            LearnerId = learnerId, Work = work
        };
        _assignments[assignment.Id] = assignment;
        return assignment;
    }

    public Submission CreateSubmission(string tenantId, Assignment assignment, string learnerId, string payload)
    {
        var submission = new Submission
        {
            Id = Guid.NewGuid().ToString("N"),
            AssignmentId = assignment.Id, TenantId = tenantId,
            LearnerId = learnerId, Payload = payload
        };
        _submissions[submission.Id] = submission;
        return submission;
    }

    public Submission? GetSubmissionByIdempotency(string tenantId, string actorId, string operation, string key)
    {
        var value = GetIdempotency(tenantId, actorId, operation, key);
        return value?.Response as Submission;
    }

    public void SaveIdempotency(string tenantId, string actorId, string operation, string key, string fingerprint, object response) =>
        _idempotency[$"{tenantId}:{actorId}:{operation}:{key}"] = (fingerprint, response);

    public (string Fingerprint, object Response)? GetIdempotency(string tenantId, string actorId, string operation, string key)
    {
        return _idempotency.TryGetValue($"{tenantId}:{actorId}:{operation}:{key}", out var value) ? value : null;
    }
}
