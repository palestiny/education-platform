# Direct Workflow Validation Plan

**Status:** Research Protocol Draft  
**Gate:** Product Foundation / Competitive Intelligence — NOT PROVEN  
**Date:** 2026-09-24  
**Implementation:** Not authorized

## Purpose

Move from market-capability evidence to direct evidence about whether a painful, frequent, costly cross-role intervention workflow exists in a specific educational segment.

This protocol is designed to validate the problem before designing the solution.

## Hypothesis under test

> A recurring educational intervention becomes materially harder when evidence, interpretation, decision ownership, intervention, follow-up and outcome evidence are fragmented across students, parents, teachers and/or educational organizations.

This is a hypothesis only. It must not be converted into a requirement until the Product Foundation Gate is explicitly reviewed.

## Initial segment strategy

Do not validate the entire education market at once.

Start with one concrete operating context where multiple roles already interact around the same learner:

- Private tutoring teacher + parent + student
- Tutoring center teacher/coordinator + parent + student
- School teacher/coordinator + parent + student

The first validation cohort should favor workflows that already involve repeated follow-up, remediation, attendance/performance concerns, missing work, parent communication or escalation.

## Participants

Target, as a research heuristic rather than a proof threshold:

- 5–8 teachers
- 5–8 parents
- 5–8 students/learners where age and consent make participation appropriate
- 3–5 coordinators/center or school operators

These numbers are starting points for qualitative discovery, not statistical validation. Increase or change the sample when evidence remains contradictory or segment-specific.

## Interview / observation method

Prefer reconstruction of a real recent case over opinion questions.

Do not ask:

- "Would you use this?"
- "Would this feature be useful?"
- "Do you like this idea?"

Ask for the last real case and reconstruct what actually happened.

### Case reconstruction

For each case capture:

1. **Trigger** — What first caused someone to notice a problem?
2. **Evidence** — What information was available at that moment?
3. **Evidence quality** — Was it complete, recent, trustworthy and understandable?
4. **Interpretation** — How did the person decide what the evidence meant?
5. **Decision owner** — Who was responsible for deciding what to do?
6. **Action owner** — Who actually performed the intervention?
7. **Intervention** — What was done?
8. **Communication** — Who had to be informed, and through which channels?
9. **Due state** — Was there a deadline, reminder or expected follow-up?
10. **Follow-up** — What happened afterward?
11. **Outcome evidence** — What proved improvement, failure or unresolved status?
12. **Closure** — How did everyone know the case was finished?
13. **Workaround** — Which tools, chats, spreadsheets, notes or memory were used?
14. **Cost** — Time, coordination effort, missed learning, financial cost or trust cost.
15. **Failure/recovery** — What happened when a person, system, schedule, connection or communication failed?
16. **Privacy/consent** — What information could each role see, and what should remain restricted?
17. **AI boundary** — If data/AI suggested an action, who verified and approved it?

## Evidence capture rules

Every observation should be tagged:

- **DIRECT OBSERVATION** — observed behavior/workflow.
- **USER REPORT** — participant's stated experience.
- **ARTIFACT EVIDENCE** — timetable, message, report, spreadsheet, record or other real artifact where permission exists.
- **MARKET EVIDENCE** — public product/research evidence.
- **INTERPRETATION** — our synthesis.
- **HYPOTHESIS** — not yet established.

Never convert a participant's opinion into a fact.

## Validation dimensions

Each reconstructed case should be assessed across:

| Dimension | Question |
|---|---|
| Frequency | How often does this workflow occur? |
| Severity | What happens if it is not handled well? |
| Cost | What does the current process consume? |
| Fragmentation | How many people/tools/channels are involved? |
| Ownership | Is responsibility clear? |
| Follow-up | Is follow-up explicit or dependent on memory? |
| Closure | Can users prove the case was resolved? |
| Outcome | Is there measurable evidence of result? |
| Evidence quality | Are signals complete/current/consistent? |
| Privacy | Are visibility boundaries clear? |
| Recovery | Can the workflow resume after failure? |
| Human accountability | Who makes the final educational decision? |
| Segment fit | Is the pain concentrated in a specific context? |
| Existing alternatives | Why are current tools insufficient? |
| Value | Would solving this change a meaningful outcome? |

## Scenario set

Use real cases where possible, then probe these scenario types:

### S1 — Repeated learning gap

Student repeatedly fails the same concept.

Goal: establish whether evidence → interpretation → intervention → reassessment is fragmented across roles.

### S2 — Attendance + performance signal

Attendance changes while academic performance changes.

Goal: determine how users interpret the signals without assuming causation, who decides to intervene and how follow-up occurs.

### S3 — Parent concern

Parent raises a concern about learning, attendance, communication or progress.

Goal: identify routing, ownership, privacy, response, escalation and closure.

### S4 — Missing work / overdue support

Required work or intervention is not completed.

Goal: identify reminder, ownership, communication and closure behavior.

### S5 — Organization-level unresolved case

A teacher has an unresolved learner issue that requires coordination or escalation.

Goal: determine whether an organization needs a case-level view rather than another dashboard.

### S6 — Weak or contradictory evidence

Data is stale, missing or contradictory.

Goal: observe whether users defer action, request more evidence or make a human judgement.

## Falsification criteria

The hypothesis should be weakened or rejected if direct evidence shows that:

- the workflow is rare or low-cost;
- current tools already solve it with little friction;
- fragmentation does not materially affect outcomes or trust;
- ownership/follow-up/closure are already clear in the target segment;
- users do not perceive a meaningful coordination problem;
- the problem exists only because our proposed workflow artificially creates it;
- value cannot be measured beyond adding another dashboard or communication channel.

## Advancement criteria

Do not require arbitrary numerical thresholds alone.

Advance toward Product Requirements only when evidence shows, for a clearly defined segment:

1. The workflow recurs in real cases.
2. The current workaround has material cost/friction.
3. Multiple roles or systems create a coordination problem.
4. Ownership and follow-up are genuinely problematic.
5. Users can identify meaningful outcome evidence.
6. Privacy/visibility constraints are concrete enough to design.
7. Failure/recovery behavior matters to the workflow.
8. Existing alternatives leave a material unmet need.
9. A measurable value hypothesis can be stated.
10. The evidence is strong enough to justify a product decision.

## Research record format

For every validated case create a structured record:

- Case ID
- Segment
- Role(s)
- Date / recency
- Trigger
- Evidence
- Evidence quality
- Interpretation
- Decision owner
- Action owner
- Intervention
- Communication
- Due state
- Follow-up
- Outcome evidence
- Closure
- Workaround
- Cost
- Failure/recovery
- Privacy/consent
- AI/human boundary
- Evidence classification
- Researcher interpretation
- Contradictory evidence
- Open questions

Do not store unnecessary personally identifiable information.

## Gate outcome options

### PASS

Direct evidence supports a specific problem and segment strongly enough to begin Product Requirements.

### GAP

Evidence supports part of the hypothesis but a specific dimension remains insufficient.

### NOT PROVEN

Evidence is insufficient to justify a product requirement.

### REJECT / PIVOT

Direct evidence materially contradicts the hypothesis or shows a different problem deserves investigation.

## Current decision

**NOT PROVEN.**

Public market evidence is useful for mapping existing capabilities, but it is not a substitute for direct workflow evidence. Current research should therefore move toward real-case reconstruction before requirements, domain modeling, UX contracts or architecture are started.
