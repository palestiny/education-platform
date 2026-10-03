namespace EducationPlatform.Infrastructure.Persistence;

public sealed class OutboxMessage
{
    public Guid Id { get; set; }
    public required string MessageType { get; set; }
    public required string AggregateType { get; set; }
    public required string AggregateId { get; set; }
    public required string Payload { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public int AttemptCount { get; set; }
    public string? LastError { get; set; }
    public required string Status { get; set; }
}
