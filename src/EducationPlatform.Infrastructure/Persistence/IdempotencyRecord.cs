namespace EducationPlatform.Infrastructure.Persistence;

public sealed class IdempotencyRecord
{
    public Guid Id { get; set; }
    public required string TenantId { get; set; }
    public required string ActorId { get; set; }
    public required string OperationScope { get; set; }
    public required string IdempotencyKey { get; set; }
    public required string RequestFingerprint { get; set; }
    public required string ResourceType { get; set; }
    public required string ResourceId { get; set; }
    public required string ResponseJson { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
