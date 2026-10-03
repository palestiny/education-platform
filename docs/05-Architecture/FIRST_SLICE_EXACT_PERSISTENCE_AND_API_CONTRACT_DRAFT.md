# First Slice Exact Persistence Schema and API Contract Draft

**Date:** 2026-09-29  
**Status:** PROPOSED — IMPLEMENTATION GATE NOT PROVEN

## 1. Purpose
Turn the accepted logical persistence and API boundaries into a concrete draft suitable for review before implementation.
This is a design artifact, not authorization to create migrations or controllers.

## 2. Persistence Model — Draft
Initial relational ownership is normalized around authoritative modules.
- Identity / Organization: principals, organization_memberships, relationships
- Learning: learning_contexts, goals, assignments, submissions
- Assessment / Evidence: assessment_definitions, assessment_attempts, assessment_results, evidence_records, evidence_lineage
- Decision / Follow-up: teacher_decisions, next_actions, follow_ups, outcomes
- Reliability / Accountability: idempotency_records, audit_records, outbox_messages
Names are provisional until schema review.

## 3. Common Record Rules
- opaque stable identifier
- tenant/organization context where applicable
- timestamps
- actor attribution where applicable
- version/concurrency token for mutable accountable records
Do not add generic fields without domain meaning.

## 4. Key Relationships
- learning_contexts identify the learner/context in which learning has meaning.
- goals belong to a learning context.
- assignments belong to a learning context and may reference a goal.
- submissions reference assignment and learner/context.
- assessment_attempts/results belong to Assessment semantics.
- evidence may reference originating submissions/results but remains independently attributable.
- evidence_lineage preserves correction/supersession history.
- teacher_decisions reference context and evidence/state basis.
- next_actions reference the decision/context when authoritative.
- follow_ups reference context and accountable owner.
- outcomes reference relevant context/goal and preserve declaration/correction authority.
Exact cardinality remains open where intentionally deferred.

## 5. Evidence Lineage
Conceptual: original evidence → correction/superseding evidence → lineage metadata.
The original record is not silently overwritten.
Lineage preserves predecessor/successor, reason, actor and timestamp.

## 6. Idempotency Record
Logical fields: id, operation_scope, actor_id, tenant_id, context_id, idempotency_key, request_fingerprint, status, resource_type, resource_id, response_metadata, created_at, completed_at.
Uniqueness is scoped to the accepted idempotency contract, not globally by key alone.

## 7. Outbox Record
Logical fields: id, message_type, aggregate_type, aggregate_id, payload, occurred_at, published_at, attempt_count, last_error (sanitized), status.
Outbox provides durable intent/reliable propagation; it is not authoritative domain state.

## 8. API Contract Draft
Base path: /api/v1

### Read Context
GET /api/v1/learning-contexts/{contextId}
Authorized projection of context, permitted learner reference, goal/assignment summaries, relevant progress/evidence summaries and next useful action where available.

### Create Assignment
POST /api/v1/learning-contexts/{contextId}/assignments
Draft request: goal reference where applicable, work definition, learner target, scheduling data where supported, Idempotency-Key.
Response: authoritative assignment state + correlation ID.

### Submit Work
POST /api/v1/assignments/{assignmentId}/submissions
Draft request: submission payload/reference, permitted client metadata, Idempotency-Key, expected version where applicable.
Response: authoritative submission state + correlation ID.

### Record Assessment Result
POST /api/v1/submissions/{submissionId}/assessment-results
Draft request: assessment reference, attempt/result payload, expected version where applicable, Idempotency-Key.
Response: authoritative assessment result + correlation ID.

### Record Evidence
POST /api/v1/evidence
Draft request: evidence type/source, learner/context, originating action/result where applicable, observation time, provenance, quality/uncertainty, visibility, Idempotency-Key.
Response: authoritative evidence + lineage metadata where relevant + correlation ID.

### Teacher Decision
POST /api/v1/learning-contexts/{contextId}/teacher-decisions
Draft request: decision payload, evidence/basis references, expected version, Idempotency-Key.
Response: authoritative decision + correlation ID.

### Next Action
POST /api/v1/learning-contexts/{contextId}/next-actions
Draft request: action definition, basis/decision reference, expected version, Idempotency-Key.
Response: authoritative next-action state + correlation ID.

### Follow-up
POST /api/v1/follow-ups
PATCH /api/v1/follow-ups/{followUpId}
Requests carry state-transition intent plus idempotency/version semantics appropriate to the operation.

### Outcome
POST /api/v1/learning-contexts/{contextId}/outcomes
Draft request: outcome value, basis/evidence references, declaration/correction reason where applicable, expected version, Idempotency-Key.
Response: authoritative outcome + correlation ID.

## 9. Common Error Contract
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
Response includes code, safe detail, correlation ID and field errors where applicable. No stack traces or sensitive learner data.

## 10. Request Metadata
- authenticated session/token
- Idempotency-Key for critical mutations
- expected-version/concurrency token where required
- correlation ID accepted from trusted clients or generated by the service
Exact header naming and auth mechanism remain open.

## 11. Pagination
Collection endpoints use opaque cursor pagination, stable ordering, explicitly supported filters/sorts, and authorization before exposure. Exact limits remain operational configuration.

## 12. Contract-Test Obligations
Valid request; malformed request; unauthorized; forbidden; non-disclosure; business-rule violation; duplicate retry; idempotency conflict; stale-version conflict; dependency failure; successful authoritative response; correlation ID; policy-filtered parent projection.

## 13. Open Items Before Implementation
1. Final tables/columns/indexes/foreign keys.
2. Exact JSON schemas and cardinalities.
3. Authentication/session mechanism.
4. Physical tenant isolation.
5. Evidence taxonomy.
6. Goal/assignment cardinality and scheduling semantics.
7. Outcome authority/correction mechanics.
8. Retention/deletion policy.
9. Pagination/filter vocabulary.
10. OpenAPI version and generation workflow.

## 14. Review Status
Persistence: concrete draft ready for review.
API: concrete contract draft ready for review.
Implementation Gate: NOT PROVEN.
No migrations, controllers or production UI are authorized by this document.