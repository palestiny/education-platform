# UX Gate Inputs — First Product Slice

Date: 2026-09-28
Status: UX Gate Preparation — PROPOSED
Implementation authorization: None

## Purpose

Translate the confirmed domain candidate into a user-facing interaction model without designing visual details or freezing implementation technology.

The UX must expose the useful parts of the domain while hiding domain complexity.

## First-Slice UX Scope

Primary actors:
- Student
- Teacher

Secondary projection:
- Parent, only where visibility policy permits.

Organization is contextual only.

The UX follows:

**Authorized Context → Goal/Assignment → Learner Action/Submission → Assessment Result/Evidence → Teacher Decision → Next Action → Follow-up (if needed) → New Evidence → Outcome**

## Core UX Jobs

### Student

1. Understand current context.
2. Know the single most useful next action.
3. Complete assigned learning work.
4. Submit/record work and know whether it was accepted.
5. Understand feedback and progress without needing internal terminology.
6. See what changed after teacher review.
7. Complete follow-up work.
8. Provide new evidence.
9. Understand the resulting outcome or uncertainty.

### Teacher

1. Open the relevant teaching context.
2. Identify learners/work requiring attention.
3. Inspect evidence and its provenance.
4. Distinguish observed evidence from interpretation/recommendation.
5. Make an accountable decision.
6. Create the learner's next action.
7. Create or update follow-up when work remains unresolved.
8. Review new evidence.
9. Declare or update an outcome only when authorized and sufficiently supported.

### Parent

The first slice does not require a full parent workflow.

Where enabled, the parent sees a controlled projection:
- current meaningful status;
- important change;
- whether a teacher action exists;
- relevant follow-up/outcome;
- any permitted parent action.

Restricted teacher notes or evidence must not appear merely because a parent relationship exists.

## Proposed Information Architecture

### Student

- Home / Next Action
- Current Learning Context
- Assignment / Learning Activity
- Submission / Result
- Progress & Evidence
- Follow-up / Next Actions

### Teacher

- Teaching Context
- Attention / Work Queue
- Learner Evidence
- Decision / Feedback
- Next Actions
- Follow-up
- Outcome / History

### Parent projection

- Child Overview
- Current Status
- Meaningful Changes
- Follow-up / Outcome when permitted

These are information structures, not final screens or routes.

## Critical Interaction States

Every first-slice experience must define at least:

- loading;
- empty;
- ready;
- submitting;
- success;
- validation failure;
- unauthorized/forbidden;
- stale/conflicting evidence;
- duplicate/retry;
- timeout/network failure;
- partial external delivery failure;
- unavailable/unknown evidence;
- corrected/superseded evidence.

The UX must distinguish:
- **unknown** from **not demonstrated**;
- **in progress** from **failed**;
- **assignment completed** from **outcome achieved**;
- **recommendation** from **teacher decision**;
- **follow-up closed** from **learning outcome achieved**.

## UX Rules Derived From Domain

1. Never expose raw domain state unless it helps the user's decision.
2. Do not imply achievement from activity completion.
3. Evidence shown to a user must preserve enough provenance to establish trust.
4. When evidence conflicts, communicate uncertainty rather than manufacturing a single truth.
5. A teacher decision must remain attributable.
6. A student action should make its expected result clear.
7. A failed notification must not make the user believe authoritative learning state was lost.
8. Retry should preserve user intent and avoid duplicate submissions.
9. Parent views are projections with explicit visibility boundaries.
10. The product should provide recovery paths instead of exposing infrastructure failures.

## Accessibility and Localization Baseline

First-slice UX should be designed for:
- keyboard/accessibility semantics;
- readable hierarchy and sufficient contrast;
- touch-friendly controls;
- Arabic RTL and English LTR;
- localization without layout assumptions;
- time/date/number formatting by locale;
- clear error and status language;
- reduced-motion compatibility where applicable.

These are product quality constraints, not optional polish.

## UX Acceptance Criteria Before Architecture

The UX Gate should not PASS until the following can be demonstrated:

- [ ] Student can understand and execute the next useful action.
- [ ] Student can submit work and recover from failure without duplicate creation.
- [ ] Student can understand result/progress and its uncertainty.
- [ ] Teacher can find relevant evidence without reconstructing context externally.
- [ ] Teacher can make an attributable decision and produce a next action.
- [ ] Follow-up is visible when unresolved work exists.
- [ ] New evidence can change what the user sees next.
- [ ] Outcome is distinct from activity completion.
- [ ] Parent projection does not expose unauthorized internal information.
- [ ] All critical failure states have understandable recovery UX.
- [ ] Mobile/responsive and RTL/LTR constraints are accounted for.
- [ ] No screen requires users to understand internal domain terminology.

## Deliberately Deferred

- visual design system;
- final navigation architecture;
- complete parent product;
- organization administration UX;
- marketplace;
- advanced analytics;
- autonomous AI workflows;
- full offline UX;
- provider-specific video/live interfaces.

## Gate Status

**UX Gate: NOT PROVEN**

This document is sufficient to begin structured UX design/review, not sufficient to authorize implementation.

Next:
**Domain Confirmation → UX Design Review → Architecture Gate → Security/Data/API Gates**
