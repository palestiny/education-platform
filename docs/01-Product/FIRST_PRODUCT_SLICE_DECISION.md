# First Product Slice Decision

Date: 2026-09-28
Status: Decision Artifact — PROPOSED
Gate: Product Foundation — NOT PROVEN
Implementation authorization: None

## Purpose

Select the first **coherent product journey slice** from the platform model.

This is not a commitment to a permanent market segment, full MVP scope, architecture, or technology stack. It defines the smallest product slice that can prove the platform's central promise end-to-end.

## Decision Principle

The first slice must complete a meaningful learning loop:

**Context → Goal → Plan → Learn/Practice → Assess → Evidence → Understand → Progress → Next Useful Action → Feedback/Follow-up → New Evidence → Outcome**

A feature that cannot contribute to completing this loop should not enter the first slice merely because it is common in the market.

## Candidate Slices

### Slice A — Student Self-Learning Loop

**Actors:** Student, Platform

**Journey:**
Context → Goal → Plan → Learn/Practice → Assess → Evidence → Progress → Next Action → Reassess

**Strengths**
- Smallest role dependency surface.
- Directly exercises the core learning semantics.
- Can establish evidence/progress foundations early.

**Trade-offs**
- Weakens the platform's cross-role thesis.
- Teacher/parent coordination remains unproven.
- Commercial operating model may require additional capabilities.

### Slice B — Teacher-Led Learning Loop

**Actors:** Student, Teacher, optionally Parent

**Journey:**
Learning context → teacher/learner goal → assignment/lesson → practice/assessment → evidence → teacher interpretation → teacher decision → student next action → follow-up → new evidence.

**Strengths**
- Demonstrates the central evidence-to-action model.
- Exercises shared semantics across two primary roles.
- Creates a natural path to parent visibility without requiring the parent to own the workflow.

**Trade-offs**
- More role/permission complexity than Slice A.
- Requires meaningful teacher workflow before the platform can prove value.
- Parent experience may remain a projection rather than a first-class workflow.

### Slice C — Managed Center/School Loop

**Actors:** Student, Teacher, Organization, optionally Parent

**Journey:**
Enrollment/context → scheduling/class → lesson/assignment → attendance/assessment → evidence → teacher action → organization ownership → communication/follow-up → outcome.

**Strengths**
- Exercises the platform as an operating system rather than only a learning tool.
- Naturally introduces organizational ownership and operational context.
- Creates a clearer route to B2B/SaaS workflows.

**Trade-offs**
- Large dependency surface: organization, roles, scheduling, attendance, communication, and possibly payments.
- High risk of building administrative breadth before the learning loop is proven.
- Tenancy and organizational semantics become early architecture concerns.

### Slice D — Full Cross-Role Learning Loop

**Actors:** Student, Teacher, Parent, Organization

**Journey:**
Context → Goal → Plan → Learn/Practice → Assess → Evidence → Understand → Progress → Next Action → Teacher Decision → Parent Projection → Follow-up → New Evidence → Outcome.

**Strengths**
- Most directly demonstrates the intended unified-platform thesis.
- Tests role handoffs and context preservation.
- Makes fragmentation/context-loss/follow-up hypotheses concrete.

**Trade-offs**
- Highest first-release complexity.
- Requires privacy/consent and relationship semantics early.
- Easy to accidentally become a broad LMS + SIS + parent portal + communication system.

## Proposed Direction

**PROPOSED: Slice B — Teacher-Led Learning Loop, with Parent as a controlled projection rather than a mandatory workflow owner.**

### Why this direction

1. It is the smallest candidate that exercises the platform's distinctive product model beyond a single-user learning tool.
2. It connects evidence to a human decision and then back to a learner action.
3. It gives the teacher a real accountable role while keeping the student at the center.
4. Parent visibility can be included only where the product context permits it, without making the first slice dependent on a complete parent product.
5. It leaves organization, payments, attendance, advanced AI, marketplace, and scale capabilities outside the first slice unless later decisions make one of them necessary.

This is a **product recommendation, not a silent decision**. Until the Product Foundation Gate is passed, it remains PROPOSED.

## First Slice — Canonical Scenario

A learner is in a defined learning context with a goal.

1. The teacher establishes or assigns the intended learning work.
2. The student receives a clear next action.
3. The student learns/practices and submits meaningful work.
4. The platform records evidence with provenance and context.
5. The system presents understandable progress/interpretation without confusing inference with fact.
6. The teacher reviews the evidence and makes an accountable decision.
7. The platform turns that decision into an appropriate student next action.
8. A follow-up can be created when work remains unresolved.
9. The student produces new evidence.
10. The platform records the resulting outcome, including uncertainty when the evidence is insufficient.
11. A parent may receive a permitted status projection when a parent relationship exists and visibility policy allows it.

## First-Slice Role Boundary

### Student — mandatory

Owns/initiates:
- learning actions;
- submissions;
- practice attempts;
- permitted self-report/preferences.

### Teacher — mandatory

Owns/initiates:
- assignments/plans within authority;
- assessment/feedback;
- interpretation where teacher-authored;
- accountable learning-support decisions;
- follow-up ownership where authorized.

### Parent — optional projection

May receive:
- permitted current status;
- meaningful changes;
- teacher/action status;
- follow-up/outcome information where policy allows.

Parent does not become authoritative over learner state merely by viewing it.

### Organization — outside first slice by default

Organization context may exist as a contextual boundary if required, but organization operations such as branches, scheduling, attendance, fees, and enterprise administration are not first-slice commitments.

## First-Slice Minimum Capability Set

1. Identity and multi-role authorization.
2. Student-teacher relationship/context.
3. Learning context.
4. Goal/assignment.
5. Learning activity or content.
6. Practice/submission.
7. Assessment/evidence capture.
8. Evidence provenance.
9. Progress representation with explicitly bounded semantics.
10. Teacher evidence review and decision.
11. Student next useful action.
12. Context-preserving feedback/communication where needed.
13. Lightweight follow-up lifecycle.
14. New evidence/reassessment.
15. Outcome representation.
16. Required privacy/visibility controls.
17. Critical reliability/recovery.
18. Audit for important accountable actions.
19. Localization/accessibility foundation.
20. Minimal support/recovery path.

## Explicitly Outside First Slice

Unless a later decision proves a dependency:

- full organization administration;
- branches and enterprise hierarchy;
- attendance management;
- payment/fees;
- marketplace;
- white-label;
- community/social graph;
- advanced BI;
- broad gamification;
- autonomous AI agents;
- multi-provider AI orchestration;
- sophisticated adaptive learning;
- global commerce;
- complete offline synchronization;
- broad enterprise integrations.

AI may assist within the slice only after an explicit AI use-case and governance decision.

## Semantic Decisions Required Before Domain Confirmation

The following are now the minimum high-impact decisions for this slice:

| Decision | Status |
|---|---|
| Student + Teacher are mandatory actors | PROPOSED |
| Parent is optional projection, not workflow owner | PROPOSED |
| Organization is contextual, not first-slice operational scope | PROPOSED |
| Learning context is required | PROPOSED |
| Goal/assignment semantics are required | OPEN |
| Evidence is first-class and provenance-bearing | PROPOSED |
| Progress is evidence-derived and traceable | PROPOSED |
| Exact mastery algorithm is not required for first slice | PROPOSED |
| Teacher decision is authoritative within granted scope | PROPOSED |
| Recommendation never silently mutates authoritative state | DEFINED |
| Follow-up is lightweight and distinct from intervention | DEFINED |
| Outcome records what the evidence demonstrates | PROPOSED |
| Parent visibility requires relationship + permission/policy | DEFINED |
| Payment is outside first slice | PROPOSED |
| Organization operations are outside first slice | PROPOSED |
| AI is not required for first-slice viability | PROPOSED |

## Product Success Evidence

The first slice should be considered product-complete only when it can demonstrate:

- a learner can complete the intended learning loop;
- a teacher can act from evidence without reconstructing context manually;
- the learner receives a clear next useful action;
- progress can be explained from underlying evidence;
- unresolved work remains visible through follow-up;
- new evidence can change the resulting state;
- failures do not silently corrupt the learning story;
- parent visibility, when enabled, does not expose unauthorized internal context.

Exact quantitative targets remain OPEN and should be defined after the first slice's business model and operating assumptions are committed.

## Gate Transition

After this proposal is reviewed, the next design work is:

**First Product Slice → Minimum Domain Decisions → Domain Confirmation → UX Gate → Architecture Gate → Security/Data/API Gates**

No implementation is authorized by this document.

## Decision Record

- **Current disposition:** PROPOSED.
- **Decision owner:** Product owner.
- **Reason:** Provides a complete two-sided learning loop while controlling first-release complexity.
- **Rejected as first default:** Full cross-role slice is too broad for initial implementation; center-managed slice introduces too much operational scope; student-only slice under-tests the platform's cross-role thesis.
- **Revisit trigger:** New market/business evidence, a mandatory commercial dependency, or discovery that teacher-led learning cannot support a viable first product journey.
