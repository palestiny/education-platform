# Candidate Domain Map

Date: 2026-09-25
Status: Domain Design Input — CANDIDATE
Gate: Domain Gate — NOT PROVEN
Implementation authorization: None

## Purpose

Translate the reviewed requirements into candidate domain concepts without prematurely freezing bounded contexts, aggregates, persistence, APIs, or architecture.

## Evidence Classification

- EVIDENCE-BACKED: repeated in validated market evidence or explicit governance/product requirements.
- SCENARIO-DERIVED: necessary to express the current target scenario but not yet directly validated.
- OPEN: materially depends on segment or unresolved product evidence.

## Semantic Backbone from Product Operating Model

The current product semantics that the domain must be able to represent are:

**Context → Goal → Learning Action → Evidence → Interpretation → Progress State → Recommendation → Decision → Follow-up → Outcome**

These are semantic concepts, not automatically separate entities, aggregates, tables, or bounded contexts. Domain design must preserve the distinctions even when implementation later combines or derives them.

### Source-of-truth constraints

- Evidence records attributable observations/submissions; it is not a generic event bus.
- Interpretation explains evidence and may be derived or persisted according to later business value/audit needs.
- Progress is traceable to evidence and context; exact calculation remains open.
- Recommendation proposes; Decision is an authorized commitment.
- Communication carries context but does not own authoritative learning/commerce state.
- Follow-up tracks remaining work; it is not automatically an Intervention Case.
- Outcome represents result, including uncertainty; it is not equivalent to activity completion.
- Dashboards are projections, not lifecycle owners.

## Candidate Concept Map

| Concept | Classification | Candidate responsibility | Current confidence |
|---|---|---|---|
| Identity | EVIDENCE-BACKED | Person identity and authentication context | High |
| Role / Permission | EVIDENCE-BACKED | Role assignment and authorization boundaries | High |
| Organization / Tenant | SCENARIO-DERIVED | Organizational ownership/isolation | Medium |
| Relationship | SCENARIO-DERIVED | Parent-child, teacher-student and other permitted relationships | Medium |
| Learning Context | SCENARIO-DERIVED | Curriculum/grade/subject/locale/goal context | Medium |
| Learning Experience | EVIDENCE-BACKED | Course/session/class/learning activity lifecycle | High |
| Learning Content | EVIDENCE-BACKED | Reusable learning material and delivery metadata | High |
| Assignment / Practice | EVIDENCE-BACKED | Assigned work and practice activity | High |
| Assessment | EVIDENCE-BACKED | Attempts, questions, submissions and assessment results | High |
| Evidence | SCENARIO-DERIVED | Attributable evidence with source/context/time/provenance/quality | Medium |
| Learner State / Progress | SCENARIO-DERIVED | Evidence- and goal-relative learner state/progress | Medium |
| Communication | EVIDENCE-BACKED | Contextual messages and role handoffs | High |
| Notification | EVIDENCE-BACKED | User notification lifecycle and delivery state | High |
| Follow-up / Work Item | SCENARIO-DERIVED | Ownership, due state and completion of asynchronous work | Medium |
| Payment | EVIDENCE-BACKED | Paid transaction lifecycle where applicable | High |
| Audit | EVIDENCE-BACKED | Traceability of important state-changing actions | High |
| Decision | SCENARIO-DERIVED | Authorized commitment/action or state change | Medium |
| Interpretation | SCENARIO-DERIVED | Meaning assigned to evidence; human/system/AI-assisted | Medium |
| Recommendation | OPEN | Proposed next useful action; not authoritative state | Low |
| Marketplace | OPEN | Multi-party discovery/commerce | Low |
| Follow-up / Work Item | SCENARIO-DERIVED | Remaining work, ownership and due state | Medium |

## Candidate Ownership Boundaries

These are working boundaries, not accepted bounded contexts:

1. Identity owns person identity and authentication.
2. Authorization owns permission evaluation, but final ownership may remain coupled to Identity/Organization design.
3. Organization owns organizational structure and tenant context if the chosen segment requires it.
4. Learning owns learning experiences and learning activities.
5. Content owns reusable educational material.
6. Assessment owns assessment definitions, attempts and results.
7. Evidence owns evidence records and provenance if evidence becomes a first-class model.
8. Learner State derives or maintains learner state from permitted evidence; the exact source-of-truth model is OPEN.
9. Communication owns messages and contextual handoffs.
10. Notification owns delivery preferences and notification state.
11. Follow-up owns work ownership and due-state only if direct workflow evidence justifies it.
12. Commerce owns payments only if monetization requires it in the beachhead.
13. Audit records important actions but should not become a generic event store.
14. AI Assistance should orchestrate bounded capabilities rather than own learner truth.

## Important Domain Distinctions

### Learning Content != Learning Experience

A video, document or question is content. A session, assignment, practice activity or class is an experience/activity using content.

### Evidence != Interpretation

Evidence records what was observed or submitted. Interpretation explains what that evidence may mean. The platform must preserve the distinction.

### Progress != Activity Count

Progress should not be defined as video views, clicks or time alone.

### Recommendation != Decision

A recommendation can inform a human decision. It must not silently become the decision in high-impact workflows.

### Communication != Intervention

Messaging is a capability. An intervention lifecycle requires ownership, action, follow-up, outcome and closure evidence.

### Dashboard != Source of Truth

Dashboards should project domain state rather than own lifecycle state.

## Candidate Aggregate Questions

No aggregate is approved yet. Domain Gate must answer:

- Is Person/Identity separate from User Account?
- Is Role assignment owned by Identity or Organization?
- Is Tenant a first-class aggregate or organizational context?
- Is Relationship a first-class aggregate?
- Is Evidence immutable/event-like, mutable, or versioned?
- Is Learner State calculated on demand, materialized, or hybrid?
- Is Assessment Result part of Assessment or Evidence?
- Is Follow-up a generic work item or part of a case?
- Is Communication attached to a context/case or independently searchable?
- What is the consistency boundary for evidence-to-progress updates?
- Which state transitions require transactions?
- Which external events require idempotency?

## Product-Boundary-Dependent Branches

The following remain conditional until the first committed product slice/commercial boundary is selected:

- Parent domain/relationship behavior
- Organization/branch management
- Attendance
- Scheduling
- Offline synchronization
- Payments
- Live learning
- Intervention Case
- AI assistance
- Intervention Case
- Marketplace
- Social / Community
- White-label

## Domain Risks

1. Creating an Intervention Case aggregate before validating the problem.
2. Treating Evidence as a generic event bus.
3. Making Learner State an opaque AI-derived score.
4. Coupling parent visibility directly to internal learner records.
5. Mixing tenant isolation rules into every domain without a coherent security model.
6. Building a generic Recommendation engine before a concrete workflow exists.
7. Turning Communication into the source of truth for learning state.

## Gate Questions

Before Architecture Gate:

1. What is the first committed product/commercial boundary and coherent journey slice?
2. Which roles and relationships actually participate in that slice?
3. Which domain owns learner context?
4. What is authoritative evidence for each first workflow?
5. What state must be durable and what state can be derived?
6. What is the required consistency level for evidence-to-state transitions?
7. What privacy/consent boundary applies to each role and relationship?
8. Which capabilities are inside the first transaction?
9. Which concepts should be modular boundaries versus internal models?
10. Which external providers are allowed to be failure-prone dependencies?
11. What observability and audit guarantees are required?
12. Which open semantics can remain configurable/derived without blocking the first slice?

## Current Decision

**Domain Gate: NOT PROVEN**

This document authorizes only further domain analysis. It does not approve bounded contexts, aggregates, database schemas, APIs, technology choices, or implementation.

Next step: produce the Domain Readiness Delta after requirements consolidation, then resolve only the domain decisions required by the first committed product slice.
