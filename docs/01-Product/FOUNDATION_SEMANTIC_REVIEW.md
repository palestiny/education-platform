# Foundation Semantic Review

**Date:** 2026-09-28  
**Status:** Product Semantics Review — PROPOSED  
**Gate:** Product Foundation — NOT PROVEN  
**Implementation authorization:** None

## Purpose

Review the Foundation Learning Loop across the three candidate initial segments without selecting, ranking, or scoring a segment.

The review separates:

- semantics that can remain common at product level;
- semantics that change materially by segment;
- requirements that can remain proposed now;
- requirements that must remain conditional/open/not proven;
- contradictions and risks that require evidence;
- direct evidence needed before a semantic decision is committed.

This document is a decision-support artifact. It does not authorize MVP commitment, domain finalization, architecture, API/data contracts, or implementation.

---

## 1. Canonical Semantic Model

The current product-level semantic loop is:

**Context → Goal → Learning Action → Evidence → Interpretation → Progress State → Next Useful Action → Feedback / Follow-up → New Evidence**

The semantic model deliberately separates observation from interpretation and recommendation from decision.

### Core semantic boundaries

| Boundary | Rule |
|---|---|
| Activity ≠ Evidence | Performing an activity does not prove learning or improvement. |
| Evidence ≠ Interpretation | Observed facts must remain distinguishable from what they may mean. |
| Interpretation ≠ Decision | A system or human interpretation does not itself authorize consequential action. |
| Progress ≠ Activity Count | Progress must be related to a goal/context and supported by evidence. |
| Recommendation ≠ Decision | The product may suggest a next action without silently making the decision. |
| Communication ≠ Follow-up | Sending a message does not prove that required work was completed. |
| Follow-up ≠ Outcome | Closing a task/status does not prove that the intended learning result occurred. |
| Outcome ≠ Assumption | Closure requires later evidence where the outcome matters. |

These are product-semantic candidates, not final domain models.

---

## 2. Semantic Review: Private Tutoring

### Conceptual loop

**Learner goal → tutor/learner learning action → practice/assessment → evidence → interpretation → progress → next useful action → tutor/parent feedback or follow-up → new evidence**

### What is semantically distinctive

1. The learner, tutor and parent may operate without a formal educational organization.
2. The tutor may be both evidence producer and decision owner.
3. Parent involvement may range from essential to absent depending on the actual learning relationship.
4. Scheduling and payment may be part of the commercial workflow without being part of the learning loop.
5. Evidence may be heavily human-observation based in addition to assessment/practice results.

### Questions that cannot be finalized yet

- Whether parent visibility is required in the first journey.
- Whether parent communication belongs inside or outside the core learning loop.
- Whether tutor observations need structured evidence capture.
- Whether scheduling/payment is required for the first transaction.
- Whether follow-up is lightweight or requires a formal work item.
- Which learning evidence is sufficiently trustworthy for progress.

### Semantic risk

A private-tutoring product can accidentally treat tutor notes, parent messages or attendance as equivalent to demonstrated learning. The semantic model must prevent that regardless of UI.

### Direct evidence needed

Reconstruct recent cases showing:

- what the learner was trying to achieve;
- what the tutor observed or measured;
- who interpreted the evidence;
- who decided the next action;
- whether a parent was involved;
- how follow-up was remembered;
- what later evidence showed;
- what happened when the tutor/parent missed the handoff.

---

## 3. Semantic Review: Tutoring Center / Academy

### Conceptual loop

**Learner goal → scheduled learning → practice/assessment → evidence → interpretation → progress → next useful action → teacher/coordinator action → parent communication/follow-up → new evidence**

### What is semantically distinctive

1. The learning context includes an organization and potentially a branch.
2. Decision ownership and action ownership may be different people.
3. Teacher-to-coordinator-to-parent handoffs may become part of the learning-support workflow.
4. Scheduling, attendance, packages/payments and teacher assignment may interact with learning context.
5. Operational state may need to remain distinguishable from learner progress.

### Questions that cannot be finalized yet

- Whether organization/branch context is foundational to the first journey.
- Whether attendance is learning evidence, operational evidence, or both in different contexts.
- Whether coordinator ownership requires a first-class follow-up/work-item model.
- Whether payment/package state is required to authorize or contextualize learning.
- Whether parent communication is a core loop step or a conditional branch.
- Whether teacher changes or branch changes affect learner-state continuity.

### Semantic risk

The product can collapse operational status into learning status. For example, attendance, package completion or scheduled-session completion may be operational facts without proving mastery.

### Direct evidence needed

Reconstruct recent cases involving:

- teacher → coordinator → parent handoffs;
- unresolved learner issues;
- ownership changes;
- missed follow-up;
- conflicting evidence;
- attendance/scheduling effects on decisions;
- later evidence of improvement or non-improvement;
- existing tools and manual reconstruction.

---

## 4. Semantic Review: School

### Conceptual loop

**Curriculum/class context → learning → practice/assessment → evidence → interpretation → progress → next useful action → teacher action → parent communication/follow-up → new evidence**

### What is semantically distinctive

1. Curriculum, class/group and academic-period context may be authoritative parts of interpretation.
2. Multiple organizational roles may have different authorization levels.
3. Parent visibility is constrained by privacy, consent, age/context and school policy.
4. Existing SIS/LMS/communication systems may already own parts of the record.
5. Teacher decision-making may occur within school policies rather than an individual teacher's discretion.

### Questions that cannot be finalized yet

- Which system is authoritative for each evidence type.
- How curriculum versions affect progress interpretation.
- Which roles may view, create or act on learner evidence.
- Parent visibility and consent semantics.
- Required integration boundaries.
- Whether school-level unresolved work is part of the first journey.
- How academic-calendar transitions affect continuity.

### Semantic risk

A school product can accidentally make a local dashboard interpretation appear to be authoritative learner truth while ignoring curriculum version, policy, authorization or external system ownership.

### Direct evidence needed

Reconstruct recent cases showing:

- class/curriculum context;
- evidence sources across systems;
- teacher interpretation and decision;
- administrative authorization where applicable;
- parent communication;
- follow-up ownership;
- later outcome evidence;
- privacy/consent boundaries;
- external-system dependency and failure/recovery.

---

## 5. Semantic Invariants Across All Three Segments

The following semantics can remain common as product-level candidates:

### 5.1 Context is required for meaningful interpretation

Evidence without sufficient context must not automatically become learner state.

### 5.2 Evidence has provenance

Consequential evidence should retain source, time, context and relevant quality/uncertainty metadata.

### 5.3 Progress is evidence-backed

Activity telemetry may support interpretation but should not be presented as learning progress by default.

### 5.4 Recommendation and decision remain separate

The system can assist; an accountable human or explicitly authorized deterministic rule owns consequential decisions.

### 5.5 Follow-up has lifecycle state

Where asynchronous work exists, due/completed/overdue/superseded states should be distinguishable from communication delivery.

### 5.6 Closure needs evidence when outcome matters

A completed task is not automatically a successful learning outcome.

### 5.7 Permissions are semantic, not cosmetic

Visibility and action rights must follow role, relationship, organization and applicable consent/policy context.

### 5.8 Failures must preserve the learning story

A failed submission, notification or external provider call must not silently erase, duplicate or corrupt the learner's evidence/context.

---

## 6. Segment-Dependent Semantic Differences

| Semantic area | Private tutoring | Tutoring center | School |
|---|---|---|---|
| Primary context | Learner + tutor + subject/goal | Learner + organization/branch + teacher + schedule | Learner + class + curriculum + academic period |
| Parent role | Potentially direct participant | Likely operational participant if validated | Potentially governed external participant |
| Decision ownership | May be concentrated | May be split | Often constrained by role/policy |
| Organization context | Optional | Potentially foundational | Potentially foundational |
| Evidence sources | Tutor observation + practice/assessment | Teacher + coordinator + operational signals | Teacher + assessment + institutional systems |
| Scheduling | Potentially commercial/learning context | Potentially core operation | Calendar/class structure may be authoritative |
| Attendance | Contextual | Potentially operationally important | Potentially institutionally required |
| Payments | Potential first transaction | Potentially integrated with packages/fees | Business model may differ |
| Privacy/consent | Relationship-dependent | Organization policy + relationships | Stronger policy/system constraints may apply |
| External systems | Usually fewer | Potentially multiple operational systems | SIS/LMS/communication integration may matter |
| Follow-up | May be lightweight | May require explicit ownership | May require policy-aware workflow |

This table describes semantic dependencies, not product quality or segment preference.

---

## 7. Requirements That Can Remain Common Now

The following can remain **PROPOSED** at product level while exact behavior stays open:

- Multi-role identity and relationship-aware authorization.
- Context preservation.
- Meaningful evidence with provenance.
- Evidence quality and uncertainty.
- Evidence-backed progress.
- Clear next useful action as a product objective.
- Teacher signal/evidence/decision/action visibility where teachers participate.
- Context-preserving communication.
- Follow-up state where asynchronous work exists.
- Outcome evidence for consequential workflows.
- Reliability/recovery for critical learning flows.
- Privacy, permissions, consent and auditability.
- Localization/RTL/LTR separation from learning semantics.
- Human accountability for high-impact decisions.
- AI assistance as bounded support rather than learner-truth ownership.

These remain candidates; they are not implementation-authorizing commitments.

---

## 8. Requirements That Must Remain Conditional / Open / Not Proven

### Conditional on segment

- Parent portal/visibility behavior.
- Organization and branch management.
- Attendance.
- Scheduling depth.
- Offline synchronization.
- Payments and fee semantics.
- Exact learning modes.
- Organization-level unresolved work.

### Open because semantics are unresolved

- Exact progress calculation.
- Mastery model.
- Evidence aggregation.
- Learner-state source of truth.
- Tenancy/isolation strategy.
- Authoritative system for evidence.
- Search scope and permission model.
- Exact business transaction/free-paid boundary.

### Not proven because the proposed workflow is differentiating or high-impact

- Formal intervention/case management.
- Autonomous or semi-autonomous recommendations.
- Controlled AI actions.
- Any claim that a cross-role intervention workflow has recurring material friction.
- Any claim about prevalence, severity, cost or outcome improvement.

---

## 9. Contradictions and Risks Revealed by the Review

### R1 — One canonical loop can hide different ownership models

The semantic sequence is common, but ownership of interpretation, decision, action and follow-up may differ materially.

**Implication:** ownership cannot be hard-coded before segment selection.

### R2 — Parent participation is not one universal semantic role

Parent can be a direct actor, an observer, a communication recipient, or outside a particular workflow.

**Implication:** do not model parent visibility as a universal dashboard requirement.

### R3 — Operational evidence can be mistaken for learning evidence

Attendance, payment, schedule completion and communication are useful context but do not automatically demonstrate mastery.

**Implication:** evidence types need semantic classification before progress logic.

### R4 — External systems may own authoritative facts

Especially in schools, a platform may consume rather than own some curriculum, enrollment, grade or identity facts.

**Implication:** source-of-truth and synchronization semantics must be resolved before data architecture.

### R5 — Follow-up does not automatically justify a case-management domain

A due reminder can be enough for some workflows; a formal intervention lifecycle may be unnecessary.

**Implication:** validate recurring material coordination friction before introducing a first-class Intervention Case.

### R6 — Progress cannot be finalized from the loop alone

The loop establishes what progress means semantically, but not how to calculate it.

**Implication:** assessment/evidence research must precede mastery/progress rules.

---

## 10. Direct Evidence Needed to Resolve the Remaining Semantic Questions

The next research cases should capture facts, not desired features.

For every real case, record:

1. Segment and participating roles.
2. Learning goal/context.
3. Actual learning action.
4. Evidence that existed at the time.
5. Evidence source and quality.
6. Interpretation made from the evidence.
7. Decision owner.
8. Action owner.
9. Communication/context handoff.
10. Follow-up requirement and due state.
11. Later evidence.
12. Outcome/closure determination.
13. Existing tools/workarounds.
14. Failure/recovery behavior.
15. Privacy/consent boundary.
16. Contradictions and unknowns.

Additional semantic probes:

- Which facts were considered authoritative?
- Which facts were disputed or stale?
- What information had to be reconstructed manually?
- What caused a decision to be delayed?
- What made follow-up succeed or fail?
- What evidence was considered enough to close the loop?
- Where did human judgment remain necessary?
- Where would an automated recommendation have been unsafe or premature?

---

## 11. Current Gate State

- Initial segment: **NOT SELECTED**
- Direct participant cases: **0**
- Foundation semantic model: **PROPOSED**
- Foundation semantic review: **PROPOSED**
- Product Foundation Gate: **NOT PROVEN**
- Domain Gate: **NOT PROVEN**
- Architecture Gate: **NOT PROVEN**
- Implementation: **NOT AUTHORIZED**

## Next Decision Sequence

**Real cases → evidence synthesis → initial segment decision → committed Core Journey → exact Foundation requirements → Product Foundation Gate → Domain/UX → Architecture**

The project should not skip from semantic review directly to implementation.

## Non-Authorization Boundary

This review does not authorize:

- segment selection;
- MVP commitment;
- bounded contexts or aggregates;
- database schema;
- API contracts;
- UX implementation;
- architecture or technology selection;
- AI provider selection;
- coding.
