# Assignment Close Authorization Contract

**Project:** Education Platform  
**Date:** 2026-10-09  
**Status:** CONTRACT FOR IMPLEMENTATION UNDER DEC-0029; runtime implementation remains incomplete  
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

The current coarse `assignment:close` authority may remain a preliminary capability check, but it is not sufficient evidence for conditions 3 or 4. Membership is not itself a permission grant. Tenant-wide override behavior is not implied.

## 3. Application authorization port

The application boundary should accept explicit, provider-neutral inputs: trusted principal ID, trusted tenant ID, assignment ID, owning context ID, and action `assignment.close`. It must not accept raw bearer tokens, HTTP request objects, provider SDK types, or client-supplied tenant/context claims as proof of authorization.

The authorization outcome must distinguish:

- **Allowed** — eligible contextual membership and the explicit resource/action grant are both established.
- **Denied** — membership or policy explicitly denies the operation.
- **Indeterminate** — the required membership/policy decision cannot be established, including dependency failure or incomplete resolution.

Only **Allowed** may proceed. **Denied** maps to a non-disclosing forbidden/not-found response consistent with the existing API contract. **Indeterminate** fails closed and must not mutate state. Exact status mapping for indeterminate outcomes must be aligned with the API error contract before implementation.

No concrete provider, persistence schema, policy engine, membership eligibility predicate, or revocation freshness budget is selected here. Until a real implementation can establish both conditions, the protected operation must not silently fall back to the coarse authority check.

## 4. Required enforcement order

1. Resolve trusted execution context; otherwise return 401.
2. Perform a tenant-scoped assignment lookup; do not load by globally supplied assignment ID and rely only on a later tenant comparison.
3. Evaluate the application authorization port for the assignment's owning context and `assignment.close`.
4. If the result is not **Allowed**, stop before mutation and before returning a protected idempotency replay.
5. Execute the existing close mutation using expected-version concurrency.
6. Preserve the existing transaction boundary for assignment state, idempotency record, audit record, and outbox message.

Authorization must be re-evaluated for every request, including requests with a previously used idempotency key. In particular, a caller whose membership or grant has been revoked must not retrieve the stored close response merely by replaying a known key. A matching idempotency key must not bypass current authorization.

The authorization check and persistence mutation have a time-of-check/time-of-use risk if membership/policy state can change between evaluation and commit. The first implementation must document this limitation and must not claim atomic revocation semantics unless the chosen provider/persistence design can actually guarantee them.

## 5. Tenant-scoped data access

Replace unscoped assignment lookup at protected API boundaries with a tenant-scoped query contract, e.g. `GetAssignment(tenantId, assignmentId)`. Review all call sites before changing the interface. A cross-tenant or missing assignment should be indistinguishable at the external boundary and should not expose assignment details.

The persistence close command must retain its own `assignmentId + tenantId` predicate even when the API has already performed a scoped lookup. This second check protects against accidental bypass of the API lookup.

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

Tests for missing membership, missing grant, and indeterminate evaluation must use explicit deterministic test doubles; they must not pretend to prove a production membership provider exists.

## 7. Implementation sequence

1. Introduce the provider-neutral application authorization contract and deterministic unit/contract tests.
2. Introduce tenant-scoped assignment lookup and update all call sites/tests.
3. Enforce authorization before mutation and before any idempotency replay can be returned; preserve the transaction's audit/outbox/idempotency behavior.
4. Add API integration tests for the denial and allow cases above, including replay after authorization revocation.
5. Run the full CI workflow and verify the exact resulting commit SHA.
6. Update the readiness matrix to separate verified contract behavior from still-unimplemented production membership/policy integration.

## 8. Explicit non-goals and open decisions

- No identity-provider/vendor selection.
- No membership table/schema or membership eligibility predicates.
- No policy engine selection.
- No implicit tenant-wide administrative override.
- No change to assignment creation or learner-submission membership requirements.
- No claim that CI passing proves production authentication or membership resolution.
- No merge of PR #2 without explicit owner approval.
