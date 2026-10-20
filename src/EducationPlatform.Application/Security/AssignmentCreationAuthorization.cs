namespace EducationPlatform.Application.Security;

public sealed record AssignmentCreationAuthorizationRequest(
    string PrincipalId,
    string TenantId,
    string LearningContextId,
    string GoalId,
    string LearnerId,
    string Action);

public interface IAssignmentCreationAuthorizer
{
    ValueTask<AuthorizationDecision> AuthorizeAsync(
        AssignmentCreationAuthorizationRequest request,
        CancellationToken cancellationToken = default);
}
