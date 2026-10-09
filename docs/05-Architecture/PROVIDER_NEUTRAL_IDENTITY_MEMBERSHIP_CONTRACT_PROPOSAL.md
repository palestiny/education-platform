# Provider-Neutral Identity & Membership Contract Proposal

**Project:** Education Platform  
**Status:** CONTRACT DIRECTION PROPOSED — ASSIGNMENT-CLOSE POLICY B ACCEPTED; OTHER DETAILS OPEN  
**Date:** 2026-10-09  
**Related:** `PRODUCTION_IDENTITY_AUTHORIZATION_DESIGN_GATE.md`, `PRODUCTION_AUTHENTICATION_ADAPTER_TDD_RED_SPECIFICATION.md`

## 1. Why this proposal exists

Stage A API boundary checks now pass, including a Production-host test proving the fixed Development/Testing bearer credential is not accepted in Production. The next gap is not another token-header test: the current domain has `Assignment` and `Submission`, while Application security exposes only a trusted `ExecutionContext(PrincipalId, TenantId, Authorities)`.

There is currently no inspected domain/application contract for platform Person identity, external issuer/subject mapping, active membership selection, account disable/revocation, or relationship-aware authorization. This proposal defines the smallest candidate boundary to review before executable Stage B contract tests. It does not add source-code types or authorize production authentication.

## 2. Non-negotiable invariants

1. **External identity is not platform identity.** A validated external issuer/subject pair maps to a stable internal `PersonId`.
2. **No implicit provisioning.** Unknown external identities return an explicit non-privileged outcome; they do not create an account or membership automatically.
3. **Authentication is not authorization.** A valid identity without an eligible active membership/policy outcome cannot execute a protected operation.
4. **Tenant is server-derived.** Client-supplied tenant identifiers and untrusted claims never establish tenant scope.
5. **Membership is contextual.** One person may hold several memberships; selection must be tied to the requested resource/action, not a global default role.
6. **Relationship is not permission.** Guardian/student or teacher/student relationship facts are inputs to policy, not grants by themselves.
7. **Fail closed on uncertainty.** Unknown, disabled, revoked, ambiguous or unavailable identity/membership state cannot fall back to test credentials or anonymous privilege.
8. **Provider-neutral application boundary.** No raw tokens, provider SDK types, raw claims, or provider-specific user objects enter Domain/Application contracts.
9. **Audit identifies the platform actor.** Successful protected mutations use internal PersonId, resolved tenant/context, operation, correlation ID and outcome.
10. **No raw credential persistence.** Tokens, passwords, Authorization headers and provider secrets must not enter application tables, audit, logs or outbox payloads.

## 3. Candidate contract boundary

These are conceptual responsibilities, not approved class/interface names.

| Responsibility | Candidate input | Candidate output | Required failure behavior |
|---|---|---|---|
| Credential verification (API/Infrastructure edge) | Incoming credential + trusted verifier configuration | Validated external identity or unauthenticated result | Invalid/missing/unavailable verifier → no authenticated context |
| External identity mapping (Application port) | Validated issuer + subject | Known internal Person reference or explicit Unknown | Unknown → reject / provisioning-required outcome; no silent creation |
| Person lifecycle check | Internal Person reference | Active / Disabled / Revoked / Unknown | Any non-active or unknown state → deny |
| Membership resolution | Person + target resource/action context | One eligible active membership, none, or ambiguous | None → deny; ambiguous → deny until a product-owned selection rule exists |
| Authorization policy | Person + membership + action + resource + relevant relationship/context | Allow / Deny with safe reason code | Default deny; relationship alone never allows |
| Execution context establishment | Authenticated Person + resolved membership + bounded operation context | Trusted current execution context | Only constructed after prior steps succeed |

The current `ExecutionContext` should remain a downstream trusted context, not a token-validation object. Whether it needs a context identifier or membership identifier should be decided from concrete use-case requirements rather than added pre-emptively.

## 4. Identity and membership semantics requiring a decision

### 4.1 External identity key

**Recommendation:** identify an external account by the pair `(issuer, subject)`, with exact issuer normalization defined by the selected provider adapter. Do not use email, display name, phone number or a mutable username as the stable identity key.

**Trade-off:** issuer/subject is stable within the issuer contract, but account migration/linking across providers requires an explicit linking and recovery policy.

### 4.2 Unknown Person

**Recommendation:** return a typed non-privileged outcome such as `UnknownIdentity`; do not auto-provision a Person, tenant membership, role or guardian relationship.

**Trade-off:** invitation/onboarding is more explicit and may add friction, but avoids account takeover and accidental tenant membership. Any later provisioning flow must be separately authorized and audited.

### 4.3 Disabled/revoked Person

**Recommendation:** check authoritative platform lifecycle state before establishing a usable execution context. Define cache duration and revocation propagation budget before production implementation.

**Open:** source of truth, administrative authority, session/token invalidation behavior, and maximum tolerated revocation delay.

### 4.4 Multiple memberships

**Recommendation:** resolve membership against the target resource/action and return ambiguity when more than one candidate remains. Do not silently choose the first membership or trust a client-selected tenant.

**Open:** user-facing tenant/context selection, whether an explicit selection token is required, and how the selection is bound to a subsequent protected request.

### 4.5 Relationship and permission

**Recommendation:** model relationship facts separately from policy decisions. A guardian relationship may permit a defined read action under valid consent and context, but does not automatically grant record mutation, impersonation, unrestricted access, or organization administration.

**Open:** child age/jurisdiction, consent, relationship verification, revocation, data visibility, emergency access and auditing require product/legal/security decisions.

## 5. Stage B contract-test proposal

Write deterministic tests before implementing adapters or persistence. Use small fakes only to test the contract, and label them contract tests—not production-provider verification.

| Proposed test | Expected outcome |
|---|---|
| Trusted issuer + subject maps to known active Person | Internal Person identifier returned |
| Unknown external identity | Explicit unknown/provisioning-required result; no Person or membership created |
| Disabled or revoked Person | Denied before a usable execution context is established |
| No active membership for target context | Denied |
| Exactly one eligible membership | That membership is selected server-side |
| Multiple eligible memberships with no explicit selection rule | Ambiguous and denied |
| Client tenant conflicts with resolved membership | Client value cannot change resolved tenant |
| Relationship exists but policy does not allow action | Denied |
| Membership exists but resource/action policy denies | Denied |
| Resolver unavailable or returns invalid/ambiguous state | Fail closed; no test-token fallback |
| Allowed operation | Execution context contains only trusted internal identity/context data; no provider claims/tokens |
| Assignment close without eligible membership in the assignment's learning context | Denied; no assignment state mutation |
| Assignment close with context membership but no explicit close-policy grant | Denied; membership alone is not permission |
| Assignment close with eligible context membership and explicit resource/action grant | Allowed, subject to existing tenant, lifecycle, idempotency and concurrency checks |
| Assignment close with membership in a different learning context | Denied; no assignment state mutation |
| Assignment close when membership/policy resolution is unavailable or indeterminate | Fail closed; no fallback to the generic `assignment:close` authority alone |

These tests should initially validate the proposed contracts and decision outcomes. They do not prove token signatures, issuer/audience validation, real provider configuration, revocation propagation, or legal consent semantics.

## 6. Assignment-close policy — owner decision accepted

On 2026-10-09, the Project Owner accepted Option B for the first protected learning journey:

> Closing an assignment requires eligible membership in the assignment's learning context and an explicit authorization policy grant for the close action on that assignment/resource.

Membership or a generic `assignment:close` authority alone is insufficient. A future tenant-wide administrative override must be a separate explicit and auditable policy; this decision does not imply such an override.

This accepted policy is a requirement for the contract and tests. It does not define the membership schema, exact membership eligibility predicate, policy engine, or authorize runtime implementation.

## 7. Options and recommendation

### Option A — Put identity and membership in a single flat authority set
- **Benefit:** fastest initial implementation.
- **Cost/risk:** conflates identity, membership and policy; encourages global role claims; makes multiple memberships and relationship-scoped access unsafe or difficult to reason about.
- **Recommendation:** reject.

### Option B — Provider-neutral identity mapping + membership resolver + policy boundary
- **Benefit:** separates external authentication from internal identity, tenant membership and resource authorization; enables deterministic tests and later provider replacement.
- **Cost/risk:** requires explicit lifecycle, membership-selection and policy contracts before implementation.
- **Recommendation:** preferred design direction, subject to owner review and the unresolved product/security/privacy decisions below.

### Option C — Implement provider-specific claims directly in Application/Domain
- **Benefit:** less adapter code in the short term.
- **Cost/risk:** provider lock-in, untrusted claim leakage, brittle tests, and tenant/role semantics tied to external token shape.
- **Recommendation:** reject.

## 8. Decisions still open

This proposal does **not** decide:
- final identity provider or commercial plan;
- automatic account provisioning or account linking;
- tenant/context selection UX;
- membership persistence/schema or physical tenant-isolation strategy;
- guardian/child consent and data-visibility policy;
- legal age, residency, retention/deletion or jurisdiction policy;
- revocation source of truth and freshness budget;
- emergency/support access;
- production authentication implementation authorization.

## 9. Exit criteria before Stage B GREEN

1. Preserve the accepted assignment-close rule in the contract and test matrix.
2. Define the minimal provider-neutral resolution result semantics and membership eligibility predicates for the first protected journey.
3. Resolve which membership checks apply to assignment creation and learner submission; do not infer them from the close decision.
4. Keep onboarding/account linking, context-selection UX, lifecycle source/revocation freshness, and guardian/consent policy explicitly open.
5. Map each test to an accepted requirement and distinguish intended RED behavior from harness/build failures.
6. Only after a separate explicit implementation authorization may GREEN begin.

**Current gate result:** Stage A boundary verified. Assignment-close policy B is owner-accepted. Provider-neutral identity/membership result semantics and other first-slice membership predicates remain under design. Production authentication and membership implementation remain unauthorized.
