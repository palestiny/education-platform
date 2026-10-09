namespace EducationPlatform.Application.Security;

public enum AuthorizationDecision
{
    Allowed,
    Denied,
    Indeterminate
}

public sealed record AssignmentCloseAuthorizationRequest(
    string PrincipalId,
    string TenantId,
    string AssignmentId,
    string LearningContextId,
    string Action);

public interface IAssignmentCloseAuthorizer
{
    ValueTask<AuthorizationDecision> AuthorizeAsync(
        AssignmentCloseAuthorizationRequest request,
        CancellationToken cancellationToken = default);
}
