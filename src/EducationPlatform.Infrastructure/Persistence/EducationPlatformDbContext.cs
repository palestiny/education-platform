using EducationPlatform.Domain.FirstSlice;
using Microsoft.EntityFrameworkCore;

namespace EducationPlatform.Infrastructure.Persistence;

public sealed class EducationPlatformDbContext(DbContextOptions<EducationPlatformDbContext> options)
    : DbContext(options)
{
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
    public DbSet<AuditRecord> AuditRecords => Set<AuditRecord>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<PersonAuthorityRecord> Persons => Set<PersonAuthorityRecord>();
    public DbSet<LearningContextAuthorityRecord> LearningContexts => Set<LearningContextAuthorityRecord>();
    public DbSet<LearningGoalAuthorityRecord> LearningGoals => Set<LearningGoalAuthorityRecord>();
    public DbSet<ContextualMembershipAuthorityRecord> ContextualMemberships => Set<ContextualMembershipAuthorityRecord>();
    public DbSet<ActionPolicyGrantAuthorityRecord> ActionPolicyGrants => Set<ActionPolicyGrantAuthorityRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("assignments");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasMaxLength(64);
            entity.Property(x => x.TenantId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.ContextId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.GoalId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.LearnerId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Work).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Version).IsConcurrencyToken();
            entity.HasIndex(x => new { x.TenantId, x.ContextId });
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.ToTable("submissions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasMaxLength(64);
            entity.Property(x => x.AssignmentId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.TenantId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.LearnerId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Payload).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(32).IsRequired();
            entity.HasIndex(x => new { x.TenantId, x.AssignmentId });
        });

        modelBuilder.Entity<IdempotencyRecord>(entity =>
        {
            entity.ToTable("idempotency_records");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.ActorId).HasMaxLength(128).IsRequired();
            entity.Property(x => x.OperationScope).HasMaxLength(128).IsRequired();
            entity.Property(x => x.IdempotencyKey).HasMaxLength(256).IsRequired();
            entity.Property(x => x.RequestFingerprint).HasMaxLength(128).IsRequired();
            entity.Property(x => x.ResourceType).HasMaxLength(64).IsRequired();
            entity.Property(x => x.ResourceId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.ResponseJson).IsRequired();
            entity.HasIndex(x => new { x.TenantId, x.ActorId, x.OperationScope, x.IdempotencyKey }).IsUnique();
        });

        modelBuilder.Entity<AuditRecord>(entity =>
        {
            entity.ToTable("audit_records");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.ActorId).HasMaxLength(128).IsRequired();
            entity.Property(x => x.ContextId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Operation).HasMaxLength(128).IsRequired();
            entity.Property(x => x.CorrelationId).HasMaxLength(128).IsRequired();
            entity.Property(x => x.ResourceType).HasMaxLength(64).IsRequired();
            entity.Property(x => x.ResourceId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.ResultingStatus).HasMaxLength(64).IsRequired();
            entity.HasIndex(x => new { x.TenantId, x.OccurredAt });
        });

        modelBuilder.Entity<PersonAuthorityRecord>(entity =>
        {
            entity.ToTable("persons");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasMaxLength(64);
            entity.Property(x => x.Status).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Version).IsConcurrencyToken();
        });

        modelBuilder.Entity<LearningContextAuthorityRecord>(entity =>
        {
            entity.ToTable("learning_contexts");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasMaxLength(64);
            entity.Property(x => x.TenantId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Version).IsConcurrencyToken();
            entity.HasAlternateKey(x => new { x.TenantId, x.Id });
            entity.HasIndex(x => new { x.TenantId, x.Id }).IsUnique();
        });

        modelBuilder.Entity<LearningGoalAuthorityRecord>(entity =>
        {
            entity.ToTable("learning_goals");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasMaxLength(64);
            entity.Property(x => x.TenantId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.ContextId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Version).IsConcurrencyToken();
            entity.HasAlternateKey(x => new { x.TenantId, x.ContextId, x.Id });
            entity.HasOne<LearningContextAuthorityRecord>().WithMany()
                .HasForeignKey(x => new { x.TenantId, x.ContextId })
                .HasPrincipalKey(x => new { x.TenantId, x.Id })
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.TenantId, x.ContextId });
        });

        modelBuilder.Entity<ContextualMembershipAuthorityRecord>(entity =>
        {
            entity.ToTable("contextual_memberships");
            entity.HasKey(x => new { x.TenantId, x.ContextId, x.PersonId, x.MembershipType });
            entity.Property(x => x.TenantId).HasMaxLength(64);
            entity.Property(x => x.ContextId).HasMaxLength(64);
            entity.Property(x => x.PersonId).HasMaxLength(64);
            entity.Property(x => x.MembershipType).HasMaxLength(32);
            entity.Property(x => x.Status).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Version).IsConcurrencyToken();
            entity.HasOne<LearningContextAuthorityRecord>().WithMany()
                .HasForeignKey(x => new { x.TenantId, x.ContextId })
                .HasPrincipalKey(x => new { x.TenantId, x.Id })
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<PersonAuthorityRecord>().WithMany()
                .HasForeignKey(x => x.PersonId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.TenantId, x.ContextId, x.Status });
        });

        modelBuilder.Entity<ActionPolicyGrantAuthorityRecord>(entity =>
        {
            entity.ToTable("action_policy_grants");
            entity.HasKey(x => new { x.TenantId, x.ContextId, x.SubjectType, x.SubjectId, x.Action });
            entity.Property(x => x.TenantId).HasMaxLength(64);
            entity.Property(x => x.ContextId).HasMaxLength(64);
            entity.Property(x => x.SubjectType).HasMaxLength(32);
            entity.Property(x => x.SubjectId).HasMaxLength(64);
            entity.Property(x => x.Action).HasMaxLength(128);
            entity.Property(x => x.Status).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Version).IsConcurrencyToken();
            entity.HasOne<LearningContextAuthorityRecord>().WithMany()
                .HasForeignKey(x => new { x.TenantId, x.ContextId })
                .HasPrincipalKey(x => new { x.TenantId, x.Id })
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.TenantId, x.ContextId, x.Action, x.Status });
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("outbox_messages");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.MessageType).HasMaxLength(128).IsRequired();
            entity.Property(x => x.AggregateType).HasMaxLength(64).IsRequired();
            entity.Property(x => x.AggregateId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Payload).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(32).IsRequired();
            entity.HasIndex(x => new { x.Status, x.OccurredAt });
        });
    }
}
