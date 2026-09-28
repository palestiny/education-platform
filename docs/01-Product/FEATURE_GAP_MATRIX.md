# Feature Gap Matrix

Date: 2026-09-28
Status: Product Planning Artifact — PROPOSED
Gate: Product Foundation — NOT PROVEN
Implementation authorization: None

## Purpose

Translate the current global market baseline into a product-scope map separating established parity, advanced capabilities, AI capabilities, scale/ecosystem capabilities, and differentiation hypotheses.

This is a decision-support artifact, not an implementation commitment.

## Product Layers

### Foundation
The minimum trustworthy learning loop:
**Context → Learn/Practice → Assess → Evidence → Progress → Next Action → Feedback/Follow-up**

### Advanced
Capabilities that deepen learning quality or managed-learning operations: mastery/skills, adaptive assessment, curriculum alignment, actionable insights, parent visibility, scheduling, attendance, payments and organization operations where the selected segment requires them.

### AI
Assistive capabilities built around real learning/teaching workflows. High-impact decisions retain explicit human accountability and require evaluation, provenance, permission-aware context and safe failure behavior.

### Scale / Ecosystem
Capabilities driven mainly by operating model and expansion: multi-tenancy, white-label, marketplace, enterprise integrations, advanced BI, content ecosystem, community and broad gamification.

## Capability Matrix

| Capability | Layer | Market maturity | Segment dependency | MVP candidate | Minimum quality bar | Evidence needed for commitment | Status |
|---|---|---|---|---|---|---|---|
| Identity & authentication | Foundation | Mature | Low | Yes | Recovery, session continuity, safe retry | Market + product decision | PROPOSED |
| Multi-role identity/relationships | Foundation | Mature | Low | Yes | Explicit permissions and relationship context | Product/domain decision | PROPOSED |
| Learner context/profile | Foundation | Mature | Medium | Yes | Provenance, current context, privacy | Product/domain decision | PROPOSED |
| Content delivery | Foundation | Mature | Low | Yes | Accessible, resumable, permission-aware | Market baseline | PROPOSED |
| Learning activities | Foundation | Mature | Low | Yes | Clear state and recovery | Market baseline | PROPOSED |
| Assignments/homework | Foundation | Mature | Medium | Yes | Submission state and recovery | Market baseline + segment | PROPOSED |
| Practice | Foundation | Mature | Low | Yes | Feedback and continuity | Market baseline | PROPOSED |
| Assessments/question banks | Foundation | Mature | Medium | Yes | Attempt integrity and clear result semantics | Market baseline + segment | PROPOSED |
| Feedback/grading | Foundation | Mature | Medium | Yes | Explainable state and correction path | Market baseline | PROPOSED |
| Evidence/provenance | Foundation | Semantics vary | Low | Yes | Source/context/time/quality retained | Product/domain decision | OPEN |
| Progress | Foundation | Mature | Low | Yes | Evidence-backed, not activity vanity | Product/domain decision | OPEN |
| Student next action | Foundation | Common direction, semantics vary | Medium | Yes | Clear rationale and safe fallback | Targeted workflow evidence | NOT PROVEN |
| Teacher workflow/insights | Foundation | Mature | Medium | Yes | Action-oriented, low cognitive load | Segment + workflow decision | PROPOSED |
| Human help/escalation | Foundation | Established | Medium | Conditional | Context preserved across handoff | Segment/workflow evidence | PROPOSED |
| Communication | Foundation | Mature | Medium | Yes | Context, permissions, delivery state | Market baseline | PROPOSED |
| Notifications | Foundation | Mature | Medium | Yes | Preference, dedupe, recovery | Market baseline | PROPOSED |
| Privacy/permissions | Foundation | Expected | Low | Yes | Least privilege, explicit visibility | Product/security decision | PROPOSED |
| Auditability | Foundation | Expected | Medium | Yes | Trace important state changes | Product/security decision | PROPOSED |
| Arabic/RTL + localization | Foundation | Established | Initial/global | Yes | Complete RTL/LTR and locale separation | Product architecture decision | PROPOSED |
| Reliability/recovery | Foundation | Expected | Low | Yes | Retry, duplicate protection, recoverable failures | Product/engineering decision | PROPOSED |
| Mastery/skill progression | Advanced | Strong | Medium | Conditional | Evidence model + explainable state | Targeted semantic validation | OPEN |
| Adaptive/personalized assessment | Advanced | Strong | Medium | Conditional | Safe adaptation + transparent basis | Segment/product evidence | OPEN |
| Curriculum alignment | Advanced | Strong | High | Conditional | Configurable, versioned | Initial segment decision | OPEN |
| Parent visibility | Advanced | Established in K-12 | High | Conditional | Confidence, consent, no surveillance overload | Segment + privacy semantics | OPEN |
| Scheduling/calendar | Advanced | Mature | High | Conditional | Time-zone/locale correctness | Segment decision | OPEN |
| Attendance | Advanced | Mature in managed learning | High | Conditional | Correctness + recovery | Segment decision | OPEN |
| Payments/fees | Advanced | Mature in operations | High | Conditional | Idempotency + reconciliation | Business model + segment | OPEN |
| AI tutoring/homework help | AI | Commercially exposed | Medium | Conditional | Grounding, safe fallback, evaluation | Product/AI gate | OPEN |
| AI practice/study generation | AI | Commercially exposed | Medium | Conditional | Source traceability + quality checks | Product/AI gate | OPEN |
| AI teacher planning/feedback | AI | Commercially exposed | Medium | Conditional | Human review + provenance | Product/AI gate | OPEN |
| Conversational/roleplay AI | AI | Commercially exposed/emerging | Medium | Later/conditional | Safety, context, evaluation | Targeted workflow evidence | OPEN |
| Voice/multimodal AI | AI | Emerging commercial | Medium | Later | Accessibility, privacy, fallback | Targeted validation | OPEN |
| Grounded/RAG assistance | AI | Emerging ecosystem | Medium | Later | Source grounding, permission filtering | Architecture + AI gate | OPEN |
| Controlled AI actions | AI | Emerging | High | Later | Explicit authorization, audit, rollback | High-impact workflow evidence | NOT PROVEN |
| Multi-tenancy | Scale/Ecosystem | Mature | High | Architecture-dependent | Strong isolation + audit | Business/architecture decision | OPEN |
| White-label | Scale/Ecosystem | Established | High | Later | Tenant isolation + branding boundaries | Business model | OPEN |
| Marketplace | Scale/Ecosystem | Established | High | Later | Commerce, trust, moderation | Business model | OPEN |
| Enterprise integrations | Scale/Ecosystem | Mature | High | Later | Contract/versioning/security | Segment/business | OPEN |
| Advanced BI | Scale/Ecosystem | Mature | High | Later | Permission-aware, actionable | Segment/business | OPEN |
| Community/social | Scale/Ecosystem | Established | High | Later | Safety/moderation | Product/segment evidence | OPEN |
| Gamification | Scale/Ecosystem | Established | Medium | Later | Educational purpose, not vanity engagement | Product evidence | OPEN |

## Interpretation

**OPEN** means the capability is not rejected; its commitment depends on missing segment, business, semantic, security or architecture decisions.

**NOT PROVEN** means targeted evidence is required because semantics or differentiation cannot safely be inferred from market presence alone.

**PROPOSED** means planning baseline, not implementation authorization.

## Critical Distinction

**Foundation is not the MVP.** Foundation defines the trustworthy capability baseline from which a coherent MVP is selected.

The MVP is:
**Initial Segment + Core Journey + Required Foundation + Segment-Required Advanced + Explicit Quality Bar**

## Research Boundary

Established market capabilities do not require one reconstructed user case each merely to establish parity. Direct evidence remains necessary for new/uncertain problems, segment-specific behavior, materially different workflows, proposed differentiators, prevalence/severity/cost claims, and high-impact semantics.

## Gate Status

**NOT PROVEN**
