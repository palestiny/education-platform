# Stage B — Transactional Authorization Design Gate

**Status:** A1 APPROVED — TRANSACTION CONTRACT DEFINED; RED TESTS REQUIRED BEFORE IMPLEMENTATION  
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

## 5. Design options considered and owner decision

- **Option A1 — transaction-aware persistence command:** pass a typed authorization intent into a store command that owns the transaction and performs final authoritative membership/policy reads and locks before mutation.
- **Option A2 — explicit unit-of-work/transaction coordinator:** an application-level transaction coordinator opens the transaction and coordinates authoritative membership checks, resource locking, and mutation through typed ports sharing the same connection/transaction.
- **Option B fallback — fresh authoritative pre-check:** if the same-database protocol cannot be demonstrated, fresh checks/no positive cache/fail-closed behavior remain mandatory, and the residual check-to-mutation race must be documented. This does not satisfy an atomic-revocation claim.

**Owner decision (2026-10-20): Option A1 is approved.** The implementation shall use a typed transaction-aware persistence command boundary. A2 is not selected because it risks leaking transaction ownership across ports and increases the chance that authorization and mutation use different transaction scopes. Option B remains the fallback if same-transaction serialization cannot be proven. This decision does not approve a concrete membership schema, provider, or production adapter.

## 6. Repository fit assessment and recommendation

Inspection of the current PR branch confirms:

- `FirstSliceService` currently calls the authorizer before calling `IFirstSliceStore`; this leaves the check-to-mutation gap.
- `PostgresFirstSliceStore` owns the EF Core database context and transaction helper. It currently checks idempotency records before opening the transaction, then performs mutation, idempotency write, audit, and outbox work atomically.
- Submission already re-reads and locks the tenant-scoped assignment row with `FOR UPDATE`, but membership/lifecycle rows do not yet exist and are not locked.
- Current authorization request records are provider-neutral Application contracts. Keep them provider-neutral; do not pass tokens, provider SDK objects, or a database transaction through public API/Application request models.

**Approved design: Option A1, refined as a typed transaction-aware command boundary.**

The Application use case should continue validating request intent and constructing a typed command. A dedicated transaction-aware command/store operation should own the PostgreSQL transaction, perform the final authoritative membership/lifecycle and resource-state reads/locks using the same DbContext/connection, then commit mutation, idempotency, audit, and outbox together. Keep provider-specific credential validation outside this transaction. Keep authorization outcomes typed and explicit; do not turn an external or remote policy call into a supposed atomic check.

Why prefer this over an explicit unit-of-work that passes a transaction across multiple ports:
- the existing persistence store already owns the EF Core transaction and atomic audit/outbox work;
- it avoids leaking EF transaction objects across module/application contracts;
- it reduces the risk of separate ports accidentally using different connections or transaction scopes;
- the trade-off is that the transaction-aware command boundary must be designed carefully so application policy meaning remains explicit and persistence does not become a hidden general-purpose authorization engine.

A1 is owner-approved. The following command contract is the implementation target; it does not authorize schema creation until the RED tests and schema-level ownership details are reviewed.

### 6.1 Typed command contract

The Application layer supplies a provider-neutral immutable command containing:
- trusted execution identifiers: tenant ID, platform Person/actor ID, and correlation ID;
- action intent (closed set / typed operation, not an arbitrary caller-provided policy string);
- for assignment creation: learning context ID, goal ID, persisted recipient Person ID, work payload, idempotency key, and request fingerprint;
- for submission: assignment ID, persisted recipient expectation, payload, idempotency key, and request fingerprint;
- cancellation token for cancellation before entering the synchronous database transaction; once the transaction begins, cancellation is passed to EF/database operations and must cause rollback if not committed.

Do not accept tenant, Person, role, membership state, eligibility, or policy facts from an untrusted request body. Derive tenant and Person from the trusted execution context; derive submission context and recipient from the persisted assignment. Keep external credential validation outside the transaction and do not make remote network calls while holding database locks.

The persistence command owns the full transaction on one DbContext/connection. It loads authoritative platform membership/lifecycle and action-policy facts from the same PostgreSQL authority, obtains the required locks, evaluates the requested action, then performs the mutation. A denied or indeterminate decision throws/returns a typed outcome that maps to 403 or 503 respectively; it must not commit a successful idempotency response, audit success record, or outbox event.

### 6.2 Lock order and serialization contract

Use one documented global order for every protected command and membership/policy lifecycle mutation:

1. Begin transaction.
2. Resolve and lock authoritative tenant/context scope needed by the operation, where such rows are part of the eventual model.
3. Lock all relevant membership/lifecycle rows in a deterministic stable-key order (e.g. sorted Person/membership IDs); verify active and eligible state.
4. Lock/read relevant action-policy rows in deterministic stable-key order; verify the exact action and resource/context scope.
5. Lock the protected resource row (for submission/close); for creation, lock/validate the relevant goal/context/recipient rows needed to ensure they remain eligible.
6. Resolve the idempotency key and replay/fingerprint state only after the current authorization decision is established. The uniqueness constraint remains the final arbiter for concurrent first use of the same key.
7. Write the protected mutation, idempotency result, audit record, and outbox message in the same transaction; commit once.

All revocation, membership disablement, and policy-change commands must lock/update the same authoritative rows and follow the same relative lock order. Revocation that acquires the relevant lifecycle row first commits before a protected command can authorize; a protected command that locks the lifecycle row first is serialized before the revocation. This is a database serialization order, not a guarantee that a command starting after revocation was requested always wins or loses.

If the final schema cannot provide stable lockable authority rows for a relevant eligibility/policy fact, stop and revisit the design rather than quietly performing a remote/pre-transaction check and claiming atomic revocation. Row locks and PostgreSQL isolation semantics must be verified with real concurrent transactions; lock syntax alone is not proof.

### 6.3 Idempotency, audit, retry, and error contract

- Never return a protected replay before rechecking current authorization within the same transaction boundary.
- A fingerprint mismatch returns the existing idempotency conflict outcome only after authorization, without disclosing the saved response.
- Denial/indeterminate outcomes produce no successful mutation/idempotency/audit-success/outbox records. Security-denial telemetry, if later added, must be separately defined and must not reveal protected resource data.
- The existing store checks idempotency before opening its transaction. The implementation must move protected idempotency lookup/replay into the transaction after authoritative authorization and required locks.
- A uniqueness race must roll back the losing attempt, then re-enter the command through the full authorization/transaction protocol before returning any stored response. Never return the winning response directly from a catch block without fresh authorization.
- Mutation, idempotency success result, audit, and outbox writes commit or roll back together.
- Retry only classified transient database errors, with a bounded policy and a fresh transaction/authorization on each retry. Do not retry denials, validation errors, or fingerprint conflicts. Cancellation before commit rolls back; no success response is returned until commit succeeds.
- No positive authorization cache is permitted. No external identity-provider/policy network calls are made while locks are held.


## 7. RED test plan required before GREEN

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

## 8. Exit criteria

This gate can pass only when:

- a selected transaction design is documented with trade-offs;
- lock order and revocation semantics are unambiguous;
- schema ownership and module ports are described without provider leakage;
- the RED suite and expected PostgreSQL test strategy are defined;
- security, idempotency, audit/outbox, and tenant-isolation obligations are traced;
- owner review is requested for any new product-level policy choice;
- no atomic guarantee is claimed until exact-head CI proves the relevant concurrency scenarios.

## 9. Current result

**Authority-ownership direction:** APPROVED.  
**Transaction design:** A1 approved; typed command contract and lock order defined; implementation gated on RED tests.  
**Membership schema and production adapter implementation:** NOT YET AUTHORIZED.  
**Atomic revocation:** NOT IMPLEMENTED / NOT CLAIMED.  
**Next action:** add failing tests for transaction-aware authorization, idempotency-after-authorization, and actual PostgreSQL command/revocation serialization; only then implement the smallest schema/command increment those tests require. Owner approval remains required before broader production identity/provider or onboarding choices.
