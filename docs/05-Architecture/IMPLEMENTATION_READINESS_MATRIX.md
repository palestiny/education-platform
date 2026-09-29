# Implementation Readiness Matrix — First Vertical Slice

**Date:** 2026-09-29  
**Status:** PROPOSED — IMPLEMENTATION GATE NOT PROVEN  
**Implementation authorization:** NOT AUTHORIZED

## 1. Purpose

Translate the accepted Product, Domain, UX, Architecture, Security, Data and API contracts into implementation obligations before production code begins.

## 2. First Vertical Slice

**Authorized Context → Goal/Assignment → Learner Action/Submission → Assessment Result/Evidence → Teacher Decision → Next Action → Follow-up → New Evidence → Outcome**

The slice must be complete enough to demonstrate the canonical journey, but it must not invent unresolved mastery, consent, retention, tenant-isolation mechanics, or provider decisions.

## 3. Traceability Matrix

| Area | Accepted source | Implementation obligation | Verification |
|---|---|---|---|
| Product journey | CORE_PRODUCT_JOURNEY | Preserve context across the complete slice | End-to-end acceptance |
| Domain | Domain Readiness | Preserve semantic distinctions and ownership | Domain tests |
| UX | UX Gate | Expose state/evidence/action/recovery without internal complexity | UX acceptance |
| Architecture | ADR-0001 | Respect module boundaries and contracts | Architecture review |
| Security | DEC-0013 | Enforce auth context at command/read boundaries | Security tests |
| Data | DEC-0014 | Persist authoritative facts; keep derived state rebuildable | Data/rebuild tests |
| API | DEC-0017 | Implement accepted /api/v1 semantics | Contract tests |
| Reliability | API/Data baseline | Idempotency, concurrency and reconciliation | Failure/retry tests |
| Audit | Security/Data/API | Audit accountable mutations and sensitive access | Audit tests |
| Observability | Architecture/API | Correlation IDs and actionable diagnostics | Observability verification |
| DoD | DEFINITION_OF_DONE | Satisfy feature completion checklist | Gate review |

## 4. Module Ownership — Candidate Implementation Boundary

- Identity & Access — identity and authentication context.
- Organizations & Relationships — tenant/org context and relationship lifecycle.
- Authorization — policy evaluation and command/read authorization.
- Learning — learning context, goals, assignments, submissions.
- Assessment — assessment definitions, attempts and results.
- Evidence — attributable evidence, provenance, correction/supersession and conflict.
- Learner State/Progress — derived/rebuildable progress projections.
- Follow-up — explicitly created unresolved work and lifecycle.
- Communication/Notifications — contextual communication and delivery state.
- Audit/Observability — accountability records and operational diagnostics.
- AI Assistance — bounded assistance only; no authoritative learner state.
- Integrations — isolated external provider boundaries.

## 5. Implementation Gate Blocking Conditions

The following must remain blocking until explicitly resolved:

1. DB schema is not yet designed.
2. Exact persistence mappings are not yet approved.
3. Exact authentication/session mechanism is open.
4. Physical tenant-isolation mechanism is open.
5. Jurisdiction-specific consent/age/privacy rules are open.
6. Exact retention periods are open.
7. Cloud/provider selection is open.
8. Exact progress/mastery algorithm is open.
9. Exact evidence taxonomy/storage mechanics are open.
10. Exact outcome authority implementation is open.

These are downstream implementation decisions, not reasons to redesign the accepted semantics.

## 6. First Implementation Design Tasks

Before coding the vertical slice:

1. Define persistence model for authoritative records.
2. Define aggregate/transaction boundaries.
3. Define module contracts and command/query ownership.
4. Define authorization policy inputs and enforcement points.
5. Define exact API request/response/error schemas.
6. Define idempotency and concurrency persistence mechanics.
7. Define audit/outbox obligations.
8. Define test strategy and contract-test cases.
9. Define observability signals.
10. Review the resulting design against the accepted gates.

## 7. TDD Entry Rule

No production implementation begins until the implementation design for the first slice is accepted.

Then:

**RED → GREEN → REFACTOR → VERIFY**

The first tests should express accepted business semantics, not framework mechanics.

## 8. Gate Status

| Gate | Status |
|---|---|
| Product | Foundation accepted for downstream design |
| Research | Market-led baseline established; targeted validation remains optional |
| Requirements | Ready for implementation mapping |
| Domain | PASS |
| UX | PASS |
| Architecture | PASS |
| Security | PASS |
| Data | PASS |
| API Contract | PASS |
| Implementation | **NOT PROVEN** |
| Testing | NOT OPEN |
| Release | NOT OPEN |
| Verification | NOT OPEN |

## 9. Recommendation

Open Implementation Gate preparation with **Persistence + Module Contract Design** first.

Do not start with controllers, EF migrations, UI pages, or infrastructure provisioning. Those artifacts would prematurely freeze unresolved implementation details.

**Next artifact:** docs/05-Architecture/FIRST_SLICE_IMPLEMENTATION_DESIGN.md
