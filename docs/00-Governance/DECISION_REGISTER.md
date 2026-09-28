# Decision Register

| ID | Area | Decision | Status | Source |
|---|---|---|---|---|
| DEC-0001 | Product | Product is a global-ready Educational Operating Platform / Student Success Platform rather than only a video/LMS product. | PROPOSED | Project Charter |
| DEC-0002 | Product | Product principle: simple on the surface, powerful underneath. | PROPOSED | Project Charter |
| DEC-0003 | Identity | Identity should support multiple roles, organizations, permissions and relationships rather than a single UserType. | PROPOSED | Project Charter |
| DEC-0004 | Architecture | Modular monolith is the candidate initial architecture direction; final decision requires Architecture Gate. | CANDIDATE | Project Charter |
| DEC-0005 | AI | AI is an assisting capability; human accountability remains explicit. | PROPOSED | Project Charter |
| DEC-0006 | Research/Product | Established educational capabilities can be validated through current market/product evidence for competitive-parity planning; direct real-user case reconstruction is required primarily for uncertain, materially differentiating, or workflow-specific hypotheses—not for every conventional feature. | PROPOSED | Competitive Feature & Review Benchmark + 2026 market review |
| DEC-0007 | Product | Product scope is organized into Foundation, Advanced, AI and Scale/Ecosystem layers; Foundation defines the trustworthy capability baseline but does not itself define the MVP. | PROPOSED | Feature Gap Matrix + Product Layers |
| DEC-0008 | Product/Research | Initial segment selection must use an explicit decision framework covering journey coherence, role/operational complexity, evidence availability, commercial/privacy implications, expansion path, Foundation-loop fit and differentiation opportunity; the framework must not silently rank or select a segment. | PROPOSED | Initial Segment Decision Framework |

## DEC-0006 Boundary

For capabilities that are already mature and broadly documented across the market—such as content delivery, assignments, assessments, progress tracking, teacher tools, parent visibility, communication, scheduling, payments where relevant, mobile/web, localization, and baseline AI assistance—we should establish the **parity baseline from current product evidence** and then define the minimum acceptable behavior.

We do **not** need a fabricated or individually reconstructed user case to justify that a conventional capability exists in the market.

Direct workflow evidence remains necessary when we are trying to establish:
- a new or uncertain user problem,
- a segment-specific requirement,
- a materially different workflow,
- a proposed differentiator,
- a claim about prevalence/severity/cost,
- or a high-impact behavior whose product semantics cannot safely be inferred from market presence alone.

This distinction prevents two opposite errors:
1. treating every standard feature as if it requires novel field research before we can design it;
2. treating a novel product hypothesis as proven merely because competitors expose a similar feature.

## DEC-0008 Boundary

The segment framework is a decision-support artifact, not a market ranking.

It makes the decision variables explicit while preserving unknowns. Conceptual Foundation loops are allowed for scenario design, but they are not treated as validated user evidence.

The framework does not authorize:
- choosing the initial segment without the required evidence,
- committing the MVP,
- finalizing the domain model,
- approving tenancy or architecture,
- or beginning implementation.

## Lifecycle

Statuses: PROPOSED, ACCEPTED, REJECTED, SUPERSEDED, OPEN.

An architectural decision that materially affects implementation should receive an ADR under adr/.
