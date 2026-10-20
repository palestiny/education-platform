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
    private readonly IFirstSliceTransactionalCommand? _transactionalCommand;

    // Compatibility constructor for existing in-memory/unit-test composition.
    public FirstSliceService(
        IFirstSliceStore store,
        IAssignmentCloseAuthorizer closeAuthorizer,
        IAssignmentCreationAuthorizer creationAuthorizer,
        ILearnerSubmissionAuthorizer submissionAuthorizer)
        : this(store, closeAuthorizer, creationAuthorizer, submissionAuthorizer, null)
    {
    }

    // Production composition supplies the transaction-aware command. Authorization
    // and idempotency are then evaluated by the same persistence transaction.
    public FirstSliceService(
        IFirstSliceStore store,
        IAssignmentCloseAuthorizer closeAuthorizer,
        IAssignmentCreationAuthorizer creationAuthorizer,
        ILearnerSubmissionAuthorizer submissionAuthorizer,
        IFirstSliceTransactionalCommand? transactionalCommand)
    {
        _store = store;
        _closeAuthorizer = closeAuthorizer;
        _creationAuthorizer = creationAuthorizer;
        _submissionAuthorizer = submissionAuthorizer;
        _transactionalCommand = transactionalCommand;
    }

    public async ValueTask<FirstSliceMutation<Assignment>> CreateAssignmentAsync(
        string tenantId, string actorId, string contextId, string goalId,
        string learnerId, string work, string? key, string correlationId,
        CancellationToken cancellationToken = default)
    {
        var fingerprint = Fingerprint($"assignment.create|{contextId}|{goalId}|{learnerId}|{work}");
        if (_transactionalCommand is not null)
        {
            return await _transactionalCommand.CreateAssignmentAsync(
                new CreateAssignmentCommand(
                    tenantId, actorId, contextId, goalId, learnerId, work,
                    key, fingerprint, correlationId),
                cancellationToken);
        }

        var authorization = await _creationAuthorizer.AuthorizeAsync(
            new AssignmentCreationAuthorizationRequest(
                actorId, tenantId, contextId, goalId, learnerId, "assignment.create"),
            cancellationToken);

        EnsureAllowed(authorization);
        cancellationToken.ThrowIfCancellationRequested();

        return _store.CreateAssignment(
            tenantId, actorId, contextId, goalId, learnerId, work,
            key, fingerprint, correlationId);
    }

    public async ValueTask<FirstSliceMutation<Submission>> CreateSubmissionAsync(
        string tenantId, string actorId, string assignmentId,
        string learnerId, string payload, string? key, string correlationId,
        CancellationToken cancellationToken = default)
    {
        var fingerprint = Fingerprint($"submission.create|{assignmentId}|{learnerId}|{payload}");
        if (_transactionalCommand is not null)
        {
            return await _transactionalCommand.CreateSubmissionAsync(
                new CreateSubmissionCommand(
                    tenantId, actorId, assignmentId, payload, key, fingerprint, correlationId),
                cancellationToken);
        }

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

        var fingerprint = Fingerprint($"assignment.close|{assignmentId}|{expectedVersion}");
        if (_transactionalCommand is not null)
        {
            return await _transactionalCommand.CloseAssignmentAsync(
                new CloseAssignmentCommand(
                    tenantId, actorId, assignmentId, expectedVersion,
                    idempotencyKey, fingerprint, correlationId),
                cancellationToken);
        }

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
