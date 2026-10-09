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
        var service = new FirstSliceService(store, new AllowCloseAuthorizer());

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
    private sealed class AllowCloseAuthorizer : IAssignmentCloseAuthorizer
    {
        public ValueTask<AuthorizationDecision> AuthorizeAsync(
            AssignmentCloseAuthorizationRequest request,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(AuthorizationDecision.Allowed);
    }
}
