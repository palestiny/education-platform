# Product Operating Model

Date: 2026-09-28
Status: Product Semantics Artifact — PROPOSED
Gate: Product Foundation — NOT PROVEN
Implementation authorization: None

## Purpose

Define the minimum semantic model that makes the Core Product Journey implementable later without prematurely choosing database tables, APIs, bounded contexts, or technology.

## 1. Authoritative Product Objects

### Context
The active scope in which an action or evidence has meaning.

Contains, as applicable:
- learner;
- actor;
- role/relationship;
- subject/course;
- class/group;
- organization;
- goal;
- locale/time context;
- permissions.

Context is not merely a UI filter.

### Goal
An intended learning or operational outcome.

A goal may originate from:
- learner;
- teacher;
- organization;
- curriculum;
- system;
- governed AI assistance.

A goal is not proof of achievement.

### Learning Action
A deliberate action taken to advance the goal.

Examples:
- learn;
- practice;
- submit;
- attend;
- review;
- retry;
- ask for help;
- complete assignment.

### Evidence
A record of something observed, submitted, assessed, or otherwise captured from an authorized source.

Minimum semantic attributes:
- what was observed;
- source;
- actor/system;
- time;
- context;
- provenance;
- uncertainty/quality where applicable;
- visibility.

Evidence should be attributable and auditable.

### Interpretation
An explanation of what evidence may mean.

Interpretation may be:
- deterministic/rule-based;
- teacher-authored;
- system-generated;
- AI-assisted.

Interpretation is not automatically authoritative learner state.

### Progress State
A representation of change relative to a goal/context.

Progress may combine multiple evidence items and interpretations.

The exact calculation model is OPEN.

### Recommendation
A proposed next action supported by available context/evidence.

A recommendation:
- must identify its basis where practical;
- may be declined/changed by an authorized human;
- does not automatically change authoritative state.

### Decision
An authorized commitment to an action or state change.

A decision must have:
- actor or governed authority;
- applicable permissions;
- relevant context;
- resulting action/state where applicable.

### Follow-up
A lightweight lifecycle for unresolved or expected work after an action.

Follow-up is not synonymous with intervention.

### Outcome
The observed result of a goal/action sequence.

Outcome may be:
- achieved;
- partially achieved;
- not demonstrated;
- contradictory;
- unevaluable.

The model must preserve uncertainty rather than force binary success.

## 2. Semantic Separation Rules

| Distinction | Rule |
|---|---|
| Context vs Goal | Context says where/for whom; Goal says what outcome is intended |
| Goal vs Action | Goal is desired outcome; Action is what someone does |
| Action vs Evidence | Action is performed; Evidence records what happened/was observed |
| Evidence vs Interpretation | Evidence is observed fact; interpretation explains possible meaning |
| Interpretation vs Progress | Interpretation explains; progress represents state/change |
| Progress vs Activity | Progress is not equivalent to usage/activity volume |
| Recommendation vs Decision | Recommendation proposes; authorized actor decides |
| Communication vs State | Message communicates; domain state remains authoritative elsewhere |
| Follow-up vs Intervention | Follow-up tracks remaining work; intervention is a higher-order governed workflow |
| Outcome vs Activity | Outcome evaluates result, not merely completion |

## 3. Source-of-Truth Rules

1. Authoritative state must have an explicit owner.
2. Dashboards are projections, not state owners.
3. AI output is not authoritative solely because it is generated.
4. Messages do not override learning or commerce state.
5. Derived progress must retain traceability to supporting evidence.
6. Recommendations must not silently mutate authoritative state.
7. External provider state must be reconciled according to the relevant domain contract.
8. Audit records preserve accountability; they do not become the business source of truth.

## 4. Ownership Model

### Student
Owns or contributes:
- personal learning actions;
- submissions;
- self-reports;
- preferences within policy.

Does not automatically own:
- teacher grades;
- organizational attendance records;
- parent-visible policy state.

### Teacher
May own or contribute:
- teaching plans;
- assignments;
- feedback;
- assessments;
- authorized decisions about learner support.

### Parent
May:
- view permitted child context;
- receive relevant progress/status;
- communicate;
- perform authorized commercial/consent actions.

Parent visibility must be policy-controlled.

### Center / School
May own:
- enrollment;
- organizational structure;
- class assignment;
- scheduling;
- attendance;
- operational policies;
- commercial records where applicable.

### Platform
Owns:
- platform policies;
- platform-level authorization infrastructure;
- service configuration;
- system audit/operational state.

This is a semantic candidate, not final legal/data ownership.

## 5. Cross-Role State Propagation

A change should move through the platform as:

**Source Action → Evidence/Authoritative State → Interpretation (if needed) → Relevant Projection → Authorized Action → Follow-up → New Evidence → Outcome**

Example:

**Student submits assessment → result/evidence recorded → progress interpretation updated → teacher sees attention item → teacher decides remediation → student receives action → reassessment produces new evidence.**

The parent may receive an appropriate status projection without receiving internal teacher workflow details.

## 6. AI Boundary

AI may assist with:
- explanation;
- content generation;
- practice generation;
- summarization;
- planning;
- recommendation;
- classification.

AI must not silently become:
- authoritative learner state;
- authoritative grade;
- permission authority;
- payment authority;
- irreversible intervention authority.

Any future controlled AI action requires:
- explicit authorization;
- permission evaluation;
- idempotency where needed;
- auditability;
- failure/recovery semantics;
- human accountability for high-impact actions.

## 7. Reliability Semantics

The product model assumes that real workflows can:
- duplicate;
- timeout;
- partially succeed;
- fail after external execution;
- be retried;
- arrive out of order.

Therefore authoritative state transitions must define:
- idempotency;
- conflict handling;
- retry behavior;
- reconciliation;
- audit trail;
- user-visible status.

## 8. Privacy / Visibility Semantics

Visibility is determined by:
**Identity + Role + Relationship + Organization/Tenant + Consent/Policy + Context**

A user having a role does not automatically grant visibility to all related data.

Parent, teacher, assistant, center, school, and platform-admin views must be explicitly scoped.

## 9. Open Semantics

Still unresolved:
- exact goal representation;
- mastery/progress algorithm;
- evidence schema;
- relationship model;
- organization/tenant semantics;
- intervention threshold and lifecycle;
- consent/age policy;
- payment state ownership;
- AI evaluation model;
- first-release learning modes.

## Gate Position

**Product Foundation — NOT PROVEN**

This model is sufficiently explicit to guide the next requirements/domain review, but it does not authorize schema, API, architecture, or implementation.

Next:
**Product Operating Model → Requirements Consolidation → Domain Readiness**
