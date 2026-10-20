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
