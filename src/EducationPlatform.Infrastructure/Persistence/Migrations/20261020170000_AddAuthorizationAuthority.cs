using EducationPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EducationPlatform.Infrastructure.Persistence.Migrations;

/// <summary>
/// Creates the platform-owned authorization authority tables for the first single-recipient
/// assignment/submission slice. Provider-specific identity credentials are intentionally excluded.
/// </summary>
[DbContext(typeof(EducationPlatformDbContext))]
[Migration("20261020170000_AddAuthorizationAuthority")]
public sealed class AddAuthorizationAuthority : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE TABLE people (
                tenant_id varchar(64) NOT NULL,
                id varchar(64) NOT NULL,
                status varchar(32) NOT NULL,
                created_at timestamptz NOT NULL DEFAULT now(),
                updated_at timestamptz NOT NULL DEFAULT now(),
                CONSTRAINT pk_people PRIMARY KEY (tenant_id, id),
                CONSTRAINT ck_people_status CHECK (status IN ('ACTIVE', 'INACTIVE', 'SUSPENDED'))
            );

            CREATE TABLE learning_contexts (
                tenant_id varchar(64) NOT NULL,
                id varchar(64) NOT NULL,
                status varchar(32) NOT NULL,
                created_at timestamptz NOT NULL DEFAULT now(),
                updated_at timestamptz NOT NULL DEFAULT now(),
                CONSTRAINT pk_learning_contexts PRIMARY KEY (tenant_id, id),
                CONSTRAINT ck_learning_contexts_status CHECK (status IN ('ACTIVE', 'INACTIVE', 'ARCHIVED'))
            );

            CREATE TABLE learning_goals (
                tenant_id varchar(64) NOT NULL,
                context_id varchar(64) NOT NULL,
                id varchar(64) NOT NULL,
                status varchar(32) NOT NULL,
                created_at timestamptz NOT NULL DEFAULT now(),
                updated_at timestamptz NOT NULL DEFAULT now(),
                CONSTRAINT pk_learning_goals PRIMARY KEY (tenant_id, context_id, id),
                CONSTRAINT fk_learning_goals_context FOREIGN KEY (tenant_id, context_id)
                    REFERENCES learning_contexts (tenant_id, id) ON DELETE RESTRICT,
                CONSTRAINT ck_learning_goals_status CHECK (status IN ('ACTIVE', 'INACTIVE', 'ARCHIVED'))
            );

            CREATE TABLE contextual_memberships (
                tenant_id varchar(64) NOT NULL,
                context_id varchar(64) NOT NULL,
                person_id varchar(64) NOT NULL,
                role varchar(64) NOT NULL,
                status varchar(32) NOT NULL,
                is_eligible boolean NOT NULL DEFAULT true,
                version bigint NOT NULL DEFAULT 1,
                created_at timestamptz NOT NULL DEFAULT now(),
                updated_at timestamptz NOT NULL DEFAULT now(),
                CONSTRAINT pk_contextual_memberships PRIMARY KEY (tenant_id, context_id, person_id, role),
                CONSTRAINT fk_contextual_memberships_context FOREIGN KEY (tenant_id, context_id)
                    REFERENCES learning_contexts (tenant_id, id) ON DELETE RESTRICT,
                CONSTRAINT fk_contextual_memberships_person FOREIGN KEY (tenant_id, person_id)
                    REFERENCES people (tenant_id, id) ON DELETE RESTRICT,
                CONSTRAINT ck_contextual_memberships_status CHECK (status IN ('ACTIVE', 'INACTIVE', 'REVOKED')),
                CONSTRAINT ck_contextual_memberships_version CHECK (version > 0)
            );

            CREATE INDEX ix_contextual_memberships_person_status
                ON contextual_memberships (tenant_id, person_id, status);
            CREATE INDEX ix_contextual_memberships_context_status
                ON contextual_memberships (tenant_id, context_id, status);

            CREATE TABLE action_policy_grants (
                tenant_id varchar(64) NOT NULL,
                context_id varchar(64) NOT NULL,
                role varchar(64) NOT NULL,
                action varchar(128) NOT NULL,
                status varchar(32) NOT NULL,
                version bigint NOT NULL DEFAULT 1,
                created_at timestamptz NOT NULL DEFAULT now(),
                updated_at timestamptz NOT NULL DEFAULT now(),
                CONSTRAINT pk_action_policy_grants PRIMARY KEY (tenant_id, context_id, role, action),
                CONSTRAINT fk_action_policy_grants_context FOREIGN KEY (tenant_id, context_id)
                    REFERENCES learning_contexts (tenant_id, id) ON DELETE RESTRICT,
                CONSTRAINT ck_action_policy_grants_status CHECK (status IN ('ACTIVE', 'REVOKED')),
                CONSTRAINT ck_action_policy_grants_version CHECK (version > 0),
                CONSTRAINT ck_action_policy_grants_action CHECK (action IN ('assignment.create', 'submission.create', 'assignment.close'))
            );

            CREATE INDEX ix_action_policy_grants_context_status
                ON action_policy_grants (tenant_id, context_id, status);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DROP TABLE IF EXISTS action_policy_grants;
            DROP TABLE IF EXISTS contextual_memberships;
            DROP TABLE IF EXISTS learning_goals;
            DROP TABLE IF EXISTS learning_contexts;
            DROP TABLE IF EXISTS people;
            """);
    }
}
