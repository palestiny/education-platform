# Production Authentication Adapter — Executable Implementation Plan

**Project:** Education Platform  
**Status:** STAGE A VERIFIED — STAGE B CONTRACT DESIGN OPEN — implementation authorization still required  
**Related gate:** `docs/05-Architecture/PRODUCTION_IDENTITY_AUTHORIZATION_DESIGN_GATE.md`  
**RED specification:** `docs/05-Architecture/PRODUCTION_AUTHENTICATION_ADAPTER_TDD_RED_SPECIFICATION.md`

## 1. Purpose and guardrails

Turn AUTH-RED-001 through AUTH-RED-015 into a controlled test-first sequence without prematurely selecting or wiring a production identity provider.

- Keep the current test bearer resolver Development/Testing-only.
- Do not add provider SDKs, production secrets, login endpoints, automatic account provisioning, or provider-specific types to Application/Domain in this plan-only stage.
- Do not treat mocks as proof of real issuer, audience, signature, revocation, or deployment configuration.
- Keep `ExecutionContext` a trusted, already-resolved platform context. Never place raw tokens or provider claims into it.
- Preserve the current idempotency, concurrency, audit, outbox, PostgreSQL migration and failure-atomicity regression suite.

## 2. Recommended implementation sequence

### Stage A — Boundary tests that can be verified against the current API

**Scope:** Existing HTTP boundary; no production identity implementation.

1. AUTH-RED-001: missing credential returns 401 and does not mutate.
2. AUTH-RED-002 (limited current-path check): unknown/malformed test credentials and unsupported schemes return 401. This is not signed-token validation.
3. AUTH-RED-007/009: cross-tenant resource access and forged request tenant cannot override the server-resolved tenant.
4. Run the full test suite and PostgreSQL integration suite.
5. Confirm the test bearer resolver is registered only in Development/Testing and cannot be enabled by a production configuration accident.

**Exit evidence (achieved for current boundary scope):** CI runs `37852804227` and `37852813185` passed on tested commit `9d9778114a562c8093ce0652075aecd2f03491a2`. The Production-host test proves the fixed Development/Testing bearer credential is not accepted in Production; the host test receives the required connection string without relaxing production startup validation. These checks are not production signed-token verification.

### Stage B — Provider-neutral identity contracts (design and tests before implementation)

Do not introduce names just because they appear in the RED spec. First review existing domain/application models and agree the minimum contracts.

1. Trusted external identity is represented by validated issuer + subject; no unvalidated claim can instantiate a trusted identity.
2. A resolver maps that identity to a stable internal Person identifier.
3. Unknown identity returns an explicit non-privileged outcome; no silent privileged provisioning.
4. Disabled/revoked Person is denied according to a documented source of truth and freshness/revocation budget.
5. A membership resolver selects the active membership relevant to the target resource; multiple or ambiguous memberships fail closed.
6. Authorization evaluates action + resource + membership + relevant relationship/context; relationship existence alone is not permission.

**Tests:** AUTH-RED-003 through AUTH-RED-008 and AUTH-RED-012. Start with deterministic contract tests, but explicitly label them as contract tests. They do not replace provider integration tests.

### Stage C — Credential verifier and real provider integration

Only after provider selection and explicit implementation authorization:

1. Implement provider-specific credential verification at the API/Infrastructure edge.
2. Validate issuer, audience, signature, expiry and relevant token properties using supported provider libraries and approved configuration.
3. Add integration tests with a real test tenant/issuer and controlled credentials, including invalid signature, wrong issuer/audience, expiry, provider outage and key rotation.
4. Ensure provider-specific models and SDK types stop at the adapter boundary.
5. Fail closed when verification or identity/membership resolution is unavailable or ambiguous.

**Evidence:** provider configuration reviewed; integration results recorded; secrets never committed; no test-only fallback in production.

### Stage D — Mutation safety, privacy and regression

1. AUTH-RED-010: audit uses internal PersonId and server-derived tenant/context, operation, correlation ID and result.
2. AUTH-RED-011: inspect logs, audit, outbox and persistence to verify no raw credential or provider secret leaks.
3. AUTH-RED-013 through AUTH-RED-015: preserve idempotency replay, concurrency rejection, durable persistence and failure atomicity.
4. Test disable/revoke propagation, recovery, provider outage and relevant account lifecycle.
5. Review safe error responses and operational diagnostics.

**Exit evidence:** full CI plus PostgreSQL integration passes on exact head SHA; security review confirms fail-closed behavior and credential redaction.

## 3. Scenario-to-work mapping

| Scenarios | Primary test layer | Dependency / limitation |
|---|---|---|
| 001–002 | API integration | Existing test resolver only; not production token validation |
| 003–004 | Adapter contract + identity resolver tests | Internal Person mapping and unknown-identity policy must be designed |
| 005 | Identity lifecycle integration | Authoritative disabled/revoked state and freshness budget required |
| 006–008 | Membership/policy tests + API integration | Membership and relationship model must exist; flat authorities are insufficient |
| 009 | API integration | Request tenant must never become authorization authority |
| 010–011 | API + persistence/log inspection | Audit schema and safe logging contract must be reviewed |
| 012 | Adapter/resolver fault injection + API integration | Every failure path must fail closed |
| 013–015 | Existing mutation/PostgreSQL regression suite | Must remain green after auth integration |

## 4. Decisions that block production implementation

The following remain explicit owner/gate decisions, not implementation details to guess:

- **Provider and commercial model:** Entra External ID remains a preferred candidate, not a final commitment. Validate projected MAU, authentication methods, MFA/SMS, support, taxes and migration costs.
- **Privacy, minors and residency:** obtain appropriate Egypt and international legal/privacy review for children, guardian consent, data-subject rights, retention/deletion, cross-border transfers, subprocessors, and identity/log data location.
- **Account lifecycle:** decide invitation, linking, recovery, disable/revoke, duplicate identity, and account deletion behavior.
- **Guardian/student policy:** platform-owned relationship and consent model; a provider profile field is not legal consent or authorization.
- **Membership ambiguity:** define the user experience and security outcome when a person belongs to multiple tenants or has multiple roles.

## 5. Definition of done

Production authentication is not complete until all are true:

- Production credential verification is tested against a real provider test environment.
- Internal Person mapping and active membership resolution are authoritative and fail closed.
- Tenant scope is server-derived and cannot be overridden by request data.
- Relationship and resource policies are tested, not inferred from role labels alone.
- Account disable/revoke and provider outage behaviors have bounded, tested semantics.
- No credentials or provider secrets leak into storage, audit, logs, or outbox.
- Existing idempotency, concurrency, audit, outbox, migration and failure-atomicity tests remain green.
- CI evidence references the exact commit under review.
- The production identity provider and legal/privacy/commercial gates have explicit approval.

## 6. Current status

Stage A current API boundary checks are CI-verified, including the dedicated Production-host test for the Development/Testing-only fake bearer credential. The next work is a proposal-only review of provider-neutral Person, membership and policy contracts against the current domain/application model. No identity/membership implementation, provider SDK integration, or production login flow is authorized by this plan. Production authentication, membership authorization, and the final identity-provider decision remain incomplete.
