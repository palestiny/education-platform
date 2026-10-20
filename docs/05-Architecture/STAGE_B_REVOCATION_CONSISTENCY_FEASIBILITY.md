# Stage B — Revocation Consistency Feasibility Assessment

**Status:** TECHNICAL ASSESSMENT COMPLETE — AUTHORITY OWNERSHIP DECISION OPEN  
**Date:** 2026-10-20  
**Decision owner:** Project Owner  
**Related:** `STAGE_B_ASSIGNMENT_CREATE_AND_SUBMISSION_AUTHORIZATION_DECISION_BRIEF.md`, `IMPLEMENTATION_READINESS_MATRIX.md`, `PRODUCTION_IDENTITY_AUTHORIZATION_DESIGN_GATE.md`, `adr/ADR-0001-initial-modular-monolith.md`

## 1. Question

The owner approved Option A (transactionally enforced revocation) if feasible; otherwise use Option B (fresh authoritative check, no positive authorization cache initially, fail closed, and document the residual race).

This assessment checks feasibility against the repository's current architecture without silently choosing an identity provider or membership schema.

## 2. Verified current facts

- The accepted architecture is a modular monolith, explicitly allowing local transactions and logical module boundaries without requiring a separate database per module.
- The current PostgreSQL `EducationPlatformDbContext` owns first-slice assignments, submissions, idempotency records, audit records, and outbox messages.
- The current context does **not** contain a Person, contextual membership, membership lifecycle, or resource-policy persistence model.
- The new Application authorization ports currently return a decision before calling the mutation store. This centralizes policy enforcement in the use case, but the current call shape does not itself keep a membership row lock/transaction open through the protected mutation.
- Production authorization adapters remain fail-closed. The Development/Testing fixtures are deterministic test fixtures, not authoritative membership state.

## 3. Feasibility result

**Option A is conditionally feasible, but not yet proven or implemented.**

It is feasible if the platform's authoritative contextual membership and lifecycle state can be read and locked in the same PostgreSQL transaction boundary as the protected assignment/submission mutation. The existing modular-monolith architecture and PostgreSQL persistence are compatible with that approach.

It is **not** established as feasible if the authoritative membership decision lives only in an external identity provider or in a separate store that cannot participate in the same transaction/locking protocol. A remote pre-check, even immediately before mutation, is not atomic revocation.

The current Application port calls alone do not provide Option A: a decision returned before the store transaction leaves a check-to-mutation gap. The final authoritative membership check/lock must be integrated into the protected command's transaction boundary, while the Application use case remains the policy orchestration boundary.

## 4. Recommended direction for owner review

**Recommendation:** keep external credential verification provider-neutral at the API/infrastructure boundary, and consider platform-owned Person + contextual membership/lifecycle state in the same PostgreSQL authority as the first-slice commands. If approved and validated, use a transaction-aware command boundary that evaluates the relevant membership/policy state under the transaction/locking protocol before the mutation commits.

Why this is the preferred path:
- supports the already-approved person-centric identity and contextual-membership model;
- gives the platform a single authoritative place to revoke membership independently of the identity provider;
- makes Option A technically attainable without distributed transactions;
- preserves provider neutrality and the modular-monolith architecture;
- keeps provider login identity separate from application membership and resource/action permission.

This is a recommendation, **not an approved schema decision**. It does not specify tables, columns, identity provider, onboarding flow, or the final transaction API.

## 5. Minimum protocol requirements if Option A is approved

Before claiming atomic revocation, a design and tests must establish:

1. The protected command and membership/lifecycle check use the same database transaction and authoritative data source.
2. The command locks the target resource and the relevant membership rows using a documented, consistent lock order.
3. Revocation updates/locks the same authoritative membership row, so command and revocation serialize.
4. If revocation commits first, a later command observes inactive/revoked membership and performs no mutation.
5. If the command obtains the relevant membership lock first, revocation waits and is serialized after that command.
6. Deadlock behavior, cancellation, transaction failure, retry/idempotency, audit/outbox atomicity, and tenant scoping are tested.
7. The command still re-evaluates authorization before returning a protected idempotency replay.
8. No raw credentials or provider SDK objects enter the Application domain.

A remote policy provider cannot be described as atomically consistent with the PostgreSQL mutation unless an actual shared transactional protocol is demonstrated.

## 6. Approved fallback if Option A cannot be implemented

Use Option B:
- perform a fresh authoritative membership/policy check for each protected command and before protected idempotency replay;
- disable positive authorization caching initially;
- fail closed on denial, ambiguity, outage, or indeterminate results;
- document the remaining check-to-mutation race and only state replication/staleness bounds that are verified from the actual source;
- do not claim immediate or atomic revocation.

Option C (positive authorization caching with TTL) remains unapproved.

## 7. Owner decision still required

The policy decision is approved; authority ownership is not. Before implementing membership persistence, the owner must accept or reject this recommendation:

- **A. Platform-owned Person/contextual membership/lifecycle authority in the same PostgreSQL transaction boundary** — recommended to make Option A feasible.
- **B. Membership remains authoritative in a remote/separate system** — use Option B unless a real shared transaction/locking mechanism can be proven.
- **C. Defer this ownership decision** — keep production protected commands fail-closed and do not implement a pretend membership adapter.

No schema or provider is selected by this assessment.

## 8. Gate result

**Technical feasibility:** Option A is conditionally feasible with same-database platform-owned membership and a transaction-aware authorization/mutation boundary.  
**Owner authority-ownership decision:** OPEN.  
**Production revocation guarantee:** NOT IMPLEMENTED / NOT CLAIMED.
