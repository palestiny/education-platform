using System.Security.Cryptography;
using System.Text;
using EducationPlatform.Application.Security;
using EducationPlatform.Domain.FirstSlice;

namespace EducationPlatform.Application.FirstSlice;

public sealed class FirstSliceService
{
    private readonly IFirstSliceStore _store;
    private readonly IAssignmentCloseAuthorizer _closeAuthorizer;

    public FirstSliceService(IFirstSliceStore store, IAssignmentCloseAuthorizer closeAuthorizer)
    {
        _store = store;
        _closeAuthorizer = closeAuthorizer;
    }

    public FirstSliceMutation<Assignment> CreateAssignment(
        string tenantId, string actorId, string contextId, string goalId,
        string learnerId, string work, string? key, string correlationId)
    {
        var fingerprint = Fingerprint($"assignment.create|{contextId}|{goalId}|{learnerId}|{work}");
        return _store.CreateAssignment(
            tenantId, actorId, contextId, goalId, learnerId, work,
            key, fingerprint, correlationId);
    }

    public FirstSliceMutation<Submission> CreateSubmission(
        string tenantId, string actorId, Assignment assignment,
        string learnerId, string payload, string? key, string correlationId)
    {
        var fingerprint = Fingerprint($"submission.create|{assignment.Id}|{learnerId}|{payload}");
        return _store.CreateSubmission(
            tenantId, actorId, assignment, learnerId, payload,
            key, fingerprint, correlationId);
    }

    public async ValueTask<Assignment> CloseAssignmentAsync(
        string tenantId, string actorId, string assignmentId, int expectedVersion,
        string? idempotencyKey, string correlationId, CancellationToken cancellationToken = default)
    {
        if (expectedVersion < 1)
            throw new InvalidOperationException("VALIDATION_FAILED");

        // Resolve the resource in trusted tenant scope inside the application use case.
        // Authorization is checked before the mutation store can return an idempotency replay.
        var assignment = _store.GetAssignment(tenantId, assignmentId);
        if (assignment is null)
            throw new InvalidOperationException("RESOURCE_NOT_FOUND");

        var authorization = await _closeAuthorizer.AuthorizeAsync(
            new AssignmentCloseAuthorizationRequest(
                actorId, tenantId, assignment.Id, assignment.ContextId, "assignment.close"),
            cancellationToken);

        if (authorization == AuthorizationDecision.Denied)
            throw new InvalidOperationException("FORBIDDEN");
        if (authorization != AuthorizationDecision.Allowed)
            throw new InvalidOperationException("AUTHORIZATION_UNAVAILABLE");

        var fingerprint = Fingerprint($"assignment.close|{assignmentId}|{expectedVersion}");
        return _store.CloseAssignment(
            tenantId, actorId, assignmentId, expectedVersion, idempotencyKey, fingerprint, correlationId);
    }

    private static string Fingerprint(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
