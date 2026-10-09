using EducationPlatform.Domain.FirstSlice;

namespace EducationPlatform.Application.FirstSlice;

public interface IFirstSliceStore
{
    // Scope protected reads by the tenant from the trusted execution context.
    Assignment? GetAssignment(string tenantId, string id);

    FirstSliceMutation<Assignment> CreateAssignment(
        string tenantId,
        string actorId,
        string contextId,
        string goalId,
        string learnerId,
        string work,
        string? idempotencyKey,
        string requestFingerprint,
        string correlationId);

    FirstSliceMutation<Submission> CreateSubmission(
        string tenantId,
        string actorId,
        Assignment assignment,
        string learnerId,
        string payload,
        string? idempotencyKey,
        string requestFingerprint,
        string correlationId);

    Assignment CloseAssignment(
        string tenantId,
        string actorId,
        string assignmentId,
        int expectedVersion,
        string? idempotencyKey,
        string requestFingerprint,
        string correlationId);
}
