# Platform Capability Map

Date: 2026-09-28
Status: Product Synthesis Artifact — PROPOSED
Gate: Product Foundation — NOT PROVEN
Implementation authorization: None

## Purpose

Consolidate the current market baseline, competitor capability review, product layers, target journeys, and requirements planning into one platform-level capability map.

This artifact reflects the clarified product direction:

**Market → Competitors → Gaps → Product Model → Unified Experience → Requirements → Domain → UX → Architecture → Build**

The platform is not being defined as a collection of disconnected role-specific apps. Students, parents, teachers, teaching assistants, centers/schools, and platform operators participate in one connected educational system.

## Product Thesis

The product should make the learning journey understandable and actionable while hiding operational complexity.

Core loop:

**Context → Learn/Practice → Assess → Evidence → Progress → Next Useful Action → Feedback/Follow-up**

Cross-role operating loop:

**Evidence → Understanding → Decision → Action → Communication → Follow-up → Outcome**

The platform should preserve the same learning story as a student moves between recorded, live, offline, practice, assessment, teacher support, and organizational workflows.

## Capability Classification

- **PARITY** — expected market capability; omission can create a material baseline gap.
- **QUALITY-MUST** — baseline capability where reliability, permissions, recovery, provenance, or UX quality is part of the requirement.
- **DIFFERENTIATION** — candidate area where the platform may improve fragmented or weak workflows; still requires product validation before commitment.
- **LATER** — valuable capability that should not distort the first coherent product journey.
- **OPEN** — depends on an unresolved product, business, semantic, security, or architecture decision.

Classification is planning guidance, not implementation authorization.

## 1. Core Platform

| Capability | Class | Primary users | Foundation | Key dependency |
|---|---|---|---|---|
| Identity & authentication | QUALITY-MUST | All | Yes | Security |
| Multi-role identity | QUALITY-MUST | All | Yes | Domain/permissions |
| Relationships & visibility | QUALITY-MUST | All | Yes | Privacy/permissions |
| Organization / tenant context | OPEN | Centers, schools, teachers, platform | Architecture influence | Tenancy model |
| Learner context | QUALITY-MUST | Student, parent, teacher, org | Yes | Evidence/domain |
| Permissions / consent | QUALITY-MUST | All | Yes | Security/policy |
| Localization, Arabic/RTL, LTR | QUALITY-MUST | All | Yes | UX/architecture |
| Accessibility | QUALITY-MUST | All | Yes | UX |
| Auditability | QUALITY-MUST | All operational roles | Yes | Security/domain |
| Reliability / recovery | QUALITY-MUST | All | Yes | Architecture |
| Search | OPEN | All | Conditional | Scale/content model |
| Support / recovery context | QUALITY-MUST | All | Yes | Operations |

## 2. Student Experience

| Capability | Class | Purpose |
|---|---|---|
| Personal learning context | QUALITY-MUST | Start in the correct curriculum/course/class/goal context |
| Learning content | PARITY | Consume structured learning material |
| Recorded learning | PARITY | Self-paced learning |
| Live learning | OPEN | Synchronous learning where required |
| Offline / in-person learning | OPEN | Preserve learning continuity outside the digital session |
| Assignments | PARITY | Directed work |
| Practice | PARITY | Skill development and reinforcement |
| Assessments / quizzes / exams | PARITY | Produce learning evidence |
| Feedback | QUALITY-MUST | Explain what happened and what to improve |
| Evidence-backed progress | DIFFERENTIATION | Replace activity-only progress with meaningful learning state |
| Clear next useful action | DIFFERENTIATION | Reduce uncertainty about what to do next |
| Human help request | QUALITY-MUST | Preserve a route to teacher/support assistance |
| Remediation / re-practice | OPEN | Requires product semantics for learning recovery |
| Personalized learning | OPEN | Depends on evidence/mastery model |
| AI tutor / homework help | OPEN | Requires AI/product/safety gate |
| AI practice / study generation | OPEN | Requires quality/provenance gate |
| Conversational / roleplay AI | LATER | Candidate advanced AI experience |
| Voice / multimodal learning | LATER | Candidate accessibility/AI experience |
| Gamification | LATER | Only where educational purpose is established |

## 3. Parent Experience

| Capability | Class | Purpose |
|---|---|---|
| Child relationship/context | QUALITY-MUST | Correct visibility boundary |
| Current learning status | PARITY | Understand what is happening |
| Progress / assessment visibility | PARITY | Understand evidence of learning |
| Attendance visibility | OPEN | Depends on managed-learning model |
| Assignment / deadline visibility | PARITY | Support the child |
| Important-change notifications | QUALITY-MUST | Surface meaningful changes without noise |
| Teacher communication | PARITY | Coordinate support |
| Organization communication | OPEN | Depends on operating model |
| Payments / subscriptions | OPEN | Depends on business model |
| Action-required workflow | DIFFERENTIATION | Tell parent what matters and whether action is needed |
| Follow-up / outcome visibility | DIFFERENTIATION | Avoid “I sent a message, now what?” ambiguity |
| Surveillance-heavy activity feed | LATER / AVOID | Do not optimize for raw monitoring instead of confidence |

## 4. Teacher / Teaching Assistant Experience

| Capability | Class | Purpose |
|---|---|---|
| Student roster/context | PARITY | Manage teaching scope |
| Classes / groups | PARITY | Organize instruction |
| Content management | PARITY | Prepare learning material |
| Assignments | PARITY | Direct student work |
| Assessments / question banks | PARITY | Evaluate learning |
| Grading / feedback / rubrics | PARITY | Return useful feedback |
| Progress / gradebook | PARITY | Understand learner state |
| Attention / insight view | QUALITY-MUST | Find learners requiring attention |
| Evidence-to-action workflow | DIFFERENTIATION | Move from signal to evidence to decision/action |
| Communication | PARITY | Coordinate with students/parents/orgs |
| Follow-up work | DIFFERENTIATION | Make unresolved work visible and recoverable |
| AI planning / feedback assistance | OPEN | Assist without replacing accountability |
| AI-generated assessments | OPEN | Requires quality and provenance controls |
| AI student recommendations | OPEN | Requires evidence and human-accountability semantics |
| Bulk operations / automation | OPEN | Depends on teacher workflow |
| Marketplace/content monetization | LATER | Business/operating-model dependent |

## 5. Center / School / Organization

| Capability | Class | Purpose |
|---|---|---|
| Organization structure | OPEN | Tenant/branch/class structure |
| Staff roles & permissions | QUALITY-MUST | Safe delegation |
| Student enrollment | PARITY | Operational lifecycle |
| Teacher assignment | PARITY | Operational ownership |
| Classes / groups / branches | PARITY | Delivery organization |
| Scheduling / calendar | OPEN | Required for managed learning |
| Attendance | OPEN | Required where physical/live attendance matters |
| Parent management | OPEN | Depends on organization model |
| Payments / fees | OPEN | Depends on commercial workflow |
| Reports / operational dashboards | PARITY | Operational visibility |
| Unresolved work / ownership | DIFFERENTIATION | Prevent operational tasks from disappearing |
| Communication hub | PARITY | Coordinate roles |
| Audit / policy controls | QUALITY-MUST | Trust and accountability |
| Content/curriculum administration | OPEN | Depends on organization depth |
| Enterprise integrations | LATER | Expansion capability |
| Advanced BI | LATER | Scale capability |
| White-label | LATER | Business model capability |

## 6. Communication & Coordination

| Capability | Class | Purpose |
|---|---|---|
| In-app communication | PARITY | Role-to-role coordination |
| Notifications | QUALITY-MUST | Timely delivery |
| Context-preserving handoff | DIFFERENTIATION | Preserve the learning story across roles |
| Delivery/read state | QUALITY-MUST | Operational reliability |
| Follow-up state | DIFFERENTIATION | Make commitments and due work visible |
| Escalation | OPEN | Depends on workflow semantics |
| Communication history | QUALITY-MUST | Preserve context and auditability |

Communication is not automatically the authoritative source of business or learning state.

## 7. Learning Evidence, Progress & Analytics

| Capability | Class | Purpose |
|---|---|---|
| Evidence capture | QUALITY-MUST | Establish trustworthy learning facts |
| Evidence provenance | QUALITY-MUST | Know source/context/time |
| Evidence quality / uncertainty | QUALITY-MUST | Avoid false certainty |
| Progress state | DIFFERENTIATION | Represent learning rather than activity |
| Mastery / skills | OPEN | Advanced semantic model |
| Curriculum alignment | OPEN | Segment/business dependent |
| Actionable teacher insights | DIFFERENTIATION | Connect evidence to useful action |
| Parent summaries | DIFFERENTIATION | Translate evidence into understandable status |
| Student progress explanation | DIFFERENTIATION | Explain why state changed |
| Advanced BI | LATER | Scale/organization analytics |
| Experimentation / product analytics | OPEN | Product operating capability |

## 8. AI Platform

AI is a capability layer, not the source of truth.

| Capability | Class | Required control |
|---|---|---|
| AI content generation | OPEN | Provenance + review |
| AI practice generation | OPEN | Quality evaluation |
| AI tutoring | OPEN | Grounding + safe fallback |
| AI homework help | OPEN | Context + academic integrity policy |
| AI teacher planning | OPEN | Human review |
| AI feedback assistance | OPEN | Human accountability |
| AI personalized recommendations | OPEN | Evidence-backed reasoning |
| Grounded/RAG assistance | LATER / OPEN | Permission-aware retrieval |
| Conversational/roleplay | LATER | Safety + evaluation |
| Voice/multimodal | LATER | Privacy + fallback |
| Controlled AI actions | LATER / NOT PROVEN | Explicit authorization + audit + rollback |

High-impact educational decisions must remain attributable to an authorized human or explicitly governed workflow.

## 9. Commerce

| Capability | Class | Purpose |
|---|---|---|
| Subscription / plan model | OPEN | Business model |
| Course/product purchase | OPEN | Commerce model |
| Fees / invoices | OPEN | Organization operations |
| Payments | OPEN | Transaction workflow |
| Refunds / disputes | OPEN | Commerce lifecycle |
| Payment reconciliation | QUALITY-MUST if enabled | Financial correctness |
| Marketplace commission | LATER | Ecosystem model |

Commerce should not be implemented until the business transaction model is explicitly defined.

## 10. Scale & Ecosystem

| Capability | Class | Architecture influence |
|---|---|---|
| Multi-tenancy | OPEN | High |
| White-label | LATER | High |
| Marketplace | LATER | Medium/high |
| Enterprise integrations | LATER | Medium |
| Content ecosystem | LATER | Medium |
| Community/social | LATER | Medium/high safety burden |
| Certifications/credentials | LATER | Depends on real credential workflow |
| Advanced BI | LATER | Medium |
| Global payments | LATER | High regulatory/operational complexity |

## Cross-Role Product Gaps We Should Design Around

The current market baseline shows that many individual capabilities already exist across products. The product opportunity is therefore not to invent a long list of isolated features.

The stronger product-level gaps to examine are:

1. **Fragmentation** — learning, assessment, communication, scheduling, payments, and parent visibility can live in separate workflows or systems.
2. **Context loss** — a message, alert, grade, or recommendation may not preserve the context that explains why it matters.
3. **Data-to-action gap** — dashboards can show information without making ownership and next action clear.
4. **Follow-up gap** — a task or concern can be created without a visible lifecycle to outcome/closure.
5. **Role handoff friction** — student, teacher, parent, and organization workflows may each work independently while the handoff between them remains manual.
6. **Complexity leakage** — users are forced to understand platform structure instead of being shown the one useful next action.
7. **AI without product context** — AI can generate content or answers without being grounded in authoritative learner context and evidence.
8. **Reliability gaps** — submissions, communication, scheduling, payments, and external AI/media operations require recoverable state rather than optimistic UI alone.

These are **product hypotheses**, not claims that every competitor fails in the same way.

## Unified Experience Model

### Student

**Context → What should I do now? → Learn/Practice → Evidence → Feedback → Progress → Next Action → Help/Follow-up**

### Parent

**Child Context → What changed? → Why it matters → What is being done → What do I need to do? → Follow-up**

### Teacher

**Teaching Context → Who/what needs attention? → Evidence → Interpretation → Decide → Act → Communicate → Follow-up**

### Center / School

**Operational Context → What needs management? → Ownership → Action → Communication → Follow-up → Outcome → Audit**

### Platform

**Context → Evidence → State → Useful Action → Outcome → Learning from the workflow**

## Dependency Rules

1. Evidence semantics precede advanced progress semantics.
2. Permissions and relationships precede cross-role visibility.
3. Learning context precedes AI personalization.
4. Reliable workflow state precedes automation.
5. Business transaction semantics precede payments.
6. Tenancy semantics precede organization-scale implementation.
7. Product workflow clarity precedes UX screen design.
8. Domain boundaries precede API and database contracts.
9. AI action authorization precedes autonomous actions.
10. Scale features must not force premature distributed architecture.

## What Is Not Decided

- Initial commercial beachhead.
- Exact MVP scope.
- Final role set and organization model.
- Final tenancy model.
- Mandatory learning modes.
- Payment model.
- Parent semantics and age/consent policy.
- Mastery/progress semantic model.
- Intervention lifecycle as a formal domain.
- AI use cases for first release.
- Final architecture and technology stack.
- Provider choices for video, payments, AI, messaging, or search.

## Product Gate Position

**Current status: NOT PROVEN**

This map is sufficient to move from scattered feature lists toward a unified product model.

It does **not** authorize implementation.

Next product work:

**Capability Map → Core Product Journey → MVP Boundary → Product Requirements → Domain Gate**

The project does not need another broad field-research cycle to continue conventional product planning. Targeted validation may still be used where a proposed differentiator or high-impact semantic cannot be responsibly inferred from market evidence.
