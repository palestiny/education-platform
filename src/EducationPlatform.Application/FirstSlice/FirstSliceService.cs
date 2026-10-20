using System.Security.Cryptography;
using System.Text;
using EducationPlatform.Application.Security;
using EducationPlatform.Domain.FirstSlice;

namespace EducationPlatform.Application.FirstSlice;

public sealed class FirstSliceService
{
    private readonly IFirstSliceStore _store;
    private readonly IAssignmentCloseAuthorizer _closeAuthorizer;
    private readonly IAssignmentCreationAuthorizer _creationAuthorizer;
    private readonly ILearnerSubmissionAuthorizer _submissionAuthorizer;

    public FirstSliceService(
        IFirstSliceStore store,
        IAssignmentCloseAuthorizer closeAuthorizer,
        IAssignmentCreationAuthorizer creationAuthorizer,
        ILearnerSubmissionAuthorizer submissionAuthorizer)
    {
        _store = store;
        _closeAuthorizer = closeAuthorizer;
        _creationAuthorizer = creationAuthorizer;
        _submissionAuthorizer = submissionAuthorizer;
    }

    public async ValueTask<FirstSliceMutation<Assignment>> CreateAssignmentAsync(
        string tenantId, string actorId, string contextId, string goalId,
        string learnerId, string work, string? key, string correlationId,
        CancellationToken cancellationToken = default)
    {
        var authorization = await _creationAuthorizer.AuthorizeAsync(
            new AssignmentCreationAuthorizationRequest(
                actorId, tenantId, contextId, goalId, learnerId, "assignment.create"),
            cancellationToken);

        EnsureAllowed(authorization);
        cancellationToken.ThrowIfCancellationRequested();

        var fingerprint = Fingerprint($"assignment.create|{contextId}|{goalId}|{learnerId}|{work}");
        return _store.CreateAssignment(
            tenantId, actorId, contextId, goalId, learnerId, work,
            key, fingerprint, correlationId);
    }

    public async ValueTask<FirstSliceMutation<Submission>> CreateSubmissionAsync(
        string tenantId, string actorId, string assignmentId,
        string learnerId, string payload, string? key, string correlationId,
        CancellationToken cancellationToken = default)
    {
        // Load the protected resource inside the Application use case. Never authorize
        // against a caller-supplied Assignment snapshot or client-supplied context.
        var assignment = _store.GetAssignment(tenantId, assignmentId);
        if (assignment is null || assignment.TenantId != tenantId)
            throw new InvalidOperationException("RESOURCE_NOT_FOUND");

        // Preserve non-disclosure for attempts to submit as someone other than the persisted recipient.
        if (!string.Equals(actorId, assignment.LearnerId, StringComparison.Ordinal) ||
            !string.Equals(learnerId, assignment.LearnerId, StringComparison.Ordinal))
            throw new InvalidOperationException("RESOURCE_NOT_FOUND");

        var authorization = await _submissionAuthorizer.AuthorizeAsync(
            new LearnerSubmissionAuthorizationRequest(
                actorId, tenantId, assignment.Id, assignment.ContextId, assignment.LearnerId,
                "submission.create"),
            cancellationToken);

        EnsureAllowed(authorization);
        cancellationToken.ThrowIfCancellationRequested();

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

        EnsureAllowed(authorization);
        cancellationToken.ThrowIfCancellationRequested();

        var fingerprint = Fingerprint($"assignment.close|{assignmentId}|{expectedVersion}");
        return _store.CloseAssignment(
            tenantId, actorId, assignmentId, expectedVersion, idempotencyKey, fingerprint, correlationId);
    }

    private static void EnsureAllowed(AuthorizationDecision authorization)
    {
        if (authorization == AuthorizationDecision.Denied)
            throw new InvalidOperationException("FORBIDDEN");
        if (authorization != AuthorizationDecision.Allowed)
            throw new InvalidOperationException("AUTHORIZATION_UNAVAILABLE");
    }

    private static string Fingerprint(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
