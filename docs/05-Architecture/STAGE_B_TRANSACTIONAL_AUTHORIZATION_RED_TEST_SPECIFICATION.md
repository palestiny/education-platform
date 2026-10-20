# Stage B — Transactional Authorization RED Test Specification

**Status:** PARTIALLY IMPLEMENTED — exact-head CI and complete concurrency proof remain required  
**Date:** 2026-10-20  
**Approved design:** A1 typed transaction-aware persistence command  
**Related:** `STAGE_B_TRANSACTIONAL_AUTHORIZATION_DESIGN_GATE.md`, `STAGE_B_REVOCATION_CONSISTENCY_FEASIBILITY.md`, `STAGE_B_IDENTITY_MEMBERSHIP_CONTRACT_TEST_PLAN.md`

## 1. Purpose and scope

Specify the smallest test-first increment that proves platform-owned membership/lifecycle authorization is serialized with protected PostgreSQL mutations. These tests are not proof of production identity-provider validation, consent policy, onboarding, or physical tenant isolation.

Do not create membership tables or production adapters before the initial command contract and test harness are reviewed. The current repository has no authoritative durable membership/policy model, so concurrency scenarios require a deliberately small test-owned authority fixture or schema proposal, not assumptions about a finished production schema.

## 2. Current implementation status and remaining proof obligations

The branch now contains platform authority records and an EF migration, a typed `IFirstSliceTransactionalCommand`, a PostgreSQL implementation that locks the tenant-scoped learning context before authority checks and idempotency replay, and a revocation writer that takes the same context lock. Production DI registers the transactional command when PostgreSQL is configured outside Development/Testing.

This is implementation evidence, not a blanket proof of atomic revocation. The required remaining checks include:
- exact-head CI for the latest test additions;
- deterministic two-connection barriers proving both lock orderings, rather than relying on task scheduling;
- every future membership, grant, person, context, and goal lifecycle writer following a documented common lock order;
- same-key behavior under concurrent conflict/deadlock, with retries restarting in a fresh transaction and reauthorizing;
- explicit, tested idempotency scope across contexts and tenants.

Legacy Application authorizers remain in the service composition for the non-transactional compatibility path. Production guarantees must be assessed against the actual production DI path and transaction-aware command, not the compatibility path.

## 3. Test harness rules

- Use PostgreSQL integration tests and at least two independent DbContext/connection instances for concurrency schedules.
- Use explicit synchronization barriers in tests (not arbitrary sleeps) to pause one transaction after acquiring the intended lock and before commit.
- Test real database blocking/commit ordering, not only mocked authorizer call order.
- Every test starts from isolated tenant/context fixtures and asserts final persisted state after both transactions complete.
- Each negative test asserts no unintended mutation, successful idempotency result, audit-success record, or outbox event was committed.
- Keep provider-neutral command contracts in Application; keep EF/PostgreSQL details in Infrastructure.
- Tests should be authored RED against the missing contract before implementation. Do not weaken the assertions to make the current pre-transaction authorizer appear atomic.
- If the branch's CI policy does not permit a knowingly red default PR, land a compiling test-contract/helper layer first and stage failing behavioral tests on the agreed test branch; do not merge a red PR.

## 4. First required RED suite

### TA-001 — Creation denied in transaction

Given a valid trusted actor and otherwise valid assignment data, with the actor's contextual membership inactive or lacking `assignment.create`, when the transaction-aware create command executes, then it returns a typed denial and commits no assignment, successful idempotency record, success audit, or outbox event.

### TA-002 — Submission denied in transaction

Given an assignment and a recipient whose contextual membership is revoked or lacks `submission.create`, when submission executes, then it is denied in the transaction and creates no submission or success reliability records.

### TA-003 — Revocation wins before protected command

Transaction R locks and revokes the authoritative membership row and commits. Command C starts/continues afterward. C must observe the inactive lifecycle state and be denied with no protected side effects.

### TA-004 — Protected command wins lock before revocation

Command C obtains the authoritative membership lock, validates policy and performs the protected mutation. Transaction R attempting to revoke the same membership must block until C commits. The final order must be C then R, and the test must demonstrate the blocking with independent connections/barriers.

### TA-005 — Assignment closure versus submission

Preserve the current assignment-row locking guarantee while extending the same transaction to include membership/policy checks. If close commits first, submission is rejected. If submission owns the documented lock order first, it may commit before close; no stale preloaded assignment snapshot may decide the outcome.

### TA-006 — Replay after membership revocation

A first command succeeds and commits its idempotency response. Membership is then revoked. Repeating the same command/key must reauthorize within the command transaction and deny without returning the protected saved response. Counts of assignment/submission, idempotency, audit, and outbox records remain unchanged by the retry.

### TA-007 — Idempotency fingerprint mismatch after denial

When the actor no longer has authority and supplies a key that has an existing response with a mismatched fingerprint, current authorization must be evaluated first. The caller receives the safe denial outcome rather than a key-conflict response that could disclose protected key usage. No writes occur.

### TA-008 — Concurrent same-key requests

Two independent commands with the same tenant, actor, operation, and idempotency key race. Exactly one protected mutation and one set of success records may commit. The losing path must roll back, start a fresh transaction, reauthorize, and only then return a replay or a safe denial. No direct replay from an exception handler without reauthorization.

### TA-009 — Indeterminate authority

If the transaction-local authority data is missing, ambiguous, or cannot be evaluated safely, command fails closed with the API's unavailable/indeterminate mapping (503 where applicable). No protected side effects commit. Database infrastructure failures must not be mislabeled as ordinary policy denial.

### TA-010 — Cross-tenant/context mismatch

An actor's membership in context A cannot authorize a resource/goal/recipient in context B or another tenant. No protected row or response payload is disclosed and no mutation/reliability success records are committed.

### TA-011 — Atomic mutation and reliability records

Inject a failure after the domain mutation is staged and before commit. Mutation, idempotency success result, audit, and outbox all roll back together. A retry after rollback can execute exactly once.

### TA-012 — Cancellation, deadlock, and bounded retry

Cancellation before commit rolls back. A classified transient database failure may be retried only a bounded number of times, each attempt in a new transaction and with a fresh authority check. Denial, validation errors, and fingerprint conflicts are not retried. Exhaustion maps to a safe unavailable outcome.

## 5. Global lock-order assertions

The tests and implementation must verify this common relative order across create, submit, close, revoke, membership-disable, and policy-change commands:

1. tenant/context authority rows needed by the command;
2. relevant membership/lifecycle rows, in deterministic stable-key order;
3. relevant action-policy rows, in deterministic stable-key order;
4. target resource rows (assignment/goal as applicable);
5. idempotency key/replay resolution after current authorization;
6. mutation plus idempotency/audit/outbox writes and commit.

Creation locks and validates the goal/context/recipient authority rows needed to preserve eligibility. Revocation and policy-change commands lock/update the same authority rows and follow the same relative ordering. The final schema may refine exact row types, but any change to the global order requires an explicit deadlock analysis and concurrency tests before implementation.

## 6. Required API/contract assertions

- Denied maps to 403; indeterminate/unavailable maps to 503; do not expose membership existence or provider diagnostics.
- Tenant and actor/Person IDs come from trusted server-side execution context, never request-body authority claims.
- Submission recipient and context are resolved from persisted assignment data, not client-supplied snapshots.
- Action intent is typed/closed at the command boundary; do not let the client choose an arbitrary policy name.
- No positive authorization cache.
- No external network/provider call while database authority locks are held.
- No successful protected replay before transaction-local reauthorization.

## 7. Existing repository harness — verified implementation map

The RED work must extend the existing PostgreSQL test harness rather than create a parallel testing framework:

- `tests/EducationPlatform.IntegrationTests/PostgresFirstSliceStoreTests.cs` is the starting point for TA-001/002/005/006/008/009/010/011/012. It uses `EDUCATION_PLATFORM_TEST_CONNECTION`, applies EF migrations, and disables parallel execution for the PostgreSQL collection.
- `CreateFreshDatabase()` clears the five current first-slice tables and returns one DbContext. Concurrency tests must create **two separately constructed contexts/connections** from the configured connection string; never share one DbContext across concurrent actors.
- `CreateContext(string connection)` constructs an independent Npgsql-backed DbContext and can be reused as the basis for a two-connection fixture.
- Existing audit/outbox failure tests already demonstrate database-enforced rollback; preserve that coverage when adding authority tables.
- `src/EducationPlatform.Application/FirstSlice/IFirstSliceStore.cs` currently exposes separate create/submit/close calls and has no transaction-aware authorization command. `FirstSliceService` invokes creation/submission authorizers before calling the store.
- `src/EducationPlatform.Infrastructure/Persistence/PostgresFirstSliceStore.cs` performs the idempotency lookup before beginning the mutation transaction. Its unique-conflict recovery must not be reused as a protected replay path without restarting authorization in a fresh transaction.
- `src/EducationPlatform.Infrastructure/Persistence/EducationPlatformDbContext.cs` now maps the platform-owned authority records in addition to first-slice persistence entities; migration `20261020170000_AddPlatformAuthorityRecords` adds the corresponding tables and tenant/context constraints. Keep schema snapshot and migration validation in CI.
- The integration test project targets .NET 10 and uses xUnit v3 plus Npgsql via the Infrastructure project. Existing CI runs PostgreSQL, build, EF migration validation, idempotent migration-script generation, and tests.

**Sequencing guard:** introduce the provider-neutral typed command contract and a deterministic PostgreSQL authority fixture first; then add concurrency tests using explicit barriers and separate contexts. Only after RED behavior is observable should production authority mappings/migration and the command implementation be added. A compiling mock-only test is not proof of transaction serialization.

## 7. Exit criteria

The RED specification becomes implementation-ready when:
1. command types and typed decision/error outcomes are named in the design contract;
2. a minimal test authority fixture is approved without implying a final production schema;
3. tests TA-001 through TA-012 are mapped to unit/integration/concurrency layers;
4. the concurrency harness demonstrates deterministic barriers and separate PostgreSQL connections;
5. expected CI handling for RED is explicit;
6. no production schema/provider/onboarding choice is smuggled into the test fixture.

The implementation gate passes only when tests are GREEN on the exact latest commit and CI proves the real PostgreSQL serialization cases with deterministic synchronization. Current transactional code and revocation writer exist, but complete atomic-revocation behavior remains **NOT PROVEN** until lock-order coverage and exact-head verification pass.
