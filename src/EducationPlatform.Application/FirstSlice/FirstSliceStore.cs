using EducationPlatform.Domain.FirstSlice;

namespace EducationPlatform.Application.FirstSlice;

public interface IFirstSliceStore
{
    Assignment? GetAssignment(string id);
    Assignment CreateAssignment(string tenantId, string contextId, string goalId, string learnerId, string work);
    Submission CreateSubmission(string tenantId, Assignment assignment, string learnerId, string payload);
    Submission? GetSubmissionByIdempotency(string tenantId, string actorId, string operation, string key);
    void SaveIdempotency(string tenantId, string actorId, string operation, string key, string fingerprint, object response);
    (string Fingerprint, object Response)? GetIdempotency(string tenantId, string actorId, string operation, string key);
}
