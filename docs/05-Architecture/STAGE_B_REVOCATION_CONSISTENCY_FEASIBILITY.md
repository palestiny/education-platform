# Stage B — Revocation Consistency Feasibility Assessment

**Status:** AUTHORITY OWNERSHIP APPROVED — TRANSACTION PROTOCOL DESIGN REQUIRED  
**Date:** 2026-10-20  
**Decision owner:** Project Owner  
**Related:** `STAGE_B_ASSIGNMENT_CREATE_AND_SUBMISSION_AUTHORIZATION_DECISION_BRIEF.md`, `IMPLEMENTATION_READINESS_MATRIX.md`, `PRODUCTION_IDENTITY_AUTHORIZATION_DESIGN_GATE.md`, `STAGE_B_TRANSACTIONAL_AUTHORIZATION_DESIGN_GATE.md`, `adr/ADR-0001-initial-modular-monolith.md`

## 1. Decision record

The project owner approved this direction on 2026-10-20:

- The platform owns the authoritative Person mapping and contextual membership/lifecycle state in the same PostgreSQL authority/transaction boundary as protected first-slice commands.
- External credential verification remains provider-neutral at the API/infrastructure boundary.
- The platform membership authority is separate from external login credentials and provider-specific SDK/token models.
- Revocation consistency follows Option A only after a transaction-aware protocol and tests demonstrate serialization between revocation and protected mutation.
- If implementation evidence later shows the shared transaction/locking boundary cannot be maintained, stop and return to the owner with the evidence; use the already-approved Option B fallback rather than claiming atomic revocation.
- Positive authorization caching (Option C) is not approved.

This approves the **authority-ownership direction**, not a provider, final database schema, onboarding process, or production implementation.

## 2. Verified current facts

- The accepted architecture is a modular monolith, allowing local transactions and logical module boundaries without requiring a separate database per module.
- The current PostgreSQL `EducationPlatformDbContext` owns first-slice assignments, submissions, idempotency records, audit records, and outbox messages.
- The current context does not yet contain the complete Person, contextual membership, membership lifecycle, or resource-policy persistence model.
- Application authorization ports currently return a decision before calling the mutation store. This does not keep a membership lock/transaction open through the protected mutation.
- Production authorization adapters remain fail-closed. Development/Testing fixtures are deterministic fixtures, not authoritative membership state.

## 3. Feasibility conclusion

**Option A is conditionally feasible, but not yet implemented or proven.**

The approved direction makes it architecturally possible for membership lifecycle changes and protected commands to serialize against the same PostgreSQL authority. The final authoritative membership/policy check must be integrated with the protected command's transaction boundary; an earlier Application pre-check alone is insufficient.

A remote identity provider may authenticate a credential and map its stable subject to a platform Person, but it does not own the platform's contextual membership lifecycle. Do not persist raw credentials or let provider SDK objects cross the Application boundary.

## 4. Required next step

Complete `STAGE_B_TRANSACTIONAL_AUTHORIZATION_DESIGN_GATE.md` before creating membership schema or production adapters. That gate must decide and test the command transaction contract, membership/revocation lock protocol, lock ordering, failure and retry behavior, tenant scoping, idempotency, and audit/outbox atomicity.

No membership tables, provider choice, or onboarding flow are authorized by this feasibility record alone.

## 5. Minimum protocol requirements

1. Protected command and authoritative membership/lifecycle check use the same PostgreSQL transaction and source of truth.
2. The command locks the target resource and relevant membership/lifecycle rows using a documented consistent lock order.
3. Revocation changes/locks the same authoritative membership row so command and revocation serialize.
4. If revocation commits first, a later command observes inactive/revoked membership and performs no protected mutation.
5. If a command obtains the relevant membership lock first, revocation waits and is serialized after that command.
6. Deadlock, cancellation, transaction failure, retry/idempotency, audit/outbox atomicity, and tenant scoping are tested.
7. Authorization is re-evaluated before returning a protected idempotency replay.
8. No raw credentials or provider SDK objects enter the Application domain.

## 6. Approved fallback if Option A cannot be proven

Use Option B:
- perform a fresh authoritative membership/policy check for each protected command and before protected idempotency replay;
- disable positive authorization caching initially;
- fail closed on denial, ambiguity, outage, or indeterminate results;
- document the remaining check-to-mutation race and only state staleness bounds verified from the actual source;
- do not claim immediate or atomic revocation.

## 7. Gate status

- **Authority ownership direction:** APPROVED by project owner on 2026-10-20.
- **Provider-neutral credential boundary:** previously accepted; no provider selected.
- **Transactional protocol:** OPEN — design and tests required.
- **Membership schema/adapters:** NOT AUTHORIZED UNTIL TRANSACTIONAL DESIGN GATE PASSES.
- **Production atomic-revocation guarantee:** NOT IMPLEMENTED / NOT CLAIMED.
