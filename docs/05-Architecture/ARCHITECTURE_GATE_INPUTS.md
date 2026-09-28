# Architecture Gate Inputs

Date: 2026-09-28
Status: Architecture Gate Preparation — NOT PROVEN
Gate: Architecture Gate — NOT PROVEN
Implementation authorization: None

## Purpose

Prepare the evidence, constraints, quality attributes, domain boundaries and unresolved decisions required for an Architecture Gate without prematurely selecting a final architecture, technology stack, tenancy model, database model, deployment topology, or provider strategy.

This document is an input to architecture review, not an architecture decision.

## Current Evidence Boundary

The repository has:
- a target product scenario;
- competitive parity baseline;
- role-oriented user journeys;
- an MVP boundary proposal;
- candidate requirements;
- a candidate domain map.

The Product Foundation and Domain Gates remain **NOT PROVEN**. Direct workflow evidence is optional targeted validation for material uncertainty, but its absence is not a blocker for normal market-led product planning.

Therefore architecture work may define constraints and evaluate options, but must not convert product proposals into implementation commitments.

## Architecture Drivers

### A1 — Learning context continuity

A learner may move among content, practice, assessment, communication and different delivery modes without losing the relevant context.

Architectural implication:
- context identity and references must be explicit;
- derived views must not become the source of truth;
- cross-module contracts must preserve required context.

Status: PROPOSED.

### A2 — Evidence provenance

Meaningful learning evidence must retain enough provenance to distinguish observation from interpretation.

Candidate provenance:
- source;
- actor/system;
- time;
- learning context;
- evidence type;
- quality/uncertainty;
- visibility classification;
- correlation/reference to the originating activity.

Status: PROPOSED; exact evidence model OPEN.

### A3 — Human accountability

AI may assist bounded workflows but must not silently become the owner of high-impact learner decisions.

Architectural implication:
- AI outputs require provenance/context;
- human decision ownership must remain representable where required;
- failure/fallback must not make AI a single point of failure;
- auditability must be proportional to impact.

Status: PROPOSED.

### A4 — Role and relationship-aware authorization

Authorization cannot depend on a single UserType.

The system must be able to express, subject to the selected segment:
- person identity;
- roles;
- organization context;
- relationships;
- permissions;
- consent/policy boundaries.

Status: PROPOSED.

### A5 — Reliability and recovery

Critical workflows must define behavior for:
- duplicate/replay;
- timeout;
- external provider failure;
- partial failure;
- retry;
- recovery;
- user-visible failure state;
- observability;
- audit where required.

Status: PROPOSED.

### A6 — Global-ready foundation

Egypt is an initial market context, not an architectural boundary.

The design must avoid hard-coding:
- language;
- RTL/LTR;
- currency;
- time zone;
- academic calendar;
- payment provider assumptions;
- country-specific workflow rules.

Status: PROPOSED.

### A7 — Privacy and safety

Visibility must be policy-driven rather than inherited from convenience.

Architectural review must account for:
- data classification;
- least-privilege access;
- parent/child relationships;
- consent/authorization;
- audit;
- retention/deletion;
- sensitive learning information;
- tenant isolation.

Status: PROPOSED; exact policy OPEN.

### A8 — Operational simplicity

The first system should minimize distributed-system complexity until scale or organizational boundaries justify it.

Candidate principle:
- prefer strong module boundaries and explicit contracts;
- introduce asynchronous/distributed mechanisms only where a concrete requirement justifies them.

Status: CANDIDATE, not an accepted architecture decision.

## Candidate Module Areas

These are candidates for modular boundaries, not approved bounded contexts:

1. Identity & Access
2. Organizations & Relationships
3. Learning
4. Content
5. Assessment
6. Evidence & Learner State
7. Communication & Notifications
8. Follow-up / Work Management
9. Commerce
10. Audit
11. AI Assistance

Potentially deferred:
- Intervention Case
- Recommendation Engine
- Marketplace
- Social/Community
- Advanced Adaptive Learning
- AI Agent Orchestration

The final module set must be derived from the validated beachhead and domain ownership, not from this list alone.

## First-Slice Architectural Boundary Candidate

The proposed first slice is the Teacher-Led Learning Loop:

**Authorized Context → Goal/Assignment → Learner Action/Submission → Assessment Result/Evidence → Teacher Decision → Next Action → Follow-up → New Evidence → Outcome**

This translates the proposed domain invariants into architecture concerns without finalizing modules, schemas, APIs, or technology.

### Candidate module boundary map

| Candidate module | Owns / protects | Must not own |
|---|---|---|
| Identity & Access | identity, authentication, role/permission evaluation | learning truth |
| Organizations & Relationships | organization membership and relationship scope where required | evidence or assessment results |
| Learning | learning context, goals, assignments, learner actions | identity/authentication |
| Assessment | assessment definition, attempts, assessment results | generic evidence history |
| Evidence | attributable evidence, provenance, correction/version lineage | authorization decisions |
| Learner State / Progress | derived learner-state calculations/projections | raw evidence ownership |
| Communication & Notification | messages and delivery state | authoritative learning state |
| Follow-up | owned unresolved work and lifecycle | outcome truth |
| Audit | accountable audit records | generic event sourcing |
| AI Assistance | bounded assistance, recommendations, explanations | authoritative grades, permissions, payments, or irreversible learner state |

“Module” is an architectural boundary candidate, not a separate deployable service or final bounded context.

### Candidate dependency direction

**Identity/Authorization → Context/Relationships → Learning/Assessment/Evidence → Derived Learner State → Communication/Follow-up projections**

Audit and observability observe authoritative transitions without becoming the source of truth. AI Assistance consumes authorized context/evidence and must not become an implicit dependency of core learning-state mutations.

## First-Slice Consistency Matrix

| Operation | Authoritative mutation | Consistency | Retry | Failure handling |
|---|---|---|---|---|
| Create assignment | Learning | Immediate durable commit | Idempotent | Reject invalid scope; safe retry |
| Submit attempt | Learning | Immediate durable commit | Idempotent | Preserve draft/retry state where applicable |
| Produce assessment result | Assessment | Durable with provenance | Idempotent by source/version | Reconcile provider/execution failure |
| Record evidence | Evidence | Immediate durable commit | Idempotent for same source/version | Preserve lineage; never silently overwrite |
| Record teacher decision | Decision owner candidate | Immediate durable commit | Idempotent | Explicit concurrency conflict |
| Create next action | Derived learner-facing state | May follow authoritative decision | Safe retry | Rebuild/reconcile if projection fails |
| Create follow-up | Follow-up | Immediate durable commit | Idempotent | No duplicate work items |
| Deliver notification | Provider boundary | Eventually consistent | Provider-safe retry | Recover delivery; never roll back learning state |
| Rebuild progress/dashboard | Derived | Eventually consistent | Repeatable | Recompute from authoritative records |

### Transaction rule

A transaction should cover only authoritative state that must change together within one local consistency boundary. Notifications, search, analytics and external providers are not assumed to share that transaction.

Where reliable asynchronous publication is required, evaluate an outbox-style mechanism. This is a reliability pattern candidate, not a technology decision.

## Idempotency, Concurrency and Reconciliation

Idempotency is required for retriable authoritative commands where duplication could create a second business fact, including assignment, submission, assessment-result ingestion, evidence, teacher decision and follow-up creation.

Idempotency keys are scoped to actor/context/command semantics rather than treated as global business identifiers.

The first slice must not rely on implicit last-write-wins for accountable learning decisions. Conflicts must remain explicit for concurrent teacher decisions, evidence corrections, authorization changes during a command, and stale client submissions.

Reconciliation is required where authoritative state and external/projection state can diverge, including notification delivery, provider-backed assessment/media state, materialized progress, dashboards/search indexes and outbox delivery. Reconciliation repairs derived/external state from authoritative records; it does not rewrite historical evidence.

## Event / Outbox Policy Candidate

1. Authoritative state is committed first.
2. Required local event/outbox intent is committed atomically with that state when reliable asynchronous publication is required.
3. Consumers are idempotent and retryable.
4. Global event ordering is not assumed unless explicitly required.
5. Events carry stable identifiers and enough context for safe processing without unnecessary personal data.
6. Events do not grant authorization by themselves.
7. Event history is not automatically the business source of truth.

**Status:** PROPOSED.

## Tenancy / Relationship / Authorization Constraints

Regardless of tenancy model:
- commands affecting learning state resolve applicable organization/relationship context where one exists;
- authorization is evaluated at command time;
- relationship existence and permission scope remain distinct;
- parent visibility is a policy-controlled projection;
- tenant isolation is enforced below UI convenience;
- historical records remain attributable after relationship end;
- authorization changes do not retroactively erase valid evidence.

Exact tenancy remains OPEN.

## External Provider Isolation

Provider-backed capabilities require explicit integration boundaries covering provider-neutral domain state, provider-specific state, correlation/idempotency, timeout/retry, failure classification, reconciliation and migration/replacement.

The core learning workflow should remain usable when a non-essential provider is unavailable.

AI, video, messaging, search and payment providers are integrations, not domain owners.

## Observability Contract Candidate

Critical transitions should emit, where safe:
- correlation/trace identifier;
- actor/role context;
- tenant/context identifier;
- command/workflow identifier;
- state transition;
- dependency outcome;
- retry/reconciliation status;
- latency;
- failure category.

Telemetry must minimize sensitive learner content. Operational logs, audit records and business evidence remain distinct.

**Status:** PROPOSED.

## Architecture Option Evaluation — First Slice

| Criterion | Modular Monolith | Services from Start | Hybrid |
|---|---|---|---|
| Local transactional learning loop | Strong | Distributed | Strong for core |
| Early product iteration | Strong | Lower | Strong |
| Operational burden | Lower | Higher | Medium |
| Boundary flexibility while semantics evolve | Strong | Lower | Medium |
| Independent scaling | Later extraction | Strong | Selective |
| Failure surface | Lower | Higher | Medium |
| Reversibility | High if disciplined | Lower | High if adapters are explicit |
| Fit with current evidence state | Strong candidate | Weak candidate | Strong candidate |

### Current architectural recommendation

**PROPOSED — not accepted:** start implementation as a **modular monolith with explicit module contracts**, while isolating infrastructure-heavy external capabilities behind adapters/integration boundaries.

Rationale:
- the first slice contains authoritative state transitions that benefit from local transactional consistency;
- domain semantics are explicit enough to define boundaries but not mature enough to justify distributed service ownership;
- external media/AI/notification/payment concerns can be isolated without distributing the learning core;
- the approach preserves migration paths when a concrete scaling or organizational boundary justifies extraction.

This is a recommendation for review, not an ADR or implementation authorization.

## Architecture Decisions Required Before Gate PASS

1. Confirm the first-slice domain contract.
2. Confirm module ownership and dependency direction.
3. Decide the tenancy/isolation strategy or explicitly scope it as a reversible phase decision.
4. Define the minimum authorization/relationship contract.
5. Define first-slice data consistency boundaries.
6. Confirm idempotency and concurrency semantics.
7. Confirm outbox/event policy where needed.
8. Confirm external-provider adapter policy.
9. Confirm observability baseline.
10. Record the final architecture decision in an ADR.

## What Architecture Preparation May Proceed With

Allowed:
- module boundary review;
- consistency matrices;
- failure/recovery design;
- dependency contracts;
- tenancy option comparison;
- security constraints;
- observability requirements;
- architecture ADR preparation.

Still not authorized:
- production code;
- final database schema;
- final API contract;
- provider selection;
- deployment topology;
- infrastructure provisioning.

## Consistency Questions

Architecture review must explicitly decide, for each critical workflow:

| Question | Current state |
|---|---|
| What is authoritative learner evidence? | OPEN |
| What state is durable? | OPEN |
| What state is derived/materialized? | OPEN |
| When must evidence and derived learner state be transactionally consistent? | OPEN |
| Which actions require idempotency? | PROPOSED |
| Which state transitions require atomicity? | OPEN |
| Which updates may be eventually consistent? | OPEN |
| How are conflicting evidence records represented? | OPEN |
| How is stale/late evidence handled? | OPEN |

## External Dependency Boundary

Potential external dependencies include:
- video/live delivery;
- payment providers;
- messaging/notification providers;
- AI providers;
- search/indexing;
- object storage;
- analytics/observability services.

Architecture must define:
1. which dependency is optional vs required;
2. what happens when it is unavailable;
3. whether the core workflow can continue;
4. retry/idempotency behavior;
5. reconciliation requirements;
6. provider data ownership;
7. migration/provider replacement strategy.

No provider is approved by this document.

## Tenancy and Isolation

The operating model may eventually support:
- individual teacher;
- tutoring center;
- academy;
- school;
- enterprise;
- marketplace/global platform.

However, the first tenancy strategy remains OPEN.

Architecture Gate must compare at least:
- shared database/shared schema with strict tenant controls;
- shared database/separate schema;
- database-per-tenant;
- hybrid evolution.

Decision criteria:
- isolation strength;
- operational complexity;
- cost;
- migration;
- backup/restore;
- analytics;
- noisy-neighbor risk;
- regulatory requirements;
- tenant scale;
- developer complexity.

No option is selected here.

## Security Architecture Inputs

Required topics:
- authentication/session lifecycle;
- authorization model;
- tenant isolation;
- relationship-aware access;
- consent;
- sensitive-data classification;
- secrets management;
- encryption;
- audit;
- abuse prevention;
- rate limiting;
- account recovery;
- provider trust boundaries;
- data retention/deletion;
- child/minor safeguards where applicable.

Security Gate remains downstream of product/domain clarification but must influence architecture constraints.

## Observability Inputs

Every critical workflow should be diagnosable without exposing unnecessary personal data.

Candidate telemetry:
- correlation/trace ID;
- actor/role context where safe;
- tenant context where safe;
- workflow/action ID;
- state transition;
- dependency call outcome;
- retry/recovery;
- latency;
- failure category;
- audit reference where applicable.

Avoid:
- logging credentials;
- raw sensitive learner data;
- unnecessary private communications;
- AI prompts/responses without an explicit retention/security decision.

## Deployment and Scale Questions

Before committing infrastructure, determine:
- expected initial segment volume;
- concurrency profile;
- content/video delivery load;
- peak assessment/live-session patterns;
- geographic distribution;
- availability target;
- recovery objectives;
- data residency requirements;
- operational team size;
- budget constraints;
- release cadence.

These are currently OPEN.

## Candidate Architecture Options

### Option A — Modular Monolith

One deployable application with strong internal module boundaries and explicit contracts.

Advantages:
- lower operational complexity;
- simpler transactions;
- faster early iteration;
- easier local development;
- supports disciplined future extraction.

Risks:
- requires strong boundary governance;
- shared database can encourage coupling;
- poor module discipline can recreate a monolith without boundaries.

Status: CANDIDATE. This matches DEC-0004 but is not accepted.

### Option B — Service-Oriented from the Start

Multiple independently deployable services.

Advantages:
- independent scaling/deployment;
- stronger runtime isolation.

Risks:
- distributed transactions;
- network failure modes;
- operational burden;
- slower early product iteration;
- premature service boundaries while domain evidence is incomplete.

Status: CANDIDATE ONLY.

### Option C — Hybrid

Start with a modular core and externalize only concrete infrastructure-heavy or failure-isolatable capabilities such as media delivery.

Advantages:
- preserves simplicity for core domain;
- isolates specialized infrastructure when justified.

Risks:
- boundary selection can become inconsistent;
- requires explicit dependency contracts.

Status: CANDIDATE ONLY.

## Architecture Evaluation Rule

Do not choose an architecture because it is fashionable or because the eventual product may reach millions of users.

Choose based on:
1. validated first workflow;
2. domain ownership;
3. consistency requirements;
4. security/isolation requirements;
5. reliability/recovery;
6. operational capacity;
7. scale evidence;
8. cost;
9. migration/reversibility.

## Architecture Gate Exit Criteria

Architecture Gate cannot PASS until:

- Product Foundation is sufficiently resolved for the first workflow;
- a first committed product slice exists;
- the product boundary is sufficiently explicit for the first workflow;
- committed MVP scope exists;
- candidate domain boundaries are reviewed;
- key aggregates/state ownership are defined;
- critical consistency requirements are known;
- security/privacy constraints are documented;
- tenancy strategy is decided or explicitly scoped as a reversible phase decision;
- external dependency failure behavior is defined;
- observability requirements are defined;
- API/data contract strategy is established;
- testing/verification strategy can validate architectural guarantees;
- architecture decision is recorded as an ADR.

## Explicit Non-Decisions

This document does NOT decide:
- ASP.NET Core/.NET;
- PostgreSQL;
- Redis;
- Next.js;
- React Native;
- cloud provider;
- Kubernetes;
- microservices;
- modular monolith;
- database schema;
- tenancy isolation model;
- AI provider;
- video provider;
- payment provider.

Those remain candidates until the relevant gates provide sufficient evidence.

## Current Gate

**Architecture Gate: NOT PROVEN**

The repository is now prepared for architecture evaluation, but not architecture commitment.

The current blockers for Architecture Gate PASS are decision closure, not absence of a participant sample. Initial segment/commercial scope matters where it changes architecture; targeted validation may be used for material unresolved uncertainty.

Next:
1. complete Domain Confirmation Review;
2. complete UX Confirmation;
3. review the first-slice architecture boundary and consistency matrix;
4. close tenancy, authorization, security and provider-boundary decisions that materially affect architecture;
5. record the final architecture decision as an ADR only after explicit review.


## Architecture Review Candidate — Post Domain/UX Review

**Date:** 2026-09-28  
**Status:** READY FOR EXPLICIT ARCHITECTURE REVIEW — NOT PROVEN  
**Implementation authorization:** None

The Domain and UX review candidates are now explicit enough to test whether the proposed architecture is internally coherent. This review does not accept the domain contract, UX model, or architecture recommendation.

### Readiness matrix

| Architecture concern | Current state | Gate impact |
|---|---|---|
| First-slice state chain | PROPOSED and internally coherent | Must be confirmed before Architecture PASS |
| Module ownership | PROPOSED | Must be confirmed before implementation |
| Dependency direction | PROPOSED | Must be confirmed |
| Authoritative vs derived state | PROPOSED | Must be resolved before Data/API PASS |
| Idempotency | PROPOSED for critical mutations | Must be reflected in API/Data contracts |
| Concurrency/conflict handling | PROPOSED | Must be reflected in domain/API behavior |
| Evidence versioning/correction | PROPOSED | Must be reflected in Data/API contracts |
| Tenant isolation | OPEN | Architecture-impacting decision |
| Relationship/authorization contract | OPEN | Architecture/Security blocker |
| Parent visibility policy | OPEN | Security/Data blocker |
| Security/privacy baseline | OPEN | Downstream Security Gate; architecture constraints required |
| External provider isolation | PROPOSED | Must be confirmed before provider-dependent implementation |
| Observability baseline | PROPOSED | Must be confirmed before implementation/release |
| Deployment topology | OPEN | Can remain deferred if first-slice operational assumptions are explicit |
| Technology stack | OPEN | Not yet required for architecture commitment |
| Final ADR | OPEN | Required for Architecture Gate PASS |

### Review findings

1. **No internal architectural contradiction is currently identified** between the proposed first-slice state chain and the candidate modular boundary map.
2. The strongest architectural pressure is around **authorization, tenancy/isolation, evidence correction/versioning, and authoritative-vs-derived state**. These should be closed before implementation rather than hidden inside code.
3. The first slice does not currently require independently deployable services merely because the future platform may become large. Scale remains an input to later extraction decisions.
4. External providers should remain replaceable dependencies. Provider-specific state must not become the learning domain's source of truth.
5. Progress, dashboards, notifications and search should remain recoverable projections where their derived nature is accepted; authoritative learning records must remain durable.
6. Architecture should preserve explicit human accountability for Teacher Decision and should not let AI Assistance become a mandatory dependency for the core learning-state transition.
7. The remaining architecture blockers are primarily **decision-closure blockers**, not missing implementation artifacts.

### Architecture review decision boundary

The following may be confirmed during Architecture Review:

- modular boundary and dependency direction;
- local consistency boundary for the first slice;
- idempotency/concurrency/reconciliation policy;
- provider isolation;
- observability baseline;
- whether tenancy is solved now or explicitly deferred as a reversible architecture phase.

The following must **not** be silently decided during Architecture Review:

- exact database tables/schema;
- final REST/GraphQL endpoints;
- exact cloud/provider selection;
- final privacy/consent policy;
- detailed UI/navigation;
- implementation details.

### Candidate architecture conclusion

**Current recommendation remains:** Modular Monolith with explicit module contracts and isolated external integrations.

This remains **PROPOSED**. It is supported by the current first-slice consistency needs and the still-evolving domain semantics, but it becomes an accepted architecture only through an explicit Architecture Gate decision and ADR.

### Immediate next gate sequence

1. Explicit Domain Confirmation.
2. Explicit UX Confirmation.
3. Architecture Review and closure of architecture-impacting decisions.
4. Security Gate.
5. Data Gate.
6. API Contract Gate.
7. Architecture ADR / Architecture Gate PASS decision.
8. Implementation Gate.

Until these are closed, implementation remains unauthorized.


## Architecture Review Decision Matrix — First Slice

**Status:** PROPOSED — NOT AN ACCEPTED ADR

| Decision | Proposed direction | Rationale | Main trade-off | Gate status |
|---|---|---|---|---|
| Application shape | Modular Monolith | Keeps first-slice consistency local while preserving explicit boundaries | Requires discipline to prevent module coupling | PROPOSED |
| Learning workflow ownership | Learning/Teacher Workflow coordinates the first-slice command flow | The workflow spans assignment, learner action, evidence, decision and next action | Coordinator can become a god-module if boundaries are weak | PROPOSED |
| Assessment boundary | Assessment owns assessment definitions, attempts and results | Prevents assessment semantics from leaking into Learning | Evidence still needs an explicit handoff contract | PROPOSED |
| Evidence boundary | Evidence is a first-class capability with provenance/version lineage | Supports correction, conflict and traceability | More modeling than treating records as mutable facts | PROPOSED |
| Learner progress | Derived/rebuildable learner-state projection | Avoids opaque mutable progress as source of truth | Rebuild/recalculation path must be reliable | PROPOSED |
| Follow-up | Explicit capability for unresolved work | Separates work lifecycle from learning outcome | May be unnecessary complexity if the first workflow never creates follow-up | PROPOSED |
| Authorization | Central policy/authorization boundary consumed by modules | Prevents relationship from being treated as permission | Requires explicit scope/context model | OPEN — SECURITY IMPACT |
| Tenant isolation | Tenant context enforced at application/data boundaries | Prevents cross-organization data leakage | Exact strategy depends on security/data decisions | OPEN |
| External providers | Adapter/integration boundary outside domain truth | Providers can fail or change without rewriting learning state | Requires reconciliation/status modeling | PROPOSED |
| Notifications | Recoverable delivery projection/integration | Delivery failure must not rollback authoritative decisions | Adds delivery state and retry handling | PROPOSED |
| Search/analytics | Derived projections | Avoids making query/read models authoritative | Requires rebuild/reconciliation | PROPOSED |
| AI | Optional bounded assistance | Core learning workflow must work without AI | AI features require separate governance/evaluation | PROPOSED |
| Observability | Structured logs + trace/correlation IDs + auditable authoritative mutations | Supports diagnosis and accountability | Operational overhead | PROPOSED |
| Deployment topology | Single deployable first slice unless operational constraints prove otherwise | Avoids premature distributed-system complexity | Later extraction requires stable contracts | DEFERRED |

### Proposed dependency direction

Identity/Auth -> Authorization -> Learning Context -> Learning Workflow

Supporting dependencies:
- Learning Workflow -> Assessment contract
- Learning Workflow -> Evidence contract
- Learning Workflow -> Learner State projection
- Learning Workflow -> Follow-up contract
- Communication/Notifications consume authorized state or explicit commands; they do not own learning truth.
- AI consumes permitted context/evidence and returns bounded assistance; it does not own authoritative learner state.
- External providers sit behind integration adapters and cannot directly mutate domain truth.

No module may depend on another module's persistence schema as an implicit contract.

### First-slice consistency boundary

Authoritative command -> durable state + local intent -> asynchronous/external effect -> delivery/provider state -> reconciliation

The authoritative state and required local audit/outbox intent are atomic where applicable. External effects are outside that atomic boundary.

A retry of the command must be idempotent. A failure after external execution must be reconciled rather than blindly replayed.

### Architecture-level invariants

1. Domain modules communicate through explicit contracts, not shared persistence assumptions.
2. Authoritative records are durable; derived projections are rebuildable.
3. Evidence corrections preserve lineage rather than overwriting history.
4. Authorization is evaluated at command time against applicable context/scope.
5. Relationship existence never grants permission by itself.
6. External provider state never becomes the sole source of learning truth.
7. AI assistance cannot silently mutate authoritative learning state.
8. No implicit last-write-wins for semantically conflicting authoritative mutations.
9. Retries of retriable authoritative commands are idempotent.
10. Projection failure must not corrupt authoritative learning state.

### Architecture blockers that remain

- exact tenant isolation strategy;
- relationship lifecycle and authorization scope model;
- parent visibility/consent policy;
- final durable-versus-derived learner-state boundary;
- exact evidence storage/versioning model;
- detailed concurrency/conflict semantics;
- technology stack and deployment target;
- final API contracts;
- final architecture ADR.

This review strengthens the candidate architecture without converting unresolved policy/data decisions into hidden implementation assumptions.


## Architecture Blocker Decision Matrix — Closure Plan

The following matrix separates decisions that require product/security/data ownership from decisions that can be closed as architecture policy.

| Blocker | Why it matters | Proposed closure direction | Owner/Gate | Status |
|---|---|---|---|---|
| Tenant isolation | Cross-tenant leakage is an architectural failure | Define tenant context as a mandatory authorization/data boundary; defer exact physical isolation until Data/Security review | Product + Security + Data | OPEN |
| Relationship + authorization scope | Relationship must not imply permission | Explicit relationship context + policy evaluation at command time | Product + Security | OPEN |
| Parent visibility/consent | Child data has policy-sensitive visibility | Parent view is a controlled projection; exact age/consent/visibility rules remain Security/Product decisions | Product + Security | OPEN |
| Durable vs derived learner state | Determines consistency, rebuild and recovery | Durable facts: context/assignment/submission/result/evidence/decision/follow-up/outcome; projections rebuildable | Domain + Data | PROPOSED |
| Evidence versioning | Corrections must not destroy historical truth | Append/version lineage with supersession and attribution; conflict remains representable | Domain + Data | PROPOSED |
| Concurrency/conflicts | Prevents silent overwrite of accountable decisions | Explicit version/conflict checks; no semantic last-write-wins | Domain + API | PROPOSED |
| Technology/deployment | Needed for implementation, not for semantic architecture | Keep technology-neutral until Architecture Gate; select stack immediately before Implementation Gate | Architecture | DEFERRED |
| API contracts | Must encode idempotency, auth, conflicts and failure semantics | Define after Domain + Security/Data closure; contract tests before implementation | API Gate | DEFERRED |

### Closure rule

A blocker may be closed only when its proposed direction is supported by an explicit decision and the decision does not silently invent a product/security/data policy.

Architecture Gate PASS requires the architecture implications to be explicit; it does not require every implementation detail to be frozen.

### Recommended next review order

1. Domain Confirmation: accept/reject the first-slice semantic contract.
2. UX Confirmation: accept/reject the interaction and trust boundaries.
3. Security-impacting architecture: authorization, relationship scope, tenant isolation and parent visibility.
4. Data-impacting architecture: durable facts, evidence lineage, derived projections and conflict/version strategy.
5. Architecture closure: module/dependency/consistency/provider/observability decisions.
6. ADR: record the accepted architecture.
7. API Contract Gate.
8. Implementation Gate.

This order prevents the architecture decision from pretending that security or data policy has already been decided.


## Security-Impacting Architecture Review — Identity, Relationship, Tenant and Authorization

### Architectural boundary

The platform must distinguish five different concerns:

1. **Identity** — who the person/system is.
2. **Role** — what capabilities the identity may potentially exercise.
3. **Relationship** — how the identity is related to another identity or learning context.
4. **Organization/Tenant** — where the action/data belongs.
5. **Authorization** — whether this actor may perform this action against this resource in this context at this time.

These are intentionally not collapsed into one enum or one permission flag.

### Proposed authorization evaluation context

A command that can affect protected state should be evaluated against:

**Actor Identity + Effective Roles + Tenant/Organization Context + Relationship Context + Resource + Requested Action + Learning/Business Context + Policy State**

Authorization is evaluated at command time. A historical relationship must remain attributable for historical records but must not automatically grant current access.

### Role is not permission

A role is a capability grouping, not unconditional access.

Examples:
- Teacher does not automatically access every student.
- Parent does not automatically access every learner record.
- Organization membership does not automatically grant access to every organization resource.
- Platform administration does not automatically imply unrestricted access to tenant learning data; privileged access must remain explicitly governed.

### Relationship is not permission

Relationships such as parent/child, teacher/student, assistant/student or organization/member provide context for policy evaluation.

They do not independently authorize every operation.

### Tenant boundary

Tenant context must be explicit at protected data and command boundaries.

The architecture must prevent a request from silently operating outside its authorized tenant context.

The exact physical isolation strategy (shared database, database-level policy/RLS, schema isolation, or separate database) remains a Security/Data decision and is not silently finalized here.

### Parent visibility boundary

Parent-facing information is treated as a **policy-authorized projection**, not direct access to underlying learning records.

The architecture must therefore support:
- selecting which state/evidence is visible;
- applying relationship and consent/policy rules;
- preventing accidental exposure of restricted teacher/internal information;
- preserving the authoritative source independently of the parent projection.

Exact age, guardian, consent, country and exceptional-access rules remain Security/Product decisions.

### Authorization failure semantics

Protected commands must distinguish at least:
- unauthenticated;
- authenticated but unauthorized;
- wrong tenant/context;
- relationship exists but does not authorize requested action;
- resource no longer accessible because authorization scope changed;
- semantic conflict after authorization succeeds.

Authorization failure must not mutate authoritative state.

### Historical access vs current authority

Ending or changing a relationship must not rewrite historical attribution.

For example, a teacher who previously assessed a learner remains the attributable actor of that historical assessment, while future commands are evaluated against the teacher's current authority.

### Architectural invariant

No module may infer authorization solely from:
- role;
- relationship;
- tenant membership;
- possession of an identifier;
- visibility of a UI element.

The authoritative command path must evaluate the applicable authorization boundary.

### Candidate dependency direction

**Identity → Role/Organization Context → Authorization Policy → Domain Command**

Domain modules may request authorization decisions through the authorization boundary but must not duplicate incompatible authorization rules.

Authorization should not become the owner of learning, assessment, evidence or commerce state.

### Security review blockers remaining

The following remain explicitly open for Security/Product confirmation:
- exact tenant isolation mechanism;
- permission model and policy vocabulary;
- relationship lifecycle and delegation;
- parent/guardian consent model;
- age/country-specific visibility rules;
- privileged support/admin access;
- audit requirements for sensitive reads and mutations;
- data retention/deletion requirements.

No final security policy or implementation is implied by this architecture candidate.
