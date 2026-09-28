# Domain Readiness & Requirement Dependency Checkpoint

**Status:** Domain Preparation Checkpoint — NOT PROVEN  
**Date:** 2026-09-28  
**Implementation authorization:** None

## Purpose

Reconcile the consolidated requirements with the Product Operating Model and identify the minimum domain decisions required for the first committed product slice.

The goal is to prevent:
1. turning candidate requirements into hidden commitments;
2. creating domain concepts only because they sound architecturally useful;
3. allowing architecture assumptions to decide unresolved product behavior.

## Requirement disposition

| Requirement area | Domain readiness | Segment dependency | Current disposition |
|---|---|---|---|
| Identity / roles | High | Low | Candidate foundation |
| Permissions / authorization | High | Medium | Candidate foundation |
| Relationships | Medium | Medium/High | Policy and first-slice dependent |
| Organization / tenant | Medium | High | Conditional |
| Learning context | Medium | High | Candidate; exact ownership open |
| Learning experience | High | Medium | Candidate foundation |
| Learning content | High | Low/Medium | Candidate foundation |
| Assignment / practice | High | Medium | Candidate foundation |
| Assessment | High | Medium | Candidate foundation |
| Evidence | Medium | Medium | Candidate; semantics still need definition |
| Evidence quality / provenance | Medium | Medium | Candidate; rules open |
| Learner state / progress | Medium | Medium | Candidate; durable-vs-derived state open |
| Teacher decision/action workflow | Medium | Medium/High | Candidate; workflow semantics defined, exact scope open |
| Parent visibility | Low/Medium | High | Conditional |
| Communication | High | Medium/High | Candidate foundation |
| Notification | High | Medium | Candidate foundation |
| Follow-up / work item | Medium | Medium | Candidate; lifecycle and closure authority open |
| Intervention case | Low | High | NOT PROVEN; do not model yet |
| Recommendation engine | Low | High | NOT PROVEN; keep recommendation as behavior/concept, not domain |
| Payment | Medium | High | Conditional |
| Attendance | Medium | High | Conditional |
| Scheduling | Medium | High | Conditional |
| Offline sync | Low/Medium | High | Open |
| AI assistance | Low/Medium | High | Open; concrete workflow required |
| Audit | High | Medium | Cross-cutting candidate |
| Search | Low/Medium | Medium/High | Open |
| Marketplace / social / advanced adaptive learning | Low | High | Deferred |

## Domain boundary corrections

### 1. Evidence is not an event bus

Evidence should represent meaningful learning/operational observations with provenance and quality. It must not become a generic dumping ground for every system event.

### 2. Interpretation is not automatically a persisted domain entity

An interpretation may be:
- derived temporarily;
- persisted when it has business value/audit implications;
- explicitly attributed to a human or system.

The persistence rule remains OPEN.

### 3. Recommendation is not Decision

A system may propose a next action, but the product must preserve the distinction between:
- evidence;
- interpretation;
- recommendation;
- human decision;
- action;
- outcome evidence.

No autonomous recommendation domain is justified yet.

### 4. Follow-up should remain generic until evidence says otherwise

A reusable work-item/follow-up concept may be useful for overdue actions and ownership. It must not be renamed InterventionCase merely to encode the current hypothesis.

### 5. Dashboard is a view

Dashboards must not become authoritative state. They consume authoritative domain state and evidence.

### 6. Communication is not the source of truth

A chat/message can carry context and decisions, but important business state must remain represented in its owning domain.

## Aggregate questions still blocking Domain Gate

1. Who owns the learner context?
2. Who can create/change a learner relationship?
3. Which evidence is authoritative?
4. Which evidence is immutable versus correctable/versioned?
5. How is conflicting evidence represented?
6. Which learner state is durable versus derived?
7. Who owns an assessment result?
8. Who owns a follow-up?
9. What evidence is required to close a follow-up?
10. Which parent/student/teacher relationships authorize access?
11. Which organization boundary, if any, is required by the first segment?
12. What state transitions require atomicity/idempotency?
13. Which external failures can leave durable partial state?
14. What actions require audit records?

## What can be prepared without closing the gate

Safe to continue:
- domain vocabulary;
- invariants and distinctions;
- dependency mapping;
- evidence taxonomy candidates;
- state-machine questions;
- authorization questions;
- failure/recovery scenarios;
- segment-specific domain variants;
- traceability from requirements to candidate concepts.

Must wait for stronger evidence:
- final bounded contexts;
- final aggregate ownership;
- final tenant model;
- final parent relationship semantics;
- intervention case domain;
- autonomous recommendation engine;
- exact AI domain boundary;
- final API/data contracts;
- implementation.


## Minimum Domain Decisions — First Slice

These decisions narrow the domain enough to support Domain Confirmation. They are **PROPOSED**, not silently finalized.

### 1. Goal and Assignment

- **Goal** = intended learning outcome within a learning context.
- **Assignment** = teacher-authorized learning work directed toward a learner or learning group.
- An assignment may support one or more goals.
- A goal may exist without an assignment.
- Assignment completion is activity state; achievement/outcome requires evidence.

**Status:** PROPOSED  
**Remaining open:** exact goal structure, assignment cardinality, due-date semantics, recurrence, and group assignment rules.

### 2. Minimum Learning Mode

The first slice does not require a provider-specific video/live/offline model.

**Proposed minimum:** an asynchronous teacher-led learning activity that can produce a learner action/submission and an assessment/evidence result.

This keeps the domain centered on the learning loop while allowing later learning modes to reuse the same semantic model.

**Status:** PROPOSED  
**Remaining open:** exact first UX modes and whether a reusable Learning Experience abstraction is needed immediately.

### 3. Evidence

Evidence is a first-class learning record with at least:

- source/type;
- actor or producing system;
- learner/context reference;
- timestamp;
- provenance;
- quality/uncertainty where meaningful;
- visibility classification;
- relationship to the relevant action/assessment.

Evidence should be correction/version aware when a correction changes its meaning or accountability. It is not a generic event log.

**Status:** PROPOSED  
**Remaining open:** exact evidence taxonomy, version model, conflict representation, retention, and whether all evidence types share one storage abstraction.

### 4. Progress

The first slice requires an explainable progress representation, but not a universal mastery algorithm.

**Proposed rule:** progress is derived from relevant evidence in the context of a goal/assignment and remains traceable to its supporting evidence.

Candidate first-slice states:

- Not Started
- In Progress
- Needs Review
- Demonstrated
- Not Demonstrated
- Unknown / Insufficient Evidence

These are candidate states, not approved API/database enums.

**Status:** PROPOSED  
**Remaining open:** exact state machine, aggregation rules, teacher override semantics, and whether progress is materialized or calculated.

### 5. Student–Teacher Relationship and Authorization

The first slice requires an explicit relationship/context that establishes why a teacher may act on a learner.

**Proposed rule:**

- Identity does not by itself grant learning access.
- Teacher authority is scoped by an authorized learning context/relationship.
- Assessment, feedback, assignment, and accountable decisions must be authorized within that scope.
- Parent access is separately evaluated from the parent relationship plus policy/consent.

**Status:** PROPOSED  
**Remaining open:** relationship lifecycle owner, organization dependence, delegation, expiry, and group/class membership semantics.

### 6. Parent Visibility

Parent is a controlled projection, not a source of learner truth.

**Proposed rule:**

- Parent visibility is computed from relationship + permission/policy + context.
- Internal teacher notes and restricted evidence are not exposed merely because a parent relationship exists.
- Parent-facing status should communicate meaningful state/change without exposing unnecessary internal workflow detail.
- Parent actions cannot silently mutate teacher-owned learning state.

**Status:** PROPOSED  
**Remaining open:** age/consent rules, guardian edge cases, visibility categories, and country-specific policy.

### 7. Outcome

Outcome is distinct from activity completion.

**Proposed candidate outcomes:**

- Achieved
- Partially Achieved
- Not Demonstrated
- Contradictory
- Unevaluable

Outcome must be supported by relevant evidence and may remain uncertain when evidence is insufficient.

**Status:** PROPOSED  
**Remaining open:** who can declare an outcome, whether it is always required, and how outcome correction/versioning works.

### 8. Durable vs Derived State

**Proposed durable state:**

- authoritative relationships/permissions;
- teacher-authorized assignments/decisions;
- learner submissions/attempts;
- attributable evidence;
- assessment records/results;
- follow-up ownership/status;
- declared outcomes when authoritative;
- audit records for important accountable actions.

**Proposed derived state:**

- dashboard projections;
- summarized progress views;
- recommendation candidates;
- convenience aggregates that can be rebuilt from authoritative records.

Learner progress may be materialized for performance, but its authoritative basis must remain traceable to evidence and context.

**Status:** PROPOSED  
**Remaining open:** exact learner-state aggregate ownership and rebuild/reconciliation strategy.

## Minimum Domain Confirmation Checklist

Before declaring Domain Gate PASS:

- [ ] First slice accepted as the current product boundary.
- [ ] Goal and Assignment distinction accepted.
- [ ] Minimum learning mode accepted.
- [ ] Evidence provenance/correction/conflict rules sufficiently defined.
- [ ] Progress semantics bounded and explainable.
- [ ] Student–Teacher authorization boundary defined.
- [ ] Parent visibility boundary defined enough for first-slice UX/security.
- [ ] Outcome semantics and authority defined.
- [ ] Durable versus derived state identified.
- [ ] Atomicity/idempotency boundaries for the first workflow identified.
- [ ] Material unresolved ambiguity either closed or explicitly deferred without blocking the first slice.

## Current Domain Decision Status

| Area | Status | Implementation consequence |
|---|---|---|
| Goal vs Assignment | PROPOSED | Do not collapse into one generic task model yet |
| Learning mode | PROPOSED | Avoid provider-specific domain commitments |
| Evidence | PROPOSED | Preserve provenance and correction semantics |
| Progress | PROPOSED | Avoid opaque mastery score |
| Teacher authorization | PROPOSED | Authorization must be contextual |
| Parent projection | PROPOSED | Do not expose internal records by default |
| Outcome | PROPOSED | Do not infer outcome from completion alone |
| Durable/derived state | PROPOSED | Dashboards/projections remain rebuildable |
| Intervention Case | OPEN / DEFERRED | No domain model yet |
| Recommendation Engine | OPEN / DEFERRED | No domain model yet |
| Payments / Attendance / Marketplace | OUTSIDE FIRST SLICE | No first-slice dependency |



## Domain Confirmation Candidate — First Slice

The following proposals resolve the remaining domain questions enough to define a coherent first-slice domain contract without freezing implementation details.

### 9. Learning Context Ownership

**Proposed owner:** the Learning domain owns the authoritative learning context for the first slice.

A learning context identifies the scope in which a goal, assignment, evidence, assessment, progress, and teacher authority have meaning. It may reference organization/class/subject/curriculum metadata when those exist, but those structures do not become required first-slice operational domains.

**Invariant:** learning records must not be interpreted outside their applicable context.

**Status:** PROPOSED.

### 10. Assessment Result vs Evidence

Assessment and Evidence are related but not identical:

- **Assessment** owns the assessment definition, attempt/submission lifecycle, and the assessment result as the result of evaluating that assessment.
- **Evidence** owns attributable observations/records that can support interpretation and progress.
- An assessment result can produce evidence.
- Not all evidence is an assessment result.
- Evidence may reference the assessment result that produced it without becoming the assessment aggregate itself.

**Invariant:** progress and outcomes may consume assessment-derived evidence, but must preserve provenance back to the assessment/result where applicable.

**Status:** PROPOSED.

### 11. Follow-up Ownership and Closure

**Proposed owner:** Follow-up owns the lifecycle of an explicitly created piece of unresolved work.

A follow-up requires:
- owner;
- related context;
- reason/trigger;
- current status;
- expected action or resolution condition;
- timestamps/audit where accountable.

**Closure authority:** the actor who owns the accountable action may close the follow-up, subject to any policy requiring new evidence. The system must not infer closure merely because a due date passed or a related activity was completed.

**Evidence rule:** when closure claims an achieved learning outcome, the follow-up may require new attributable evidence or an explicit teacher decision; closure and outcome remain distinct concepts.

**Status:** PROPOSED.

### 12. Evidence Correction and Conflict

Evidence is **version-aware rather than silently mutable** when a change affects meaning or accountability.

Proposed behavior:
- preserve the original record/version;
- create a correction/superseding version with attribution and reason;
- preserve lineage between versions;
- derived progress may use the currently valid version according to policy;
- conflicting evidence is represented as distinct attributable evidence, not overwritten to manufacture consistency;
- interpretation/progress must be able to reflect insufficient or contradictory evidence.

**Invariant:** the system never silently rewrites historical evidence to make a later interpretation appear true.

**Status:** PROPOSED.

### 13. Teacher–Student Relationship Lifecycle

The relationship/authorization model should distinguish:
- relationship existence;
- authorization scope;
- active/inactive lifecycle;
- context/group/class scope where applicable.

**Proposed rule:** teacher authority exists only while the relevant relationship/context authorization is active. Historical records remain attributable after the relationship ends, but new actions are rejected unless another valid authorization exists.

Relationship creation/change is an authorization concern and should be owned by the smallest authoritative scope available in the chosen product configuration; it must not be inferred from messaging or assignment activity.

**Status:** PROPOSED.

### 14. Atomicity and Idempotency Boundaries

For the first learning loop, the platform should treat these as separate durable operations:

1. teacher creates/authorizes assignment;
2. student submits/records attempt;
3. assessment produces result/evidence;
4. teacher records decision;
5. student receives/accepts next action;
6. follow-up is created/updated when required;
7. new evidence is recorded;
8. outcome is declared when authorized.

Within each operation:
- the authoritative state change and its required local audit/outbox record should be atomic where applicable;
- client retries must not create duplicate assignments, submissions, decisions, or follow-ups when an idempotency key is required;
- external delivery (notification/message/provider) must not be assumed to be transactionally atomic with the domain state;
- delivery failures produce recoverable delivery state, not rollback of already-authoritative learning state.

**Status:** PROPOSED.

### 15. Durable State and Rebuild/Reconciliation

Authoritative records are the recovery basis. Derived projections may be rebuilt.

**Proposed rule:**
- authoritative relationship, assignment, attempt/submission, assessment result, evidence, teacher decision, follow-up, and declared outcome records are durable;
- progress summaries, dashboards, recommendation candidates, and convenience aggregates are rebuildable;
- if a materialized projection disagrees with authoritative records, reconciliation repairs the projection rather than rewriting authoritative history;
- every derived learner-state calculation must identify the evidence/context basis sufficiently for explanation.

**Status:** PROPOSED.

### 16. Minimum First-Slice State Transition Chain

The canonical durable chain is:

**Authorized Context → Goal/Assignment → Learner Action/Submission → Assessment Result/Evidence → Teacher Decision → Next Action → Follow-up (if needed) → New Evidence → Outcome**

The following must remain explicitly distinct:
- assignment completion vs learning achievement;
- assessment result vs evidence;
- evidence vs interpretation;
- interpretation/progress vs authoritative teacher decision;
- recommendation vs decision;
- follow-up closure vs outcome declaration.

This chain is a domain invariant, not an API or database design.

**Status:** PROPOSED.

## Domain Confirmation Candidate Checklist

The current proposal is sufficient to enter a Domain Confirmation review, but not sufficient to claim PASS.

- [x] First-slice actors and journey are documented.
- [x] Goal vs Assignment distinction proposed.
- [x] Minimum learning mode bounded.
- [x] Evidence provenance/correction/conflict direction proposed.
- [x] Progress is bounded as evidence-derived and explainable.
- [x] Teacher authorization boundary proposed.
- [x] Parent projection boundary proposed.
- [x] Outcome distinction proposed.
- [x] Durable vs derived state proposed.
- [x] Learning context owner proposed.
- [x] Assessment Result vs Evidence relationship proposed.
- [x] Follow-up ownership/closure direction proposed.
- [x] Atomicity/idempotency boundaries proposed.
- [x] Rebuild/reconciliation direction proposed.
- [ ] Product owner confirmation of the proposed domain contract.
- [ ] Security/privacy confirmation.
- [ ] Data/API design confirmation.

**Candidate status:** DOMAIN CONFIRMATION READY FOR REVIEW — NOT PROVEN.

## Domain Gate checklist

### Product / Evidence
- [ ] First committed product/commercial boundary defined
- [ ] Coherent first journey slice defined
- [ ] Market baseline and requirement traceability reviewed
- [ ] Evidence quality/provenance semantics understood
- [ ] Targeted direct validation completed where a material uncertainty requires it

### Ownership
- [ ] Learner context owner defined
- [ ] Evidence owner/source defined
- [ ] Assessment ownership defined
- [ ] Follow-up ownership defined
- [ ] Closure authority defined

### State
- [ ] Durable state identified
- [ ] Derived state identified
- [ ] Important transitions defined
- [ ] Duplicate/replay semantics known
- [ ] Conflict semantics known

### Access
- [ ] Role relationships known
- [ ] Parent/guardian authorization known where applicable
- [ ] Organization boundary known where applicable
- [ ] Sensitive evidence visibility defined

### External dependencies
- [ ] Provider failure cases identified
- [ ] Partial-success behavior known
- [ ] Retry/idempotency boundaries known
- [ ] Recovery ownership known

## Current gate decision

**Domain Gate: NOT PROVEN**

This checkpoint reduces ambiguity but does not create evidence that is not available.

The current sequence is:

**Product boundary → Requirements consolidation → Domain readiness → Domain confirmation → UX/Architecture/Security/Data/API gates → Implementation.**

Targeted direct cases are optional evidence for material uncertainty; they are not a prerequisite for ordinary parity/product planning.

No architecture or implementation decision is authorized by this document.
