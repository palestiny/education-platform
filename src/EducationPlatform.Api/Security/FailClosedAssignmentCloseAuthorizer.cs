using EducationPlatform.Application.Security;

namespace EducationPlatform.Api.Security;

/// <summary>
/// Safe production default until a real membership and resource-policy adapter is approved and configured.
/// </summary>
public sealed class FailClosedAssignmentCloseAuthorizer : IAssignmentCloseAuthorizer
{
    public ValueTask<AuthorizationDecision> AuthorizeAsync(
        AssignmentCloseAuthorizationRequest request,
        CancellationToken cancellationToken = default) =>
        ValueTask.FromResult(AuthorizationDecision.Indeterminate);
}
