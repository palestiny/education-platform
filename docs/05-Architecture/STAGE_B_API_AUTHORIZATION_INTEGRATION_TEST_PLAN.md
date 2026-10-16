# Stage B — API Authorization Integration Test Plan

**Status:** Test design for review — no production provider or membership adapter is implemented by this plan.  
**Date:** 2026-10-16  
**Branch:** `feat/stage-b-resolution-port-contract`

## 1. Purpose

Turn the known production identity gaps into observable API acceptance tests. These tests must run against the API pipeline, not only call Application resolver methods with fakes. Provider-specific cryptographic tests are a separate suite and can only be finalized after a provider is selected.

Do not change production authentication from fail-closed to permissive to make a test pass.

## 2. Test layers

1. **Application contract tests:** deterministic fake ports verify resolver orchestration and fail-closed outcomes.
2. **API integration tests:** run the real HTTP pipeline and DI configuration, with controlled test adapters for identity/membership/policy.
3. **Provider adapter tests:** after provider selection, verify signature, issuer, audience, expiry/not-before, key rotation, malformed credentials, and configuration failure using the selected provider's supported validation stack.
4. **Durable integration tests:** after the membership/policy persistence model is approved, verify tenant isolation, revocation, transaction boundaries and concurrent updates against the actual database.

A fake adapter can test API composition and HTTP mapping; it cannot prove cryptographic credential validation or production provider configuration.

## 3. API acceptance matrix

| Scenario | Request identity / context | Expected outcome | Required invariant |
|---|---|---|---|
| Missing credential | No Authorization header | 401 | No trusted context and no protected mutation |
| Malformed/invalid credential | Invalid token through test adapter | 401 | No provider error detail leaks |
| Valid identity, unknown platform person | Validated external identity with no mapping | Safe denial; default 403 unless approved non-disclosure mapping requires otherwise | No implicit person or membership creation |
| Person disabled/revoked | Known identity, inactive lifecycle | 403 or approved safe denial | No protected mutation |
| No eligible membership | Active person, no membership for action/context | 403 or approved non-disclosing response | No tenant/context from client is trusted |
| Multiple eligible memberships | More than one eligible context and no valid server-bound selection | Safe denial until selection is resolved | Never select the first membership arbitrarily |
| Wrong tenant/context | Identity is valid but requested resource is outside eligible membership | 403 or 404 per endpoint disclosure policy | No cross-tenant reads/writes |
| Authority string without resource permission | Broad role/authority but policy denies specific resource/action | 403 | Role alone is not a grant |
| Guardian relationship only | Verified relationship but policy denies requested operation | 403 | Relationship alone is not permission |
| Learner submits as another learner | Principal does not match assignment learner | Safe denial | No submission or idempotency replay disclosure |
| Policy dependency unavailable | Policy returns Indeterminate | 503 or approved safe denial mapping | Never treat unavailable as allow |
| Membership source unavailable | Membership resolver returns Indeterminate | 503 or approved safe denial mapping | No trusted execution context |
| Authorized contextual action | Active person, one eligible membership, explicit policy allow | Success | Tenant/context derived server-side |
| Revoked membership before request | Membership revoked before protected request | Denied | Revocation observed within approved freshness objective |
| Revocation races protected mutation | Revocation concurrent with write | Result conforms to the approved consistency model | No claim of atomicity without proof |
| Protected idempotency replay after revocation | Previously successful request replayed after access revoked | Denied; no protected response body | Authorization checked before replay disclosure |
| Auditability | Successful and denied high-impact requests | Correlated audit evidence | Actor, tenant/context, action and outcome recorded without credentials |
| Secret hygiene | Any tested request path | No token/password in app logs, audit, outbox or persistence | Credentials never cross the Application boundary |

The precise HTTP code for unknown person, wrong tenant, and unavailable dependencies is subject to the API's approved non-disclosure and availability policy. The security invariant is the same regardless of mapping: no access and no protected mutation.

## 4. Mutation assertions

For every denied or indeterminate write path, snapshot the durable state before and after the request. Assert no unexpected change to:
- assignments;
- submissions;
- idempotency records;
- audit/outbox records, except an explicitly approved security-denial audit event;
- membership/person rows (no implicit provisioning).

Do not assert that all audit rows must remain unchanged: security denial events may be expected. Instead assert that no business mutation occurred and that any security audit event contains no credential material.

For protected idempotency replay, assert both that the response is denied and that sensitive cached response content is not returned.

## 5. First executable slice

Before a real provider is selected, the smallest useful API composition suite should prove:
1. production configuration with no approved credential adapter remains fail-closed;
2. API DI invokes the provider-neutral resolver only when a validated identity is supplied by a trusted adapter;
3. missing identity, denied policy, and indeterminate membership/policy cannot create a trusted execution context;
4. assignment creation, learner submission, and assignment close reject unauthorized contexts without business-state mutation;
5. cancellation and dependency failure do not become an allow;
6. audit assertions distinguish expected security-denial records from forbidden business writes.

Tests should be written RED first. A compile failure is not acceptable evidence of a behavioral RED test; the test harness must compile before the behavioral assertion can be evaluated.

## 6. Entry criteria before implementation

Do not build the durable production adapters until the owner has approved:
- identity provider operating model and credential/session flow;
- account registration, linking, invitation and recovery rules;
- membership and context eligibility per operation;
- authoritative lifecycle source and maximum revocation staleness;
- relationship verification and guardian/learner policy;
- tenant isolation and data retention/residency requirements;
- authorization-versus-mutation consistency model;
- safe HTTP response mapping for unknown identities and cross-tenant resources.

## 7. Exit criteria

The Stage B API integration gate can pass only when:
- provider adapter tests verify actual credential validation under approved configuration;
- the API uses durable person, lifecycle, membership and policy sources;
- all denied/indeterminate write cases prove no business mutation or protected replay disclosure;
- revocation behavior meets the approved objective;
- audit/secret-hygiene checks pass;
- the exact candidate commit passes full CI, including build, unit/application/integration tests, EF migration validation and migration-script generation.

**Current result:** Design/test-plan work only. Production identity integration remains incomplete and fail-closed.
