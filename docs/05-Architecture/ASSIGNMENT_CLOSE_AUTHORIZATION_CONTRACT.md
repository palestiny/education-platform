# Assignment Close Authorization Contract

**Project:** Education Platform  
**Date:** 2026-10-09  
**Status:** APPLICATION USE-CASE ENFORCEMENT IMPLEMENTED AND CI VERIFIED; production membership/policy integration remains incomplete  
**Branch:** `chore/architecture-gate-preparation`  
**Related:** `FIRST_PROTECTED_LEARNING_JOURNEY_AUTHORIZATION_MAP.md`, `PRODUCTION_IDENTITY_AUTHORIZATION_DESIGN_GATE.md`, DEC-0029

## 1. Purpose and boundary

Define the minimum application/API contract for closing an assignment without selecting an identity provider, defining a membership schema, or inventing eligibility rules that have not been approved.

This contract applies only to `POST /api/v1/assignments/{assignmentId}/close`. It does not define authorization for assignment creation or learner submission.

## 2. Required allow conditions

A close operation may proceed only when all of the following are true:

1. A trusted execution context has been established by the configured authentication boundary.
2. The assignment is found within the caller's trusted tenant scope.
3. The actor has an eligible membership in the assignment's owning learning context, according to the future approved membership contract.
4. An explicit policy grant allows this actor to perform the close action on this assignment resource.
5. The request passes existing expected-version and domain-state rules.

The current coarse `assignment:close` authority remains a preliminary API capability check, but it is not sufficient evidence for conditions 3 or 4. The application service independently enforces the resource-level authorization port, so direct callers of the service cannot bypass this check. Membership is not itself a permission grant. Tenant-wide override behavior is not implied.

## 3. Application authorization port

The application use-case boundary accepts explicit, provider-neutral inputs: trusted principal ID, trusted tenant ID, assignment ID, owning context ID, and action `assignment.close`. It must not accept raw bearer tokens, HTTP request objects, provider SDK types, or client-supplied tenant/context claims as proof of authorization.

The authorization outcome must distinguish:

- **Allowed** — eligible contextual membership and the explicit resource/action grant are both established.
- **Denied** — membership or policy explicitly denies the operation.
- **Indeterminate** — the required membership/policy decision cannot be established, including dependency failure or incomplete resolution.

Only **Allowed** may proceed. **Denied** maps to a non-disclosing forbidden/not-found response consistent with the existing API contract. **Indeterminate** fails closed and must not mutate state. The implemented API maps it to `503 AUTHORIZATION_UNAVAILABLE`; this indicates the authorization decision could not be established, not that the caller is authorized.

No concrete provider, persistence schema, policy engine, membership eligibility predicate, or revocation freshness budget is selected here. Until a real implementation can establish both conditions, the protected operation must not silently fall back to the coarse authority check.

## 4. Required enforcement order

1. Resolve trusted execution context; otherwise return 401.
2. The application service performs a tenant-scoped assignment lookup; do not load by globally supplied assignment ID and rely only on a later tenant comparison.
3. The application service evaluates the authorization port for the assignment's owning context and `assignment.close`.
4. If the result is not **Allowed**, stop before mutation and before returning a protected idempotency replay.
5. Execute the existing close mutation using expected-version concurrency.
6. Preserve the existing transaction boundary for assignment state, idempotency record, audit record, and outbox message.

Authorization must be re-evaluated for every request, including requests with a previously used idempotency key. In particular, a caller whose membership or grant has been revoked must not retrieve the stored close response merely by replaying a known key. A matching idempotency key must not bypass current authorization.

The authorization check and persistence mutation have a time-of-check/time-of-use risk if membership/policy state can change between evaluation and commit. The first implementation must document this limitation and must not claim atomic revocation semantics unless the chosen provider/persistence design can actually guarantee them.

## 5. Tenant-scoped data access

Replace unscoped assignment lookup at protected API boundaries with a tenant-scoped query contract, e.g. `GetAssignment(tenantId, assignmentId)`. Review all call sites before changing the interface. A cross-tenant or missing assignment should be indistinguishable at the external boundary and should not expose assignment details.

The persistence close command retains its own `assignmentId + tenantId` predicate for a new mutation, even though the application service has already performed a scoped lookup. The application service must remain the authorized entry point: persistence-level idempotency replay is not independently protected by the store and must only be reachable after the service re-checks authorization.

## 6. Verification matrix (RED before GREEN)

The contract tests must cover at least:

| Case | Expected result | Must not happen |
|---|---|---|
| No trusted execution context | 401 | Any assignment lookup/mutation |
| Assignment absent or belongs to another tenant | Non-disclosing not-found outcome | Cross-tenant details or mutation |
| No eligible membership in the assignment context | Denied | Assignment/version/idempotency/audit/outbox mutation |
| Membership exists but no explicit close grant | Denied | Mutation |
| Both membership and resource/action grant allow | Close succeeds | Existing reliability guarantees regress |
| Membership/policy decision is indeterminate or unavailable | Fail closed | Mutation or permissive fallback |
| Authorization is revoked before a retry with the same idempotency key | Denied | Protected replay response |
| Same authorized request/key is retried | Existing replay semantics are preserved | Duplicate mutation/audit/outbox |
| Different request reuses the key | Existing idempotency conflict | Mutation |
| Stale expected version | Existing concurrency conflict | Second close commit |
| Audit or outbox persistence fails | Existing atomic rollback behavior | Partial authoritative commit |

Tests for missing membership, missing grant, wrong-resource grants, and indeterminate evaluation must use explicit deterministic test doubles; they must not pretend to prove a production membership provider exists. A grant for one assignment must not authorize a different assignment. The Development/Testing fixture uses an explicit wildcard grant only for its synthetic teacher identity so API tests can close dynamically created assignments; that wildcard is a fixture convenience, not the production policy model.

## 7. Implementation sequence

1. Provider-neutral authorization contract and deterministic fixture are implemented.
2. Tenant-scoped assignment lookup is implemented.
3. The application service now enforces authorization before mutation and before an idempotency replay can be returned.
4. Integration tests exercise direct application-service denial/no mutation, authorization re-evaluation before replay, trusted resource/context/action construction, and cancellation propagation.
5. Exact-head CI passed for branch head `28deb9006974245364cbea83faac0fb8793db1f5` (run #543): https://github.com/palestiny/education-platform/actions/runs/37926748960. This verifies the branch head including the current authorization implementation and its documentation; production membership/policy integration remains unimplemented.
6. Production membership/policy integration remains open and must not be conflated with fixture-based enforcement.

## 8. Explicit non-goals and open decisions

- No identity-provider/vendor selection.
- No membership table/schema or membership eligibility predicates.
- No policy engine selection.
- No implicit tenant-wide administrative override.
- No change to assignment creation or learner-submission membership requirements.
- No claim that CI passing proves production authentication or membership resolution.
- No merge of PR #2 without explicit owner approval.
