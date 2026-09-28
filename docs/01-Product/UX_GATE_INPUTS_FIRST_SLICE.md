# UX Gate Inputs — First Product Slice

Date: 2026-09-28
Status: UX Design Review Candidate — PROPOSED
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


## UX Design Review — First Slice

### Review Objective

Turn the UX preparation model into a reviewable end-to-end interaction contract. This defines what each role must be able to understand, decide, do, and recover from. It does not freeze visual design, route names, component libraries, API shapes, or technology.

### Canonical Student Flow

**Entry → Context → Next Action → Assignment → Learn/Practice → Submit → Result → Understand → Next Action/Follow-up → New Evidence → Outcome**

#### Student interaction contract

| Stage | Primary user need | Primary action | Required visible state |
|---|---|---|---|
| Entry | Know where they are | Continue/open context | Authorized context |
| Context | Understand why this work exists | View goal/assignment | Goal + expected completion |
| Learning | Do the work | Learn/practice | Activity state |
| Submit | Know whether work was received | Submit/retry | Accepted / rejected / pending |
| Result | Understand what happened | Review result/evidence | Result + provenance/uncertainty |
| Next action | Know what to do now | Start next action | Clear action + rationale |
| Follow-up | Know what remains unresolved | Complete follow-up | Open / due / completed |
| New evidence | Demonstrate change | Submit new evidence | New evidence/version |
| Outcome | Understand the result | Review outcome | Achieved / partial / not demonstrated / uncertain |

The student home/context should prioritize **one primary next useful action** while allowing access to supporting context. It must not turn every available task into an equally weighted action.

### Canonical Teacher Flow

**Entry → Teaching Context → Attention Queue → Learner Context → Evidence → Interpretation/Recommendation → Teacher Decision → Next Action → Follow-up → New Evidence → Outcome**

#### Teacher interaction contract

| Stage | Primary user need | Primary action | Required visible state |
|---|---|---|---|
| Context | Know class/learner scope | Open context | Active authority/context |
| Attention | Know what needs review | Select work/learner | Reason + urgency/state |
| Evidence | Understand what is known | Inspect evidence | Source, time, provenance, uncertainty |
| Interpretation | Understand possible meaning | Review interpretation/recommendation | Clearly non-authoritative |
| Decision | Make accountable judgment | Decide/record | Actor + decision + timestamp/context |
| Next action | Specify learner work | Create/assign action | Expected result |
| Follow-up | Track unresolved work | Set/update follow-up | Owner + due/closure state |
| New evidence | Verify change | Review new evidence | New vs superseded evidence |
| Outcome | Record supported result | Declare/update outcome | Evidence basis + uncertainty |

The teacher workflow should support **inspect → decide → act** without forcing repeated navigation between unrelated role dashboards.

### Parent Projection Flow

**Child Context → Current Status → Meaningful Change → Teacher/Organization Action Visibility → Permitted Parent Action → Follow-up/Outcome**

The parent projection is intentionally narrower than the teacher workflow. It should answer:
1. What changed?
2. What does that mean at the permitted level?
3. Is someone already acting?
4. What, if anything, should I do?

It must not expose private teacher notes, restricted evidence, internal recommendations, or organizational information merely because the parent has a relationship with the student.

### Information Hierarchy

For the first slice, information priority is:

1. **What context am I in?**
2. **What matters now?**
3. **What action can I take?**
4. **What evidence explains this?**
5. **What happened after the action?**
6. **What remains unresolved?**
7. **What is the resulting outcome?**

Secondary metadata must not visually compete with the next useful action.

### Interaction-State Rules

| State | User-facing meaning | UX behavior |
|---|---|---|
| Loading | System is retrieving state | Preserve context; avoid false empty state |
| Empty | Nothing currently requires action | Explain why, provide useful next path |
| Submitting | User action is being processed | Prevent accidental duplicate intent |
| Success | Authoritative operation accepted | Confirm what changed and next step |
| Validation failure | User input cannot be accepted | Explain correction without losing valid input |
| Unauthorized/Forbidden | User lacks current authority | Explain access boundary; do not leak restricted data |
| Duplicate/retry | Same intent was already processed or is being retried | Show existing authoritative result where possible |
| Timeout/network failure | Completion is uncertain | Do not claim failure if server state is unknown; provide safe retry/reconciliation path |
| Partial delivery failure | Authoritative state exists but notification/projection delivery failed | Show state separately from delivery status |
| Unknown evidence | Evidence unavailable/insufficient | Preserve uncertainty; do not infer a result |
| Conflicting evidence | Multiple valid records disagree | Surface conflict and its effect on interpretation/progress |
| Corrected/superseded | Earlier evidence was replaced/corrected | Preserve lineage and explain current version |

### Progress UX Boundary

The UX may present progress states and evidence-backed explanations, but it must not freeze a universal mastery formula at this stage.

The first slice should communicate:
- current position relative to the relevant goal/context;
- what evidence supports the displayed state;
- whether evidence is sufficient, incomplete, stale, or conflicting;
- what action can produce the next useful evidence.

Avoid presenting activity counts, completion percentages, or AI-generated scores as achievement unless the underlying semantics explicitly support that interpretation.

### Evidence Presentation

Evidence should answer **what happened, when, from which source, and how certain/complete it is**.

The UX should use concise provenance cues rather than exposing internal storage/event terminology. When evidence is corrected or superseded, the user should understand that the current view reflects a newer version without losing historical accountability.

### Recovery Patterns

Every authoritative student/teacher action needs a visible recovery path:

- **Safe retry:** repeat the same intent without creating a second authoritative record.
- **Reconciliation:** when completion is uncertain, check current state before allowing another mutation.
- **Draft preservation:** preserve user-entered information when validation/network failure occurs.
- **Conflict review:** do not silently overwrite concurrent teacher decisions or evidence.
- **Delivery recovery:** allow notification/projection retry without rolling back authoritative learning state.
- **Permission change:** re-evaluate authorization at action time and explain when access has changed.

### Responsive, Mobile, RTL/LTR Implications

The first slice must remain usable on narrow screens without hiding the semantic distinction between action, evidence, decision, and outcome.

Responsive design must preserve:
- primary action visibility;
- readable evidence/provenance;
- status clarity;
- recovery controls;
- touch target usability;
- long localized text;
- Arabic RTL and English LTR ordering;
- locale-specific dates, numbers, and time expressions.

No critical workflow may depend only on hover, pointer precision, color, or wide-screen side-by-side comparison.

### Accessibility Review Baseline

The first-slice design should target:
- semantic headings and landmarks;
- keyboard-complete workflows;
- visible focus;
- screen-reader meaningful labels/status updates;
- non-color-only status communication;
- sufficient text and control readability;
- error association with the affected field/action;
- predictable focus after submission, failure, and state changes;
- reduced-motion compatibility.

### UX Acceptance Review

Before UX can be considered ready for the Architecture Gate, reviewers should be able to trace one complete successful journey and the relevant failure/recovery branches for both Student and Teacher:

- [ ] Student can enter an authorized context and identify the next useful action.
- [ ] Student can complete and submit work with explicit submission state.
- [ ] Duplicate, timeout, and network uncertainty do not create misleading duplicate state.
- [ ] Student can understand result/evidence without internal domain terminology.
- [ ] Progress is evidence-backed without freezing the mastery algorithm.
- [ ] Teacher can move from context to evidence without reconstructing missing context.
- [ ] Teacher can distinguish evidence from interpretation/recommendation.
- [ ] Teacher decision is attributable and produces an explicit next action.
- [ ] Follow-up ownership/due state is understandable.
- [ ] New evidence visibly affects the relevant state when appropriate.
- [ ] Outcome is not implied by completion alone.
- [ ] Parent projection respects visibility boundaries.
- [ ] Critical failure states have safe recovery paths.
- [ ] Responsive/mobile and RTL/LTR behavior is accounted for.
- [ ] Accessibility baseline is preserved.
- [ ] No unresolved UX ambiguity is silently converted into product behavior.

### Open UX Questions

1. What is the minimum set of learning modes exposed in the first slice?
2. Which progress language is understandable without implying an unapproved mastery algorithm?
3. Which teacher attention signals are deterministic versus recommendation-based?
4. What exact evidence provenance is sufficient for trust without overwhelming the user?
5. Which parent status fields are permitted by the eventual consent/visibility policy?
6. Which follow-up states need user-facing due dates versus softer reminders?
7. Which first-slice workflows must remain fully usable on mobile before desktop expansion?
8. Which accessibility target/conformance level will be adopted as a formal product requirement?

These remain OPEN until resolved through the relevant product/domain/security/UX decisions.

## UX Gate Position After Review

**UX Gate: READY FOR UX CONFIRMATION — NOT PROVEN**

The UX model is now sufficiently detailed for structured UX confirmation and subsequent Architecture Gate preparation. It still does not authorize implementation.

Next:
**Domain Confirmation → UX Confirmation → Architecture Gate → Security Gate → Data Gate → API Contract Gate → Implementation Gate**


## UX Confirmation Review Candidate — 2026-09-28

This review checks whether the first-slice UX model is sufficiently bounded for explicit confirmation without silently resolving open product/security decisions.

### Confirmation disposition

| UX area | Review result | Gate impact |
|---|---|---|
| Student flow | Complete enough to review | Confirmation required |
| Teacher flow | Complete enough to review | Confirmation required |
| Parent projection | Boundary is explicit | Security/privacy confirmation required |
| Information hierarchy | Clear around current state → evidence → next action | Confirmation required |
| Failure/recovery states | Core states covered | Confirmation required |
| Evidence/provenance presentation | Trust-oriented direction defined | Exact density remains open |
| Progress language | Bounded without mastery-score commitment | Product/domain confirmation required |
| Mobile/responsive/RTL/LTR | Baseline captured | Formal accessibility target remains open |
| Accessibility | Baseline captured | Requirement target remains open |

### UX confirmation checklist

- [ ] Student can identify the current useful action without reading internal workflow state.
- [ ] Teacher can distinguish evidence, interpretation, decision, and next action.
- [ ] Parent sees only policy-authorized projection.
- [ ] Completion is not presented as learning achievement.
- [ ] Unknown/insufficient/conflicting evidence has explicit UX treatment.
- [ ] Retry/reconciliation preserves authoritative state and does not create duplicate actions.
- [ ] No open UX question is silently converted into implementation behavior.

### UX review conclusion

**UX Gate: READY FOR EXPLICIT CONFIRMATION — NOT PROVEN**

Open questions remain bounded and can be carried into Product, Domain, Security, or Accessibility decisions without authorizing implementation.
