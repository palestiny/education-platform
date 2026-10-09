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
        var result = await _authorizer.AuthorizeAsync(Request(
            principalId: "authorized-teacher"));

        Assert.Equal(AuthorizationDecision.Allowed, result);
    }

    [Fact]
    public async Task Membership_in_a_different_context_does_not_allow_close()
    {
        var result = await _authorizer.AuthorizeAsync(Request(
            principalId: "authorized-teacher",
            learningContextId: "context-b"), TestContext.Current.CancellationToken);

        Assert.Equal(AuthorizationDecision.Denied, result);
    }

    [Fact]
    public async Task A_grant_for_an_unrecognized_action_does_not_allow_close()
    {
        var result = await _authorizer.AuthorizeAsync(Request(
            principalId: "authorized-teacher",
            action: "assignment.delete"), TestContext.Current.CancellationToken);

        Assert.Equal(AuthorizationDecision.Denied, result);
    }

    [Fact]
    public async Task Membership_without_grant_is_denied_by_the_fixture()
    {
        var result = await _authorizer.AuthorizeAsync(Request(
            principalId: "close-member-without-grant"), TestContext.Current.CancellationToken);

        Assert.Equal(AuthorizationDecision.Denied, result);
    }

    [Fact]
    public async Task Grant_without_membership_is_denied_by_the_fixture()
    {
        var result = await _authorizer.AuthorizeAsync(Request(
            principalId: "close-grant-without-membership"), TestContext.Current.CancellationToken);

        Assert.Equal(AuthorizationDecision.Denied, result);
    }

    [Fact]
    public async Task Unavailable_authorization_is_not_treated_as_allow()
    {
        var result = await _authorizer.AuthorizeAsync(Request(
            principalId: "close-authorization-indeterminate"), TestContext.Current.CancellationToken);

        Assert.Equal(AuthorizationDecision.Indeterminate, result);
    }

    private static AssignmentCloseAuthorizationRequest Request(
        string principalId,
        string tenantId = "tenant-a",
        string learningContextId = "context-a",
        string action = "assignment.close") =>
        new(principalId, tenantId, "assignment-a", learningContextId, action);
}
