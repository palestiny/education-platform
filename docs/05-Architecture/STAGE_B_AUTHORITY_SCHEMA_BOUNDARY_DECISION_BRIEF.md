# Stage B — Authority Schema Boundary Decision Brief

**Status:** OWNER-APPROVED IMPLEMENTATION DIRECTION — schema/code not yet implemented  
**Decision date:** 2026-10-20  
**Owner decision:** Option A approved. The platform owns and persists tenant-scoped learning contexts and learning goals in the same PostgreSQL transaction authority as membership, policy, and protected commands.  
**Previously approved:** platform-owned Person mapping and contextual membership lifecycle; A1 typed transaction-aware persistence command.  
**Related:** `STAGE_B_TRANSACTIONAL_AUTHORIZATION_DESIGN_GATE.md`, `STAGE_B_TRANSACTIONAL_AUTHORIZATION_RED_TEST_SPECIFICATION.md`, `STAGE_B_REVOCATION_CONSISTENCY_FEASIBILITY.md`

## 1. Decision and scope

The owner approved **Option A**. Do not reopen this decision unless implementation discovers a concrete contradiction. The authority will be platform-owned and relational, with all final authorization checks and protected mutations using one PostgreSQL transaction boundary.

This approval chooses ownership and transaction topology. It does **not** choose an identity provider, credential verification protocol, onboarding UX, guardian/consent policy, final table names/columns, or physical tenant isolation.

## 2. Minimum authority model to implement

Implement these provider-neutral records in the platform's PostgreSQL authority. Refine exact EF types, keys, constraints, and migration details against existing code before committing:

- **Person:** stable internal Person ID. External identity linkage must use a provider-neutral issuer + subject pair with a uniqueness constraint. Never treat client-supplied IDs or unverified claims as authenticated identity.
- **LearningContext:** stable context ID scoped to a tenant, with lifecycle state/version. A context is not interchangeable across tenants.
- **LearningGoal:** stable goal ID scoped to a tenant and one learning context, with lifecycle/eligibility state/version. A goal cannot be eligible in a different context merely because its ID is supplied.
- **ContextualMembership:** Person + tenant + context, membership/eligibility lifecycle state, and concurrency version. Enforce uniqueness for the intended active membership identity and define lifecycle transitions explicitly.
- **ActionPolicyGrant:** membership-scoped typed action code (initially `assignment.create` and `submission.create`), lifecycle state/version, and uniqueness rules preventing ambiguous duplicate active grants.
- **Assignment linkage:** assignment creation must verify the actor's eligible membership and `assignment.create` grant, the recipient's eligible membership in the same tenant/context, and the eligible goal's ownership by that context. Persist references/constraints so invalid cross-tenant/context combinations cannot be represented silently.

Avoid broad role/permission frameworks in this first slice. Use typed action codes and explicit eligibility predicates. Exact schema must enforce tenant/context alignment with composite foreign keys where practical, not only application string comparisons.

## 3. Transaction and lock contract

The A1 transaction-aware persistence command owns the PostgreSQL transaction. It performs final authoritative checks and locks before mutation, idempotency replay, audit/outbox writes, and commit.

Required global lock order, refined to actual rows:
1. tenant-scoped learning context row;
2. relevant membership rows in deterministic stable-key order (actor and recipient, where both apply);
3. relevant action-policy grant rows in deterministic stable-key order;
4. learning goal row for assignment creation, or assignment row for submission/close;
5. idempotency key serialization/lookup only after current authorization checks;
6. protected mutation plus idempotency/audit/outbox records, committed together.

Membership revocation/suspension, context disablement, goal disablement, and policy-grant revocation must acquire compatible locks and update the same authoritative rows. Their commit order must be observable by protected commands:
- if revocation commits before the protected command obtains the authoritative locks, the command denies and creates no protected side effects;
- if the protected command obtains the relevant locks first, revocation waits until that command commits/rolls back.

For operations involving more than one resource, lock rows in deterministic order to avoid deadlocks. A deadlock/serialization retry must restart the entire command and re-run all authoritative checks. Do not call an external identity provider or remote policy service while holding database locks.

## 4. Idempotency and replay contract

- Do not return a stored response before final authorization in the current transaction.
- A retry after membership/policy revocation must be denied even if a prior successful response exists.
- Fingerprint mismatch is checked only after authorization, so it cannot disclose protected idempotency state to a now-unauthorized caller.
- A unique-key race loser must roll back and restart the full authorization protocol in a fresh transaction. It must never deserialize and return a saved response directly from a unique-violation catch.
- Mutation, idempotency, audit, and outbox rows commit or roll back as one unit.

## 5. Verified current gap

The inspected current EF model persists assignments, submissions, idempotency, audit, and outbox. It does not yet persist Person, LearningContext, LearningGoal, ContextualMembership, or ActionPolicyGrant authority records. Assignment `ContextId`, `GoalId`, and `LearnerId` are bounded strings rather than references to this authority model.

The current `FirstSliceService` performs authorization before calling the store. `PostgresFirstSliceStore.ExecuteAtomic` performs idempotency lookup before beginning a transaction, and the unique-conflict catch can return a stored response after rollback without a new authorization pass. Those paths must be replaced by the A1 command boundary; documentation alone does not close the gap.

## 6. Implementation sequence (RED → GREEN)

1. Add PostgreSQL integration-test fixtures/factories using two independent connections and deterministic coordination barriers.
2. Add failing tests for TA-001/002 (denial has no side effects), TA-003/004 (revocation/command serialization), TA-006/007 (replay and fingerprint checks after revocation), TA-008 (same-key race), and TA-011/012 (rollback/cancellation/deadlock retry).
3. Add the authority entities, EF configuration, migration and model snapshot; use composite tenant/context constraints where supported.
4. Add a typed transaction-aware command for creation and submission. Move final authorization, row locks, idempotency lookup, mutation, audit/outbox writes, and commit into that command.
5. Make revocation/policy-change operations use the same locks and order. Do not claim atomicity until the two-connection PostgreSQL tests prove both lock-order outcomes.
6. Run the complete solution test suite, migration validation, idempotent migration-script generation, and exact-head CI. Preserve PR #5 open and unmerged until review.

## 7. Out of scope / explicit non-claims

- No identity provider or credential-verification technology has been selected.
- No guardian/consent policy, enrollment workflow, or platform owner administration UX has been approved.
- No physical PostgreSQL row-level-security/tenant-isolation strategy has been selected.
- No production authorization adapter exists until implemented and tested.
- Atomic revocation is **not yet implemented or proven**.
