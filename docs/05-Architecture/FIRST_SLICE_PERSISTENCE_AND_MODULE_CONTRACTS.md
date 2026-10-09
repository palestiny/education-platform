# First Slice Persistence and Module Contracts

**Date:** 2026-09-29  
**Status:** PROPOSED — IMPLEMENTATION GATE NOT PROVEN

## 1. Objective

Define minimum persistence ownership and inter-module contracts for the first vertical slice without coupling modules through database tables.

Canonical chain: **Authorized Context → Goal/Assignment → Learner Action/Submission → Assessment Result/Evidence → Teacher Decision → Next Action → Follow-up → New Evidence → Outcome**

## 2. Persistence Ownership

| Module | Authoritative records | Derived/read models |
|---|---|---|
| Identity & Access | identity/principal state | session projections |
| Organizations & Relationships | organization/tenant membership, relationship lifecycle | organization views |
| Authorization | durable policy configuration where required | permission projections |
| Learning | learning contexts, goals, assignments, submissions | learner/context projections |
| Assessment | assessment definitions, attempts, results | assessment summaries |
| Evidence | evidence, provenance, lineage, correction/supersession | evidence views |
| Learner State | none as historical source of truth | progress/mastery/attention projections |
| Follow-up | follow-up work items and lifecycle | queues/reminders |
| Communication | messages and delivery state | inbox/notification projections |
| Audit | accountability records | operational views |
| AI Assistance | bounded assistance records where auditability requires | recommendations/summaries |
| Integrations | provider references/reconciliation state where required | external-status projections |

## 3. Core Logical Records

These are logical records, not final database tables.

- **LearningContext:** meaning and authorization scope for learning activity.
- **Goal:** intended learning outcome; distinct from assignment/completion.
- **Assignment:** teacher-authorized work associated with a goal/context.
- **Submission/Attempt:** learner action against authorized work.
- **AssessmentResult:** result owned by assessment semantics; may produce evidence.
- **Evidence:** attributable observation with source/type, actor/system, learner/context, observation time, provenance, quality/uncertainty, visibility and correction/supersession lineage.
- **TeacherDecision:** authorized human decision; distinct from recommendation.
- **NextAction:** committed next useful action when authoritative business state.
- **FollowUp:** explicitly created unresolved work with owner, context, trigger, expected action, status and closure metadata.
- **Outcome:** declared result; candidate semantics remain Achieved, Partially Achieved, Not Demonstrated, Contradictory, Unevaluable.

## 4. Integrity Rules

The eventual schema must enforce stable IDs, ownership/tenant context, attribution, authorized references, idempotency uniqueness, concurrency protection, historical evidence lineage, explicit correction/supersession, valid lifecycle transitions and duplicate-operation prevention. Exact SQL constraints/indexes remain downstream.

## 5. Idempotency

Logical **IdempotencyRecord** contains operation scope, actor, tenant/context, key, request fingerprint, status, resulting resource/reference, response metadata and timestamps.

Rules: same semantic request returns the original result; materially different reuse returns `IDEMPOTENCY_CONFLICT`; fail-after-commit retries reconcile durable state; idempotency is never the authoritative domain state. Exact retention is open.

## 6. Concurrency

Accountable records use a version/concurrency token and expected-version command input. Stale versions return `CONCURRENCY_CONFLICT`. No silent last-write-wins for teacher decisions, evidence corrections, outcomes or equivalent accountable mutations.

## 7. Module Contracts

**Learning:** CreateAssignment, UpdateAssignment, RecordSubmission; queries for context/assignment/submission.

**Assessment:** RecordAssessmentResult; query result. Does not directly own Evidence storage.

**Evidence:** RecordEvidence, CorrectEvidence, SupersedeEvidence; query evidence and lineage.

**Learner State:** projection/rebuild operations only; consumes evidence/result changes.

**Follow-up:** CreateFollowUp, UpdateFollowUp, CloseFollowUp; query/list.

**Authorization:** EvaluateAccess(principal, role, relationship, organization, context, action, policy). No module bypasses authorization by convention.

## 8. Transaction Rules

A transaction may atomically modify records inside its owning boundary. Each command validates authorization and business preconditions, enforces idempotency/concurrency as applicable, persists authoritative state plus local audit/outbox intent, then commits. Cross-module side effects occur after durable local commit through recoverable mechanisms.

## 9. Rebuild Principle

Progress, attention lists, dashboards, recommendation candidates, notification projections, search indexes and analytics aggregates should remain rebuildable from authoritative records where practical. Rebuild preserves authorization scope, attribution, evidence lineage and tenant context.

## 10. Deferred Decisions

Final relational tables, columns/types, EF mapping, evidence taxonomy, mastery algorithm, exact outcome authority rules, authentication/session provider, physical tenant isolation, retention periods and cloud/provider remain open.

## 11. Review Result

**READY FOR IMPLEMENTATION DESIGN REVIEW — NOT IMPLEMENTATION AUTHORIZATION.**
