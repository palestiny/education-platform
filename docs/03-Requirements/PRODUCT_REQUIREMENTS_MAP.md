# Product Requirements Map

Date: 2026-09-25
Status: Requirements Planning Draft — PROPOSED
Gate: Product Foundation / Competitive Intelligence — NOT PROVEN
Implementation authorization: None

## Purpose

Create traceability from the target product scenario and planning artifacts into candidate requirements without inventing unvalidated business rules.

## Requirement Status

- **PROPOSED** — supported by current product scenario/competitive baseline but not yet committed.
- **OPEN** — requires evidence or an explicit product decision.
- **CONDITIONAL** — depends on initial segment.
- **NOT PROVEN** — specifically requires direct evidence because it is novel, uncertain, materially differentiating, high-impact, or workflow-specific.
- **COMMITTED** — may only be used after the relevant Product/Research gate and decision record authorize it.

## Semantic Alignment with Product Operating Model

The Product Operating Model is now the semantic reference for requirement interpretation. Requirements must preserve these distinctions:

- Context ≠ Goal
- Goal ≠ Learning Action
- Learning Action ≠ Evidence
- Evidence ≠ Interpretation
- Interpretation ≠ Progress State
- Progress ≠ Activity volume
- Recommendation ≠ Decision
- Communication ≠ Authoritative State
- Follow-up ≠ Intervention
- Outcome ≠ Activity completion

This alignment does not approve schemas, APIs, aggregates, bounded contexts, or implementation.

## Candidate Requirement Map

| ID | Requirement area | Candidate requirement | Source | Status |
|---|---|---|---|---|
| PR-001 | Identity | Support a person having multiple roles and relationships | Charter / Target Scenario | PROPOSED |
| PR-002 | Context | Preserve organization, curriculum, locale and permission context | Target Scenario | PROPOSED |
| PR-003 | Learning | Support multiple learning modes without losing learner context | Target Scenario / Parity Backlog | PROPOSED |
| PR-004 | Evidence | Record meaningful learning evidence with source/context/time and provenance | Target Scenario / Product Operating Model | PROPOSED |
| PR-005 | Evidence quality | Preserve provenance, quality and uncertainty so interpretation is not confused with observed evidence | Target Scenario / Product Operating Model | PROPOSED |
| PR-006 | Progress | Present progress as an evidence-traceable state/change relative to goal/context, not activity counts alone | Target Scenario / Product Operating Model | PROPOSED |
| PR-007 | Next useful action | Present a clear next useful action with understandable basis where practical; do not silently mutate authoritative state | Target Scenario / Product Operating Model | PROPOSED |
| PR-008 | Teacher workflow | Let teachers move from signal to evidence to decision to action | Target Scenario / Teacher Journey | PROPOSED |
| PR-009 | Parent workflow | Present relevant child status and required action without surveillance overload | Target Scenario / Parent Journey | CONDITIONAL |
| PR-010 | Organization workflow | Expose required unresolved operational/learning work with ownership | Target Scenario / Organization Journey | CONDITIONAL |
| PR-011 | Communication | Preserve relevant context across role handoffs without making communication the source of truth | Target Scenario / Product Operating Model | PROPOSED |
| PR-012 | Follow-up | Represent due/follow-up state for asynchronous work where required | Target Scenario | PROPOSED |
| PR-013 | Outcome | Represent outcome separately from activity completion and preserve uncertainty where outcome is not demonstrated | Target Scenario / Product Operating Model | PROPOSED |
| PR-014 | Intervention | If a governed intervention workflow is committed later, manage Detect → Explain → Decide → Assign → Intervene → Follow-up → Measure → Close/Escalate | Intervention Validation / Product Operating Model | NOT PROVEN |
| PR-015 | Reliability | Critical workflows must define retry/recovery behavior | Working Rules / MVP Boundary | PROPOSED |
| PR-016 | Offline/recovery | Support offline/low-connectivity behavior where segment evidence requires it | Parity Backlog | CONDITIONAL |
| PR-017 | AI assistance | AI may assist defined workflows while preserving human accountability | Charter / Target Scenario | PROPOSED |
| PR-018 | AI governance | High-impact actions must not be silently delegated to AI | Target Scenario / Governance | PROPOSED |
| PR-019 | Privacy | Visibility must respect role, relationship, consent and policy boundaries | Charter / Target Scenario | PROPOSED |
| PR-020 | Localization | Separate locale, language, RTL/LTR, currency and time-zone concerns from core learning logic | Charter / Parity Backlog | PROPOSED |
| PR-021 | Tenant isolation | Organization data must be isolated according to the final tenancy model | Charter / Gap Register | OPEN |
| PR-022 | Payments | Paid workflows require safe retry/idempotency/reconciliation semantics | Parity Backlog | CONDITIONAL |
| PR-023 | Support | User-facing recovery/support must preserve enough context to resolve failures | Target Scenario | PROPOSED |
| PR-024 | Audit | Important decisions and state-changing actions need traceable audit records | Target Scenario / Working Rules | PROPOSED |
| PR-025 | Search | Permission-aware search is required when content/user scale makes navigation insufficient | Parity Backlog | OPEN |

## Requirement Disposition After Semantic Consolidation

### Stable enough for cross-role product planning

- Identity, roles and authorization context.
- Learning experiences/content, assignments/practice and assessment.
- Evidence with provenance and quality semantics.
- Evidence-traceable progress, while the calculation remains open.
- Next useful action as a recommendation/behavior, not an implicit decision.
- Contextual communication and follow-up semantics.
- Outcome as a separate result concept.
- Reliability, privacy, audit and localization as cross-cutting requirements.
- AI assistance with explicit human/accountability boundaries.

### Remains explicitly open or conditional

- Exact goal representation and progress/mastery calculation.
- Relationship and consent/age semantics.
- Organization/tenant operating model.
- First-release learning modes and commercial boundary.
- Scheduling, attendance, offline, payments, live learning and other operational capabilities where not yet committed.
- Intervention as a governed workflow.
- AI evaluation and any controlled AI actions.
- Search and marketplace/community capabilities.

## Open Requirement Questions

1. What is the first committed commercial/product boundary?
2. Which roles and relationships are first-class in that boundary?
3. What learning modes are included in the first coherent journey slice?
4. Is parent participation part of that slice, and under what consent/visibility policy?
5. Is organization/tenant support required in the first release?
6. Which evidence types are available and trustworthy for the first journey?
7. What constitutes sufficient evidence for a recommendation/interpretation?
8. Which differentiation/intervention workflow, if any, is worth committing after product and market analysis?
9. What privacy/consent rules apply to the chosen roles and age groups?
10. What is the initial business transaction?
11. Which capabilities are free vs paid?
12. What metrics define product value?

## Traceability Rule

Every committed requirement must trace to at least one of:

- validated user evidence,
- verified market requirement,
- explicit product decision,
- regulatory/security requirement,
- technical constraint.

A requirement without a traceable reason remains OPEN. Market-established parity is a valid traceable basis for planning; it becomes COMMITTED only after the relevant Product Gate decision.

## Downstream Rule

This map feeds:

**Requirements → Domain Design → UX Contracts → Architecture → API/Data Contracts → Implementation → Tests → Verification**

It does not authorize implementation by itself.

## Current Gate

**Product Foundation / Competitive Intelligence: NOT PROVEN**

The conventional feature baseline is now sufficiently established to continue parity and requirements planning. The remaining Product Foundation work is explicit product commitment: initial commercial beachhead, business model, first coherent journey boundary, and targeted evidence only where a novel/differentiating/high-impact workflow cannot be responsibly defined from current market evidence. Standard parity capabilities do not require reconstructed participant cases.

