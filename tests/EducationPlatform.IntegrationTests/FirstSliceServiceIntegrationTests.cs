using EducationPlatform.Application.FirstSlice;
using EducationPlatform.Application.Security;
using Xunit;

namespace EducationPlatform.IntegrationTests;

public sealed class FirstSliceServiceIntegrationTests
{
    [Fact]
    public void Idempotent_assignment_creation_reuses_the_authoritative_result()
    {
        var store = new InMemoryFirstSliceStore();
        var service = new FirstSliceService(store, new MutableCloseAuthorizer(AuthorizationDecision.Allowed));

        var first = service.CreateAssignment(
            "tenant-a", "authorized-teacher", "context-a", "goal-a", "learner-a", "{}", "integration-key", "correlation-a");

        var second = service.CreateAssignment(
            "tenant-a", "authorized-teacher", "context-a", "goal-a", "learner-a", "{}", "integration-key", "correlation-a");

        Assert.Equal(first.Value.Id, second.Value.Id);
        Assert.True(second.Replayed);
        Assert.Equal(first.Value.Id, second.Value.Id);
    }

    [Fact]
    public void Assignment_lookup_requires_matching_tenant()
    {
        var store = new InMemoryFirstSliceStore();

        Assert.NotNull(store.GetAssignment("tenant-a", "assignment-a"));
        Assert.Null(store.GetAssignment("tenant-b", "assignment-a"));
        Assert.Null(store.GetAssignment("tenant-a", "missing-assignment"));
    }
    [Fact]
    public async Task Close_denial_is_enforced_inside_application_service_without_mutation()
    {
        var store = new InMemoryFirstSliceStore();
        var authorizer = new MutableCloseAuthorizer(AuthorizationDecision.Denied);
        var service = new FirstSliceService(store, authorizer);
        var created = service.CreateAssignment(
            "tenant-a", "authorized-teacher", "context-a", "goal-a", "learner-a",
            "{}", "close-denied-create", "correlation-a").Value;

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CloseAssignmentAsync(
                "tenant-a", "authorized-teacher", created.Id, 1, "close-denied",
                "correlation-close-denied", TestContext.Current.CancellationToken));

        Assert.Equal("FORBIDDEN", error.Message);
        Assert.Equal(1, store.GetAssignment("tenant-a", created.Id)!.Version);
        Assert.Equal(1, authorizer.CallCount);
    }

    [Fact]
    public async Task Close_idempotency_replay_rechecks_authorization_in_application_service()
    {
        var store = new InMemoryFirstSliceStore();
        var authorizer = new MutableCloseAuthorizer(AuthorizationDecision.Allowed);
        var service = new FirstSliceService(store, authorizer);
        var created = service.CreateAssignment(
            "tenant-a", "authorized-teacher", "context-a", "goal-a", "learner-a",
            "{}", "close-replay-create", "correlation-a").Value;

        await service.CloseAssignmentAsync(
            "tenant-a", "authorized-teacher", created.Id, 1, "close-replay-key",
            "correlation-close", TestContext.Current.CancellationToken);

        authorizer.Decision = AuthorizationDecision.Denied;
        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CloseAssignmentAsync(
                "tenant-a", "authorized-teacher", created.Id, 1, "close-replay-key",
                "correlation-retry", TestContext.Current.CancellationToken));

        Assert.Equal("FORBIDDEN", error.Message);
        Assert.Equal(2, authorizer.CallCount);
    }

    [Fact]
    public async Task Indeterminate_authorization_is_enforced_inside_application_service_without_mutation()
    {
        var store = new InMemoryFirstSliceStore();
        var authorizer = new MutableCloseAuthorizer(AuthorizationDecision.Indeterminate);
        var service = new FirstSliceService(store, authorizer);
        var created = service.CreateAssignment(
            "tenant-a", "authorized-teacher", "context-a", "goal-a", "learner-a",
            "{}", "close-indeterminate-create", "correlation-a").Value;

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CloseAssignmentAsync(
                "tenant-a", "authorized-teacher", created.Id, 1, "close-indeterminate",
                "correlation-close-indeterminate", TestContext.Current.CancellationToken));

        Assert.Equal("AUTHORIZATION_UNAVAILABLE", error.Message);
        Assert.Equal(1, store.GetAssignment("tenant-a", created.Id)!.Version);
        Assert.Equal(1, authorizer.CallCount);
    }

    [Fact]
    public async Task Cross_tenant_close_is_not_authorized_or_mutated()
    {
        var store = new InMemoryFirstSliceStore();
        var authorizer = new MutableCloseAuthorizer(AuthorizationDecision.Allowed);
        var service = new FirstSliceService(store, authorizer);
        var created = service.CreateAssignment(
            "tenant-a", "authorized-teacher", "context-a", "goal-a", "learner-a",
            "{}", "close-tenant-create", "correlation-a").Value;

        var error = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.CloseAssignmentAsync(
                "tenant-b", "authorized-teacher", created.Id, 1, "close-cross-tenant",
                "correlation-close-cross-tenant", TestContext.Current.CancellationToken));

        Assert.Equal("RESOURCE_NOT_FOUND", error.Message);
        Assert.Equal(0, authorizer.CallCount);
        Assert.Equal(1, store.GetAssignment("tenant-a", created.Id)!.Version);
    }

    private sealed class MutableCloseAuthorizer(AuthorizationDecision initialDecision)
        : IAssignmentCloseAuthorizer
    {
        public AuthorizationDecision Decision { get; set; } = initialDecision;
        public int CallCount { get; private set; }

        public ValueTask<AuthorizationDecision> AuthorizeAsync(
            AssignmentCloseAuthorizationRequest request,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            return ValueTask.FromResult(Decision);
        }
    }
}
