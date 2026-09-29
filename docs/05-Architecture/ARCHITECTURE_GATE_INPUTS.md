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

## Security Decision Closure Matrix — Candidate

**Date:** 2026-09-29  
**Status:** READY FOR EXPLICIT SECURITY DECISION — NOT PROVEN  
**Implementation authorization:** None

This matrix turns the remaining Security Gate questions into explicit decision records. It does not select a legal/privacy policy or silently choose a physical isolation mechanism.

| Decision | Options to evaluate | Current recommendation for review | Trade-off / consequence | Status |
|---|---|---|---|---|
| Tenant isolation | Shared schema + tenant controls; separate schema; database-per-tenant; hybrid | Prefer a phaseable strategy that enforces tenant context and isolation at every protected boundary while preserving a later migration path | Shared model lowers cost/ops but increases blast-radius and enforcement discipline; stronger physical isolation increases cost/ops | OPEN |
| Authorization vocabulary | RBAC-only; RBAC + relationships; policy/relationship-aware authorization | RBAC + relationship/context-aware policy; keep roles separate from permissions | More expressive and safer for parent/teacher/org cases, but policy evaluation is more complex | OPEN |
| Relationship lifecycle | Permanent relationship; scoped/time-bounded relationship; delegated relationship with expiry/revocation | Scoped, attributable relationships with explicit lifecycle and revocation | More lifecycle state, but avoids treating relationship existence as permanent permission | OPEN |
| Parent/guardian visibility | Broad child-record access; curated projection; policy/consent-driven projection | Policy/consent-driven projection with least-privilege defaults | Requires explicit visibility rules and policy evaluation; reduces accidental exposure | OPEN |
| Age/country rules | Global rule; configurable policy; country-specific implementation | Configurable policy boundary; country rules remain configuration/policy, not hard-coded domain assumptions | More configuration complexity, but preserves global readiness | OPEN |
| Privileged support/admin | Unrestricted support; role-only access; scoped/break-glass access | Scoped, time-bounded/break-glass access with strong audit and explicit purpose | Adds operational controls, but limits privileged-access blast radius | OPEN |
| Sensitive reads/mutations | Audit only writes; audit sensitive reads + mutations; audit everything | Audit sensitive reads and high-impact mutations, with data minimization | Better accountability, but more audit volume and privacy considerations | OPEN |
| Retention/deletion | Permanent history; fixed retention; policy-driven lifecycle | Policy-driven lifecycle with explicit correction/deletion/legal-hold semantics | More lifecycle complexity; required for trustworthy privacy/data governance | OPEN |
| Abuse/security controls | Minimal auth controls; baseline controls; adaptive/risk-based controls | Baseline controls first; adaptive controls only where justified | Baseline reduces obvious abuse without prematurely building a risk engine | OPEN |

### Security invariants proposed for gate closure

1. Role, relationship and permission remain separate concepts.
2. Protected commands evaluate authorization at command time.
3. Tenant/context boundaries are enforced below UI visibility.
4. Parent/guardian access is a controlled projection, not unrestricted record access.
5. Privileged access is attributable and auditable.
6. Authorization failure produces no authoritative mutation.
7. Historical attribution survives relationship changes unless an explicit lawful deletion policy requires otherwise.
8. Security telemetry minimizes learner content and secrets.

These invariants are proposed architectural constraints, not a completed security policy.

## Data Decision Closure Matrix — Candidate

**Date:** 2026-09-29  
**Status:** READY FOR EXPLICIT DATA DECISION — NOT PROVEN  
**Implementation authorization:** None

| Decision | Options to evaluate | Current recommendation for review | Trade-off / consequence | Status |
|---|---|---|---|---|
| Authoritative first-slice facts | Durable domain records; event-only history; mixed | Durable domain records for accountable business facts, with derived projections rebuilt from them | More explicit persistence ownership; easier audit/recovery than event-only semantics | OPEN |
| Evidence taxonomy | One generic evidence type; fixed categories; extensible typed categories | Extensible typed evidence with mandatory provenance and context fields | Better evolution; requires taxonomy governance and versioning | OPEN |
| Evidence storage | Evidence as embedded assessment data; first-class evidence store; hybrid | First-class evidence capability, while Assessment remains owner of assessment semantics | Preserves multiple evidence sources without conflating them with assessment results | OPEN |
| Evidence correction | In-place overwrite; append-only correction/supersession; immutable original + current projection | Preserve original lineage and represent correction/supersession explicitly | More storage/history, substantially better provenance | OPEN |
| Conflicting evidence | Last-write-wins; reject conflict; explicit conflict state | Explicit conflict/insufficient state; no silent last-write-wins | Requires downstream interpretation/progress handling | OPEN |
| Learner-state materialization | Fully computed on read; durable aggregate; hybrid projection | Rebuildable projection with explicit materialization only where justified | Rebuild cost vs read performance; avoids derived state becoming hidden truth | OPEN |
| Concurrency | Last-write-wins; optimistic versioning; explicit domain conflict | Explicit conflict + optimistic version mechanics where needed | More client/recovery work, protects accountable decisions | OPEN |
| Atomicity | Large transaction; per-operation transactions; mixed | Small local transactions around facts that must change together; outbox intent where required | Limits coupling while preserving local consistency | OPEN |
| Data classification | One sensitivity level; coarse classes; field/domain classes | Domain-level classification with stricter treatment for learner/assessment/communication/audit data | More policy metadata; better least-privilege enforcement | OPEN |
| Tenant data boundary | Application-only filter; DB-enforced controls; physical isolation | Defense-in-depth; exact physical mechanism follows Security decision and scale/regulatory evidence | More implementation discipline, avoids relying on UI/application conventions alone | OPEN |
| Retention/deletion | Permanent; fixed global TTL; policy-driven lifecycle | Policy-driven lifecycle, preserving required lineage/legal-hold semantics | More lifecycle machinery, but avoids irreversible blanket retention assumptions | OPEN |

### Data invariants proposed for gate closure

1. Authoritative business facts are durable, attributable and owned.
2. Derived state is rebuildable from authoritative records.
3. Evidence correction preserves lineage.
4. Assessment Result remains distinct from Evidence.
5. Semantic conflicts are representable.
6. Retriable authoritative commands have deterministic duplicate semantics.
7. Projection/provider failure cannot corrupt authoritative business state.
8. Recovery repairs derived/external state from authoritative records.
9. Historical attribution is preserved unless an explicit data-governance policy changes that requirement.
10. Data classification is enforced at access boundaries, not only documented.

## Security/Data Architecture Impact Reconciliation — Candidate

Before API Contract Gate, Security and Data decisions must be translated into concrete architecture consequences:

| Closure area | Architecture consequence | Must be explicit before implementation |
|---|---|---|
| Tenant isolation | Tenant context must flow through protected command and data access paths | Yes |
| Authorization | Module contracts must receive/resolve authorization context; no UI-only authorization | Yes |
| Relationship lifecycle | Historical records require stable attribution independent of current relationship status | Yes |
| Parent projection | Parent-facing reads must use policy-controlled projections | Yes |
| Evidence lineage | Evidence APIs/storage must support correction/supersession without destructive overwrite | Yes |
| Concurrency | Accountable mutations need explicit version/conflict semantics | Yes |
| Idempotency | Command identity must be scoped to actor/context/intent | Yes |
| Data classification | Sensitive data access must be enforceable and auditable | Yes |
| Retention/deletion | Ownership and lifecycle metadata must exist before persistence design is frozen | Yes |
| Projection recovery | Derived state needs rebuild/reconciliation path from authoritative records | Yes |

### Gate dependency rule

Security and Data decisions do not require the final API schema to be written first. They require the minimum semantics that the API and persistence layers must preserve.

Conversely, API/Data implementation must not invent unresolved Security policy, consent rules, retention rules or tenant-isolation guarantees.

**Current status:** Security/Data architecture impact is sufficiently mapped for explicit decision review; neither gate is PASS.

## Decision Closure Worksheet — Owner Review Packet

Date: 2026-09-29
Status: READY FOR EXPLICIT OWNER DECISION — NOT PROVEN
Implementation authorization: None

This section converts the remaining architecture/security/data work into a bounded owner-review packet. It does not accept any proposal automatically.

### A. Architecture decision

Question: Accept the proposed first-slice architecture?

Proposal under review:
- Modular Monolith;
- explicit logical module contracts;
- isolated external integration adapters;
- local consistency for authoritative first-slice mutations;
- rebuildable derived projections;
- explicit idempotency/concurrency/reconciliation;
- command-time authorization;
- human accountability for high-impact decisions.

If accepted:
- ADR-0001 may move from PROPOSED to ACCEPTED;
- DEC-0012 may move from PROPOSED to ACCEPTED;
- Architecture Gate can be closed subject to its remaining dependent gates;
- implementation is still NOT AUTHORIZED until Security, Data and API gates are closed.

If not accepted:
- keep ADR-0001 and DEC-0012 PROPOSED;
- record the rejected/changed architecture and affected downstream decisions before continuing.

Decision status: OPEN — Project Owner.

### B. Domain contract decision

The owner review packet for the first slice is:

Authorized Context → Goal/Assignment → Learner Action/Submission → Assessment Result/Evidence → Teacher Decision → Next Action → Follow-up → New Evidence → Outcome

The following semantic distinctions are part of the proposal:
- Goal ≠ Assignment;
- completion ≠ achievement/outcome;
- Assessment Result ≠ Evidence;
- Evidence ≠ Interpretation;
- Recommendation ≠ authorized Decision;
- Follow-up closure ≠ Outcome;
- historical evidence ≠ mutable current fact;
- projection ≠ source of truth.

Decision status: OPEN — Project Owner.

### C. Security decisions

The Security Gate still requires explicit closure of:
1. tenant isolation;
2. authorization vocabulary;
3. relationship lifecycle/delegation;
4. parent/guardian visibility and consent;
5. age/country policy boundary;
6. privileged support/admin access;
7. sensitive-read/high-impact mutation audit;
8. retention/deletion lifecycle;
9. baseline abuse/security controls.

The current review recommendations remain proposals only.

Decision status: OPEN — Security/Product Owner review.

### D. Data decisions

The Data Gate still requires explicit closure of:
1. authoritative first-slice facts;
2. evidence taxonomy and storage;
3. correction/supersession lineage;
4. conflicting/insufficient evidence representation;
5. learner-state materialization/rebuild policy;
6. concurrency/version mechanics;
7. transaction/outbox boundaries;
8. data classification;
9. tenant data boundary;
10. retention/deletion lifecycle.

The current review recommendations remain proposals only.

Decision status: OPEN — Data/Architecture/Product Owner review.

### E. Decisions that remain intentionally deferred

Even after the above decision set is closed, the following remain downstream or evidence-dependent unless separately promoted:
- exact database schema/tables/indexes;
- final REST/GraphQL contract;
- exact cloud/vendor;
- final video/AI/payment provider;
- deployment topology beyond the accepted first-slice architecture boundary;
- universal mastery/progress algorithm;
- advanced intervention/recommendation engine;
- marketplace/social/advanced adaptive-learning domains.

### F. Gate transition rule

No downstream gate may silently consume an unresolved decision.

Required sequence after owner review:

Architecture Decision → Domain Confirmation → UX Confirmation → Security/Data Decision Closure → Architecture Impact Reconciliation → API Contract Gate → Implementation Gate

If an owner decision changes a prior assumption, affected requirements, domain boundaries, UX, security, data and API artifacts must be reconciled before implementation.

Current overall status: decision-ready, not approved, implementation not authorized.


## Decision Closure Worksheet — Architecture → Security → Data

**Date:** 2026-09-29  
**Purpose:** One explicit closure surface for the Project Owner before API Contract and implementation work.  
**Status:** OPEN FOR OWNER DECISION — NO DECISION IMPLIED

### A. Architecture Decision

**Decision under review**

Accept or reject the proposed first-slice architecture:

**Modular Monolith + explicit logical module contracts + isolated external integration adapters**

If accepted, first-slice commitments are: explicit logical module boundaries; contract-based cross-module dependencies; durable attributable authoritative facts; rebuildable derived projections; deterministic idempotency for critical commands; explicit concurrency/conflict handling; replaceable/reconcilable external providers; bounded AI assistance that is not required for authoritative core learning-state transitions.

**Not decided by this acceptance:** database tables/indexes, API shapes, physical tenant isolation, detailed consent/age/country policy, retention/deletion/legal-hold rules, cloud/vendor selection, implementation details.

**Owner decision:** OPEN  
**Required transition:** ADR-0001 + DEC-0012 may become ACCEPTED only after explicit owner acceptance.

### B. Security Decision Set

| Decision | Review direction | Owner decision |
|---|---|---|
| Tenant isolation | Phaseable isolation with enforced tenant context at every protected boundary and later migration path | OPEN |
| Authorization | RBAC + relationship/context-aware policy | OPEN |
| Relationship lifecycle | Scoped, attributable, revocable relationships | OPEN |
| Parent/guardian visibility | Policy/consent-controlled projection with least privilege | OPEN |
| Age/country policy | Configurable policy boundary; country rules not hard-coded | OPEN |
| Privileged support/admin | Scoped, time-bounded/break-glass access with audit | OPEN |
| Sensitive access | Audit sensitive reads + high-impact mutations with minimization | OPEN |
| Retention/deletion | Policy-driven lifecycle with correction/deletion/legal-hold semantics | OPEN |
| Abuse controls | Baseline controls first; adaptive controls only where justified | OPEN |

**Security acceptance condition:** API and persistence designs must not claim guarantees stronger than the accepted decisions.

### C. Data Decision Set

| Decision | Review direction | Owner decision |
|---|---|---|
| Authoritative facts | Durable domain records + rebuildable projections | OPEN |
| Evidence taxonomy | Extensible typed evidence with provenance/context | OPEN |
| Evidence storage | First-class evidence capability; Assessment owns assessment semantics | OPEN |
| Evidence correction | Preserve original lineage + explicit correction/supersession | OPEN |
| Conflicting evidence | Explicit conflict/insufficient representation; no silent LWW | OPEN |
| Learner state | Rebuildable projection; materialize only when justified | OPEN |
| Concurrency | Explicit conflict + optimistic version mechanics where needed | OPEN |
| Atomicity | Small local transactions + outbox where required | OPEN |
| Classification | Domain-level classification with stricter learner/assessment/communication/audit handling | OPEN |
| Tenant data boundary | Defense-in-depth; physical mechanism follows Security/scale/regulatory decision | OPEN |
| Retention/deletion | Policy-driven lifecycle preserving required lineage/legal holds | OPEN |

**Data acceptance condition:** The first persistence model must preserve accepted provenance, correction, conflict, ownership, concurrency, classification and recovery semantics.

### D. Closure Sequence

1. Owner accepts/rejects Architecture proposal.
2. If accepted, mark ADR-0001 and DEC-0012 ACCEPTED and close Architecture Gate.
3. Resolve Security Decision Set.
4. Resolve Data Decision Set.
5. Reconcile any Security/Data decision that changes architecture.
6. Confirm affected Domain and UX decisions.
7. Build API Contract Gate from accepted semantics.
8. Build persistence design from accepted domain/data/security semantics.
9. Authorize implementation only after those gates pass.

A rejection or material architecture change requires impact review rather than silently editing downstream artifacts.

### E. Decision Integrity Rule

No OPEN item may become ACCEPTED through implementation convenience, code structure, database schema, API behavior, provider choice, or framework defaults.

If a decision changes after implementation begins:

**New Evidence → Impact Analysis → Change Proposal → Decision → Affected Artifacts → Implementation → Verification**

**Current closure status:** **ARCHITECTURE ACCEPTED / DOWNSTREAM SECURITY + DATA + API GATES OPEN**
