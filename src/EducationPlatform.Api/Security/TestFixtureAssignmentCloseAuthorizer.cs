using EducationPlatform.Application.Security;

namespace EducationPlatform.Api.Security;

/// <summary>
/// Deterministic test/development fixture only. These sets model membership and resource-action grants
/// independently; they are not a production membership or policy implementation.
/// </summary>
public sealed class TestFixtureAssignmentCloseAuthorizer : IAssignmentCloseAuthorizer
{
    private static readonly HashSet<string> EligibleContextMembers =
        new(StringComparer.Ordinal) { "authorized-teacher", "close-member-without-grant" };

    private static readonly HashSet<string> AssignmentCloseGrants =
        new(StringComparer.Ordinal) { "authorized-teacher", "close-grant-without-membership" };

    public ValueTask<AuthorizationDecision> AuthorizeAsync(
        AssignmentCloseAuthorizationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Action != "assignment.close" ||
            request.TenantId != "tenant-a" ||
            request.LearningContextId != "context-a")
            return ValueTask.FromResult(AuthorizationDecision.Denied);

        if (request.PrincipalId == "close-authorization-indeterminate")
            return ValueTask.FromResult(AuthorizationDecision.Indeterminate);

        var hasMembership = EligibleContextMembers.Contains(request.PrincipalId);
        var hasGrant = AssignmentCloseGrants.Contains(request.PrincipalId);

        return ValueTask.FromResult(
            hasMembership && hasGrant
                ? AuthorizationDecision.Allowed
                : AuthorizationDecision.Denied);
    }
}
