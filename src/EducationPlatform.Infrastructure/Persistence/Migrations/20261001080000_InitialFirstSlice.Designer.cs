using EducationPlatform.Domain.FirstSlice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EducationPlatform.Infrastructure.Persistence.Migrations;

[DbContext(typeof(EducationPlatformDbContext))]
partial class InitialFirstSlice
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.12");

        modelBuilder.Entity<Assignment>(b =>
        {
            b.Property<string>("Id").HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<string>("ContextId").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<string>("GoalId").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<string>("LearnerId").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<string>("Status").IsRequired().HasMaxLength(32).HasColumnType("character varying(32)");
            b.Property<string>("TenantId").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<int>("Version").IsConcurrencyToken().HasColumnType("integer");
            b.Property<string>("Work").IsRequired().HasColumnType("text");
            b.HasKey("Id");
            b.HasIndex("TenantId", "ContextId");
            b.ToTable("assignments");
        });

        modelBuilder.Entity<Submission>(b =>
        {
            b.Property<string>("Id").HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<string>("AssignmentId").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<string>("LearnerId").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<string>("Payload").IsRequired().HasColumnType("text");
            b.Property<string>("Status").IsRequired().HasMaxLength(32).HasColumnType("character varying(32)");
            b.Property<string>("TenantId").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<int>("Version").HasColumnType("integer");
            b.HasKey("Id");
            b.HasIndex("TenantId", "AssignmentId");
            b.ToTable("submissions");
        });

        modelBuilder.Entity<IdempotencyRecord>(b =>
        {
            b.Property<Guid>("Id").HasColumnType("uuid");
            b.Property<string>("ActorId").IsRequired().HasMaxLength(128).HasColumnType("character varying(128)");
            b.Property<DateTimeOffset>("CreatedAt").HasColumnType("timestamp with time zone");
            b.Property<string>("IdempotencyKey").IsRequired().HasMaxLength(256).HasColumnType("character varying(256)");
            b.Property<string>("OperationScope").IsRequired().HasMaxLength(128).HasColumnType("character varying(128)");
            b.Property<string>("RequestFingerprint").IsRequired().HasMaxLength(128).HasColumnType("character varying(128)");
            b.Property<string>("ResourceId").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<string>("ResourceType").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<string>("ResponseJson").IsRequired().HasColumnType("text");
            b.Property<string>("TenantId").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.HasKey("Id");
            b.HasIndex("TenantId", "ActorId", "OperationScope", "IdempotencyKey").HasDatabaseName("IX_idempotency_records_TenantId_ActorId_OperationScope_Idempot~").IsUnique();
            b.ToTable("idempotency_records");
        });

        modelBuilder.Entity<AuditRecord>(b =>
        {
            b.Property<Guid>("Id").HasColumnType("uuid");
            b.Property<string>("ActorId").IsRequired().HasMaxLength(128).HasColumnType("character varying(128)");
            b.Property<string>("CorrelationId").IsRequired().HasMaxLength(128).HasColumnType("character varying(128)");
            b.Property<DateTimeOffset>("OccurredAt").HasColumnType("timestamp with time zone");
            b.Property<string>("Operation").IsRequired().HasMaxLength(128).HasColumnType("character varying(128)");
            b.Property<string>("ResourceId").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<string>("ResourceType").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<string>("ResultingStatus").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<string>("TenantId").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.HasKey("Id");
            b.HasIndex("TenantId", "OccurredAt");
            b.ToTable("audit_records");
        });

        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.Property<Guid>("Id").HasColumnType("uuid");
            b.Property<int>("AttemptCount").HasColumnType("integer");
            b.Property<string>("AggregateId").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<string>("AggregateType").IsRequired().HasMaxLength(64).HasColumnType("character varying(64)");
            b.Property<string>("LastError").HasColumnType("text");
            b.Property<string>("MessageType").IsRequired().HasMaxLength(128).HasColumnType("character varying(128)");
            b.Property<DateTimeOffset>("OccurredAt").HasColumnType("timestamp with time zone");
            b.Property<string>("Payload").IsRequired().HasColumnType("text");
            b.Property<DateTimeOffset?>("PublishedAt").HasColumnType("timestamp with time zone");
            b.Property<string>("Status").IsRequired().HasMaxLength(32).HasColumnType("character varying(32)");
            b.HasKey("Id");
            b.HasIndex("Status", "OccurredAt");
            b.ToTable("outbox_messages");
        });
    }
}
