namespace EducationPlatform.Application.Security;

public sealed record LearnerSubmissionAuthorizationRequest(
    string PrincipalId,
    string TenantId,
    string AssignmentId,
    string LearningContextId,
    string LearnerId,
    string Action);

public interface ILearnerSubmissionAuthorizer
{
    ValueTask<AuthorizationDecision> AuthorizeAsync(
        LearnerSubmissionAuthorizationRequest request,
        CancellationToken cancellationToken = default);
}
