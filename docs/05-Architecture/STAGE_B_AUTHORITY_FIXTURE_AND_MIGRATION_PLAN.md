# Stage B — Authority Fixture and Migration Implementation Plan

**Status:** Implementation plan; no production authority schema is claimed implemented.  
**Approved direction:** platform-owned authority records in the same PostgreSQL transaction boundary as protected first-slice commands.  
**Scope:** the first single-recipient assignment/submission slice only.

## 1. Existing verified boundary

The current EF Core context maps only assignments, submissions, idempotency records, audit records, and outbox messages. Assignment context, goal, and learner identifiers are bounded strings, not foreign keys to authoritative rows. The typed Application command contract exists at `IFirstSliceTransactionalCommand.cs`, but `FirstSliceService` is not wired to it and there is no transactional implementation yet.

The branch has since added concurrent same-key assignment creation and assignment-close coverage. The test compilation fixes are on commit `75eb9cd081b602e2d8236115c6945fec069470ae`; CI run [39454067674](https://github.com/palestiny/education-platform/actions/runs/39454067674) completed successfully, including build, EF migration validation, idempotent migration script generation, and the PostgreSQL test suite. This verifies the latest tested commit, not transactional revocation behavior. The next implementation gate remains the authority schema plus a generated and reviewed EF migration; do not introduce authority entities into the active DbContext without updating the migration and model snapshot in the same change.

## 2. Minimum authority records for the approved first slice

Use platform-owned, tenant-scoped records with stable IDs and lifecycle state:

- **Person** — internal person ID; lifecycle status. External identity-provider subject is not treated as the platform Person ID and provider protocol remains out of scope.
- **LearningContext** — context ID, tenant ID, lifecycle status.
- **LearningGoal** — goal ID, tenant ID, context ID, lifecycle status.
- **ContextualMembership** — tenant ID, context ID, Person ID, membership role/type, lifecycle status, concurrency/version metadata.
- **ActionPolicyGrant** — tenant ID, context ID, role or subject scope, closed action identifier, grant/revocation lifecycle metadata.

Use unique constraints and composite foreign keys/indexes to prevent cross-tenant/context references. Do not store provider access tokens or introduce an identity-provider-specific schema in this slice. Guardian relationships, consent, onboarding, group audience assignments, and physical tenant isolation remain separate design gates.

## 3. Required transactional behavior

For create, submit, close, revoke, membership-disable, and policy-change commands:

1. Start a database transaction before any protected idempotency response is read.
2. Resolve tenant/context/resource authority within trusted server-side scope.
3. Acquire locks in the approved global order: context/tenant rows; relevant membership/lifecycle rows sorted by stable key; relevant policy rows sorted by stable key; target resource; idempotency key/replay resolution; writes.
4. Evaluate current membership lifecycle and explicit action policy inside that transaction. Missing, ambiguous, or unavailable authority fails closed; infrastructure errors must not be translated into ordinary denial.
5. For submission, derive recipient and context from the locked persisted assignment, never request-supplied recipient/context snapshots.
6. Commit mutation, idempotency response, audit, and outbox together.
7. On a duplicate-key race, roll back, open a fresh transaction, reauthorize, then either deny or return a replay. Never return the stored protected response directly from the unique-conflict handler.

Revocation and policy-change commands must lock/update the same authoritative rows using the same relative order. Do not hold authority locks while making external provider/network calls.

## 4. Test-first order

- **A. Test fixture:** add disposable authority rows to the existing PostgreSQL integration harness, with two independently created DbContexts/connections for concurrent actors.
- **B. RED tests:** implement TA-001/002 denial with no writes; TA-003/004 revoke-versus-command lock ordering with explicit barriers; TA-006 replay after revocation; TA-008 same-key races; TA-010 tenant/context mismatch; TA-011 rollback. Preserve the existing assignment-close/submission race coverage.
- **C. Production model:** add EF entity configurations and a reviewed migration with indexes/constraints; do not hand-edit generated migration metadata without validating the EF snapshot and idempotent script.
- **D. Command implementation:** implement one transaction-aware PostgreSQL command adapter; wire Application use cases to it without retaining pre-transaction authorization as the final decision.
- **E. Verification:** run build, migrations validation, idempotent migration script generation, and the full PostgreSQL suite. Inspect the exact commit's CI run before claiming the guarantee is proven.

## 5. Exit criteria and non-claims

This plan is not itself proof of atomic revocation. That claim remains **NOT IMPLEMENTED / NOT PROVEN** until concurrency tests demonstrate real database blocking/commit ordering and pass on the exact final commit.

Do not infer identity-provider selection, credential-validation protocol, guardian/consent rules, onboarding, group assignment behavior, or physical tenant isolation from this plan.

## 6. Exact-head verification checkpoint — 2026-10-20

- Prior docs-only checkpoint `82d20616b2a68c46e4d0c5e211cccb6012c2b079` passed CI run #633.
- The subsequent checkpoint commit `07dfcea1f4f76205545374d1f74b22a5053f4b0b` passed CI run #634.
- CI confirms restore, build, EF migration validation, idempotent migration-script generation, and tests as reported by the repository workflow. It does not add authority tables or change runtime behavior.
- **Next implementation checkpoint remains:** add the PostgreSQL authority fixture and RED concurrency/replay tests, then implement the EF model and migration using the repository's actual .NET/EF tooling. Do not manually claim migration correctness from documentation-only CI.
- PR #5 remains open, draft, and unmerged.

## 7. Exact-head verification checkpoint — 2026-10-20 (revocation replay coverage)

- Added PostgreSQL integration test `Revoked_creation_authorization_cannot_replay_durable_assignment_response`: after a successful assignment creation, the mutable authorizer denies the same request/key; the retry must return `FORBIDDEN` and counts for assignments, idempotency, audit, and outbox remain unchanged.
- Added PostgreSQL integration test `Revoked_submission_authorization_cannot_replay_durable_submission_response`: after a successful submission, the authorizer denies the same request/key; the retry must return `FORBIDDEN` and counts for submissions, idempotency, audit, and outbox remain unchanged.
- Commit: `4858e7b50c70c928db5ff88a46aeec8288854623`.
- CI run [39455136254](https://github.com/palestiny/education-platform/actions/runs/39455136254) completed successfully. This includes restore/build, EF migration validation, idempotent migration script generation, and PostgreSQL integration tests.
- These tests prove the current Application service invokes its authorizer before reaching the persistence replay path for assignment creation and submission. They use a mutable test authorizer; they do **not** prove that authoritative membership/policy state is locked in the same database transaction, nor do they prove revocation-versus-command serialization.
- Next implementation remains the PostgreSQL authority entities and reviewed EF-generated migration, followed by the transaction-aware command adapter and true two-connection revoke-versus-command tests. Atomic revocation remains **NOT IMPLEMENTED / NOT PROVEN**.

## 8. Authority persistence foundation checkpoint — 2026-10-20

- Added persistence records for platform-owned Person, LearningContext, LearningGoal, ContextualMembership, and ActionPolicyGrant in `AuthorityRecords.cs`.
- Added EF mappings and migration `20261020170000_AddPlatformAuthorityRecords`, including context-scoped foreign keys for goals, memberships, and policy grants, and a Person foreign key for memberships.
- Added PostgreSQL integration coverage that proves a learning goal cannot reference a context under another tenant and a membership cannot reference a missing Person.
- The integration fixture now clears authority tables in dependent-first order to keep test isolation.
- Exact-head CI for commit `201f3ea655048f967663d65ef31a7bf7d7def56c`: [run 39457015087](https://github.com/palestiny/education-platform/actions/runs/39457015087). Restore, build, EF migration validation, idempotent migration script generation, and the PostgreSQL test suite all completed successfully.
- **Important boundary:** these are authority persistence records and relational integrity constraints only. The current first-slice service/store still does not read or lock these authority rows as part of protected mutations. Assignments do not yet have foreign keys to the authority records, and the transactional command contract is not wired. This checkpoint therefore does not prove atomic revocation, policy-change serialization, or authority-backed assignment eligibility.
- Next: add fixture helpers to seed authoritative Person/context/goal/membership/grant rows; implement one PostgreSQL transaction-aware command adapter that locks and checks them before replay resolution; wire all three Application commands to that adapter; then prove revoke-vs-create/submit/close behavior with independent DbContexts and explicit transaction barriers. Keep the current store path only as a deliberate compatibility/test path, not as the production authorization boundary.

## 9. Current implementation checkpoint — 2026-10-20 (authority schema exists; command boundary still missing)

Verified against branch head `bb143f276c33cda46890a54db6b2aeeb9a4cb6cd` and PR #5:

- The authority tables, EF mappings, migration, and relational-integrity tests are present; the prior checkpoint wording in sections 4 and 8 that says the migration is still a future step is historical and superseded by section 8's completed migration checkpoint.
- `IFirstSliceTransactionalCommand` and its typed command records exist, but `FirstSliceService` still authorizes through separate Application ports and then calls `IFirstSliceStore`. `Program.cs` registers `PostgresFirstSliceStore`, not a transaction-aware command adapter.
- Production authorization adapters remain fail-closed. This is safer than allowing requests through, but means the production API is not yet backed by the newly added authority records.
- Existing store replay paths can read a durable idempotency response before any authorization check inside the same PostgreSQL transaction. The Application pre-check helps ordinary retries but does not satisfy the approved A1 transaction boundary.
- No revocation command/writer currently enforces the same context-row lock protocol. Until every authority mutation follows that protocol, a context lock in a protected command alone would not prove serialization.

### Required next implementation slice

1. Add deterministic PostgreSQL fixture builders for Person, tenant-scoped LearningContext, LearningGoal, membership, and explicit action grants.
2. Implement the PostgreSQL transaction-aware command adapter, including a common context lock protocol and final checks before idempotency replay. Do not silently reuse the existing store methods as if they were already safe.
3. Route the three protected use cases through that adapter in production DI; preserve a clearly bounded test/in-memory path where needed.
4. Add two-connection tests with transaction barriers for revoke-versus-create, revoke-versus-submit, revoke-versus-close, and replay-after-revocation; include same-key unique-conflict recovery.
5. Add/verify authority revocation writers using the same lock order, then run the complete CI workflow on the exact final commit.

**Status:** authority schema = implemented and CI-verified; transaction-aware command wiring = not implemented; atomic revocation = **NOT PROVEN**. PR #5 remains open, draft, and unmerged.

## 10. Documentation checkpoint CI result — 2026-10-20

- Commit `2e5fcdbc9e8c60fcb02eea78dc3d0834a4285ea9` passed GitHub Actions run [39459467470](https://github.com/palestiny/education-platform/actions/runs/39459467470).
- This run validates the repository's existing restore/build/migration/test workflow for the documentation checkpoint. It does **not** validate a transaction-aware authority command because no such adapter has been implemented in this commit.
- The next code change must be runtime behavior plus tests, not another documentation-only completion claim. The acceptance criteria remain: authority checks and idempotency replay inside the same PostgreSQL transaction; all protected commands routed through that boundary; revocation writers share its lock protocol; independent-connection concurrency tests pass on the exact head.

## 10. First transaction-aware command implementation — 2026-10-20

Commit `5c162d9` adds `PostgresFirstSliceTransactionalCommand`, and commit `37ffc108` wires it into production composition. The Application service delegates creation, submission, and close to the typed command when it is registered; existing test/in-memory composition retains the prior constructor path.

The adapter currently:
- Locks the tenant-scoped learning-context row before final authority checks.
- Requires an active context, active Person records, active contextual membership, and an explicit active `PERSON` action grant.
- Requires an active goal in the same tenant/context for assignment creation.
- Locks and re-reads the assignment before submission/close; submission rejects the wrong recipient and closed assignments.
- Performs current authority checks before idempotency replay lookup.
- Persists mutation, idempotency result, audit record, and outbox message in one database transaction.

**Not yet proven / follow-up required:**
- The new implementation has not yet passed CI on its current head at the time of writing.
- No dedicated two-connection concurrency tests have been added for revocation races or same-key replay conflicts.
- There is not yet a platform authority-revocation writer that takes the same context lock. Therefore the adapter's context lock does not by itself prove revocation serialization; atomic revocation remains **NOT PROVEN** until all authority writers follow this lock protocol and independent-connection tests pass.
- Production authorization uses the authority records directly; without provisioned authority rows it fails closed. The production credential-to-Person provisioning path remains out of scope.

## 11. Revocation writer and independent-connection regression — 2026-10-20

This section supersedes the earlier "no revocation writer" statement in section 10:

- Added `PostgresAuthorityRevocationWriter` for action-grant and membership revocation. It takes the same tenant/context `FOR UPDATE` lock before changing authority state and commits the change in its own transaction.
- Updated the assignment-creation replay test to revoke through that writer and verify a durable response is not returned after revocation.
- Added an independent-connection PostgreSQL test that holds the context lock while revoking a grant, starts assignment creation from another connection, commits the revocation, and asserts creation fails without assignment/idempotency/audit/outbox writes.
- CI for current head `14f881be82fc0ba1ae077a51baa38262d088845e` is still running at the time of this checkpoint: https://github.com/palestiny/education-platform/actions/runs/39461404030

The shared lock protocol is now implemented for the protected commands and these two authority-revocation methods. **Do not call atomic revocation fully proven until the current CI passes and all future context, membership, grant, and lifecycle mutation paths are required to use this writer/lock protocol.** Same-key race recovery and broader concurrency coverage remain follow-up work.

