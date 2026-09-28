# Requirements Consolidation & Domain Readiness Delta

Date: 2026-09-28
Status: Consolidation Artifact — PROPOSED
Gate: Domain Gate — NOT PROVEN
Implementation authorization: None

## Purpose

Consolidate the candidate requirements against the Product Operating Model and make the resulting domain-readiness boundary explicit.

This artifact does not select a commercial beachhead, approve an MVP, freeze bounded contexts, or authorize implementation.

## 1. Consolidation Result

The requirements now align to the following semantic chain:

**Context → Goal → Learning Action → Evidence → Interpretation → Progress State → Recommendation → Decision → Follow-up → Outcome**

The important result is not that every item becomes a separate domain object. The result is that later domain design must not collapse meanings that require different authority, provenance, lifecycle, or visibility.

## 2. Requirement-to-Semantic Mapping

| Requirement | Primary semantic dependency | Domain implication | Status |
|---|---|---|---|
| PR-001 Identity | Context / Decision | Identity, role and authorization context | PROPOSED |
| PR-002 Context | Context | Context must be reconstructable for relevant actions/evidence | PROPOSED |
| PR-003 Learning modes | Goal / Learning Action / Evidence | Learning experience model must support multiple modes without losing context | PROPOSED |
| PR-004 Evidence | Evidence | Evidence needs provenance and source semantics | PROPOSED |
| PR-005 Evidence quality | Evidence / Interpretation | Quality and uncertainty must survive downstream interpretation | PROPOSED |
| PR-006 Progress | Evidence / Progress State | Progress must be traceable to evidence; calculation remains OPEN | PROPOSED |
| PR-007 Next useful action | Recommendation / Decision | Recommendation cannot silently become authoritative state | PROPOSED |
| PR-008 Teacher workflow | Evidence / Interpretation / Decision / Action | Teacher workflow must connect signal to authorized action | PROPOSED |
| PR-009 Parent workflow | Context / Relationship / Projection | Visibility depends on relationship, policy and consent | CONDITIONAL |
| PR-010 Organization workflow | Context / Follow-up / Decision | Operational ownership depends on organization model | CONDITIONAL |
| PR-011 Communication | Context / Projection | Communication carries context but does not own business state | PROPOSED |
| PR-012 Follow-up | Follow-up | Work ownership and due state need explicit lifecycle | PROPOSED |
| PR-013 Outcome | Outcome / Evidence | Outcome must be distinct from activity completion | PROPOSED |
| PR-014 Intervention | Recommendation / Decision / Follow-up / Outcome | Keep as governed workflow hypothesis, not a premature aggregate | NOT PROVEN |
| PR-015 Reliability | Decision / State transitions | Critical transitions need idempotency, recovery and reconciliation semantics | PROPOSED |
| PR-016 Offline | Learning Action / Evidence | Conditional capability; requires first-slice need | CONDITIONAL |
| PR-017 AI assistance | Interpretation / Recommendation / Action | AI is bounded assistance, not learner truth | PROPOSED |
| PR-018 AI governance | Decision / Authorization | High-impact AI actions require explicit authorization/accountability | PROPOSED |
| PR-019 Privacy | Context / Relationship / Policy | Visibility is an explicit policy boundary | PROPOSED |
| PR-020 Localization | Context | Locale is contextual configuration, not embedded learning logic | PROPOSED |
| PR-021 Tenant isolation | Context / Authorization | Exact tenancy model remains OPEN | OPEN |
| PR-022 Payments | Decision / Outcome / External state | Commerce state needs idempotency and reconciliation | CONDITIONAL |
| PR-023 Support | Context / Follow-up | Recovery must retain enough workflow context | PROPOSED |
| PR-024 Audit | Decision / State change | Audit preserves accountability without becoming business state | PROPOSED |
| PR-025 Search | Context / Authorization | Permission-aware retrieval remains scale-dependent | OPEN |

## 3. Domain Readiness Delta

### Stable enough to carry forward

1. Identity, role and authorization context are foundational concepts.
2. Learning Experience, Learning Content, Assignment/Practice and Assessment remain candidate foundation concepts.
3. Evidence is a first-class semantic concern, with provenance/quality, but not a generic event stream.
4. Interpretation must remain distinguishable from evidence.
5. Progress must remain distinguishable from raw activity and must be evidence-traceable.
6. Recommendation and Decision must remain distinct.
7. Communication and dashboards are projections/capabilities, not authoritative learning state.
8. Follow-up can be modeled as a generic remaining-work concept without prematurely creating an Intervention Case.
9. Outcome is a result concept and may be uncertain.
10. Reliability, privacy, audit and AI accountability are domain-level constraints even before exact implementation.

### Still OPEN

- exact Goal representation;
- exact Evidence schema and immutability/versioning rules;
- conflicting/corrected evidence handling;
- Progress/Mastery calculation;
- durable versus derived learner state;
- Relationship semantics and authorization;
- Organization/Tenant semantics;
- parent/guardian consent and age policy;
- first-release learning modes;
- scheduling/attendance/offline/payment semantics;
- Intervention lifecycle;
- AI evaluation and controlled action model;
- aggregate and transaction boundaries;
- external provider reconciliation.

## 4. What Changed from the Previous Domain Readiness Position

The previous checkpoint treated a validated initial segment and reconstructed participant cases as prerequisites for normal progression.

That boundary is now superseded by the market-led product direction:

- direct cases are optional targeted validation;
- the first committed product/commercial boundary is the relevant product decision;
- standard parity requirements can proceed from verified market evidence;
- unresolved high-impact or differentiating assumptions remain OPEN/NOT PROVEN rather than being invented;
- domain work can proceed to readiness analysis without pretending that unresolved business choices are already decided.

## 5. Minimum Decisions Before Domain Confirmation

The following decisions are the minimum meaningful blockers; they do not require a full field study:

1. Define the first coherent product journey slice.
2. Define which roles/relationships participate in that slice.
3. Define the minimum learning modes included.
4. Define the minimum evidence and progress semantics needed for that slice.
5. Define the visibility/consent boundary required for participating roles.
6. Define whether organization/tenant and commerce are inside or outside the first slice.
7. Identify any high-impact assumption that cannot responsibly be defined from current market evidence and requires targeted validation.

Everything else should remain explicitly conditional or deferred rather than being guessed.

## 6. First Product Slice Delta

The proposed first slice is now documented in `docs/01-Product/FIRST_PRODUCT_SLICE_DECISION.md`.

Current proposed boundary:

- Student + Teacher are the mandatory actors.
- Parent is a controlled visibility projection, not a first-slice workflow owner.
- Organization may provide context where required but organization operations are outside the first slice by default.
- The first slice is a teacher-led learning loop that closes the cycle from goal/assignment through evidence, teacher decision, student next action, follow-up, new evidence and outcome.
- Payment, attendance, broad organization administration, marketplace, white-label and broad autonomous AI are outside the first slice by default.
- Exact goal semantics, evidence schema, progress calculation and some relationship/visibility rules remain OPEN until domain confirmation.

This reduces the domain decision surface enough to move into explicit domain confirmation without pretending that unresolved semantics are already final.

### Minimum Domain Confirmation Checklist

| Decision | Current status |
|---|---|
| Mandatory actors | PROPOSED |
| Parent visibility boundary | PROPOSED |
| Organization scope | PROPOSED |
| Learning context | PROPOSED |
| Goal / assignment model | OPEN |
| Evidence provenance model | PROPOSED |
| Progress semantics | PROPOSED / calculation OPEN |
| Teacher decision authority | PROPOSED |
| Follow-up lifecycle | DEFINED at semantic level |
| Outcome semantics | PROPOSED |
| Privacy / relationship boundary | DEFINED at principle level; detailed policy OPEN |
| First learning mode(s) | OPEN |
| AI necessity | Not required for first-slice viability |

## 6. Gate Position

**Requirements Consolidation: COMPLETE FOR CURRENT PRODUCT MODEL**

**Domain Readiness: IMPROVED / NOT PROVEN**

**Architecture: NOT PROVEN**

**Implementation: NOT AUTHORIZED**

Next:
**First Product Slice → Minimum Domain Decisions → Domain Confirmation → UX/Architecture Gates**
