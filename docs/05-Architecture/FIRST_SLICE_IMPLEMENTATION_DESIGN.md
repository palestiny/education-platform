# First Slice Implementation Design

**Date:** 2026-09-29  
**Status:** PROPOSED — IMPLEMENTATION GATE NOT PROVEN  
**Purpose:** Convert the accepted semantic/domain/API boundaries into an implementation-ready design without prematurely freezing framework or infrastructure choices.

## 1. Scope

The first vertical slice is:

**Authorized Context → Goal/Assignment → Learner Action/Submission → Assessment Result/Evidence → Teacher Decision → Next Action → Follow-up → New Evidence → Outcome**

This document defines implementation boundaries, not production code.

## 2. Logical Module Boundary

### Identity & Access
Owns identity and authentication context. It does not decide learning authority by itself.

### Organizations & Relationships
Owns organizational context and relationship lifecycle. A relationship does not automatically equal permission.

### Authorization
Evaluates:
**Identity + Role + Relationship + Organization/Tenant + Policy/Consent + Learning Context**

Authorization is enforced at application command/query boundaries, not only in the UI.

### Learning
Owns learning context, goals, assignments, submissions and learner actions.

### Assessment
Owns assessment definitions, attempts and assessment results.

### Evidence
Owns attributable observations/evidence, provenance, visibility, quality/uncertainty and correction/supersession lineage.

### Learner State / Progress
Consumes authoritative evidence and produces rebuildable derived state. It does not become the historical source of truth.

### Follow-up
Owns explicitly created unresolved work: owner, context, trigger, expected action, status and closure metadata.

### Audit / Observability
Owns accountability records and operational diagnostics. Audit is not the domain event store.

### Communication / Notifications
Owns contextual messages and delivery state. Messages do not mutate authoritative learning state.

### AI Assistance
Provides bounded assistance only. AI output cannot silently become learner state, teacher decision, permission, payment or irreversible intervention.

### Integrations
External systems are isolated behind adapters. External delivery/projection failure must not silently rewrite authoritative learning state.

## 3. Aggregate / Transaction Direction

Do not create one aggregate containing the entire learning journey.

Use small authoritative transaction boundaries around business commitments:

1. Create/update Goal/Assignment.
2. Record Submission/Attempt.
3. Record Assessment Result.
4. Record Evidence.
5. Record Teacher Decision.
6. Commit Next Action where it is authoritative business state.
7. Create/update Follow-up.
8. Record Outcome.

Each command must define:
- authorization preconditions,
- business invariants,
- idempotency identity,
- concurrency/version expectations,
- authoritative writes,
- audit/outbox requirements,
- resulting state.

Cross-module work should prefer explicit contracts and durable intent over distributed transactions.

## 4. Persistence Direction

Authoritative durable records:

- learning context
- goals/assignments
- submissions/attempts
- assessment results
- evidence
- teacher decisions
- authoritative next-action commitments
- follow-ups
- outcomes
- required audit records
- idempotency records where required
- outbox records where required

Derived/rebuildable records:

- progress projections
- dashboards
- attention views
- recommendation candidates
- notification projections
- search indexes
- analytics aggregates

Historical evidence correction must preserve lineage. No destructive overwrite for accountable evidence.

## 5. Consistency Model

### Strong/local consistency
Use local transactions for changes that must be atomically committed inside one module/transaction boundary.

### Eventual consistency
Use explicit outbox-driven propagation for rebuildable projections, notifications and integration effects where appropriate.

### No implicit last-write-wins
Accountable mutations use expected-version/concurrency checks.

### Reconciliation
If a command commits locally but an external/projection action fails, the authoritative state remains committed and the dependent state becomes recoverable/retriable.

## 6. Command vs Query

Commands:
- create/update assignment
- submit work
- record assessment result
- record evidence
- create teacher decision
- commit next action
- create/update follow-up
- declare/correct outcome

Queries:
- learning context projection
- assignment/submission state
- evidence view
- progress projection
- follow-up view
- authorized parent projection

Queries must apply visibility and authorization rules before returning data.

## 7. Cross-Module Contract Rule

Modules may depend on explicit interfaces/contracts, not internal tables.

Forbidden:
- direct cross-module table writes,
- UI-driven business state mutation,
- hidden shared mutable state,
- importing another module's persistence implementation as an API.

Allowed:
- application contracts,
- domain-owned commands,
- read projections,
- durable events/outbox messages where justified.

## 8. Reliability Contract

Critical POST operations require idempotency.

Expected behavior:

**Same semantic retry → original result**

**Same key + materially different request → IDEMPOTENCY_CONFLICT**

**Retry after fail-after-commit → reconcile durable result**

Accountable concurrent mutation:

**stale version → CONCURRENCY_CONFLICT**

No duplicate authoritative submission, decision, outcome or equivalent mutation may be created by a normal retry.

## 9. Security Enforcement

Every protected operation receives an explicit authorization context.

Minimum inputs:

- principal identity
- active role/authority
- organization/tenant context
- relationship context
- learning context
- applicable policy/consent state

Parent access is a projection with policy-controlled fields, not unrestricted access to student/teacher records.

Sensitive reads and high-impact mutations require auditable access paths.

## 10. API Adapter Boundary

HTTP/API concerns remain outside domain logic:

**HTTP Request → Authentication → Authorization → Validation → Application Command → Domain Rules → Persistence → Outbox/Audit → Response**

The domain must not depend on HTTP status codes, controller objects or transport-specific concerns.

## 11. Testing Architecture

Required layers:

### Domain tests
State transitions, invariants, correction lineage, conflicts and outcome semantics.

### Application tests
Authorization context, idempotency, transaction behavior, cross-module contracts and failure recovery.

### Persistence tests
Constraints, versioning, durable/rebuildable separation and reconstruction.

### API contract tests
Request/response/error schema and observable semantics.

### Integration tests
Real database/provider boundaries where behavior cannot be proven with unit tests.

### End-to-end slice test
Exercise the complete canonical first-slice journey.

## 12. Observability

Every command should carry or generate:

- correlation ID
- actor/principal context
- tenant/organization context where applicable
- operation name
- outcome/status
- duration
- safe failure classification

Logs must not leak sensitive student/parent data unnecessarily.

Important mutations must be traceable through audit records without treating logs as business truth.

## 13. Data Migration / Recovery

Before production schema implementation:

- schema changes require migration scripts,
- authoritative data must be recoverable,
- derived projections must be rebuildable,
- correction lineage must survive rebuilds,
- outbox processing must tolerate retry,
- failed projections must be detectable and reconcilable.

## 14. What Remains Deliberately Open

This design does not silently decide:

- exact table/entity schema,
- exact ORM mapping,
- exact authentication provider/session mechanism,
- physical tenant-isolation strategy,
- jurisdiction-specific consent/age rules,
- exact retention periods,
- cloud/vendor,
- universal mastery algorithm,
- final evidence taxonomy,
- final UI component structure.

Those decisions require their appropriate downstream design review.

## 15. Implementation Gate Checklist

Before implementation authorization:

- [ ] persistence model reviewed
- [ ] aggregate boundaries reviewed
- [ ] module contracts reviewed
- [ ] exact API schemas reviewed
- [ ] auth enforcement points reviewed
- [ ] idempotency storage/behavior reviewed
- [ ] concurrency mechanism reviewed
- [ ] audit/outbox obligations reviewed
- [ ] migration strategy reviewed
- [ ] test plan reviewed
- [ ] observability plan reviewed
- [ ] Definition of Done mapped
- [ ] first-slice acceptance scenarios written
- [ ] no unresolved semantic decision is hidden in code

**Current status: IMPLEMENTATION GATE NOT PROVEN.**
