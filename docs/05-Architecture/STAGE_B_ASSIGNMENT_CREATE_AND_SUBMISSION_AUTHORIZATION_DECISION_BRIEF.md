# Stage B — Assignment Creation and Learner Submission Authorization Decision Brief

**Status:** PROPOSED — PROJECT OWNER DECISION REQUIRED BEFORE BROADENING IMPLEMENTATION  
**Date:** 2026-10-16  
**Scope:** First-slice assignment creation and learner submission only  
**Related:** `PRODUCTION_IDENTITY_AUTHORIZATION_DESIGN_GATE.md`, `STAGE_B_IDENTITY_MEMBERSHIP_CONTRACT_TEST_PLAN.md`, `STAGE_B_RESOLUTION_PORT_CONTRACT.md`

## 1. Why this gate is needed

The current first slice has durable assignment/submission persistence, tenant-scoped assignment lookup, a closed-assignment guard, and regression coverage for cross-tenant and wrong-learner requests. Assignment-close has an Application-layer authorization port, but assignment creation and learner submission do not yet use durable contextual membership/resource-policy adapters.

The current API test bearer and deterministic authorization fixtures prove test scenarios only. They are not production identity or membership proof. This brief proposes the minimum policy contract; it does not authorize a provider, membership schema, onboarding flow, or production adapter.

## 2. Assignment creation — options

### Option A: Keep the current minimal first-slice behavior

Accept a trusted actor/tenant context and validate basic request fields; persist the requested learning context, goal, and learner references.

- **Benefit:** smallest implementation; avoids guessing the membership model.
- **Risk:** a valid actor context alone does not prove that the actor may create assignments in that context, that the goal belongs to it, or that the selected learner is eligible.
- **Disposition:** not sufficient for production authorization.

### Option B: Context membership plus explicit resource/action policy (recommended)

Before mutation, resolve the target context and verify server-side that:
1. the actor has an active, eligible membership in that exact tenant and learning context;
2. policy explicitly allows `assignment.create` for that actor/context;
3. the referenced goal is valid for the same context;
4. the selected learner is eligible to receive an assignment in that context;
5. all authoritative tenant/context values come from trusted server-side resolution and persisted relationships, not client claims.

Fail closed for denied, ambiguous, unavailable, or indeterminate membership/policy results.

- **Benefit:** consistent with DEC-0023 through DEC-0026 and the accepted assignment-close direction.
- **Cost:** requires approved membership eligibility rules and a durable policy/membership source before production wiring.

## 3. Learner submission — options

### Option A: Assignment recipient + tenant + open-state checks only

Require the authenticated internal person to match the assignment's persisted learner ID, require tenant-scoped resource access, and require the assignment to remain open.

- **Benefit:** aligns with the current single-recipient assignment model and is easy to explain.
- **Risk:** does not establish that the learner's membership in the learning context is still active or that the context/relationship remains eligible after assignment creation.

### Option B: Recipient identity plus active contextual membership (recommended)

Before mutation, verify all of the following using trusted server-side data:
1. the authenticated internal person is the persisted assignment recipient;
2. the assignment belongs to the trusted tenant and resolved learning context;
3. the learner has an active, eligible membership/participation in that exact context at submission time;
4. policy explicitly permits `submission.create` for this assignment/action;
5. the assignment remains open at the persistence boundary, with the current row-lock/re-read guard retained.

If any required check is denied, ambiguous, or unavailable, do not create a submission or idempotency/audit/outbox success records. Cross-tenant and wrong-recipient responses must continue using approved non-disclosure behavior.

- **Benefit:** prevents an old assignment reference from serving as continuing authorization after membership is removed.
- **Cost:** requires a source of truth and a defined revocation freshness/consistency contract.

### Option C: Audience/group-based assignment access

Allow submissions based on class/group enrollment or assignment audience, rather than only the persisted single recipient.

- **Benefit:** supports future classroom-wide assignments.
- **Risk:** expands the domain and changes current semantics, requiring audience cardinality, eligibility, recipient snapshots, and revocation behavior to be designed.

- **Disposition:** defer; do not implement group/audience semantics in this slice.

## 4. Recommended first-slice contract

Approve **Assignment creation Option B** and **Learner submission Option B**, with these boundaries:

- Keep the current one-assignment-to-one-recipient model.
- Evaluate authorization in the Application use case, not only in HTTP endpoints or persistence code.
- The Application contract receives internal actor identity and resource/context identifiers resolved from trusted server-side state; it never receives raw bearer tokens or provider SDK objects.
- Authorization outcomes are explicit: `Allowed`, `Denied`, or `Indeterminate`. Only `Allowed` proceeds.
- Re-evaluate access for each new command; do not treat a previously issued assignment as permanent authorization.
- Keep the PostgreSQL submission row-lock/re-read check, idempotency, expected-version concurrency, audit, and outbox guarantees.
- Test denial, resolver outage, inactive/revoked membership, wrong context, wrong recipient, cross-tenant requests, and no-mutation behavior at Application and PostgreSQL/API integration layers.
- Define revocation consistency explicitly before claiming immediate revocation. A pre-mutation authorization check alone cannot guarantee atomicity with a membership change unless the storage/transaction protocol supports it.

## 5. Decisions still required from the owner

1. Approve or reject Assignment creation Option B.
2. Approve or reject Learner submission Option B.
3. Confirm that group/class-audience assignments remain out of scope for the first slice.
4. Require that membership removal blocks new submissions after the configured consistency boundary; the maximum acceptable revocation delay must be decided before production readiness can be claimed.

No identity provider, membership table, account onboarding, guardian policy, or production adapter is selected by this brief.

## 6. Gate result

**Status: OPEN — do not broaden implementation until the owner accepts the contract.**

This proposal narrows the decision and trade-offs. It does not claim production authorization is implemented, and passing test-fixture CI does not close this gate.
