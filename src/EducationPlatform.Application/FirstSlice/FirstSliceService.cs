using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EducationPlatform.Domain.FirstSlice;

namespace EducationPlatform.Application.FirstSlice;

public sealed class FirstSliceService
{
    private readonly IFirstSliceStore _store;

    public FirstSliceService(IFirstSliceStore store) => _store = store;

    public Assignment CreateAssignment(string tenantId, string actorId, string contextId, string goalId, string learnerId, string work, string? key)
    {
        var fingerprint = Fingerprint($"{contextId}|{goalId}|{learnerId}|{work}");
        if (!string.IsNullOrWhiteSpace(key))
        {
            var existing = _store.GetIdempotency(tenantId, actorId, "assignment.create", key);
            if (existing is not null)
            {
                if (existing.Value.Fingerprint != fingerprint)
                    throw new InvalidOperationException("IDEMPOTENCY_CONFLICT");
                return (Assignment)existing.Value.Response;
            }
        }

        var assignment = _store.CreateAssignment(tenantId, contextId, goalId, learnerId, work);
        if (!string.IsNullOrWhiteSpace(key))
            _store.SaveIdempotency(tenantId, actorId, "assignment.create", key, fingerprint, assignment);
        return assignment;
    }

    public Submission CreateSubmission(string tenantId, string actorId, Assignment assignment, string learnerId, string payload, string? key)
    {
        if (assignment.IsClosed) throw new InvalidOperationException("BUSINESS_RULE_VIOLATION");

        var fingerprint = Fingerprint($"{assignment.Id}|{learnerId}|{payload}");
        if (!string.IsNullOrWhiteSpace(key))
        {
            var existing = _store.GetIdempotency(tenantId, actorId, "submission.create", key);
            if (existing is not null)
            {
                if (existing.Value.Fingerprint != fingerprint)
                    throw new InvalidOperationException("IDEMPOTENCY_CONFLICT");
                return (Submission)existing.Value.Response;
            }
        }

        var submission = _store.CreateSubmission(tenantId, assignment, learnerId, payload);
        if (!string.IsNullOrWhiteSpace(key))
            _store.SaveIdempotency(tenantId, actorId, "submission.create", key, fingerprint, submission);
        return submission;
    }

    private static string Fingerprint(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
