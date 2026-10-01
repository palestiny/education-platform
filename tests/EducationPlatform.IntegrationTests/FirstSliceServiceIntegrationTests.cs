using EducationPlatform.Application.FirstSlice;
using Xunit;

namespace EducationPlatform.IntegrationTests;

public sealed class FirstSliceServiceIntegrationTests
{
    [Fact]
    public void Idempotent_assignment_creation_reuses_the_authoritative_result()
    {
        var store = new InMemoryFirstSliceStore();
        var service = new FirstSliceService(store);

        var first = service.CreateAssignment(
            "tenant-a", "authorized-teacher", "context-a", "goal-a", "learner-a", "{}", "integration-key");

        var second = service.CreateAssignment(
            "tenant-a", "authorized-teacher", "context-a", "goal-a", "learner-a", "{}", "integration-key");

        Assert.Equal(first.Id, second.Id);
        Assert.Same(first, second);
    }
}
