using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationPlatform.Infrastructure.Persistence.Migrations;

[Migration("20261001080000_InitialFirstSlice")]
public partial class InitialFirstSlice : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "assignments",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                ContextId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                GoalId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                LearnerId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Work = table.Column<string>(type: "text", nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Version = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_assignments", x => x.Id));

        migrationBuilder.CreateTable(
            name: "submissions",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                AssignmentId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                LearnerId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Payload = table.Column<string>(type: "text", nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Version = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_submissions", x => x.Id));

        migrationBuilder.CreateTable(
            name: "idempotency_records",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                ActorId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                OperationScope = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                IdempotencyKey = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                RequestFingerprint = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                ResourceType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                ResourceId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                ResponseJson = table.Column<string>(type: "text", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_idempotency_records", x => x.Id));

        migrationBuilder.CreateTable(
            name: "audit_records",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                ActorId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                ContextId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Operation = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                CorrelationId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                ResourceType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                ResourceId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                ResultingStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_audit_records", x => x.Id));

        migrationBuilder.CreateTable(
            name: "outbox_messages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                MessageType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                AggregateType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                AggregateId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Payload = table.Column<string>(type: "text", nullable: false),
                OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                PublishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                AttemptCount = table.Column<int>(type: "integer", nullable: false),
                LastError = table.Column<string>(type: "text", nullable: true),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_outbox_messages", x => x.Id));

        migrationBuilder.CreateIndex("IX_assignments_TenantId_ContextId", "assignments", new[] { "TenantId", "ContextId" });
        migrationBuilder.CreateIndex("IX_submissions_TenantId_AssignmentId", "submissions", new[] { "TenantId", "AssignmentId" });
        migrationBuilder.CreateIndex("IX_idempotency_records_TenantId_ActorId_OperationScope_Idempot~", "idempotency_records", new[] { "TenantId", "ActorId", "OperationScope", "IdempotencyKey" }, unique: true);
        migrationBuilder.CreateIndex("IX_audit_records_TenantId_OccurredAt", "audit_records", new[] { "TenantId", "OccurredAt" });
        migrationBuilder.CreateIndex("IX_outbox_messages_Status_OccurredAt", "outbox_messages", new[] { "Status", "OccurredAt" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("outbox_messages");
        migrationBuilder.DropTable("audit_records");
        migrationBuilder.DropTable("idempotency_records");
        migrationBuilder.DropTable("submissions");
        migrationBuilder.DropTable("assignments");
    }
}
