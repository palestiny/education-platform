# Production Authentication Adapter — TDD RED Specification

**Project:** Education Platform  
**Status:** SPECIFICATION READY — RED EXECUTION NOT YET VERIFIED  
**Prerequisite:** Production Identity & Authorization Design Gate PASS; provider strategy remains CONDITIONAL PASS.  
**Scope:** Provider-neutral contract and executable security behavior only. This document does not authorize adding an Entra SDK, production credentials, or a production login flow.

## 1. Purpose

Prove the observable boundary between HTTP authentication, provider-specific verification, platform identity resolution, tenant/membership context, authorization, and protected mutations before implementing the production adapter.

Canonical flow:

`Browser / Mobile → Managed CIAM → Provider Adapter → Platform Person Mapping → Membership Context → Authorization Policy → Use Case`

Application and Domain must not depend on provider SDK types, token formats, claim names, or provider-specific user objects.

## 2. Decisions and boundaries

- **Authentication is not authorization.** A valid external identity does not imply a platform membership or permission.
- **Platform identity is canonical.** External subject/issuer identifiers map to a stable internal `PersonId`; provider identifiers are not used as tenant authority.
- **Tenant context is server-derived.** Never trust a tenant ID supplied by a request body, query, or unvalidated claim.
- **Membership is contextual.** A person may have multiple memberships; authorization must resolve the membership appropriate to the protected resource and requested action.
- **Fail closed.** Invalid, missing, ambiguous, disabled, or revoked identity/context cannot fall through to anonymous or privileged access.
- **No credential persistence.** Raw access/refresh tokens, passwords, authorization headers, and provider secrets must not be written to application tables, audit records, logs, or outbox payloads.
- **Auditability is preserved.** Successful protected mutations identify the internal actor, authorized tenant/context, operation, correlation ID, and result.
- **Provider adapter placement:** provider-specific SDK and protocol handling belongs in the Infrastructure/API edge. Application consumes only provider-neutral identity and execution-context contracts.
- **Current test bearer resolver remains Development/Testing-only** and is not evidence of production authentication.

## 3. RED scenario matrix

Each scenario must have an executable test with a clear failure reason. A test-harness/build failure is not valid behavioral RED evidence.

| ID | Scenario | Expected contract |
|---|---|---|
| AUTH-RED-001 | No credential on protected mutation | HTTP 401, `AUTHENTICATION_REQUIRED`; no mutation |
| AUTH-RED-002 | Malformed, expired, wrong-issuer, wrong-audience, or invalid-signature credential | HTTP 401; no mutation; safe response contains no credential detail |
| AUTH-RED-003 | Valid external identity with a known platform Person mapping | Adapter yields provider-neutral authenticated principal; no provider object crosses the boundary |
| AUTH-RED-004 | Valid external identity with no platform Person mapping | Explicit rejected/provisioning-required outcome; never silently create a privileged person |
| AUTH-RED-005 | Disabled or revoked platform Person | Denied before protected mutation; no stale identity grants access |
| AUTH-RED-006 | Authenticated Person with no active membership in the resource tenant | HTTP 403 or approved non-disclosure response; no mutation |
| AUTH-RED-007 | Person has membership in Tenant A but attempts a Tenant B resource | Rejected; no cross-tenant read or mutation |
| AUTH-RED-008 | Guardian/student or teacher/student relationship exists but requested action is not permitted by policy | HTTP 403 or approved non-disclosure; relationship alone grants no permission |
| AUTH-RED-009 | Request contains a forged tenant ID while authenticated membership resolves to another tenant | Server-derived tenant wins; forged tenant is ignored/rejected; no cross-tenant mutation |
| AUTH-RED-010 | Authorized protected mutation succeeds | Mutation and audit use internal PersonId, server-derived tenant/context, operation, correlation ID, and resulting status |
| AUTH-RED-011 | Inspect persistence/log/audit/outbox after auth flow | No raw token, refresh token, password, Authorization header, or provider secret persisted |
| AUTH-RED-012 | Provider/identity resolver unavailable or returns ambiguous membership | Fail closed; no fallback to test bearer or anonymous privilege; no mutation |
| AUTH-RED-013 | Same idempotency key retried by same authorized actor/request | Original result replayed; exactly one authoritative mutation and audit/outbox contract retained |
| AUTH-RED-014 | Stale expected version or concurrent protected mutation | Existing concurrency contract remains enforced; stale write rejected |
| AUTH-RED-015 | Existing durable persistence/failure atomicity suite | Idempotency, audit, outbox, migration alignment and rollback tests remain green |

## 4. Contract shape to validate before implementation

Names are proposals for the RED design, not approved source-code symbols:

- `IExternalIdentityAuthenticator`: validates the incoming authentication mechanism and returns a safe provider-neutral authentication result.
- `ExternalIdentity`: immutable tuple of trusted issuer and subject identifiers after credential validation.
- `IPlatformIdentityResolver`: maps a trusted external identity to an internal Person and checks account state.
- `IExecutionContextAccessor`: exposes only the server-established principal, tenant and bounded authorities/context to application use cases.
- `IAuthorizationPolicyEvaluator`: evaluates action + resource + person + active membership + relevant relationship/business context.

Before writing tests, confirm whether existing `IExecutionContextAccessor` and `ExecutionContext` can support the scenarios without turning a flat authority set into the entire authorization model. Do not add speculative interfaces or provider-specific packages merely to satisfy this document.

## 5. Execution protocol

1. Inspect the current branch implementation and test conventions.
2. Map each RED scenario to an existing or proposed test file and identify required test doubles.
3. Build the test harness first; prove it compiles and can execute.
4. Run targeted tests and capture the actual failure reasons.
5. Classify each failure as intended behavioral RED, harness defect, build defect, or unrelated regression.
6. Fix harness defects without implementing the production behavior under test.
7. Record RED evidence in the checkpoint only after CI confirms the intended failures.
8. Only after a valid RED result and explicit implementation authorization may GREEN begin.

## 6. Explicit non-goals

- Selecting/committing the final provider contract or commercial plan.
- Implementing Microsoft Entra SDK/OIDC middleware.
- Deciding password, refresh-token, session, passkey, MFA, recovery or social-provider UX.
- Defining legal age verification, guardian consent or jurisdiction-specific privacy policy.
- Automatically provisioning unknown external identities.
- Introducing microservices or a second identity database.
- Treating mock/unit tests as proof of real provider configuration or production deployment.

## 7. Exit criteria

**RED is verified only when** the required tests compile and run in CI, each expected behavioral failure is distinguished from harness/build failure, and the evidence is linked to the exact commit/run. This document alone does not mean RED has run or passed.

**Current result:** Specification authored; RED execution pending. Production authentication implementation remains unauthorized until provider validation and the implementation gate are explicitly closed.
