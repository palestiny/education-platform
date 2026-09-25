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
- **NOT PROVEN** — depends on direct workflow validation.
- **COMMITTED** — may only be used after the relevant Product/Research gate and decision record authorize it.

## Candidate Requirement Map

| ID | Requirement area | Candidate requirement | Source | Status |
|---|---|---|---|---|
| PR-001 | Identity | Support a person having multiple roles and relationships | Charter / Target Scenario | PROPOSED |
| PR-002 | Context | Preserve organization, curriculum, locale and permission context | Target Scenario | PROPOSED |
| PR-003 | Learning | Support multiple learning modes without losing learner context | Target Scenario / Parity Backlog | PROPOSED |
| PR-004 | Evidence | Record meaningful learning evidence with source/context/time | Target Scenario | PROPOSED |
| PR-005 | Evidence quality | Preserve evidence quality/uncertainty and avoid unsupported conclusions | Target Scenario | PROPOSED |
| PR-006 | Progress | Present progress from evidence rather than activity counts alone | Target Scenario | PROPOSED |
| PR-007 | Student next action | Present a clear next useful action with understandable rationale | Target Scenario / Student Journey | PROPOSED |
| PR-008 | Teacher workflow | Let teachers move from signal to evidence to decision to action | Target Scenario / Teacher Journey | PROPOSED |
| PR-009 | Parent workflow | Present relevant child status and required action without surveillance overload | Target Scenario / Parent Journey | CONDITIONAL |
| PR-010 | Organization workflow | Expose required unresolved operational/learning work with ownership | Target Scenario / Organization Journey | CONDITIONAL |
| PR-011 | Communication | Preserve relevant context across role handoffs | Target Scenario | PROPOSED |
| PR-012 | Follow-up | Represent due/follow-up state for asynchronous work where required | Target Scenario | PROPOSED |
| PR-013 | Outcome | Capture outcome evidence before closing important intervention work | Target Scenario | PROPOSED |
| PR-014 | Intervention | Manage Detect → Explain → Decide → Assign → Intervene → Follow-up → Measure → Close/Escalate | Intervention Validation | NOT PROVEN |
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

## Open Requirement Questions

1. What is the initial beachhead segment?
2. Which roles are truly first-class in that segment?
3. What learning mode is essential for the beachhead?
4. Is parent participation essential, optional, or absent?
5. Is organization/tenant support required in the first release?
6. Which evidence types are actually available and trustworthy?
7. What constitutes sufficient evidence for a recommendation?
8. Which intervention workflow, if any, has demonstrated recurring material friction?
9. What privacy/consent rules apply to the chosen segment and age groups?
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

A requirement without a traceable reason remains OPEN.

## Downstream Rule

This map feeds:

**Requirements → Domain Design → UX Contracts → Architecture → API/Data Contracts → Implementation → Tests → Verification**

It does not authorize implementation by itself.

## Current Gate

**Product Foundation / Competitive Intelligence: NOT PROVEN**

The immediate blocker remains direct validation of real recent workflows. Public competitor evidence is sufficient to build the parity planning baseline but not sufficient to prove the proposed intervention differentiator or finalize the MVP.

