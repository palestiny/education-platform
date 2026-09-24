# Competitor Workflow Walkthroughs

Status: Research Draft

Gate: Product Foundation / Competitive Intelligence — NOT PROVEN

Implementation is not authorized by this document.

## Workflow Validation Matrix

| Competitor / Workflow | State / Trigger | Evidence | Decision / Action | Follow-up / New Evidence | Cross-role coordination status |
|---|---|---|---|---|---|
| Khan Academy | Learner/course/class skill state | Mastery, assignment/progress and assessment data | Targeted practice, reteach/support, personalized next step | Practice/progress and reassessment | Documented across learner/teacher; broader parent/org intervention closure still requires validation |
| Khan Academy Interim Assessments | Assessment state | Responses, reasoning/misconception signals, unfinished learning | Personalized practice aligned to needs | Practice progress and later assessment | Strong assessment→action loop; cross-role intervention ownership/closure not established by this evidence |
| Classera | Learner/institution state | Interactions, assessments, AI/personalization and analytics described by provider | Adaptive exams, personalized learning, AI-supported actions | Ongoing analytics/interactions | Institutional breadth documented; exact intervention closure workflow needs validation |
| Abwaab Ask the Teacher | Learner submits question | Question, optional screenshot, curriculum context | Specialized teacher answers/supports | Response and continued learning | Human intervention documented; no evidence yet of persistent cross-role intervention lifecycle |
| Nagwa | Curriculum/content state | Curriculum-oriented public material | OPEN | OPEN | Insufficient current evidence |
| Egypt/MENA operating-layer products | Operational/student state | Attendance, classes, payments, parent communication, progress | Communication, scheduling, reporting, operational actions | Subsequent operational/progress records | Multi-role operational coordination documented; learning intervention closure needs validation |

## Scenario validation

### Scenario A — Repeated concept failure
**Question:** Does the platform preserve the evidence, identify the likely need, assign an intervention owner, verify remediation, and capture outcome evidence?

Current public evidence:
- Khan Academy documents assessment → understanding → personalized next step → progress.
- This proves the learning loop exists in-market, but does not prove a cross-role intervention case with explicit owner/due/closure across student, parent, teacher and organization.

**Status:** HYPOTHESIS OPEN.

### Scenario B — Attendance drop + performance drop
**Question:** Can attendance and learning evidence be combined without producing an unsupported causal claim?

**Required validation:** evidence provenance, time alignment, confidence, human interpretation, privacy boundaries and intervention ownership.

**Status:** NOT PROVEN.

### Scenario C — Parent concern requiring action
**Question:** Can a parent receive an understandable concern and an appropriate next action without surveillance overload or exposing inappropriate teacher/student data?

**Status:** NOT PROVEN.

### Scenario D — Teacher remediation + verification
**Question:** Can a teacher assign remediation, define completion/outcome criteria, and verify whether the intervention changed evidence?

Khan Academy documents targeted support/reteaching and reassessment-oriented workflows, but the specific persistent intervention lifecycle remains unproven.

**Status:** HYPOTHESIS OPEN.

### Scenario E — Organization unresolved intervention
**Question:** Can an organization see unresolved interventions, ownership, due state, escalation and closure across multiple teachers/classes?

**Status:** NOT PROVEN.

### Scenario F — AI recommendation with incomplete/contradictory evidence
**Question:** Can AI distinguish recommendation from fact, expose evidence/provenance/confidence, request human review when needed, and avoid unsupported intervention?

**Status:** NOT PROVEN.

## Validation gaps

1. Parent visibility and consent boundaries.
2. Teacher evidence assembly burden.
3. Organization exception/intervention closure.
4. Explicit owner, due state, outcome and closure criteria.
5. AI recommendation vs human approval.
6. Stale, missing, contradictory or low-confidence evidence.
7. Connectivity/playback/scheduling/communication recovery.
8. Measurable learning/operational value versus added complexity.
9. Initial segment with sufficiently frequent/high-cost problem.
10. Real user evidence from students, parents, teachers and organizations.

## Next research step

Run **Intervention Workflow Validation** using public documentation/reviews where available and direct user evidence where access is possible. Record:

- trigger;
- role;
- evidence source;
- provenance/confidence;
- interpretation;
- decision owner;
- intervention;
- notification/visibility;
- failure/recovery;
- due state;
- closure criteria;
- outcome evidence;
- privacy/consent implications.

Do not convert scenarios into requirements until the Product Foundation Gate is explicitly reviewed.
