# Stage B — Identity & Membership Contract Test Plan

**Project:** Education Platform  
**Status:** READY FOR OWNER REVIEW — NO SOURCE CONTRACTS OR IMPLEMENTATION AUTHORIZED  
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

## 4. Owner decision worksheet — recommended defaults

| Decision | Recommended default | Why | Still needs owner confirmation |
|---|---|---|---|
| Unknown external identity | Reject with a safe non-privileged outcome; onboarding/invitation is a separate flow | Prevents silent account/tenant creation | Yes |
| Disabled/revoked Person | Deny before context establishment; no stale privileged fallback | Makes lifecycle enforcement fail closed | Yes: source of truth and revocation freshness budget |
| Zero eligible membership | Deny | Identity alone does not imply tenant access | Yes |
| Multiple eligible memberships | Deny as ambiguous until an explicit server-bound context selection is defined | Avoids arbitrary tenant selection | Yes: UX and selection semantics |
| Relationship versus permission | Evaluate action/resource policy; relationship is evidence/input, not a universal grant | Protects student/minor data and prevents role overreach | Yes: exact first-slice policies and legal/consent constraints |
| Resolver outage | Deny protected action and return safe operational error | Avoids fail-open behavior | Yes: public status/error semantics |
| Stable external account key | Trusted `(issuer, subject)` mapped to internal Person ID | Avoids mutable email/display-name identity | Yes: account linking and migration policy |

## 5. Explicitly deferred

Do not decide through these tests:
- final provider/commercial plan;
- account linking, invitations, recovery or automatic provisioning;
- student age, guardian consent, relationship verification and jurisdiction policy;
- provider session, refresh-token, MFA or passkey flow;
- membership/identity database schema and physical tenant isolation;
- audit retention, deletion and cross-border data-residency policy;
- production implementation authorization.

## 6. TDD entry criteria

Before converting this plan into executable RED tests:
1. Owner confirms or revises the recommended defaults above for the first protected learning journey.
2. Product/security decisions define the precise relationship and resource/action policy under test.
3. The minimal provider-neutral port and result semantics are reviewed for fit with existing `ExecutionContext`.
4. Tests are written to fail for the intended missing behavior—not because the test harness cannot start or compile.
5. RED evidence is linked to exact-head CI before any GREEN implementation begins.

**Current result:** Stage B test plan is prepared. No new runtime behavior is claimed. Production authentication implementation remains unauthorized.
