# First Slice Exact Contract Specification
**Date:** 2026-09-30  
**Status:** PROPOSED — IMPLEMENTATION GATE NOT PROVEN

## 1. Contract Rules
Base path: /api/v1. Resource IDs are opaque strings. JSON uses camelCase. Mutation responses include correlationId. Critical mutations require Idempotency-Key. Accountable mutable mutations require expectedVersion where applicable. Server-side authorization uses identity, role, relationship, organization/tenant, policy/consent and learning context.

## 2. Standard Response
Success: data + meta.correlationId. Error: error.code + safe message + optional fieldErrors + meta.correlationId. Never expose stack traces, provider internals or unnecessary sensitive learner data.

## 3. Create Assignment
POST /api/v1/learning-contexts/{contextId}/assignments
Request: goalId where applicable, learnerId, work definition. Response 201: id, contextId, goalId, learnerId, status=ACTIVE, version, correlationId. Exact work schema, scheduling, recurrence and goal cardinality remain open.

## 4. Submit Work
POST /api/v1/assignments/{assignmentId}/submissions
Request: submission payload/reference. Response 201: id, assignmentId, status=SUBMITTED, version, correlationId. Same semantic idempotency retry returns the original authoritative result.

## 5. Record Assessment Result
POST /api/v1/submissions/{submissionId}/assessment-results
Request: assessmentId and result payload. Response 201: authoritative assessment result. Assessment owns assessment semantics; a result may generate evidence but is not synonymous with every evidence record.

## 6. Record Evidence
POST /api/v1/evidence
Request: type, learnerId, contextId, source, observedAt, origin references where applicable, provenance, quality/uncertainty and visibility. Response 201: authoritative evidence plus lineage metadata where relevant. Correction is not destructive overwrite.

## 7. Teacher Decision
POST /api/v1/learning-contexts/{contextId}/teacher-decisions
Request: decision payload, evidence basis references, expectedVersion. Response 201: authoritative decision. A recommendation cannot silently create this decision.

## 8. Next Action
POST /api/v1/learning-contexts/{contextId}/next-actions
Request: action definition, basis/decision reference, expectedVersion. Response 201: authoritative next-action state.

## 9. Follow-up
POST /api/v1/follow-ups
Request: contextId, ownerId, trigger, expectedAction, idempotency key. PATCH /api/v1/follow-ups/{followUpId}: transition + expectedVersion. Closure requires an authorized state transition and is not an Outcome.

## 10. Outcome
POST /api/v1/learning-contexts/{contextId}/outcomes
Request: value, evidence basis references, expectedVersion. Response 201: authoritative outcome. Correction/supersession preserves lineage where permitted.

## 11. Context Projection
GET /api/v1/learning-contexts/{contextId}
Caller-specific policy-controlled projection may contain context, authorized learner reference, goal/assignment summaries, evidence/progress summaries, current next action and relevant follow-up status. Protected information is not exposed merely because the caller knows the context ID.

## 12. Error Mapping
400 VALIDATION_FAILED; 401 AUTHENTICATION_REQUIRED; 403 FORBIDDEN; 404 RESOURCE_NOT_FOUND; 422 BUSINESS_RULE_VIOLATION; 409 IDEMPOTENCY_CONFLICT or CONCURRENCY_CONFLICT; 429 RATE_LIMITED; 503 DEPENDENCY_UNAVAILABLE; 500 UNEXPECTED_ERROR.

## 13. Acceptance Scenarios
Assignment: authorized creation, unauthorized rejection, same-key same-request replay, same-key different-request conflict, invalid precondition causes no mutation.
Submission: authorized creation, retry without duplicate, closed assignment rejection, fail-after-commit reconciliation.
Evidence: attribution, authorization rejection, correction lineage, conflicting evidence representation, no silent progress mutation.
Teacher Decision: authorized decision, stale-version conflict, recommendation cannot create decision, auditability.
Follow-up: authorized creation, invalid owner/context rejection, invalid transition rejection, closure distinct from outcome.
Outcome: authorized declaration with valid basis, completion alone cannot create achievement, correction lineage, unauthorized declaration rejection.
Projection: authorized projection, restricted data omitted/non-disclosed, projection failure cannot rewrite authoritative history.

## 14. Deliberately Open
Exact deferred JSON cardinalities; evidence taxonomy; goal/assignment scheduling and recurrence; authentication/session; physical tenant isolation; jurisdiction/age/consent; retention/deletion periods; OpenAPI workflow; pagination/filter vocabulary; ORM/provider/cloud.

## 15. Gate Assessment
First-slice API behavior is concrete enough for implementation-readiness review without requiring implementation to invent semantics.
Contract Design: READY FOR IMPLEMENTATION GATE REVIEW.
Implementation Gate: NOT PROVEN.