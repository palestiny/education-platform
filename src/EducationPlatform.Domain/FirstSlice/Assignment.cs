namespace EducationPlatform.Domain.FirstSlice;

public sealed class Assignment
{
    public required string Id { get; init; }
    public required string TenantId { get; init; }
    public required string ContextId { get; init; }
    public required string GoalId { get; init; }
    public required string LearnerId { get; init; }
    public required string Work { get; init; }
    public string Status { get; private set; } = "ACTIVE";
    public int Version { get; private set; } = 1;

    public bool IsClosed => Status == "CLOSED";

    public void Close()
    {
        Status = "CLOSED";
        Version++;
    }
}
