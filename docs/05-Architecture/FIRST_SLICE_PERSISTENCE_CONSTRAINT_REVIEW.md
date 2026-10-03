# First Slice Persistence Constraint Review

**Date:** 2026-09-30  
**Status:** PROPOSED — IMPLEMENTATION GATE NOT PROVEN

## 1. Purpose
Review the concrete persistence draft for invariants that must be represented before implementation, while keeping intentionally deferred product decisions open.

## 2. Recommended Constraints

### Ownership and tenant scope
Every protected authoritative record must have an unambiguous ownership/context path. Cross-tenant references are rejected unless an explicitly authorized platform-level relationship exists.

### Historical attribution
Evidence, assessment results, teacher decisions, follow-ups and outcomes preserve actor and context. Ending a relationship must not erase historical attribution.

### Evidence immutability
An accepted evidence version is not destructively overwritten. Corrections create attributable lineage.

### Explicit lifecycle
Assignments, submissions, follow-ups and outcomes require explicit lifecycle transitions. Completion, closure and achievement are not interchangeable.

### Concurrency
Mutable accountable records require optimistic concurrency. Stale commands return a conflict rather than silently replacing newer state.

### Idempotency
Critical mutation identity is scoped by operation + actor + tenant/context. A reused key with a different semantic request is rejected.

### Referential integrity
A record cannot reference an object outside its authorized ownership/context boundary.

### Projection separation
Progress, dashboards, attention views, search and analytics must not be required to preserve authoritative learning history.

## 3. Recommended Minimal State Machines

Assignment: Draft → Active → Closed. A closed assignment cannot accept a new submission unless an explicit reopening rule is introduced.

Submission: Created → Submitted → Assessed. Processing retry must not create duplicate authoritative submissions.

Follow-up: Open → InProgress → Closed. Closure requires authorized closure action; activity alone does not imply closure.

Outcome: Declared → Corrected/Superseded where correction is permitted. Outcome cannot be inferred solely from assignment completion.

These are implementation-facing recommendations and remain subject to final contract review.

## 4. Delete / Retention Boundary
For the first slice, destructive deletion of authoritative evidence, assessment results, teacher decisions and outcomes should not be treated as ordinary CRUD. Policy-driven retention/deletion must preserve required lineage. Exact periods and legal behavior remain OPEN.

## 5. Indexing Direction
Final schema should support lookup by tenant/context; learner/context timelines; assignment-to-submissions; submission-to-assessment results; evidence lineage; follow-up ownership/status; idempotency scope/key; audit actor/context/time; and outbox pending/retry state. Exact indexes require implementation/data review.

## 6. Transaction Boundary Review
Recommended atomic units: assignment mutation; submission; assessment result; evidence/correction; teacher decision; authoritative next action; follow-up mutation; outcome declaration/correction. Each unit owns local invariants. Cross-module propagation is asynchronous/recoverable where needed.

## 7. Decision
Accept the above as schema design constraints, not final physical schema.

Still OPEN: exact tables/columns, foreign keys, evidence taxonomy, goal/assignment cardinality, scheduling/recurrence, outcome authority implementation, retention/deletion, physical tenant isolation, ORM/provider.

## 8. Gate Impact
This review removes persistence ambiguity that could otherwise invite accidental business rules during coding.

**Implementation Gate remains NOT PROVEN.**

Next: exact first-slice JSON request/response contracts and acceptance scenarios, followed by Implementation Gate review.