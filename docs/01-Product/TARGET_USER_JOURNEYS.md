# Target User Journeys

Date: 2026-09-25
Status: Planning Artifact — PROPOSED
Gate: Product Foundation / Competitive Intelligence — NOT PROVEN
Implementation authorization: None

## Purpose

Turn the target product scenario into role-based journeys without prematurely freezing screens, APIs, or domain models.

The journeys describe outcomes and handoffs. UX and technical contracts come later.

## Shared Journey Pattern

**Context → Goal → Next Action → Evidence → Understanding → Action → Feedback → Follow-up → Outcome**

The platform should hide unnecessary complexity while preserving evidence and accountability underneath.

---

## 1. Student Journey

### Goal

Know what to do next, complete meaningful learning work, understand progress, and recover when something goes wrong.

### Journey

1. Enter the correct learning context.
2. See one clear next useful action.
3. Learn through the appropriate mode.
4. Practice or complete assigned work.
5. Receive understandable feedback.
6. See evidence-backed progress.
7. Request human help when needed.
8. Follow targeted remediation when appropriate.
9. Reassess or demonstrate new evidence.
10. Continue or change the plan.

### Failure paths

- Network interruption
- Session expiration
- Failed submission
- Video/live failure
- Confusing feedback
- Unable to get teacher help
- Duplicate submission

### Product quality target

The student should not need to understand internal state machines, analytics, providers, or recovery mechanics.

---

## 2. Parent Journey

### Goal

Understand how the child is doing, know whether attention is required, and support the child without unnecessary surveillance.

### Journey

1. See children and permitted learning contexts.
2. Review concise current status.
3. Understand important changes through evidence-backed summaries.
4. See whether a teacher/organization is already acting.
5. Take a requested action only when necessary.
6. Communicate with the responsible party when permitted.
7. Receive follow-up/outcome information for important cases.

### Alert pattern

**What happened → Why it matters → What is being done → What I need to do**

### Failure paths

- Notification missed
- Communication channel unavailable
- Conflicting information
- Consent/visibility restriction
- Stale information

### Product quality target

Parent visibility should increase confidence and clarity rather than become a stream of raw activity metrics.

---

## 3. Teacher Journey

### Goal

Know who needs attention, understand why, make a professional decision, act efficiently, and verify the result.

### Journey

1. Open teaching context.
2. See prioritized attention items.
3. Inspect underlying evidence.
4. Distinguish evidence from interpretation/recommendation.
5. Decide an action or intentionally take no action.
6. Assign/perform the intervention.
7. Communicate context to the appropriate roles.
8. Set a follow-up point where needed.
9. Review new evidence.
10. Close, adjust, or escalate the case.

### Failure paths

- Weak/conflicting evidence
- Recommendation unavailable
- Student context incomplete
- Parent visibility restricted
- External communication failure
- Overdue follow-up
- AI failure

### Product quality target

Teacher workflow should reduce reconstruction and administrative overhead, not turn teachers into dashboard analysts.

---

## 4. Organization Journey

### Goal

Operate learning services reliably, see unresolved cases, manage ownership, protect data, and understand outcomes.

### Journey

1. Establish organization/branch/class context.
2. Manage people, roles, permissions and relationships.
3. Configure learning operations.
4. Monitor attendance, schedules, assessments and workload where relevant.
5. See unresolved/overdue cases.
6. Confirm ownership and escalation.
7. Review communication and operational state.
8. Inspect outcome evidence.
9. Audit important actions.
10. Improve operating policy.

### Failure paths

- Wrong ownership
- Missing evidence
- Permission conflict
- Duplicate records
- Payment/attendance mismatch
- Communication failure
- Branch or tenant isolation issue

### Product quality target

Operations should support learning rather than bury learning under administrative screens.

---

## 5. Cross-Role Intervention Journey

This is a candidate differentiator and remains **NOT PROVEN**.

**Signal → Evidence → Interpretation → Human Decision → Action Owner → Intervention → Communication → Follow-up → New Evidence → Close/Escalate**

The journey is only worth making a first-class product workflow if direct research shows recurring material friction.

---

## 6. Journey Design Rules

1. Every important recommendation must have understandable evidence.
2. Every high-impact human decision has an accountable owner.
3. Every asynchronous intervention has a visible due/follow-up state where relevant.
4. Important failures have a recovery path.
5. Parent visibility respects consent, age/context and organizational policy.
6. AI can assist but must not silently own high-impact decisions.
7. A dashboard is a view; it is not the lifecycle itself.
8. Unknown remains unknown.
9. The same learning story should survive movement across learning modes.
10. No role should be forced to understand platform internals.

## Validation Needed

Before converting these journeys into committed requirements:

- real recent cases,
- initial segment selection,
- role-specific frequency/cost,
- current workaround reconstruction,
- privacy/consent boundaries,
- outcome evidence,
- failure/recovery evidence.

No code is authorized by this document.
