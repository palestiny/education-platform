# Competitor Workflow Walkthroughs

Status: Research Draft

Gate: Product Foundation / Competitive Intelligence — NOT PROVEN

Implementation is not authorized by this document.

## Workflow Validation Matrix

| Competitor | State | Evidence | Interpretation / Insight | Decision / Action | Follow-up / New Evidence |
|---|---|---|---|---|---|
| Khan Academy | Learner/course/class skill state | Mastery levels, assignment/progress data, assessment results | Teacher can identify where learners are and what they need next | Targeted practice, reteach/support, personalized next step | Subsequent practice/progress and reassessment |
| Khan Academy Interim Assessments | Assessment state | Responses plus reasoning/misconception signals and identified unfinished learning | Assessment is used to understand needs, not only score | Personalized practice aligned to attention areas | Practice progress feeds the next learning cycle |
| Classera | Learner/institution state | Interactions, assessments, AI/personalization data and analytics described by provider | Platform intends to derive real-time insights and adapt learning | Adaptive exams, personalized learning and AI-supported actions | Ongoing learning interactions and analytics |
| Abwaab | Question/learning need | Student question, optional screenshot, curriculum context | A learner need is interpreted by a specialized teacher | Human teacher answer/support | New response and continued learning |
| Nagwa | Curriculum/content state | Public material establishes curriculum-oriented content | Full evidence-to-intervention workflow is not sufficiently documented in current research | OPEN | OPEN |
| Egypt/MENA operating-layer products | Operational/student state | Attendance, classes, payments, parent communication, progress and related records | Products coordinate operational information across roles | Communication, scheduling, reporting and operational actions | Subsequent operational/progress records |

## Important research correction

Khan Academy publicly documents assessment-to-personalized-practice and teacher next-step workflows. Classera publicly documents AI personalization, adaptive assessment and real-time insights.

Therefore the earlier broad learning-orchestration hypothesis is too broad to serve as differentiation by itself.

## Narrower hypothesis

The remaining hypothesis worth testing is:

> Evidence → interpreted state → human decision/approval → assigned intervention → follow-up → new evidence, coordinated across roles with role-appropriate visibility and auditability.

This is a PRODUCT HYPOTHESIS, not a requirement.

## Validation gaps

1. Do parents need cross-role intervention visibility, and what is appropriate to expose?
2. Can teachers coordinate interventions without manually assembling evidence from separate tools?
3. Can organizations track unresolved learning/operational exceptions to closure?
4. Does an intervention have an explicit owner, due state, outcome and follow-up evidence?
5. Can AI recommend while preserving human approval and explainability?
6. How are stale, missing, contradictory or low-confidence evidence handled?
7. How does the workflow recover after connectivity, playback, scheduling or communication failure?
8. Does added coordination reduce work or improve outcomes enough to justify its complexity?
9. Which initial user segment experiences the problem frequently enough to support a focused product?

## Next research step

Move from competitor capability discovery to **Intervention Workflow Validation**.

Candidate scenarios:
- Student repeatedly fails a concept.
- Student attendance drops while assessment performance also drops.
- Parent receives a concern requiring action.
- Teacher assigns remediation and needs to verify completion.
- Center detects an unresolved intervention across multiple teachers.
- AI produces a recommendation with incomplete or contradictory evidence.

For each scenario collect:
- current competitor behavior;
- user role;
- evidence source;
- confidence/provenance;
- decision owner;
- intervention action;
- notification/visibility;
- recovery path;
- closure criteria;
- measurable outcome.

Do not convert scenarios into requirements until the Product Foundation Gate is explicitly reviewed.
