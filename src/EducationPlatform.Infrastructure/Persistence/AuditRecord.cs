namespace EducationPlatform.Infrastructure.Persistence;

public sealed class AuditRecord
{
    public Guid Id { get; set; }
    public required string TenantId { get; set; }
    public required string ActorId { get; set; }
    public required string ContextId { get; set; }
    public required string Operation { get; set; }
    public required string CorrelationId { get; set; }
    public required string ResourceType { get; set; }
    public required string ResourceId { get; set; }
    public required string ResultingStatus { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
}
