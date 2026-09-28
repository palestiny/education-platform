# Initial Segment Decision Framework

**Status:** Decision-Support Artifact — PROPOSED  
**Gate:** Product Foundation — NOT PROVEN  
**Implementation:** Not authorized  
**Date:** 2026-09-28

## Purpose

Provide a neutral, explicit framework for comparing the candidate initial segments before any beachhead decision is made.

This artifact does **not** rank the segments, assign an overall score, or select a winner. It makes the decision variables and downstream consequences visible so that a later product decision can be evidence-based.

## Candidate Segments

1. Private tutoring — teacher + parent + student
2. Tutoring center / academy — teacher + coordinator + parent + student
3. School — teacher + coordinator/admin + parent + student

## Decision Criteria

| Criterion | Private tutoring | Tutoring center / academy | School |
|---|---|---|---|
| Problem / need fit | OPEN | OPEN | OPEN |
| Core journey coherence | OPEN | OPEN | OPEN |
| Role count and relationship complexity | Lower role count is plausible; verify actual combinations | Multiple operational and learning roles are likely; verify | Multiple learning, family and administrative roles are likely; verify |
| Operational complexity | Likely concentrated around individual teaching relationships; verify | Likely includes scheduling, attendance, ownership and coordination; verify | Likely includes classes, calendars, policies, administration and integrations; verify |
| Payment / commercial complexity | Depends on tutor-to-family transaction | May include family payments, packages, branches or staff workflows | May include institutional contracts, fees and/or external payment systems |
| Evidence availability | Depends on access to tutors, parents and recent learner cases | Depends on access to centers and permission to inspect workflows | Depends on school access, permissions and institutional constraints |
| Implementation complexity | Potentially narrower first operating boundary; verify | Organization/branch/permission complexity may be foundational | Organization, academic context, privacy and integration complexity may be foundational |
| Expansion path | Individual tutor → center/academy → broader platform is a possible path | Center → multi-branch/academy → broader platform is a possible path | School → organization/network → broader platform is a possible path |
| Regulatory / privacy risk | Depends strongly on learner age and data collected | Depends on minors, organizational policies and data sharing | Potentially substantial because of institutional and student-data governance; verify |
| Differentiation opportunity | OPEN; requires direct workflow evidence for novel claims | OPEN; requires direct workflow evidence for novel claims | OPEN; requires direct workflow evidence for novel claims |
| Dependence on advanced / AI capabilities | Should not be assumed; Foundation must stand without speculative AI | Should not be assumed; Foundation must stand without speculative AI | Should not be assumed; Foundation must stand without speculative AI |
| Foundation-loop fit | Candidate loop can be centered on learner ↔ tutor | Candidate loop must coordinate learner ↔ teacher ↔ coordinator/parent where relevant | Candidate loop must coordinate learner ↔ teacher ↔ parent ↔ school context where relevant |

**Important:** Words such as “likely”, “potentially”, and “may” above are planning hypotheses, not validated market facts.

## Evidence Required Before Selection

For the segment decision, direct evidence should establish where possible:

- recurring real recent workflow;
- material friction or cost;
- cross-role dependency;
- evidence fragmentation;
- decision ownership;
- action ownership;
- follow-up burden;
- closure visibility;
- outcome evidence;
- current tools and workarounds;
- failure and recovery behavior;
- privacy/consent boundaries;
- human accountability;
- a segment-specific value hypothesis.

No numeric threshold is fixed in advance. Contradictory evidence must be preserved rather than averaged away.

## Candidate Foundation Loops

These are conceptual scenarios, not validated user journeys.

### Private tutoring

**Goal → learning/session → practice/assessment → evidence → progress → next action → feedback/follow-up**

Primary relationship to test: student ↔ tutor, with parent involvement where applicable.

### Tutoring center / academy

**Goal → scheduled learning → practice/assessment → evidence → progress → next action → teacher/coordinator action → parent communication/follow-up**

Additional context to test: organization, branch, teacher assignment, attendance, scheduling and payment.

### School

**Curriculum/class context → learning → practice/assessment → evidence → progress → next action → teacher action → parent communication/follow-up**

Additional context to test: class/group, academic calendar, school policy, authorization, privacy and integrations.

## What Can Be Designed Before Selection

The following can remain product-level principles or planning candidates:

- multi-role identity and relationships;
- evidence as a first-class concept;
- evidence-backed progress;
- clear next useful action;
- context-preserving communication;
- follow-up/outcome visibility;
- reliability and recovery;
- privacy, permissions, consent and audit;
- global-ready localization foundations.

These do not define the first MVP by themselves.

## What Must Stay Segment-Dependent

Do not commit yet to:

- exact role set;
- parent portal semantics;
- organization/tenant depth;
- attendance/scheduling depth;
- offline requirements;
- payment flow;
- organization-level unresolved work;
- intervention as a first-class domain;
- exact AI use cases;
- exact learning modes;
- exact data/API contracts.

## Decision Sequence

**Direct evidence → segment decision → committed core journey → Foundation requirements → segment-required advanced capabilities → domain confirmation → UX/architecture gates.**

Selecting architecture, technology, or implementation scope before this sequence is complete would invert the project's governance order.

## Current Decision State

**Initial segment: NOT SELECTED.**

**Product Foundation Gate: NOT PROVEN.**

The repository is ready for targeted validation, but this framework does not substitute for participant evidence when the decision depends on real workflow behavior.

## Next Action

Collect and record real recent cases for the candidate segments using the existing research instruments. Then update the segment matrix and this framework with evidence-backed findings, including contradictory evidence.

