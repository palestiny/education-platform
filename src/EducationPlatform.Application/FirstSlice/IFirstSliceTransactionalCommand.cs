using EducationPlatform.Domain.FirstSlice;

namespace EducationPlatform.Application.FirstSlice;

/// <summary>
/// Provider-neutral command contract for operations whose final authorization decision
/// must be serialized with the PostgreSQL mutation, idempotency result, audit, and outbox.
/// TenantId and ActorId must be derived from trusted server-side execution context.
/// </summary>
public interface IFirstSliceTransactionalCommand
{
    ValueTask<FirstSliceMutation<Assignment>> CreateAssignmentAsync(
        CreateAssignmentCommand command,
        CancellationToken cancellationToken = default);

    ValueTask<FirstSliceMutation<Submission>> CreateSubmissionAsync(
        CreateSubmissionCommand command,
        CancellationToken cancellationToken = default);

    ValueTask<Assignment> CloseAssignmentAsync(
        CloseAssignmentCommand command,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Typed assignment-creation intent. The implementation must resolve and lock the
/// authoritative context, goal, recipient, membership, and action-policy rows in its
/// own transaction; caller-supplied values are intent, not proof of eligibility.
/// </summary>
public sealed record CreateAssignmentCommand(
    string TenantId,
    string ActorId,
    string ContextId,
    string GoalId,
    string LearnerId,
    string Work,
    string? IdempotencyKey,
    string RequestFingerprint,
    string CorrelationId);

/// <summary>
/// Typed submission intent. Assignment context and persisted recipient must be resolved
/// from authoritative persisted data inside the transaction, not from a caller snapshot.
/// </summary>
public sealed record CreateSubmissionCommand(
    string TenantId,
    string ActorId,
    string AssignmentId,
    string Payload,
    string? IdempotencyKey,
    string RequestFingerprint,
    string CorrelationId);

/// <summary>
/// Typed assignment-close intent. The transaction-aware implementation must authorize
/// before resolving a protected idempotency replay and must lock/re-read the assignment.
/// </summary>
public sealed record CloseAssignmentCommand(
    string TenantId,
    string ActorId,
    string AssignmentId,
    int ExpectedVersion,
    string? IdempotencyKey,
    string RequestFingerprint,
    string CorrelationId);
