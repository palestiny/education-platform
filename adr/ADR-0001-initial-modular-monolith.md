# ADR-0001 — Initial Modular Monolith Architecture for the First Product Slice

**Status:** PROPOSED  
**Date:** 2026-09-28  
**Decision owner:** Project Owner  
**Gate:** Architecture Gate — NOT PROVEN

## Context

The platform is being designed as a global-ready educational operating platform. The first coherent workflow under review is the Teacher-Led Learning Loop:

**Authorized Context → Goal/Assignment → Learner Action/Submission → Assessment Result/Evidence → Teacher Decision → Next Action → Follow-up → New Evidence → Outcome**

The system must preserve cross-role context, attributable evidence, human accountability, reliable retries/recovery, role/relationship-aware authorization and the distinction between authoritative learning facts and derived projections.

Future scale does not by itself establish that a distributed-services architecture is required for the first slice.

## Problem

We need an initial architecture that preserves strong domain boundaries, keeps authoritative learning state reliable, supports explicit authorization and tenant/relationship context, isolates external providers, allows projections to be rebuilt, avoids premature distributed-system complexity, and leaves credible paths for later extraction or topology changes.

## Decision

**PROPOSED:** Start the first product slice as a **Modular Monolith** with explicit logical module boundaries, internal contracts and isolated external integration adapters.

The initial deployment should be a single deployable application unless an operational constraint proves that separate deployment is necessary.

Logical boundaries:
1. Identity & Access
2. Organizations & Relationships
3. Authorization
4. Learning
5. Assessment
6. Evidence
7. Learner State / Progress
8. Follow-up
9. Communication / Notifications
10. Audit / Observability
11. AI Assistance
12. External Integrations

These are logical boundaries, not a requirement for one service/process/database schema per module.

### Dependency direction

**Identity/Access → Organization/Relationship Context → Authorization → Learning Workflow**

The Learning workflow may consume explicit contracts from Assessment, Evidence, Follow-up and derived Learner State.

Communication/Notifications consume authorized state or explicit commands and do not own learning truth.

AI consumes permitted context/evidence and returns bounded assistance. Core authoritative learning mutations must not require AI availability.

External providers are accessed through integration adapters. Provider-specific state must not become the platform's learning source of truth.

## Architecture policies

### 1. Authoritative vs derived state

Authoritative business facts are durable and attributable.

Examples:
- learning context;
- goals/assignments;
- submissions/attempts;
- assessment results;
- evidence;
- teacher decisions;
- follow-up state;
- outcomes;
- required audit records.

Derived state should be rebuildable from authoritative records: progress summaries, dashboards, recommendations, search indexes, analytics aggregates and notification delivery projections.

### 2. Evidence lineage

Evidence is a first-class provenance-bearing capability.

Meaning-changing corrections preserve historical evidence and create attributable version/supersession lineage. Conflicting evidence remains representable.

Assessment Result and Evidence remain distinct concepts.

### 3. Authorization

Protected commands evaluate authorization at command time using applicable actor, role/policy, organization/tenant, relationship, resource, action and learning/business context.

Relationship existence does not itself grant permission.

Parent visibility is a policy-controlled projection rather than unrestricted access to underlying learner records.

### 4. Consistency and transactions

Only state that must change together belongs inside the same local consistency boundary.

External delivery, search, analytics and provider operations are not assumed to share the authoritative transaction.

Where reliable asynchronous publication is needed, an outbox-style local intent may be committed atomically with authoritative state.

### 5. Idempotency

Retriable authoritative commands must have deterministic duplicate semantics.

A retry must not create a second business fact. Idempotency identity is scoped to command/actor/context semantics and is not a substitute for domain identity.

### 6. Concurrency

Accountable learning mutations must not use implicit last-write-wins semantics.

Stale state, concurrent decisions, evidence corrections and authorization changes must be detectable and resolved explicitly.

### 7. Recovery and reconciliation

Derived projections and external provider state must be recoverable from authoritative records.

Reconciliation repairs divergence; it does not rewrite historical evidence or silently change authoritative business facts.

### 8. Human accountability and AI

AI is bounded assistance. It may help with explanation, planning, summarization, practice, classification or recommendations where authorized.

AI cannot silently become authoritative for grades, permissions, payments, learner truth or irreversible interventions.

High-impact decisions remain attributable to an authorized decision owner.

### 9. Observability

Critical state transitions should be traceable with correlation/workflow identifiers and appropriate audit records.

Operational telemetry must minimize unnecessary sensitive learner content.

## Alternatives considered

### A. Microservices from day one

Advantages: independent deployment and scaling, runtime isolation and natural independently owned capabilities.

Trade-offs: distributed transactions, service-to-service failure modes, operational overhead, duplicated infrastructure/observability and premature topology decisions.

**Disposition:** Not selected for the first slice. Future extraction remains possible when a concrete scaling, isolation, organizational or operational requirement justifies it.

### B. Conventional monolith without explicit module boundaries

Advantage: fastest initial implementation.

Trade-offs: higher risk of domain coupling, persistence/schema coupling, hidden cross-module dependencies and harder future extraction.

**Disposition:** Not selected.

### C. Modular monolith

Advantages: local transactions, simpler operations, explicit domain boundaries, easier debugging/recovery, lower initial operational complexity and a path toward later service extraction.

Trade-offs: module boundaries require discipline; the single deployment can become a scaling boundary; future extraction may require distributed coordination.

**Disposition:** Proposed initial architecture.

## Consequences

Positive:
- First slice can be implemented without premature distributed complexity.
- Domain semantics remain visible in architecture.
- Authoritative learning state can remain strongly consistent within local boundaries.
- Derived projections can fail/rebuild without corrupting learning truth.
- External providers can be replaced or reconciled.
- Future service extraction remains possible at selected boundaries.

Accepted costs:
- Module boundaries require continuous enforcement.
- Initial deployable is not independently scalable per module.
- Some future extraction work may require distributed consistency patterns.
- Tenant isolation and authorization still require deliberate Security/Data decisions.

## Explicitly deferred

This ADR does not decide exact database schema/tables/indexes, evidence taxonomy/storage, API contracts, physical tenant-isolation mechanism, detailed consent/age/country policy, retention/deletion/legal-hold policy, or final cloud/vendor choices.

Those decisions belong to downstream Security, Data and API gates.

## Evidence and confidence

Evidence used:
- current product/domain requirements in repository;
- first-slice domain confirmation candidate;
- architecture gate preparation;
- security-impacting architecture review;
- data-impacting architecture review;
- architecture closure review.

This is a proposal derived from the current first-slice requirements, not proof that future production workloads will remain within a single deployment.

**Confidence:** Medium for the first-slice architectural shape; intentionally lower for future scale/topology.

## Related decisions

- DEC-0003 — multi-role identity model.
- DEC-0005 — bounded AI assistance and human accountability.
- DEC-0010 — shared cross-role product journey.
- DEC-0011 — first-slice domain contract candidate.
- Architecture Gate Inputs — docs/05-Architecture/ARCHITECTURE_GATE_INPUTS.md.

## Acceptance boundary

This ADR becomes **ACCEPTED** only after explicit Architecture Gate review confirms the decision and its policies.

Until then:
- the ADR is PROPOSED;
- Architecture Gate remains NOT PROVEN;
- implementation remains unauthorized.
