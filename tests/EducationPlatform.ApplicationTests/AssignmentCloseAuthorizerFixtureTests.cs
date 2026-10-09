using EducationPlatform.Api.Security;
using EducationPlatform.Application.Security;
using Xunit;

namespace EducationPlatform.ApplicationTests;

public sealed class AssignmentCloseAuthorizerFixtureTests
{
    private readonly TestFixtureAssignmentCloseAuthorizer _authorizer = new();

    [Fact]
    public async Task Membership_and_explicit_resource_action_grant_allow_close()
    {
        var result = await _authorizer.AuthorizeAsync(
            Request(principalId: "authorized-teacher"),
            TestContext.Current.CancellationToken);

        Assert.Equal(AuthorizationDecision.Allowed, result);
    }

    [Fact]
    public async Task Membership_in_a_different_context_does_not_allow_close()
    {
        var result = await _authorizer.AuthorizeAsync(
            Request(principalId: "authorized-teacher", learningContextId: "context-b"),
            TestContext.Current.CancellationToken);

        Assert.Equal(AuthorizationDecision.Denied, result);
    }

    [Fact]
    public async Task A_grant_for_an_unrecognized_action_does_not_allow_close()
    {
        var result = await _authorizer.AuthorizeAsync(
            Request(principalId: "authorized-teacher", action: "assignment.delete"),
            TestContext.Current.CancellationToken);

        Assert.Equal(AuthorizationDecision.Denied, result);
    }

    [Fact]
    public async Task Explicit_resource_grant_does_not_apply_to_a_different_assignment()
    {
        var authorizer = TestFixtureAssignmentCloseAuthorizer.WithGrants(
        [
            new AssignmentCloseGrant(
                "authorized-teacher", "tenant-a", "context-a", "assignment-a", "assignment.close")
        ]);

        var allowed = await authorizer.AuthorizeAsync(
            Request(principalId: "authorized-teacher", assignmentId: "assignment-a"),
            TestContext.Current.CancellationToken);
        var differentResource = await authorizer.AuthorizeAsync(
            Request(principalId: "authorized-teacher", assignmentId: "assignment-b"),
            TestContext.Current.CancellationToken);

        Assert.Equal(AuthorizationDecision.Allowed, allowed);
        Assert.Equal(AuthorizationDecision.Denied, differentResource);
    }

    [Fact]
    public async Task Membership_without_grant_is_denied_by_the_fixture()
    {
        var result = await _authorizer.AuthorizeAsync(
            Request(principalId: "close-member-without-grant"),
            TestContext.Current.CancellationToken);

        Assert.Equal(AuthorizationDecision.Denied, result);
    }

    [Fact]
    public async Task Grant_without_membership_is_denied_by_the_fixture()
    {
        var result = await _authorizer.AuthorizeAsync(
            Request(principalId: "close-grant-without-membership"),
            TestContext.Current.CancellationToken);

        Assert.Equal(AuthorizationDecision.Denied, result);
    }

    [Fact]
    public async Task Unavailable_authorization_is_not_treated_as_allow()
    {
        var result = await _authorizer.AuthorizeAsync(
            Request(principalId: "close-authorization-indeterminate"),
            TestContext.Current.CancellationToken);

        Assert.Equal(AuthorizationDecision.Indeterminate, result);
    }


    [Fact]
    public async Task Fail_closed_authorizer_propagates_cancellation()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var authorizer = new FailClosedAssignmentCloseAuthorizer();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await authorizer.AuthorizeAsync(Request(principalId: "authorized-teacher"), cancellation.Token));
    }

    private static AssignmentCloseAuthorizationRequest Request(
        string principalId,
        string tenantId = "tenant-a",
        string learningContextId = "context-a",
        string action = "assignment.close",
        string assignmentId = "assignment-a") =>
        new(principalId, tenantId, assignmentId, learningContextId, action);
}
