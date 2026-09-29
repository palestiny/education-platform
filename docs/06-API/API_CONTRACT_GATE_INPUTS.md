# API Contract Gate Inputs — First Slice

**Date:** 2026-09-29  
**Status:** CLOSURE REVIEW — API Gate NOT PROVEN  
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

**Current recommendation:** API Gate is ready to close after endpoint-level contract specification and review; it is not yet PASS.
