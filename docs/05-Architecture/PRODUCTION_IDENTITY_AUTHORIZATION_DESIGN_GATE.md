# Production Identity & Authorization Design Gate

**Status:** OPEN — DESIGN DECISIONS CLOSED FOR FIRST IMPLEMENTATION BOUNDARY  
**Date:** 2026-10-07  
**Decision owner:** Project Owner

## Boundary

HTTP Authentication → Application Execution Context → Tenant Context → Authorization Policy → Use Case

## Decisions

### DEC-0020 — Provider-neutral production authentication adapter
ACCEPTED. Provider-specific OIDC/OAuth2/JWT validation and SDK objects remain inside the API/infrastructure adapter. Application use cases remain provider-neutral.

### DEC-0021 — Authentication is distinct from authorization
ACCEPTED. Authentication establishes identity; authorization decides whether that identity may perform an operation against a resource in its current context. A valid token is never sufficient by itself.

### DEC-0022 — Person-centric identity, contextual membership
ACCEPTED. A person is the stable identity. Organization membership, roles, relationships and permissions are contextual. Do not model the complete authorization system as one global role claim.

### DEC-0023 — Server-derived tenant context
ACCEPTED and already CI verified. Client-supplied tenant identifiers are never authoritative.

### DEC-0024 — Relationship is not permission
ACCEPTED. Parent-of, teacher-of, learner-of and similar relationships are facts consumed by policy; they do not automatically grant unrestricted access.

### DEC-0025 — Policy-based authorization
ACCEPTED. Authorization evaluates principal, tenant/organization, membership/role, relationship, resource, action and relevant business/learning context. ExecutionContext authorities are bounded application inputs, not the entire authorization source of truth.

### DEC-0026 — Fail closed
ACCEPTED. Missing identity, invalid credentials, ambiguous membership or authorization failure never degrades into access. Default outcomes are 401 for unauthenticated and 403 for authenticated-but-unauthorized, with approved non-disclosure responses where required.

### DEC-0027 — Credential lifecycle
ACCEPTED. Access-token/session lifecycle is an infrastructure concern. Raw bearer, refresh tokens and passwords are never persisted by the application.

### DEC-0028 — Security audit without secrets
ACCEPTED. Security-sensitive authorization outcomes and high-impact mutations remain attributable through audit/correlation mechanisms without storing credential material.

## Authorization model

Principal → Membership → Role/Policy → Relationship/Context → Resource → Action

Examples:
- Teacher can create an assignment only when authorized for its learning context.
- Learner can submit only for an accessible assignment and authenticated learner identity.
- Parent relationship does not automatically authorize learner-record mutation.
- Organization administration is scoped and does not imply platform-owner authority.

## Production adapter responsibilities

1. Validate credentials according to the selected protocol/provider.
2. Establish authenticated principal identity.
3. Validate issuer, audience, signature, expiry and relevant credential constraints.
4. Resolve or validate platform membership context.
5. Map provider identity to platform identity.
6. Produce the provider-neutral execution context.
7. Fail closed on invalid or ambiguous context.

The adapter must not implement learning business rules.

## Deferred choices

- exact IdP/provider;
- managed vs self-hosted identity service;
- claim-to-platform identity mapping;
- refresh/session strategy;
- MFA/passkeys;
- account recovery;
- invitation/onboarding;
- age/guardian policy;
- jurisdiction-specific consent/privacy;
- emergency/support access.

No provider is selected by this gate.

## Required security verification

Before production authorization is considered implemented, CI must prove:
1. missing/invalid credential → 401;
2. valid identity without authority → 403;
3. wrong tenant/membership → rejected;
4. relationship without permission → rejected;
5. authorized operation → succeeds;
6. disabled/revoked identity cannot perform protected mutation;
7. provider-specific credential data does not cross the application boundary;
8. protected mutations retain actor/tenant/correlation auditability;
9. raw credentials are not persisted;
10. existing idempotency, concurrency, audit and outbox guarantees remain green.

## Gate result

**Design Gate: PASS for the provider-neutral boundary.**

**Implementation authorization: NOT YET GRANTED.**

A concrete provider/session strategy and executable RED tests are required before production authentication implementation is declared complete.