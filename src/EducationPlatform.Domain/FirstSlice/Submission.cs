namespace EducationPlatform.Domain.FirstSlice;

public sealed class Submission
{
    public required string Id { get; init; }
    public required string AssignmentId { get; init; }
    public required string TenantId { get; init; }
    public required string LearnerId { get; init; }
    public required string Payload { get; init; }
    public string Status { get; init; } = "SUBMITTED";
    public int Version { get; init; } = 1;
}
