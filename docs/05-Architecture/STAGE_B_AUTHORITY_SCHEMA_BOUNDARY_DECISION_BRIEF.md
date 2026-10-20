# Stage B — Authority Schema Boundary Decision Brief

**Status:** BLOCKING DESIGN DECISION — NO AUTHORITY TABLES CREATED  
**Date:** 2026-10-20  
**Approved decisions:** platform-owned Person/contextual membership lifecycle; A1 typed transaction-aware persistence command  
**Related:** `STAGE_B_TRANSACTIONAL_AUTHORIZATION_DESIGN_GATE.md`, `STAGE_B_TRANSACTIONAL_AUTHORIZATION_RED_TEST_SPECIFICATION.md`, `STAGE_B_REVOCATION_CONSISTENCY_FEASIBILITY.md`

## 1. Why this is the next gate

A1 is approved, and the current store/transaction boundary has been inspected. Before writing executable concurrency tests against durable membership state, the repository must define which persisted rows are authoritative and lockable.

The current EF model contains assignments, submissions, idempotency records, audit records, and outbox messages. It does **not** contain durable platform Person mappings, contextual memberships/lifecycle, action policies, learning-context records, or learning-goal records. Existing assignment `ContextId`, `GoalId`, and `LearnerId` are bounded strings, not foreign keys to an authoritative eligibility model.

Therefore, merely adding a membership row and locking it would not prove that:
- the referenced learner is eligible in the assignment's context;
- the goal belongs to that same context and remains eligible;
- the actor is allowed to perform the exact action;
- revocation/policy changes serialize with the command;
- all rows representing the decision are read from the same source of truth.

## 2. Proposed minimum authority boundary (for owner review)

Keep this small and provider-neutral. This is a proposed domain boundary, **not an approved final schema**.

- **Person mapping:** stable platform Person ID mapped to an external issuer/subject tuple. Credential secrets and provider SDK objects are not stored in this domain table.
- **Learning context:** tenant-scoped context/organization/classroom identifier that defines the authorization boundary.
- **Contextual membership:** Person + tenant + context + lifecycle state + eligibility state/version. Membership lifecycle transitions (activate/suspend/revoke) update this authoritative row.
- **Action policy grant:** tenant/context + principal/membership scope + typed action code + lifecycle state/version. Only the platform authority can grant/revoke these rows.
- **Learning goal eligibility:** a durable, context-scoped goal/learning-target record (or an already-existing authoritative module contract that provides equivalent lockable state).
- **Assignment/recipient references:** assignment creation must validate the goal and learner under the same context and ensure their authoritative rows cannot be invalidated between final validation and commit.

Use stable IDs and tenant-scoped unique constraints. Decide exact natural keys, foreign keys, deletion behavior, indexes, status enums, and optimistic versions only after inspecting product/domain contracts. Do not store arbitrary provider claims as membership or policy.

## 3. Two implementation shapes to compare

### Option S1 — platform-owned minimum relational authority

Add a small set of platform-owned relational entities in the same PostgreSQL database/transaction authority. Protected commands and lifecycle changes lock the relevant membership, policy, context, and goal rows in the agreed global order.

**Advantages:** strongest ability to prove revocation and eligibility serialization; relational constraints and indexes can express tenant/context boundaries.

**Costs/risks:** introduces new durable domain entities and lifecycle rules; must decide context and goal ownership, how membership is provisioned, and how stable external subjects map to Person records. Must not pretend that table presence alone solves identity verification or enrollment.

### Option S2 — existing authoritative module contract

If another module is intended to own learning contexts, goals, or eligibility, use its typed provider-neutral port only if it can participate in the same PostgreSQL connection/transaction and exposes a documented lock/serialization contract.

**Advantages:** preserves domain ownership and avoids duplicate sources of truth.

**Costs/risks:** no such durable authority/transaction-sharing contract has yet been verified in the current first-slice model. A remote service or a separate transaction is not sufficient for an atomic-revocation claim.

## 4. Recommendation

Prefer S1 for membership and policy lifecycle only if the product architecture confirms that the platform owns these records. For context and goal eligibility, first confirm the existing domain ownership contract rather than silently inventing duplicate context/goal tables. If no authoritative context/goal model exists, bring a narrow S1 proposal back for approval before implementation.

Do not proceed with production membership schema or adapters until the context/goal authority boundary is resolved. A test-only fake can test Application decisions, but it cannot prove real PostgreSQL revocation serialization.

## 5. Required owner decision

Confirm the source of truth for learning contexts and goals:

- **Option A:** platform owns and persists tenant-scoped learning contexts and goals in the same PostgreSQL authority; membership, policy, goal/context eligibility, and protected command can be locked and validated in one transaction.
- **Option B:** an existing domain module owns contexts/goals and supplies typed transaction-aware access on the same PostgreSQL connection/transaction. The module and contract must be named and verified before coding.
- **Option C:** contexts/goals are external or remote. This cannot satisfy the current atomic A1 promise as-is; either revise the atomicity scope or explicitly use the approved fresh-check fallback with its residual race.

This is not a provider selection. External credential validation remains provider-neutral regardless of the selected context/goal authority.

## 6. Exit criteria

- One authoritative owner is named for context and goal eligibility.
- The minimum persisted rows participating in authorization are identified.
- A1 lock ordering is refined to include only actual authoritative rows, with a deadlock analysis.
- RED tests TA-001–TA-012 map to schema/command operations.
- Only then create migration/entity changes and executable PostgreSQL concurrency tests.

**Current status:** A1 transaction-aware command is approved; authority schema boundary is not fully resolved; no membership/context/goal schema has been created; atomic revocation remains not implemented/not proven.
