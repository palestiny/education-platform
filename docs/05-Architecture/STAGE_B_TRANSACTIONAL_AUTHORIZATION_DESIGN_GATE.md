# Stage B — Transactional Authorization Design Gate

**Status:** DESIGN IN PROGRESS — NO SCHEMA OR PRODUCTION ADAPTER AUTHORIZED  
**Date:** 2026-10-20  
**Decision owner:** Project Owner  
**Authority ownership:** Approved — platform-owned Person/contextual membership/lifecycle state in the same PostgreSQL transaction boundary as protected first-slice commands  
**Related:** `STAGE_B_REVOCATION_CONSISTENCY_FEASIBILITY.md`, `STAGE_B_ASSIGNMENT_CREATE_AND_SUBMISSION_AUTHORIZATION_DECISION_BRIEF.md`, `IMPLEMENTATION_READINESS_MATRIX.md`, `PRODUCTION_IDENTITY_AUTHORIZATION_DESIGN_GATE.md`

## 1. Purpose

Turn the approved authority-ownership direction into a verifiable transaction contract before adding membership schema or production authorization adapters. This is a design gate, not permission to implement an assumed schema.

## 2. Approved constraints

1. The platform owns stable Person mapping and contextual membership/lifecycle state.
2. External credential verification remains provider-neutral and isolated at the API/infrastructure boundary.
3. Authentication is not authorization; provider claims alone do not grant contextual access.
4. Protected commands fail closed on missing, inactive, ambiguous, unavailable, or indeterminate authority.
5. Assignment creation requires active eligible membership and explicit `assignment.create` policy in the same trusted context as the goal and learner.
6. Learner submission requires authenticated Person = persisted recipient, tenant/context alignment, active eligible membership, explicit `submission.create` policy, and an open assignment at the persistence boundary.
7. First slice remains one assignment to one persisted recipient; group/class audiences remain deferred.
8. No positive authorization cache is approved.
9. Preserve idempotency, expected-version concurrency, audit/outbox atomicity, tenant scoping, and non-disclosure behavior.
10. No identity provider, concrete schema, onboarding flow, guardian-consent policy, or physical tenant-isolation strategy is selected by this gate.

## 3. Transaction contract to design

The proposed protocol must make the final authorization decision and protected mutation part of one PostgreSQL transaction. Application use cases remain the policy-orchestration boundary, but the transaction-aware command/store boundary must re-read and lock authoritative membership/lifecycle facts before committing the mutation.

The design must specify:

- transaction owner and lifetime;
- which component supplies the trusted Person, tenant, context, resource, and action;
- how create and submit commands perform their final membership/policy check;
- which membership and resource rows are locked and why;
- a global lock order shared by protected commands and membership revocation;
- how membership revocation/disablement updates the same authoritative lifecycle record;
- transaction isolation assumptions and what is/isn't guaranteed;
- idempotency lookup/replay ordering and authorization re-check behavior;
- audit and outbox writes in the same transaction as the mutation;
- cancellation, timeout, deadlock, serialization failure, and retry semantics;
- API mapping for denied versus unavailable/indeterminate authorization;
- tenant scoping and prevention of cross-context information disclosure.

Do not claim atomic revocation if any final authority check remains a remote pre-check outside the transaction or if revocation can update a different authority without a shared serialization mechanism.

## 4. Required concurrency semantics

The implementation plan and tests must establish these observable outcomes:

1. **Revocation commits first:** a subsequently serialized protected command sees inactive/revoked membership and creates no protected mutation, idempotency success record, audit event, or outbox event.
2. **Protected command locks first:** revocation waits for the command's transaction; the command may complete under the documented serialization order, then revocation commits.
3. **Denied or indeterminate authorization:** no protected mutation or successful idempotency result is committed.
4. **Replay after revocation:** protected idempotency replay re-evaluates current authorization and does not disclose the protected result after access is revoked.
5. **Tenant/context mismatch:** command is rejected without leaking resource existence or creating mutation/reliability records.
6. **Transaction failure/cancellation:** all transactional mutation, idempotency, audit, and outbox changes roll back together.
7. **Deadlock/retry:** retries are bounded and safe; the same logical request cannot create duplicate effects.

Tests must use separate database contexts/connections where needed to prove actual blocking/serialization, not merely sequentially call mocked methods.

## 5. Design options to compare before choosing the contract

- **Option A1 — transaction-aware persistence command:** pass a typed authorization intent into a store command that owns the transaction and performs final authoritative membership/policy reads and locks before mutation.
- **Option A2 — explicit unit-of-work/transaction coordinator:** an application-level transaction coordinator opens the transaction and coordinates authoritative membership checks, resource locking, and mutation through typed ports sharing the same connection/transaction.
- **Option B fallback — fresh authoritative pre-check:** if the same-database protocol cannot be demonstrated, fresh checks/no positive cache/fail-closed behavior remain mandatory, and the residual check-to-mutation race must be documented. This does not satisfy an atomic-revocation claim.

The gate must compare module-boundary clarity, testability, lock ownership, transaction leakage, cancellation, retry safety, and complexity. Recommend one option with explicit trade-offs before implementation. Do not silently select schema or widen the product scope.

## 6. RED test plan required before GREEN

At minimum, author executable tests for:

- command versus revocation serialization in both lock orders;
- create and submit authorization inside the final mutation boundary;
- inactive membership and missing action policy;
- tenant/context mismatch and persisted-recipient mismatch;
- no mutation/idempotency/audit/outbox records after denial or indeterminate authority;
- authorization re-check before idempotency replay after revocation;
- rollback of mutation, idempotency, audit, and outbox on injected failure;
- cancellation, deadlock/serialization failure, and bounded retry;
- lock-order consistency across create, submit, and revoke;
- exact API outcomes and non-disclosure behavior.

RED must fail because the intended behavior is absent, not because the test harness or database setup is broken.

## 7. Exit criteria

This gate can pass only when:

- a selected transaction design is documented with trade-offs;
- lock order and revocation semantics are unambiguous;
- schema ownership and module ports are described without provider leakage;
- the RED suite and expected PostgreSQL test strategy are defined;
- security, idempotency, audit/outbox, and tenant-isolation obligations are traced;
- owner review is requested for any new product-level policy choice;
- no atomic guarantee is claimed until exact-head CI proves the relevant concurrency scenarios.

## 8. Current result

**Authority-ownership direction:** APPROVED.  
**Transaction design:** OPEN.  
**Membership schema and production adapter implementation:** NOT YET AUTHORIZED.  
**Atomic revocation:** NOT IMPLEMENTED / NOT CLAIMED.  
**Next action:** compare A1 and A2 against the existing repository contracts, recommend one, and present any remaining owner decisions before writing implementation code.
