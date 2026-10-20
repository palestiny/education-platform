# Stage B — Assignment Creation and Learner Submission Authorization Decision Brief

**Status:** OWNER DECISIONS ACCEPTED — IMPLEMENTATION MUST FOLLOW THE APPROVED CONTRACT  
**Proposal date:** 2026-10-16  
**Owner decision recorded:** 2026-10-20  
**Scope:** First-slice assignment creation and learner submission only  
**Related:** `PRODUCTION_IDENTITY_AUTHORIZATION_DESIGN_GATE.md`, `STAGE_B_IDENTITY_MEMBERSHIP_CONTRACT_TEST_PLAN.md`, `STAGE_B_RESOLUTION_PORT_CONTRACT.md`

## 1. Why this gate is needed

The current first slice has durable assignment/submission persistence, tenant-scoped assignment lookup, a closed-assignment guard, and regression coverage for cross-tenant and wrong-learner requests. Assignment-close has an Application-layer authorization port, but assignment creation and learner submission do not yet use durable contextual membership/resource-policy adapters.

The current API test bearer and deterministic authorization fixtures prove test scenarios only. They are not production identity or membership proof. This brief records the approved minimum policy contract; it does not authorize a provider, membership schema, onboarding flow, or production adapter.

## 2. Assignment creation — decision

**APPROVED: Option B — Context membership plus explicit resource/action policy.**

Before mutation, resolve the target context and verify server-side that:
1. the actor has an active, eligible membership in that exact tenant and learning context;
2. policy explicitly allows `assignment.create` for that actor/context;
3. the referenced goal is valid for the same context;
4. the selected learner is eligible to receive an assignment in that context;
5. all authoritative tenant/context values come from trusted server-side resolution and persisted relationships, not client claims.

Fail closed for denied, ambiguous, unavailable, or indeterminate membership/policy results.

This requires approved membership eligibility rules and authoritative membership/policy sources before production wiring. Minimal field validation alone is not sufficient for production authorization.

## 3. Learner submission — decision

**APPROVED: Option B — Recipient identity plus active contextual membership.**

Before mutation, verify all of the following using trusted server-side data:
1. the authenticated internal person is the persisted assignment recipient;
2. the assignment belongs to the trusted tenant and resolved learning context;
3. the learner has an active, eligible membership/participation in that exact context at submission time;
4. policy explicitly permits `submission.create` for this assignment/action;
5. the assignment remains open at the persistence boundary, with the current row-lock/re-read guard retained.

If any required check is denied, ambiguous, or unavailable, do not create a submission or idempotency/audit/outbox success records. Cross-tenant and wrong-recipient responses must continue using approved non-disclosure behavior.

## 4. Assignment audience scope

**APPROVED: defer group/class-audience assignments.** Keep the first slice at **one assignment → one persisted recipient**. Do not implement group/audience semantics in this increment. Classroom-wide assignments require a separate design for audience cardinality, eligibility, recipient snapshots, and revocation behavior.

## 5. Revocation consistency — approved direction

**APPROVED: Option A is the target when membership and the protected command can share an authoritative database transaction/locking protocol. If that is not technically possible, use Option B.**

### Option A — Transactionally enforced revocation (preferred target)

Membership state and the protected command participate in a shared database transaction/locking protocol, so revocation and submission are serialized against the same authoritative membership state.

- Target guarantee: a submission cannot commit based on membership that was already revoked earlier in the serialization order.
- Requires a shared transactional boundary and carefully designed lock ordering.
- This is an approved target, **not a claim about the current implementation**. Feasibility must be demonstrated against the eventual membership ownership and persistence boundary.

### Option B — Fresh authoritative check (required fallback)

If Option A cannot be supported, read membership/policy from the authoritative source for every command, disable positive authorization caching initially, and fail closed when the source is unavailable or the result is indeterminate.

- This provides fresher decisions but is **not atomic revocation** unless the source and mutation share a consistency protocol.
- Document the residual check-to-mutation race and any verified source replication delay.
- Do not claim a maximum revocation delay until the authority source's consistency and failure behavior have been verified.

Option C (time-bounded positive authorization caching) is **not approved** for the initial slice. It would require a separate explicit owner decision accepting an enforceable exposure window.

## 6. Shared enforcement and verification requirements

- Enforce authorization in the Application use case, not only in HTTP endpoints or persistence code.
- Application contracts receive internal actor identity and resource/context identifiers resolved from trusted server-side state; never pass raw bearer tokens or provider SDK objects into the Application layer.
- Authorization outcomes are explicit: `Allowed`, `Denied`, or `Indeterminate`. Only `Allowed` proceeds.
- Re-evaluate access for each new command and before returning a protected idempotency replay.
- On denial or inability to verify, do not create success records for submissions, idempotency, audit, or outbox.
- Preserve tenant isolation, the PostgreSQL submission row-lock/re-read check, idempotency, expected-version concurrency, audit, and outbox guarantees.
- Add/maintain tests for denied and indeterminate decisions, resolver outage, inactive/revoked membership, wrong context, wrong recipient, cross-tenant requests, replay authorization, concurrent revocation/command behavior where supported, and no-mutation guarantees.
- Test bearer tokens and deterministic authorization fixtures are test-only and must never be represented as production authentication or authorization.

## 7. Decisions and remaining gates

### Owner decisions — recorded as accepted on 2026-10-20

1. Assignment creation Option B — **APPROVED**.
2. Learner submission Option B — **APPROVED**.
3. Group/class-audience assignments — **DEFERRED / OUT OF SCOPE** for this slice.
4. Revocation consistency — **Option A preferred if feasible; otherwise Option B with no positive cache initially, fail-closed behavior, and explicit residual-race documentation**.

### Still required before production readiness

- Prove whether the membership authority can participate in the protected command's transaction/locking protocol; choose Option A only if the actual ownership boundary supports it, otherwise implement the approved Option B fallback.
- Define membership eligibility and authoritative sources without guessing a provider or schema.
- **Implemented and CI-verified:** provider-neutral Application authorization ports for assignment creation and learner submission; the Application use case now authorizes before calling the store, loads the assignment in trusted tenant scope for submissions, verifies the persisted recipient, and re-checks authorization before protected idempotency replay. The API maps denied decisions to 403 and indeterminate decisions to 503. Development/Testing uses deterministic fixtures only; production adapters remain fail-closed. The accepted contract is therefore wired at the Application boundary, but durable membership/policy integration and production eligibility evidence remain open.
- Resolve production identity-provider selection, real credential validation, durable Person/contextual membership, and resource-policy adapters through their separate design gates.
- Keep guardian policy, onboarding, age/consent/privacy, and other out-of-scope choices explicit rather than silently deciding them here.

No identity provider, membership table, account onboarding, guardian policy, or production adapter is selected by this brief.

## 8. Gate result

**Owner policy decision gate: PASS — decisions recorded above.**  
**Implementation / production authorization gate: OPEN.**

These approvals authorize implementation of the specified policy contract and tests, not a provider choice or a claim that production authorization/revocation is already implemented. The Application boundary and deterministic contract tests are implemented and CI-verified, but fixture CI does not prove durable membership/policy integration or atomic revocation; production readiness remains OPEN.
