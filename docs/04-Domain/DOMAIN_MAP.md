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
| Evidence | SCENARIO-DERIVED | Time/context/source-bound learning evidence | Medium |
| Learner State / Progress | SCENARIO-DERIVED | Evidence-derived learner state and progress views | Medium |
| Communication | EVIDENCE-BACKED | Contextual messages and role handoffs | High |
| Notification | EVIDENCE-BACKED | User notification lifecycle and delivery state | High |
| Follow-up / Work Item | SCENARIO-DERIVED | Ownership, due state and completion of asynchronous work | Medium |
| Payment | EVIDENCE-BACKED | Paid transaction lifecycle where applicable | High |
| Audit | EVIDENCE-BACKED | Traceability of important state-changing actions | High |
| AI Assistance | SCENARIO-DERIVED | Bounded AI-assisted workflows with human accountability | Medium |
| Intervention Case | OPEN | Cross-role intervention lifecycle | Low |
| Recommendation | OPEN | Evidence-based next-action recommendations | Low |
| Marketplace | OPEN | Multi-party discovery/commerce | Low |
| Social / Community | OPEN | Peer/community interaction | Low |

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

## Segment-Dependent Branches

The following must remain conditional until the beachhead is selected:

- Parent domain/relationship behavior
- Organization/branch management
- Attendance
- Scheduling
- Offline synchronization
- Payments
- Live learning
- Intervention Case
- AI assistance
- Marketplace
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

1. What is the validated initial segment?
2. Which roles actually participate in the first workflow?
3. Which domain owns learner context?
4. What is authoritative evidence for each first workflow?
5. What state must be durable?
6. What state can be derived?
7. What is the required consistency level?
8. What privacy/consent boundary applies to each role?
9. Which capabilities are inside the first transaction?
10. Which concepts should be modular boundaries versus internal models?
11. Which external providers are allowed to be failure-prone dependencies?
12. What observability and audit guarantees are required?

## Current Decision

**Domain Gate: NOT PROVEN**

This document authorizes only further domain analysis. It does not approve bounded contexts, aggregates, database schemas, APIs, technology choices, or implementation.

Next step: produce Architecture Gate inputs after segment and requirement decisions are sufficiently resolved.
