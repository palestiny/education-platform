# Opportunity Map

Date: 2026-09-24
Status: Research Draft
Gate: Product Foundation / Competitive Intelligence — NOT PROVEN

## Purpose

Translate the current evidence into researchable opportunity areas without turning hypotheses into requirements.

This document is intentionally decision-support material. It does not authorize implementation, architecture, APIs, data models, or UI behavior.

## Evidence-to-Opportunity Map

| ID | Evidence / signal | Opportunity area | Confidence | What must be proven next | Potential product implication |
|---|---|---|---|---|---|
| OPP-001 | Major platforms already provide content, practice, assessment, interaction, parent visibility and/or institutional tooling. | Coordinate the learning journey instead of adding isolated features. | INFERENCE | Observe real journeys across multiple platforms and users; identify coordination failures that materially affect outcomes. | Candidate end-to-end orchestration layer. |
| OPP-002 | Khan Academy already provides parent progress, assignments and activity reporting. | Convert progress data into understandable, appropriately scoped decisions/actions for families. | PRODUCT OPPORTUNITY | Interview parents; test whether current dashboards answer what matters now, why it matters, and what action is appropriate. | Parent experience may focus on confidence + next action rather than raw monitoring. |
| OPP-003 | UNESCO reports limited robust evidence of EdTech added value and emphasizes educational objectives/evidence. | Make learning evidence and outcomes first-class product concepts. | VERIFIED MARKET CONCERN + PRODUCT OPPORTUNITY | Define which evidence is reliable enough for decisions; validate whether users value evidence-based guidance. | Avoid optimizing primarily for views, clicks, minutes or completion. |
| OPP-004 | Public reviews across Noon, Abwaab and Khan Academy contain materially similar examples involving reliability, access/curriculum fit, navigation and usability. | Treat reliability, recovery and support continuity as part of the learning experience. | EARLY PATTERN SIGNAL | Collect larger, dated samples across platforms and segments; distinguish transient incidents from repeated systemic issues and measure learning impact. | Reliability/recovery should be evaluated as product capabilities, not only technical concerns. |
| OPP-005 | Recent research on school platformisation reports benefits from streamlined administration/communication alongside concerns about monitoring, digital exclusion and teacher digital wellbeing. | Design visibility and automation with explicit human boundaries. | RESEARCH EVIDENCE | Validate these tensions in target markets and user segments; identify acceptable visibility/notification boundaries. | Privacy, consent, explainability and human control should shape future requirements. |
| OPP-006 | Existing products span different contexts: consumer learning, curriculum support, professional learning, school/LMS and marketplace models. | Build a core model that supports multiple operating contexts without hard-coding one market. | PRODUCT OPPORTUNITY | Validate tenant/role/context needs and identify which capabilities are genuinely shared. | Global-ready domain model with configurable local context remains a hypothesis. |

## Current High-Value Research Questions

1. When a learner is struggling, can the learner identify the next useful action without searching across multiple tools?
2. Can a parent understand current learning state, the reason for concern, and an appropriate action without becoming a surveillance operator?
3. Can a teacher identify which students need attention and why, without manually assembling data from multiple places?
4. Can an organization coordinate teaching, attendance, assessment, communication and intervention without fragmented workflows?
5. Which evidence is strong enough to justify a recommendation or intervention?
6. What happens when connectivity, playback, scheduling, payment or communication fails during a learning journey?
7. Which parts of the experience must remain human-controlled even when AI is available?
8. Which requirements are global platform invariants, and which should be local configuration?
9. Which reliability failures are merely annoying, and which materially interrupt learning continuity?
10. What recovery behavior allows a learner to resume from the same learning context after failure?

## Research Method for the Next Pass

### A. User Evidence

Collect evidence from:
- students,
- parents,
- teachers,
- teaching assistants,
- center/school operators.

Use interviews, surveys, support/review analysis and workflow walkthroughs.

For every finding record:
- date,
- user segment,
- context,
- direct evidence,
- frequency,
- severity,
- current workaround,
- affected journey,
- confidence.

### B. Competitor Workflow Walkthroughs

For each selected competitor, trace one complete scenario rather than counting features:

Student:
Goal → discovery → learning → practice → assessment → feedback → next action.

Parent:
Child state → evidence → interpretation → concern → action → follow-up.

Teacher:
Class state → student evidence → prioritization → intervention → follow-up.

Organization:
Operational state → exception → assignment → resolution → reporting.

### C. Validation Rules

- A single review remains a USER REPORT.
- Multiple independent reports may establish an EARLY PATTERN, but not automatically a systemic market weakness.
- A systemic claim requires broader evidence and careful segmentation.
- Official product claims are evidence of documented capability, not independent proof of effectiveness.
- A hypothesis cannot become a requirement until the Product Foundation Gate or a later approved decision explicitly promotes it.

## Current Gate Assessment

Status: NOT PROVEN

Reason:
The evidence is sufficient to continue focused discovery, and reliability/access/navigation now has an early-pattern signal, but the evidence is still insufficient to lock differentiation, detailed requirements, domain boundaries, or architecture.

## Decision Impact

No implementation is authorized by this document.

The candidate learning-orchestration direction and reliability/recovery direction remain PRODUCT OPPORTUNITY hypotheses.
