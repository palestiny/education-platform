# API Contract Gate Inputs — First Slice

**Date:** 2026-09-29  
**Status:** ACCEPTED — API GATE PASS  
**Architecture:** ACCEPTED  
**Security/Data baseline:** ACCEPTED  
**Implementation authorization:** None

## 1. Purpose
Translate accepted product/domain/security/data semantics into a reviewable API contract before database schema or production implementation. The API is an application boundary, not the source of domain truth.

## 2. First-Slice Contract Boundary
Authorized Context → Goal/Assignment → Learner Action/Submission → Assessment Result/Evidence → Teacher Decision → Next Action → Follow-up → New Evidence → Outcome

## 3. Candidate Command/Resource Surface
- GET /api/v1/learning-contexts/{contextId}
- POST /api/v1/learning-contexts/{contextId}/assignments
- GET /api/v1/assignments/{assignmentId}
- POST /api/v1/assignments/{assignmentId}/submissions
- GET /api/v1/assignments/{assignmentId}/submissions/{submissionId}
- POST /api/v1/submissions/{submissionId}/assessment-results
- POST /api/v1/evidence
- POST /api/v1/learning-contexts/{contextId}/teacher-decisions
- POST /api/v1/learning-contexts/{contextId}/next-actions
- POST /api/v1/follow-ups
- PATCH /api/v1/follow-ups/{followUpId}
- POST /api/v1/learning-contexts/{contextId}/outcomes

These are candidate contracts, not final API approval. Assessment Result and Evidence remain distinct. Recommendations cannot silently create authoritative teacher decisions. Parent views are policy-controlled projections.

## 4. Common Mutation Contract
Every authoritative mutation must define actor, tenant/context, authorization context, request schema, validation, business preconditions, idempotency, concurrency where needed, authoritative mutation, audit/outbox behavior where required, resulting state, errors and observability.

## 5. Idempotency
Retriable authoritative mutations require deterministic idempotency. Recommended key scope: actor + tenant/context + operation. Same semantic request returns the original result; materially different reuse returns conflict; retry after fail-after-commit reconciles to the existing authoritative result.

## 6. Concurrency
Mutations depending on current state should support an expected-version mechanism. Incompatible concurrent changes are explicit conflicts; no implicit last-write-wins for accountable learning decisions.

## 7. Error Model — Candidate
| Condition | HTTP | Retry |
|---|---:|---|
| malformed request | 400 | no |
| authentication missing/invalid | 401 | no |
| authenticated but not permitted | 403 | no |
| resource/context not visible | 404 or policy-specific non-disclosure | no |
| domain validation/precondition failure | 422 | no |
| idempotency/concurrency conflict | 409 | reconcile first |
| rate limit | 429 | according to server guidance |
| transient dependency failure | 503 | controlled retry |
| unexpected server failure | 500 | controlled retry |

Stable public error codes remain OPEN until API review.

## 8. Authorization Contract
Authorization is evaluated at command/read time. It cannot rely only on role, resource ID possession, previous communication, or UI state.

Effective authorization context: Identity + Role + Relationship + Organization/Tenant + Policy/Consent + Learning Context.

## 9. Audit / Observability
Audit important authoritative actions including assignment changes, assessment/evidence changes, teacher decisions, next-action commitments, follow-up state changes, outcomes and privileged access. Telemetry must minimize learner content, credentials, tokens and sensitive payloads.

## 10. External Failure / Recovery
External delivery or projection failure must not silently roll back authoritative learning state. Authoritative state and asynchronous delivery/projection state must remain separately observable and recoverable.

## 11. API Gate Review Checklist
- [ ] Domain Confirmation accepted.
- [ ] First-slice command/resource boundary accepted.
- [ ] Authorization context accepted.
- [ ] Request/response contracts defined.
- [ ] Validation and business preconditions defined.
- [ ] Idempotency defined per mutation.
- [ ] Concurrency/conflict semantics defined.
- [ ] Error codes and mappings accepted.
- [ ] Audit/observability defined.
- [ ] Failure/recovery semantics defined.
- [ ] Parent projection visibility mapped.
- [ ] Tenant context propagation defined.
- [ ] Sensitive-data exposure reviewed.
- [ ] API-to-domain traceability complete.
- [ ] API-to-data ownership traceability complete.
- [ ] Contract tests specified.

## 12. Explicitly Deferred
Database tables/indexes, ORM, exact field naming where domain decisions remain open, physical tenant isolation, jurisdiction-specific privacy/consent rules, exact retention periods, cloud/vendor, and implementation framework details.

## 13. Current Gate Status
**API Contract Gate: READY FOR REVIEW — NOT PROVEN**

No implementation is authorized by this document.

## API Contract Closure Review — 2026-09-29

### Preconditions now satisfied

- Architecture Gate: PASS
- Security baseline: PASS
- Data baseline: PASS
- Domain Gate: PASS
- UX Gate: PASS

### Closure rule

API Gate may pass only when each authoritative mutation has an explicit contract for authorization, validation, business preconditions, idempotency, concurrency/conflict, response, errors, audit and recovery, and when each read has explicit visibility/data ownership semantics.

### Endpoint review matrix

| Operation | Authorization | Idempotency | Concurrency | Evidence/State | Audit | Status |
|---|---|---|---|---|---|---|
| Read learning context | Identity + relationship + tenant/context | N/A | N/A | projection | sensitive read where applicable | READY |
| Create assignment | Teacher authority + context | Required | Expected version where needed | authoritative assignment/goal | Required | READY |
| Create submission | Learner authority + assignment scope | Required | Assignment version/availability | authoritative submission | Required | READY |
| Record assessment result | Assessment authority | Required | Expected version | assessment result | Required | READY |
| Create evidence | Source/actor authority + context | Required | Evidence lineage/version | authoritative evidence | Required | READY |
| Create teacher decision | Teacher authority + context | Required | Required | authoritative decision | Required | READY |
| Create next action | Authorized decision/context | Required | Required | authoritative commitment | Required | READY |
| Create/update follow-up | Owner authority + context | Required for create | Required for state change | follow-up state | Required | READY |
| Declare outcome | Explicit outcome authority + evidence sufficiency | Required | Required | authoritative outcome | Required | READY |
| Parent projection | Parent relationship + policy/consent | N/A | N/A | controlled projection | sensitive read | READY |

### Remaining API decisions

1. Final public resource names and exact JSON schemas.
2. Exact stable domain error-code vocabulary.
3. Exact idempotency header/storage/retention mechanics.
4. Exact pagination/filtering/sorting contract for reads.
5. Exact authentication token/session mechanism.
6. Formal OpenAPI version and generated-contract workflow.

These are implementation/API details, not unresolved domain semantics. They must be closed during the API Gate review before implementation authorization.

**Current recommendation:** API Gate is accepted and PASS.

## API Contract Closure Proposal — 2026-09-29

### 1. Public contract baseline

- API versioning: /api/v1 for the first public contract.
- JSON: UTF-8 JSON request/response bodies.
- Resource identifiers: opaque stable IDs; clients must not depend on database keys.
- Timestamps: ISO 8601 with explicit offset/UTC normalization at the API boundary.
- Mutation responses return the authoritative resulting resource/state plus a correlation identifier.
- Reads return policy-filtered projections; protected-resource non-disclosure follows the security policy.

### 2. Authentication/session contract

Protected resources require an authenticated principal. The concrete token/session provider remains an infrastructure choice, but every protected command is evaluated against resolved identity and authorization context before mutation.

### 3. Error contract

Stable machine-readable error codes are required:
- AUTHENTICATION_REQUIRED
- FORBIDDEN
- RESOURCE_NOT_FOUND
- VALIDATION_FAILED
- BUSINESS_RULE_VIOLATION
- IDEMPOTENCY_CONFLICT
- CONCURRENCY_CONFLICT
- RATE_LIMITED
- DEPENDENCY_UNAVAILABLE
- UNEXPECTED_ERROR

Responses include a correlation ID and safe actionable detail; sensitive internal diagnostics never cross the API boundary.

### 4. Idempotency contract

All critical POST mutations require an idempotency key. The key is scoped to authenticated actor + tenant/context + operation. Repeating the same semantic request returns the original authoritative result. Reusing the key for a materially different request returns IDEMPOTENCY_CONFLICT. Fail-after-commit retries reconcile against the durable idempotency record rather than executing the mutation again.

### 5. Concurrency contract

Accountable state mutations use an expected-version/concurrency token where concurrent modification is possible. A stale mutation returns CONCURRENCY_CONFLICT; no implicit last-write-wins for teacher decisions, evidence corrections, outcomes, or equivalent accountable state.

### 6. Pagination/read contract

Collection endpoints use cursor-based pagination by default. The cursor is opaque. Default ordering is stable and server-defined; clients may request only explicitly supported sort/filter fields. Pagination must respect the caller's authorized projection.

### 7. OpenAPI/contract workflow

The API contract is the source of truth for externally observable behavior. OpenAPI is maintained alongside the codebase, contract tests validate request/response/error shapes, and observable semantic changes require contract review before merge.

### Closure assessment

The remaining API questions are bounded to contract mechanics and infrastructure selection rather than unresolved product/domain semantics. DB schema, ORM, cloud/provider, jurisdiction-specific policy, exact retention periods, and implementation remain downstream decisions.

**Recommendation:** ACCEPT this API contract baseline and mark API Gate → PASS, then open the Implementation Gate. Production implementation still requires the Implementation Gate to verify traceability, test obligations, observability, security enforcement, and Definition of Done.


## API Gate Acceptance — 2026-09-29

The Project Owner accepted the API Contract Closure Proposal.

### Accepted baseline

- Versioned /api/v1 boundary.
- Stable opaque resource identifiers.
- Explicit authentication and authorization context.
- Stable machine-readable error categories.
- Idempotency for critical mutations.
- Explicit concurrency/conflict semantics; no implicit last-write-wins for accountable state.
- Cursor-based pagination for scalable collections.
- OpenAPI maintained as the externally observable contract.
- Contract tests required for request/response/error behavior.
- Correlation identifiers and safe diagnostics.
- Parent and other protected projections remain policy-controlled.
- DB schema, ORM, provider/cloud, jurisdiction-specific rules and exact retention periods remain downstream/open.

**API Gate: PASS.**

API acceptance does not authorize production implementation by itself. Implementation Gate remains required.
