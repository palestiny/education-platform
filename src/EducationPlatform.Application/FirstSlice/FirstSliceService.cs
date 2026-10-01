using System.Security.Cryptography;
using System.Text;
using EducationPlatform.Domain.FirstSlice;

namespace EducationPlatform.Application.FirstSlice;

public sealed class FirstSliceService
{
    private readonly IFirstSliceStore _store;

    public FirstSliceService(IFirstSliceStore store) => _store = store;

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
        if (assignment.IsClosed)
            throw new InvalidOperationException("BUSINESS_RULE_VIOLATION");

        var fingerprint = Fingerprint($"submission.create|{assignment.Id}|{learnerId}|{payload}");
        return _store.CreateSubmission(
            tenantId, actorId, assignment, learnerId, payload,
            key, fingerprint, correlationId);
    }

    private static string Fingerprint(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
