# Stage B — Identity Resolution Port Contract

**Project:** Education Platform  
**Status:** PROPOSED CONTRACT — OWNER REVIEW REQUIRED BEFORE SOURCE IMPLEMENTATION  
**Date:** 2026-10-16  
**Branch:** `feat/stage-b-resolution-port-contract`  
**Inputs:** `STAGE_B_IDENTITY_MEMBERSHIP_CONTRACT_TEST_PLAN.md`, `PROVIDER_NEUTRAL_IDENTITY_MEMBERSHIP_CONTRACT_PROPOSAL.md`, `PRODUCTION_IDENTITY_AUTHORIZATION_DESIGN_GATE.md`

## 1. Purpose

Translate the accepted security baseline into the smallest Application-facing contract needed to resolve a validated external identity into a trusted, contextual platform execution context.

This document proposes a contract seam and testable outcomes. It does **not** select an identity provider, define a database schema, implement login/session flows, authorize automatic account creation, or settle guardian-consent/legal policy.

Names below are conceptual and remain provisional until approved.

## 2. Required sequence and trust boundary

`ValidatedExternalIdentity → InternalPersonResolution → PersonLifecycleCheck → ContextualMembershipResolution → ResourceActionPolicy → TrustedExecutionContext`

The API/Infrastructure authentication adapter alone validates credentials and creates the validated external identity input. Application contracts must never accept raw bearer/refresh tokens, passwords, provider SDK objects, or untrusted raw claims.

The client must not supply authoritative `PersonId`, `TenantId`, membership, role, relationship, learning-context, or permission values. Resource identifiers may be supplied to identify the requested operation, but access scope must be resolved and verified server-side.

Only the final successful outcome may establish the trusted execution context consumed by protected use cases.

## 3. Proposed provider-neutral contract responsibilities

| Port / responsibility | Input | Explicit outcome | Must not do |
|---|---|---|---|
| External identity mapping | Validated issuer + subject | Known internal person / unknown identity / unavailable | Create a person or membership implicitly |
| Person lifecycle resolution | Internal person reference | Active / disabled / revoked / unknown / unavailable | Treat missing or indeterminate state as active |
| Contextual membership resolution | Person + requested resource/action context | One eligible membership / none / ambiguous / unavailable | Trust client tenant or choose the first membership |
| Resource/action authorization | Internal actor + resolved membership + resource + action + relevant relationship/context | Allowed / denied / indeterminate | Infer permission from role label or relationship alone |
| Trusted context establishment | Successful prior results | Provider-neutral execution context | Run after any failed, ambiguous, or indeterminate stage |

These responsibilities may be composed behind one Application service if that keeps the contract smaller. Do not create one interface per row automatically; choose the minimum seam that permits deterministic tests and keeps provider-specific behavior outside Application/Domain.

## 4. Required externally distinguishable outcomes

At the internal contract boundary, preserve enough information to distinguish:

- `UnknownIdentity`: the validated external identity has no internal person mapping.
- `PersonInactive`: the mapped person is disabled, revoked, or otherwise not eligible.
- `NoEligibleMembership`: no membership applies to the requested resource/action context.
- `AmbiguousMembership`: multiple memberships remain and no approved selection protocol resolves them.
- `Denied`: a known policy rule rejects the requested action.
- `Indeterminate`: a required dependency or policy result cannot be established.
- `AuthorizedContext`: all required resolution and policy checks succeeded.

These are semantic categories, not approved C# enum names. Avoid representing all denial paths as an empty identity or empty authority collection; doing so erases important security and operational distinctions.

HTTP response mapping is a separate API contract. Do not expose sensitive account-existence or cross-tenant details where the approved non-disclosure policy requires a uniform response.

## 5. Invariants to encode in tests

1. A valid external identity does not itself grant platform access.
2. Unknown identity never causes implicit Person, tenant membership, role, or relationship creation.
3. Disabled, revoked, unknown, or unavailable lifecycle state never establishes a trusted context.
4. Zero eligible memberships denies; multiple unresolved memberships fail closed.
5. Tenant/context authority is server-derived; conflicting client values cannot change the resolved scope.
6. Relationship facts are policy inputs, never standalone grants.
7. Only an explicit allow for the requested resource/action can establish an authorized context.
8. Resolver/policy exceptions and indeterminate outcomes fail closed; no development/test credential fallback is allowed in Production.
9. Raw credential material and provider-specific claims/objects do not cross into Application/Domain or enter logs, audit, outbox, or persistence.
10. Cancellation propagates and must not be converted into a permissive or generic authorization result.
11. Protected operations re-evaluate authorization before mutation and before returning protected idempotency replay data.
12. The current assignment-close contract remains enforced; this proposal does not silently change assignment-create or learner-submit rules.

## 6. Test layers and proof boundaries

### Application contract tests with deterministic fakes

Cover known/unknown person, inactive person, zero/one/multiple memberships, policy denial, resolver unavailability, conflicting client tenant, cancellation, and successful trusted-context construction. Assert that denied or indeterminate outcomes produce no trusted context and no protected mutation.

### API integration tests

Prove the authentication boundary actually invokes the resolver and that protected endpoints return the approved safe response without mutation for denied/unavailable contexts.

### Provider adapter tests

Only after a provider is selected, test issuer, audience, signature, expiry, key rotation, and provider-specific configuration against the real adapter/test environment. Contract fakes do not prove cryptographic validation.

### Durable integration tests

Only after membership persistence and isolation are designed and approved, test tenant isolation, membership revocation behavior, transactional consistency, and concurrent changes against the chosen database.

A passing contract test proves the specified fake-backed boundary behavior only. It does not prove a production provider, membership store, policy engine, revocation freshness, or atomic authorization/mutation.

## 7. Explicitly open decisions

Do not infer or implement these from this proposal:

- identity provider and production credential/session strategy;
- account linking, invitation, recovery, and onboarding workflows;
- person lifecycle source of truth and maximum revocation staleness;
- membership eligibility predicates for each operation;
- context-selection UX and server-bound selection protocol;
- guardian/learner relationship verification, age, consent, and jurisdiction rules;
- membership schema, physical tenant isolation, audit retention, deletion, and data residency;
- public error mapping where account/resource disclosure is a concern;
- atomicity guarantees between authorization evaluation and persistence mutation.

## 8. Recommended next implementation step

After owner review of this contract:

1. Add a focused Application contract test file with deterministic fake resolvers and explicit outcome assertions.
2. Make the test harness compile; classify expected behavioral failures as RED rather than accepting compiler errors as behavioral evidence.
3. Implement the minimum provider-neutral contract and orchestration required by the tests.
4. Run the full solution build, unit/application/integration tests, EF migration validation, and migration-script generation in CI.
5. Update the readiness matrix and checkpoint with exact-head evidence.
6. Keep Production authentication fail-closed until a real provider adapter and approved membership/policy sources exist.

**Gate status:** This proposal narrows the contract for review; it does not pass the production identity implementation gate or authorize a provider-specific integration.
