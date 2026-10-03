# First Slice Durable Persistence Boundary

**Date:** 2026-10-01  
**Status:** IMPLEMENTATION IN PROGRESS

## Decision

The first slice will use a dedicated EducationPlatform.Infrastructure project for durable PostgreSQL persistence. Domain and Application remain persistence-provider independent; the API composes the infrastructure implementation.

The PostgreSQL provider is Npgsql through EF Core 10. The repository currently targets .NET 10, and the selected stable package lines are EF Core 10.0.12 and Npgsql EF Core provider 10.0.3.

## Boundary

- Domain: authoritative business objects and invariants.
- Application: use cases and store/module contracts.
- Infrastructure: EF Core mappings, PostgreSQL persistence, transactions, idempotency, audit and outbox persistence.
- API: transport/authentication composition only.

## First durable records

The initial persistence implementation starts with:
- assignments
- submissions

Reliability records are intentionally the next persistence increment:
- idempotency records
- audit records
- outbox messages

## Rules

1. No application service depends directly on EF Core.
2. Tenant ownership remains explicit on authoritative records.
3. Assignment version is mapped as an optimistic concurrency token.
4. Idempotency must become durable and unique at the database boundary before the in-memory adapter is removed.
5. Audit and outbox writes must share the authoritative transaction where required.
6. EnsureCreated is not the production migration strategy.
7. Integration tests must exercise PostgreSQL behavior rather than replacing the provider with an in-memory database.

## Verification boundary

The current Infrastructure project and DbContext establish the persistence boundary only. They do not constitute durable first-slice completion. The API still uses the temporary in-memory adapter until the transactional PostgreSQL adapter and failure-mode tests are proven in CI.


## Durable Mutation Contract — 2026-10-01

The first-slice persistence boundary now treats a critical mutation as one atomic operation rather than separate application-level writes.

For assignment/submission creation, the durable adapter is responsible for the transaction containing:
1. authoritative resource creation;
2. idempotency reservation/result persistence when a key is supplied;
3. audit record creation;
4. outbox message creation.

The database uniqueness constraint on `(tenant_id, actor_id, operation_scope, idempotency_key)` is part of the correctness mechanism. A concurrent unique-key race is reconciled to the already committed idempotent result rather than creating a second authoritative resource.

The Application layer remains provider-independent. The first-slice service computes semantic request fingerprints and delegates the atomic persistence boundary through the store contract. PostgreSQL-specific transaction and constraint handling remains in Infrastructure.

### Verification status

- PostgreSQL EF model: IMPLEMENTED.
- PostgreSQL atomic store: IMPLEMENTED.
- Durable idempotency/audit/outbox persistence: IMPLEMENTED.
- Audit context attribution: IMPLEMENTED.
- PostgreSQL CI service: IMPLEMENTED.
- PostgreSQL migrations apply successfully in CI: VERIFIED.
- PostgreSQL integration tests: VERIFIED.
- Durable API end-to-end persistence test: VERIFIED.
- Latest GitHub Actions verification: **PASS — 15 tests, 0 failed, 0 skipped** (run #203).
- Initial EF migration metadata/index naming was corrected after CI exposed an Npgsql identifier-length mismatch; the final migration/snapshot pair now matches the provider-generated model.
- Production migration deployment strategy: VERIFIED in CI; production execution remains deployment-controlled and is not claimed as executed.
- Optimistic concurrency behavior/API expected-version precondition: VERIFIED in CI.
- Atomic rollback on audit persistence failure: VERIFIED in CI.
- Atomic rollback on outbox persistence failure: VERIFIED in CI.
- Production migration deployment strategy: NOT YET CLOSED.

Therefore the Persistence Gate is now **PASS — VERIFIED in CI** for the first-slice persistence boundary. Production database execution itself remains an operational deployment responsibility, not a repository-test claim.


## Production Migration / Deployment Closure — 2026-10-04

The production migration strategy is explicitly deployment-controlled and externally executed.

### Strategy

**Expand → Compatible Deploy → Migrate/Backfill → Switch → Contract/Remove**

1. **Expand:** add only backward-compatible schema changes and supporting indexes/columns.
2. **Compatible Deploy:** deploy application code that can operate against both the previous and expanded schema where required.
3. **Migrate/Backfill:** execute the versioned EF Core migration as a dedicated deployment step/job, outside normal application startup. Run materially large backfills separately.
4. **Switch:** enable behavior requiring the expanded schema only after migration/backfill verification.
5. **Contract/Remove:** remove obsolete schema/code only in a later deployment after compatibility is no longer required.

### Production rules

- Production must not rely on `EnsureCreated()`.
- Production must not depend on every API instance running schema migrations during startup.
- The deployment pipeline owns migration execution and records the migration result.
- Application startup may validate compatibility, but must not silently mutate production schema.
- Destructive or breaking migrations require a separately reviewed deployment step and rollback/forward-fix plan.
- Long-running data backfills are not hidden inside ordinary EF migrations.
- The migration artifact is generated and reviewed from the exact repository version being deployed.

### CI controls

The repository workflow now validates the migration boundary by installing the pinned `dotnet-ef` 10.0.12 CLI, running `dotnet ef migrations has-pending-model-changes`, generating an idempotent migration SQL script, and uploading that script as a workflow artifact before the PostgreSQL integration suite runs.

This proves model/migration alignment and that a deployment-consumable idempotent script can be generated. It does **not** claim that production has been migrated; production execution remains a deployment responsibility.

### Gate status

Production migration/deployment strategy: **DEFINED + CI-VERIFIED**.

The Persistence Gate is **PASS** based on GitHub Actions run #251, which successfully executed migration validation, generated the idempotent deployment script, uploaded the artifact, and completed the PostgreSQL-backed test suite.