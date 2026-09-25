# Architecture Gate Inputs

Date: 2026-09-25
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

The Product Foundation and Domain Gates remain **NOT PROVEN** because direct workflow evidence from real participants has not yet been collected.

Therefore architecture work may define constraints and evaluate options, but must not convert unvalidated product hypotheses into implementation commitments.

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
- initial segment is selected;
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

The immediate product blocker remains direct validation of real recent workflows and initial-segment selection.

Next:
1. resolve segment-dependent requirements where evidence permits;
2. collect direct workflow cases;
3. refine domain ownership/state rules;
4. evaluate architecture options against the selected workflow;
5. create an ADR only after an explicit architecture decision.
