namespace EducationPlatform.Infrastructure.Persistence;

/// <summary>Platform-owned person identity, independent of external credential providers.</summary>
public sealed class PersonAuthorityRecord
{
    public required string Id { get; set; }
    public required string Status { get; set; }
    public int Version { get; set; } = 1;
}

/// <summary>Tenant-scoped learning context used as the authority lock boundary.</summary>
public sealed class LearningContextAuthorityRecord
{
    public required string Id { get; set; }
    public required string TenantId { get; set; }
    public required string Status { get; set; }
    public int Version { get; set; } = 1;
}

/// <summary>A goal belongs to exactly one tenant-scoped learning context.</summary>
public sealed class LearningGoalAuthorityRecord
{
    public required string Id { get; set; }
    public required string TenantId { get; set; }
    public required string ContextId { get; set; }
    public required string Status { get; set; }
    public int Version { get; set; } = 1;
}

/// <summary>Current membership of a platform Person in a context.</summary>
public sealed class ContextualMembershipAuthorityRecord
{
    public required string TenantId { get; set; }
    public required string ContextId { get; set; }
    public required string PersonId { get; set; }
    public required string MembershipType { get; set; }
    public required string Status { get; set; }
    public int Version { get; set; } = 1;
}

/// <summary>Explicit tenant/context-scoped grant for a closed action identifier.</summary>
public sealed class ActionPolicyGrantAuthorityRecord
{
    public required string TenantId { get; set; }
    public required string ContextId { get; set; }
    public required string SubjectType { get; set; }
    public required string SubjectId { get; set; }
    public required string Action { get; set; }
    public required string Status { get; set; }
    public int Version { get; set; } = 1;
}
