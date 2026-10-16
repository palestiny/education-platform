using EducationPlatform.Application.Identity;
using EducationPlatform.Application.Security;
using Xunit;

namespace EducationPlatform.ApplicationTests;

public sealed class IdentityExecutionContextResolverTests
{
    [Fact]
    public async Task Unknown_identity_never_creates_execution_context_or_resolves_membership()
    {
        var fakes = new ResolverFakes
        {
            Person = new(IdentityResolutionOutcome.UnknownIdentity)
        };

        var result = await fakes.CreateResolver().ResolveAsync(Identity, "assignment-1", "assignment.close");

        Assert.Equal(IdentityResolutionOutcome.UnknownIdentity, result.Outcome);
        Assert.Null(result.Context);
        Assert.Equal(0, fakes.LifecycleCalls);
        Assert.Equal(0, fakes.MembershipCalls);
        Assert.Equal(0, fakes.PolicyCalls);
    }

    [Fact]
    public async Task Inactive_person_fails_closed_before_membership_and_policy()
    {
        var fakes = new ResolverFakes { Lifecycle = PersonLifecycleState.Inactive };

        var result = await fakes.CreateResolver().ResolveAsync(Identity, "assignment-1", "assignment.close");

        Assert.Equal(IdentityResolutionOutcome.PersonInactive, result.Outcome);
        Assert.Null(result.Context);
        Assert.Equal(0, fakes.MembershipCalls);
        Assert.Equal(0, fakes.PolicyCalls);
    }

    [Theory]
    [InlineData(IdentityResolutionOutcome.NoEligibleMembership)]
    [InlineData(IdentityResolutionOutcome.AmbiguousMembership)]
    [InlineData(IdentityResolutionOutcome.Indeterminate)]
    public async Task Non_unique_or_unavailable_membership_never_creates_context(
        IdentityResolutionOutcome membershipOutcome)
    {
        var fakes = new ResolverFakes
        {
            Membership = new(membershipOutcome, "server-tenant", "context-a", new HashSet<string>())
        };

        var result = await fakes.CreateResolver().ResolveAsync(Identity, "assignment-1", "assignment.close");

        Assert.Equal(membershipOutcome, result.Outcome);
        Assert.Null(result.Context);
        Assert.Equal(0, fakes.PolicyCalls);
    }

    [Theory]
    [InlineData(AuthorizationDecision.Denied, IdentityResolutionOutcome.Denied)]
    [InlineData(AuthorizationDecision.Indeterminate, IdentityResolutionOutcome.Indeterminate)]
    public async Task Policy_denial_or_indeterminate_result_never_creates_context(
        AuthorizationDecision decision,
        IdentityResolutionOutcome expected)
    {
        var fakes = new ResolverFakes { Decision = decision };

        var result = await fakes.CreateResolver().ResolveAsync(Identity, "assignment-1", "assignment.close");

        Assert.Equal(expected, result.Outcome);
        Assert.Null(result.Context);
    }

    [Fact]
    public async Task Successful_resolution_uses_server_resolved_tenant_and_context()
    {
        var fakes = new ResolverFakes();

        var result = await fakes.CreateResolver().ResolveAsync(Identity, "assignment-1", "assignment.close");

        Assert.Equal(IdentityResolutionOutcome.AuthorizedContext, result.Outcome);
        Assert.NotNull(result.Context);
        Assert.Equal("person-1", result.Context.PrincipalId);
        Assert.Equal("server-tenant", result.Context.TenantId);
        Assert.Contains("assignment:close", result.Context.Authorities);
        Assert.Equal("server-tenant", fakes.LastPolicyRequest!.TenantId);
        Assert.Equal("context-a", fakes.LastPolicyRequest.LearningContextId);
        Assert.Equal("assignment-1", fakes.LastPolicyRequest.ResourceId);
        Assert.Equal("assignment.close", fakes.LastPolicyRequest.Action);
    }

    [Fact]
    public async Task Cancellation_propagates_without_becoming_an_authorization_result()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        var fakes = new ResolverFakes();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await fakes.CreateResolver().ResolveAsync(
                Identity, "assignment-1", "assignment.close", source.Token));
    }

    private static readonly ValidatedExternalIdentity Identity = new("https://issuer.example", "subject-1");

    private sealed class ResolverFakes
    {
        public PersonIdentityResolution Person { get; init; } =
            new(IdentityResolutionOutcome.AuthorizedContext, "person-1");
        public PersonLifecycleState Lifecycle { get; init; } = PersonLifecycleState.Active;
        public ContextualMembershipResolution Membership { get; init; } =
            new(IdentityResolutionOutcome.AuthorizedContext, "server-tenant", "context-a",
                new HashSet<string>(["assignment:close"]));
        public AuthorizationDecision Decision { get; init; } = AuthorizationDecision.Allowed;
        public int LifecycleCalls { get; private set; }
        public int MembershipCalls { get; private set; }
        public int PolicyCalls { get; private set; }
        public ResourceActionAuthorizationRequest? LastPolicyRequest { get; private set; }

        public IdentityExecutionContextResolver CreateResolver() => new(
            new IdentityResolver(Person),
            new LifecycleResolver(this),
            new MembershipResolver(this),
            new PolicyResolver(this));

        private sealed class IdentityResolver(PersonIdentityResolution result) : IExternalIdentityPersonResolver
        {
            public ValueTask<PersonIdentityResolution> ResolvePersonAsync(
                ValidatedExternalIdentity identity, CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return ValueTask.FromResult(result);
            }
        }

        private sealed class LifecycleResolver(ResolverFakes owner) : IPersonLifecycleResolver
        {
            public ValueTask<PersonLifecycleState> GetLifecycleStateAsync(
                string personId, CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();
                owner.LifecycleCalls++;
                return ValueTask.FromResult(owner.Lifecycle);
            }
        }

        private sealed class MembershipResolver(ResolverFakes owner) : IContextualMembershipResolver
        {
            public ValueTask<ContextualMembershipResolution> ResolveMembershipAsync(
                string personId, string resourceId, string action,
                CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();
                owner.MembershipCalls++;
                return ValueTask.FromResult(owner.Membership);
            }
        }

        private sealed class PolicyResolver(ResolverFakes owner) : IResourceActionPolicy
        {
            public ValueTask<AuthorizationDecision> AuthorizeAsync(
                ResourceActionAuthorizationRequest request,
                CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();
                owner.PolicyCalls++;
                owner.LastPolicyRequest = request;
                return ValueTask.FromResult(owner.Decision);
            }
        }
    }
}
