# Stage B — Identity & Membership Contract Test Plan

**Project:** Education Platform  
**Status:** BASELINE DECISIONS ACCEPTED — CONTRACT SEMANTICS AND IMPLEMENTATION GATE STILL OPEN  
**Date:** 2026-10-09  
**Inputs:** `PROVIDER_NEUTRAL_IDENTITY_MEMBERSHIP_CONTRACT_PROPOSAL.md`, `PRODUCTION_AUTHENTICATION_ADAPTER_TDD_RED_SPECIFICATION.md`

## 1. Scope

This plan narrows Stage B to the minimum provider-neutral contracts needed to prevent a valid external login from becoming implicit platform access. It intentionally does not implement a provider, database schema, Person aggregate, membership persistence, login/session flow or guardian consent policy.

Current `ExecutionContext` remains the downstream trusted context. Stage B tests should focus on the decision that must occur before an execution context is established.

## 2. Minimum contract seam to test

Conceptual seam (names remain unapproved):

`ValidatedExternalIdentity → PlatformPersonResolution → PersonLifecycleCheck → ContextualMembershipResolution → Action/ResourcePolicy → TrustedExecutionContext`

The contract must make each stage's result explicit. It must not represent every denial as an empty identity or an empty authority collection, because that can hide the difference between unauthenticated, unknown, disabled, no membership, ambiguity, and policy denial.

Suggested outcome categories (not source-code enum names):
- `AuthenticatedKnownPerson`
- `UnknownIdentity`
- `PersonDisabledOrRevoked`
- `NoEligibleMembership`
- `AmbiguousMembership`
- `PolicyDenied`
- `ResolverUnavailable`
- `AuthorizedContext`

Only the final `AuthorizedContext` outcome may establish a trusted execution context.

## 3. Proposed deterministic test matrix

| Test ID | Setup | Expected result | Side-effect assertion |
|---|---|---|---|
| STAGEB-001 | Validated issuer/subject maps to active internal Person | Known Person outcome | No Person/membership is created |
| STAGEB-002 | Validated issuer/subject has no mapping | Unknown identity | No automatic provisioning or role grant |
| STAGEB-003 | Mapped Person is disabled | Denied | No execution context; no protected mutation |
| STAGEB-004 | Mapped Person is revoked | Denied | No execution context; no protected mutation |
| STAGEB-005 | Active Person has zero eligible membership for target resource | No eligible membership | No execution context |
| STAGEB-006 | Active Person has exactly one eligible membership | Membership selected server-side | Client tenant value has no influence |
| STAGEB-007 | Active Person has multiple eligible memberships and no approved selection | Ambiguous, fail closed | No default/first membership selected |
| STAGEB-008 | Membership is active but policy denies action on resource | Policy denied | Relationship/role label alone cannot allow |
| STAGEB-009 | Guardian/teacher relationship exists but required policy condition is absent | Policy denied | No protected data disclosure or mutation |
| STAGEB-010 | Person/membership resolver throws or reports unavailable | Resolver unavailable, fail closed | No test bearer or anonymous fallback |
| STAGEB-011 | All resolution stages succeed | Trusted context contains internal identifiers only | No raw token/provider claims persisted or emitted |
| STAGEB-012 | Client tenant conflicts with selected membership | Server-resolved tenant remains authoritative | Cross-tenant mutation does not occur |

### Test-layer boundaries
- These are contract tests with deterministic fakes; they validate platform decisions, not cryptographic token verification.
- API integration tests prove enforcement at protected endpoints.
- A real-provider test environment is required later to verify issuer, audience, signature, expiry, key rotation and provider-specific configuration.
- PostgreSQL integration tests are required when durable identity/membership persistence is separately approved.
- Legal/guardian-consent behavior cannot be proven by a mock policy test alone; the policy itself must first be approved.

## 4. Decision status — accepted baseline versus unresolved details

The repository's `PRODUCTION_IDENTITY_AUTHORIZATION_DESIGN_GATE.md` records accepted owner decisions DEC-0020 through DEC-0028. This test plan must honor those decisions rather than reopening them as if undecided.

| Decision | Accepted baseline | Still open / must not be inferred |
|---|---|---|
| Unknown external identity | No implicit platform access; fail closed. Do not create Person/membership or grant roles merely because an external identity authenticated. | Exact invitation/onboarding/account-linking workflow. |
| Disabled/revoked Person | Deny before establishing a trusted execution context; no fail-open fallback. | Authoritative lifecycle source and maximum revocation-staleness budget. |
| Zero eligible membership | Deny; identity alone does not establish tenant/context access. | Exact membership eligibility predicates for each resource/action. |
| Multiple eligible memberships | Never arbitrarily choose first/default membership; fail closed absent an approved selection. | User experience and server-bound context-selection protocol. |
| Relationship versus permission | Relationship is policy input, not an automatic grant. Evaluate actor, tenant, membership/role, relationship, resource, action and context. | Exact guardian/consent policies and jurisdiction-specific rules. |
| Resolver outage/indeterminate state | Fail closed for protected actions. | Safe external error mapping and operational alerting details. |
| External account key | Provider-neutral identity maps to internal Person; provider claims do not become application authority. | Account-linking, migration, and selected provider protocol/configuration. |
| Assignment close | Owner accepted Option B: require eligible membership in the assignment's learning context plus an explicit resource/action policy grant. | Exact membership eligibility, resolver contract and implementation remain open; current coarse `assignment:close` check is not sufficient production proof. |
| Assignment create/learner submit | Apply contextual policy and server-derived tenant rules; never trust client-supplied tenant as authority. | Whether membership is required for each operation and the exact eligibility predicates remain to be specified.

## 5. Explicitly deferred

Do not decide through these tests:
- final provider/commercial plan;
- account linking, invitations, recovery or automatic provisioning;
- student age, guardian consent, relationship verification and jurisdiction policy;
- provider session, refresh-token, MFA or passkey flow;
- membership/identity database schema and physical tenant isolation;
- audit retention, deletion and cross-border data-residency policy;
- production implementation authorization.

## 6. TDD entry criteria and next authorized design step

The accepted security baseline plus the owner's 2026-10-09 approval of assignment-close Option B are sufficient to specify that policy's contract tests. They are not sufficient to silently choose onboarding, account-linking, consent, revocation freshness, membership-selection UX, or exact membership eligibility predicates.

Next:
1. Define the smallest provider-neutral resolution result categories and port responsibilities; keep names explicitly provisional until reviewed.
2. Specify deterministic tests for unknown identity, disabled/revoked Person, zero/one/multiple memberships, policy denial, resolver outage, server-derived tenant, and no trusted context on denial.
3. Separate unit contract tests from API integration tests and real-provider cryptographic verification.
4. If executable RED tests are added, ensure the test harness compiles and classify the intended behavioral failures clearly. Do not merge a knowingly red default branch without an agreed test-branch/CI strategy.
5. Only after the contract is reviewed and implementation is explicitly authorized, add GREEN behavior and production adapter integration.

**Current result:** Accepted authorization principles are the baseline; the exact provider-neutral port, membership eligibility rules, onboarding/account-linking, lifecycle freshness and context-selection details remain open. No provider, schema, or production runtime behavior is authorized by this plan.


## 7. Accepted assignment-close contract cases

The Project Owner accepted close-policy Option B on 2026-10-09. These cases are now required test outcomes, not recommendations. They remain test specifications; no runtime implementation is claimed.

| Test ID | Given | When | Expected result | Required side-effect assertion |
|---|---|---|---|---|
| CLOSE-POLICY-001 | Actor has no eligible membership in the assignment's learning context | Actor requests close with a generic `assignment:close` authority | Deny | Assignment state/version and audit/outbox mutation contract are preserved; no close mutation |
| CLOSE-POLICY-002 | Actor has eligible context membership but policy does not grant close | Actor requests close | Deny | Assignment remains unchanged; denial does not leak cross-context details |
| CLOSE-POLICY-003 | Actor has eligible context membership and explicit close grant for this resource/action | Actor requests close with valid expected version and idempotency key | Allow, subject to existing domain validation | Exactly one authoritative close mutation; actor, tenant, context and correlation remain attributable |
| CLOSE-POLICY-004 | Actor has membership only in another learning context within the same tenant | Actor requests close | Deny | No close mutation; same-tenant membership is not treated as target-context membership |
| CLOSE-POLICY-005 | Membership/policy resolver is unavailable or returns indeterminate state | Actor requests close | Fail closed with safe operational outcome | No generic-authority fallback and no assignment mutation |
| CLOSE-POLICY-006 | Actor lacks the coarse `assignment:close` authority | Actor requests close even if a fake policy would otherwise allow | Deny | Defense-in-depth remains; the new policy does not bypass existing required authority checks |
| CLOSE-POLICY-007 | Actor is in the right context and has close policy grant, but assignment belongs to a different tenant | Actor requests close | Reject using the approved non-disclosure response | No cross-tenant state change |
| CLOSE-POLICY-008 | Actor is authorized and sends a retry with the same idempotency key and same request | Retry after successful close | Replay the original result according to the existing idempotency contract | No duplicate close/audit/outbox effect |
| CLOSE-POLICY-009 | Actor is authorized but supplies a stale expected version | Actor requests close | Concurrency conflict | No overwrite of newer state; existing conflict semantics remain intact |

### Test-layer placement

- **Policy/contract tests:** CLOSE-POLICY-001 through 006 with deterministic resolver and policy fakes.
- **API integration tests:** prove the policy is actually invoked by `POST /api/v1/assignments/{assignmentId}/close`, including the no-mutation outcomes and existing error semantics.
- **PostgreSQL integration tests:** required once durable membership/policy inputs are designed and approved; do not pretend in-memory fakes prove persistence isolation.
- **Security/provider tests:** separate; these cases do not prove external credential cryptographic validation.

### Explicit implementation blocker

The current close endpoint checks only trusted execution context, the coarse `assignment:close` authority, and tenant equality before calling the close use case. It does not currently resolve target-context membership or evaluate a resource-specific close grant. These cases therefore describe a known behavioral gap; they must not be marked passing until executable tests and the implementation exist. Do not silently invent membership tables or production identity-provider behavior to make the tests pass.



## 8. Recommended application seam for the accepted close policy

**Recommendation:** introduce a narrow Application-layer authorization port for assignment close rather than placing membership rules only in the HTTP endpoint or adding them directly to the persistence store.

Conceptual flow:

`API trusted ExecutionContext + Assignment → Application close-authorization port → explicit decision → existing close mutation`

The port should receive the authenticated internal principal and the persisted assignment (including its tenant and learning-context IDs). It should not receive raw bearer tokens, provider SDK objects, or trust client-supplied tenant/context values. The policy decision should distinguish at least:
- **Allowed** — eligible target-context membership and explicit close grant are both established.
- **Denied** — a known policy/membership condition fails.
- **Indeterminate/unavailable** — membership or policy state could not be established; fail closed.

The HTTP layer should map those outcomes to approved safe responses; exact public error mapping remains an API decision. Domain/Application should not depend on HTTP status codes or provider-specific claims.

### Ordering and replay safety

The close authorization decision must happen **before** the close mutation and before any idempotency replay is returned to the caller. A previously successful idempotency key must not become a way for an actor whose access has since been revoked to retrieve a protected response. Once authorization succeeds, the existing idempotency, expected-version/concurrency, audit and outbox guarantees must remain intact.

### Why not put the check only in the API or store?

- **API-only check:** risks bypass if the same use case is invoked by another entry point.
- **Persistence-store check:** mixes access policy with data mutation/persistence and is harder to test independently.
- **Application port:** centralizes the use-case authorization boundary and supports deterministic fake implementations in contract tests while keeping the policy implementation provider-neutral.

This is a design recommendation, not an implemented source contract. Before GREEN, define the port/result type and register an implementation for each environment; do not silently substitute an allow-all production implementation.
