using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationPlatform.Infrastructure.Persistence.Migrations;

[Migration("20261020170000_AddPlatformAuthorityRecords")]
public partial class AddPlatformAuthorityRecords : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "persons",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Version = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_persons", x => x.Id));

        migrationBuilder.CreateTable(
            name: "learning_contexts",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Version = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_learning_contexts", x => x.Id);
                table.UniqueConstraint("AK_learning_contexts_TenantId_Id", x => new { x.TenantId, x.Id });
            });

        migrationBuilder.CreateTable(
            name: "learning_goals",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                ContextId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Version = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_learning_goals", x => x.Id);
                table.UniqueConstraint("AK_learning_goals_TenantId_ContextId_Id", x => new { x.TenantId, x.ContextId, x.Id });
                table.ForeignKey(
                    name: "FK_learning_goals_learning_contexts_TenantId_ContextId",
                    columns: x => new { x.TenantId, x.ContextId },
                    principalTable: "learning_contexts",
                    principalColumns: new[] { "TenantId", "Id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "contextual_memberships",
            columns: table => new
            {
                TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                ContextId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                PersonId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                MembershipType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Version = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_contextual_memberships", x => new { x.TenantId, x.ContextId, x.PersonId, x.MembershipType });
                table.ForeignKey(
                    name: "FK_contextual_memberships_learning_contexts_TenantId_ContextId",
                    columns: x => new { x.TenantId, x.ContextId },
                    principalTable: "learning_contexts",
                    principalColumns: new[] { "TenantId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_contextual_memberships_persons_PersonId",
                    column: x => x.PersonId,
                    principalTable: "persons",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "action_policy_grants",
            columns: table => new
            {
                TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                ContextId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                SubjectType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                SubjectId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Action = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Version = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_action_policy_grants", x => new { x.TenantId, x.ContextId, x.SubjectType, x.SubjectId, x.Action });
                table.ForeignKey(
                    name: "FK_action_policy_grants_learning_contexts_TenantId_ContextId",
                    columns: x => new { x.TenantId, x.ContextId },
                    principalTable: "learning_contexts",
                    principalColumns: new[] { "TenantId", "Id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_action_policy_grants_TenantId_ContextId_Action_Status",
            table: "action_policy_grants",
            columns: new[] { "TenantId", "ContextId", "Action", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_contextual_memberships_PersonId",
            table: "contextual_memberships",
            column: "PersonId");

        migrationBuilder.CreateIndex(
            name: "IX_contextual_memberships_TenantId_ContextId_Status",
            table: "contextual_memberships",
            columns: new[] { "TenantId", "ContextId", "Status" });

        migrationBuilder.CreateIndex(
            name: "IX_learning_goals_TenantId_ContextId",
            table: "learning_goals",
            columns: new[] { "TenantId", "ContextId" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "action_policy_grants");
        migrationBuilder.DropTable(name: "contextual_memberships");
        migrationBuilder.DropTable(name: "learning_goals");
        migrationBuilder.DropTable(name: "persons");
        migrationBuilder.DropTable(name: "learning_contexts");
    }
}
