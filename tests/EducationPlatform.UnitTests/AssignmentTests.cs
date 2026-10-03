using EducationPlatform.Domain.FirstSlice;
using Xunit;

namespace EducationPlatform.UnitTests;

public sealed class AssignmentTests
{
    [Fact]
    public void New_assignment_starts_active_at_version_one()
    {
        var assignment = new Assignment
        {
            Id = "assignment-1",
            TenantId = "tenant-a",
            ContextId = "context-a",
            GoalId = "goal-a",
            LearnerId = "learner-a",
            Work = "{}"
        };

        Assert.Equal("ACTIVE", assignment.Status);
        Assert.Equal(1, assignment.Version);
        Assert.False(assignment.IsClosed);
    }

    [Fact]
    public void Closing_assignment_changes_status_and_closed_projection()
    {
        var assignment = new Assignment
        {
            Id = "assignment-1",
            TenantId = "tenant-a",
            ContextId = "context-a",
            GoalId = "goal-a",
            LearnerId = "learner-a",
            Work = "{}"
        };

        assignment.Close();

        Assert.Equal("CLOSED", assignment.Status);
        Assert.True(assignment.IsClosed);
    }
}
