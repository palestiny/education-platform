using EducationPlatform.Application.Security;

namespace EducationPlatform.Api.Security;

/// <summary>
/// Deterministic test/development fixture only. Membership and resource-action grants are
/// independent. The wildcard grant is a test convenience for dynamically created assignments,
/// not a production membership or policy implementation.
/// </summary>
public sealed class TestFixtureAssignmentCloseAuthorizer : IAssignmentCloseAuthorizer
{
    private static readonly HashSet<string> EligibleContextMembers =
        new(StringComparer.Ordinal) { "authorized-teacher", "close-member-without-grant" };

    private readonly HashSet<AssignmentCloseGrant> _grants;

    public TestFixtureAssignmentCloseAuthorizer()
        : this(
        [
            new AssignmentCloseGrant(
                "authorized-teacher", "tenant-a", "context-a", "*", "assignment.close"),
            new AssignmentCloseGrant(
                "close-grant-without-membership", "tenant-a", "context-a", "assignment-a", "assignment.close")
        ])
    {
    }

    private TestFixtureAssignmentCloseAuthorizer(IEnumerable<AssignmentCloseGrant> grants)
    {
        _grants = grants.ToHashSet();
    }

    public static TestFixtureAssignmentCloseAuthorizer WithGrants(
        IEnumerable<AssignmentCloseGrant> grants) => new(grants);

    public ValueTask<AuthorizationDecision> AuthorizeAsync(
        AssignmentCloseAuthorizationRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (request.TenantId != "tenant-a" ||
            request.LearningContextId != "context-a")
            return ValueTask.FromResult(AuthorizationDecision.Denied);

        if (request.PrincipalId == "close-authorization-indeterminate")
            return ValueTask.FromResult(AuthorizationDecision.Indeterminate);

        var hasMembership = EligibleContextMembers.Contains(request.PrincipalId);
        var hasGrant = _grants.Any(grant =>
            grant.PrincipalId == request.PrincipalId &&
            grant.TenantId == request.TenantId &&
            grant.LearningContextId == request.LearningContextId &&
            (grant.AssignmentId == request.AssignmentId || grant.AssignmentId == "*") &&
            grant.Action == request.Action);

        return ValueTask.FromResult(
            hasMembership && hasGrant
                ? AuthorizationDecision.Allowed
                : AuthorizationDecision.Denied);
    }
}

public sealed record AssignmentCloseGrant(
    string PrincipalId,
    string TenantId,
    string LearningContextId,
    string AssignmentId,
    string Action);
