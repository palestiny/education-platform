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


| DEC-0009 | Product/Research | The platform is market-led rather than dependent on social/field case research. We will use established market capabilities and documented competitor gaps to design the integrated education platform. Direct user research is optional and targeted: it may validate uncertain/high-impact differentiators, but absence of field cases does not block parity planning, product definition, domain design, or implementation once the relevant product/design/architecture gates are otherwise satisfied. | PROPOSED | Product direction clarification, 2026-09-28 |

## DEC-0009 Boundary

The project objective is to build a unified education platform connecting students, parents, teachers and educational organizations/centers, with useful capabilities that address known market gaps where evidence exists.

We are **not** treating the product as a social research project, a sampled study, or a hypothesis that must be proven through participant cohorts before normal product work can continue.

Research remains useful, but it has a narrower role:

- market and competitor research establishes the capability baseline and visible gaps;
- product analysis converts those findings into requirements and coherent workflows;
- direct user research is optional targeted validation for uncertain or materially differentiated assumptions;
- no fictional case, participant sample, or prevalence claim is required merely to build conventional platform capabilities.

Therefore:

- **0 direct cases is not a blocker by itself.**
- The absence of direct cases must not be described as the primary reason implementation is unauthorized.
- Implementation remains gated by product, requirements, domain, UX, architecture, security, data and API decisions—not by a mandatory field-study milestone.
- If a future feature depends on a claim that cannot be responsibly inferred from market evidence, that feature can be marked OPEN/NOT PROVEN or validated later without stopping the rest of the platform.

This decision supersedes the earlier assumption that a direct-case collection gate is the next mandatory project milestone.


| DEC-0010 | Product | The platform's first coherent product model is a shared cross-role journey: Context → Goal → Plan → Learn/Practice → Assess → Evidence → Understand → Progress → Next Useful Action → Feedback/Follow-up → New Evidence → Outcome. Role experiences are specialized views/actions over shared context and state, not disconnected products. | PROPOSED | Core Product Journey + Platform Capability Map, 2026-09-28 |

## DEC-0010 Boundary

This is a product semantic decision candidate, not a domain or architecture decision.

It establishes the direction that:
- student, parent, teacher, and organization experiences remain connected;
- evidence is distinct from interpretation;
- progress is distinct from raw activity;
- recommendations are distinct from decisions;
- communication is distinct from authoritative learning/business state;
- follow-up may exist without automatically creating formal intervention cases;
- critical workflows must preserve context and recoverability.

It does not finalize:
- the commercial beachhead;
- MVP scope;
- role set;
- tenancy;
- progress/mastery calculation;
- intervention semantics;
- AI scope;
- APIs, database schema, bounded contexts, or technology.

## Lifecycle

Statuses: PROPOSED, ACCEPTED, REJECTED, SUPERSEDED, OPEN.

An architectural decision that materially affects implementation should receive an ADR under adr/.


| DEC-0011 | Domain | First-slice domain contract candidate separates Context, Goal, Assignment, Submission/Attempt, Assessment Result, Evidence, Teacher Decision, Next Action, Follow-up and Outcome; authoritative state is attributable/contextual, derived projections are rebuildable, and corrections preserve evidence lineage. | PROPOSED | Domain Readiness Checkpoint, 2026-09-28 |

## DEC-0011 Boundary

This is a **domain-contract proposal** prepared for Domain Confirmation.

It establishes proposed invariants:
- learning records are interpreted within an applicable learning context;
- assessment results and evidence are distinct;
- assignment completion does not prove learning achievement;
- teacher authority is contextual and time-bounded by valid authorization;
- evidence corrections preserve historical lineage;
- conflicting evidence is represented rather than silently overwritten;
- follow-up closure is distinct from outcome declaration;
- retriable authoritative mutations require explicit idempotency behavior;
- derived progress/projections are rebuildable from authoritative records;
- external delivery failure does not silently undo authoritative learning state.

It does not finalize:
- database schemas;
- API contracts;
- bounded contexts/modules;
- tenant architecture;
- exact progress algorithm;
- privacy/consent policy;
- technology/provider choices.

Status remains **PROPOSED** until the Domain Confirmation gate is explicitly passed.


| DEC-0012 | Architecture | Initial architecture candidate: Modular Monolith with explicit logical module contracts, command-time authorization, authoritative-vs-derived state policy, evidence lineage, idempotency/concurrency/reconciliation policies and isolated external integrations. | PROPOSED | ADR-0001 + Architecture Closure Review, 2026-09-28 |

## DEC-0012 Boundary

This decision is represented by ADR-0001 and remains **PROPOSED** until the Architecture Gate is explicitly passed.

It does not finalize:
- database schema;
- API contracts;
- physical tenant isolation;
- consent/age/country policy;
- retention/deletion policy;
- cloud/vendor selection;
- implementation.

If accepted, later decisions may refine or supersede this architecture without silently changing its meaning.
