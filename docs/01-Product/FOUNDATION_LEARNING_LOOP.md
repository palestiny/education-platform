# Foundation Learning Loop

**Status:** Product Semantics Draft — PROPOSED  
**Gate:** Product Foundation — NOT PROVEN  
**Implementation:** Not authorized  
**Date:** 2026-09-28

## Purpose

Define the smallest product-level learning loop that can remain meaningful across the candidate initial segments without prematurely selecting a segment, finalizing bounded contexts, or authorizing implementation.

The loop is a semantic model for requirements and research. It is not an API, database schema, aggregate model, UX specification, or architecture decision.

## Canonical Loop

**Context → Goal → Learning Action → Evidence → Interpretation → Progress State → Next Useful Action → Feedback / Follow-up → New Evidence**

The loop is intentionally evidence-driven.

A product event such as opening a video, attending a session, submitting homework, or receiving a grade is not automatically progress. It becomes useful evidence only when its meaning, source, timing and relationship to the learner's goal are sufficiently clear.

## 1. Context

Context answers: **"What situation are we operating in?"**

Candidate context can include:

- learner;
- subject/course;
- curriculum or learning objective;
- class/group/session;
- teacher or tutor;
- organization/branch where applicable;
- locale, academic period and permissions;
- current goal;
- relevant recent learning history.

Context is not the same as learner state. Context describes the frame in which evidence is interpreted.

## 2. Goal

Goal answers: **"What are we trying to achieve?"**

Examples:

- understand a concept;
- complete a learning objective;
- prepare for an assessment;
- improve a demonstrated skill;
- complete assigned work;
- recover from a demonstrated learning gap.

A goal may be explicit or derived from an assigned curriculum/learning plan. The exact goal model remains segment-dependent.

## 3. Learning Action

Learning Action answers: **"What did the learner or educator actually do?"**

Possible actions:

- lesson/session;
- explanation;
- video or reading;
- practice;
- assignment;
- assessment;
- feedback;
- remediation;
- human help;
- follow-up activity.

An action is an observable activity. It must not be treated as proof of learning by itself.

## 4. Evidence

Evidence answers: **"What do we actually know?"**

Candidate evidence types:

| Evidence | Meaning |
|---|---|
| Assessment result | Observed performance on a defined assessment |
| Practice result | Observed performance during practice |
| Assignment submission | Work produced by the learner |
| Teacher/tutor observation | Human observation with source/context |
| Attendance/participation | Evidence of presence/participation, not mastery |
| Learner self-report | Learner-reported understanding, difficulty or confidence |
| Parent observation | Parent-reported context, where authorized and relevant |
| AI-generated signal | Machine-produced signal that requires provenance and uncertainty |
| Communication record | Evidence that information was exchanged, not proof of outcome |

Every consequential evidence item should conceptually retain:

- source;
- timestamp;
- context;
- subject/skill/objective where known;
- observed value;
- provenance;
- confidence/uncertainty where applicable;
- actor or system that produced it;
- permissions governing visibility.

### Evidence vs Interpretation

**Evidence:** "Student answered 6 of 10 questions correctly."

**Interpretation:** "Student may have difficulty with fractions."

The second statement must not be stored or presented as if it were the first.

## 5. Interpretation

Interpretation answers: **"What might the evidence mean?"**

Interpretation can be produced by:

- learner;
- teacher/tutor;
- authorized organization role;
- deterministic product logic;
- AI assistance.

Interpretation must remain distinguishable from source evidence.

AI-generated interpretation must not silently become authoritative learner state. High-impact interpretations require appropriate human accountability and traceability.

## 6. Progress State

Progress answers: **"What has changed relative to the goal?"**

Progress should not be defined only as:

- time spent;
- number of videos watched;
- number of logins;
- attendance count;
- number of completed screens.

Those may be activity signals.

A candidate progress state may combine:

- demonstrated performance;
- curriculum/objective completion;
- skill/mastery evidence where defined;
- consistency over time;
- confidence/uncertainty;
- recency;
- unresolved learning gaps;
- goal status.

The exact calculation, thresholds, mastery semantics and aggregation rules remain **OPEN** until segment and assessment evidence are sufficiently defined.

## 7. Next Useful Action

Next Useful Action answers: **"Given the current evidence and goal, what is the most useful next step?"**

It may be:

- continue;
- practice;
- retry;
- review a prerequisite;
- ask for human help;
- complete an assignment;
- reassess;
- communicate with a relevant person;
- take no action yet because evidence is insufficient.

The system may suggest an action, but **recommendation is not decision**.

Where a consequential action affects a learner, parent, teacher or organization, ownership and authorization must be explicit.

## 8. Feedback / Follow-up

Feedback answers: **"What happened after the action, and what needs to happen next?"**

Follow-up should make unresolved work visible without turning every learning event into a case-management workflow.

Candidate follow-up states:

- not required;
- planned;
- due;
- completed;
- overdue;
- cancelled;
- superseded;
- escalated.

Whether a formal intervention/work-item domain is required remains **NOT PROVEN**.

## 9. New Evidence and Loop Closure

A loop closes when a later evidence point allows the product or human to determine whether the previous action:

- produced the intended improvement;
- produced partial improvement;
- produced no demonstrated improvement;
- produced contradictory evidence;
- cannot yet be evaluated.

Closure is therefore evidence-based, not merely status-based.

## Segment Variants

### Private Tutoring

Conceptual flow:

**Learner goal → tutor/learner session → practice/assessment → evidence → interpretation → progress → next action → tutor/parent feedback or follow-up → new evidence**

Key questions remaining open:

- exact parent involvement;
- scheduling/payment scope;
- how tutor observations become evidence;
- whether follow-up needs a formal work item.

### Tutoring Center / Academy

Conceptual flow:

**Learner goal → scheduled learning → practice/assessment → evidence → interpretation → progress → next action → teacher/coordinator action → parent communication/follow-up → new evidence**

Additional context may include:

- organization;
- branch;
- teacher assignment;
- attendance;
- scheduling;
- package/payment state.

Whether all of these belong in the first product boundary remains open.

### School

Conceptual flow:

**Curriculum/class context → learning → practice/assessment → evidence → interpretation → progress → next action → teacher action → parent communication/follow-up → new evidence**

Additional context may include:

- class/group;
- academic calendar;
- school policies;
- authorization;
- privacy/consent;
- existing SIS/LMS integrations.

The exact school operating model remains uncommitted.

## Common Semantics vs Segment-Dependent Semantics

### Safe as product-level candidates

These can guide further requirements work without selecting a segment:

- evidence is distinct from interpretation;
- activity is distinct from progress;
- progress is relative to a goal/context;
- recommendations are distinct from decisions;
- follow-up is distinct from communication;
- evidence should retain provenance and uncertainty where relevant;
- consequential actions require ownership and authorization;
- loop closure requires later evidence;
- reliability and recovery are part of trustworthy learning behavior.

### Must remain segment-dependent

Do not finalize yet:

- exact roles and relationships;
- parent visibility and consent;
- organization/tenant depth;
- curriculum model;
- attendance semantics;
- scheduling;
- payments;
- offline synchronization;
- formal intervention/case management;
- mastery calculation;
- AI recommendations and controlled actions;
- exact learning modalities;
- API/data contracts.

## Research Implications

This semantic model identifies the questions that direct evidence should answer.

A real case should establish, where observable:

1. the learner's goal/context;
2. the actual learning action;
3. evidence used by a human or system;
4. how evidence was interpreted;
5. who owned the decision;
6. what action was taken;
7. how communication/context was preserved;
8. what follow-up was required;
9. what later evidence established;
10. what happened when evidence was missing, stale, conflicting or wrong.

This does not mean every conventional feature requires a direct case. Direct validation remains focused on uncertain, segment-specific, materially different, differentiating, high-impact or prevalence/severity/cost claims.

## Current Unknowns

- Initial segment: **NOT SELECTED**
- Direct participant cases: **0**
- Product Foundation Gate: **NOT PROVEN**
- Progress calculation: **OPEN**
- Mastery semantics: **OPEN**
- Intervention domain: **NOT PROVEN**
- Parent semantics: **CONDITIONAL / OPEN**
- Tenancy model: **OPEN**
- AI scope and governance details: **OPEN**
- MVP boundary: **PROPOSED / NOT PROVEN**

## Non-Authorization Boundary

This document does **not** authorize:

- segment selection;
- MVP commitment;
- bounded-context finalization;
- aggregate design;
- database schema;
- API contracts;
- UX implementation;
- architecture selection;
- technology selection;
- AI provider selection;
- coding.

## Next Gate

Use this model to:

1. inspect whether the three candidate segment loops are semantically coherent;
2. identify which semantics are genuinely common;
3. collect direct real cases where uncertainty materially affects the product decision;
4. select the initial segment explicitly;
5. commit the first Core Journey;
6. define exact Foundation requirements;
7. then proceed to Product Foundation Gate review.
