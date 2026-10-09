# Implementation Readiness Matrix — First Technical Slice

**Date:** 2026-10-09  
**Status:** FIRST TECHNICAL SLICE VERIFIED; BROADER LEARNING JOURNEY INCOMPLETE  
**Implementation authorization:** Applies only to the existing first technical slice. It does not authorize production identity-provider integration or the still-unimplemented contextual membership/policy checks.

## 1. Purpose

Track which accepted Product, Domain, UX, Architecture, Security, Data and API obligations have evidence in the existing first technical slice, while keeping broader journey and production-readiness gaps visible. This matrix is not a blanket authorization for all future implementation.

## 2. First Technical Slice and broader target

**Verified technical slice:** authenticated request context → assignment creation/closure → learner submission, backed by PostgreSQL persistence and the currently implemented API contract.

**Broader product journey (not yet complete):** Authorized Context → Goal/Assignment → Learner Action/Submission → Assessment Result/Evidence → Teacher Decision → Next Action → Follow-up → New Evidence → Outcome.

The broader journey must not invent unresolved mastery, consent, retention, tenant-isolation mechanics, or provider decisions.

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

## 5. Current implementation status

Evidence supports the following within the **existing first technical slice**:

1. Durable PostgreSQL schema and mappings: implemented and CI verified.
2. Migration/model alignment: CI verified for the current test environment.
3. Authentication/application context boundary: implemented and CI verified for the current host/test resolver boundary; this is not production credential verification.
4. Server-derived tenant enforcement: implemented and CI verified for current API scenarios.
5. Logical tenant isolation with defense-in-depth: accepted for the current phase.
6. Idempotency, expected-version concurrency, and transactional audit/outbox behavior: covered by the existing regression suite; preserve these guarantees in future authorization work.

The following remain open or incomplete:

- Production identity-provider selection and real credential-validation adapter.
- Durable Person and contextual membership resolution.
- Production resource/action authorization backed by durable membership and policy sources remains open. A provider-neutral assignment-close authorization port and deterministic Development/Testing fixture are now enforced by the Application service before mutation or idempotency replay; API integration tests cover the fixture, while direct service tests cover denial/no mutation and re-authorization before replay. This is not production membership/policy integration.
- Membership eligibility rules for assignment creation and learner submission.
- Physical tenant isolation.
- Jurisdiction-specific consent, age and privacy rules.
- Final retention periods and deployment configuration.
- Universal progress/mastery algorithm.
- Expanded evidence taxonomy/storage mechanics and broader outcome-authority implementation.

These must remain explicit gaps; passing the current first-slice suite does not prove them implemented.

## 6. First-Slice Design and Verification Obligations

The initial first-slice design and implementation have already been carried out. Do not treat this historical list as permission to restart or silently broaden that work. For new behavior, first define the accepted contract and verification obligations:

1. Define persistence ownership and aggregate/transaction boundaries.
2. Define module contracts and command/query ownership.
3. Define authorization policy inputs and enforcement points.
4. Define exact API request/response/error schemas.
5. Preserve idempotency and concurrency semantics.
6. Preserve audit/outbox obligations.
7. Separate unit/contract tests from API and persistence integration tests.
8. Review new behavior against accepted gates before GREEN implementation.

## 7. TDD Entry Rule

For each new behavior, implement only after its contract and required authorization are explicit:

**RED → GREEN → REFACTOR → VERIFY**

RED must represent an intentional behavioral failure, not a broken test harness. Do not leave the default PR/branch knowingly red without an agreed test-branch/CI strategy.

## 8. Gate Status

| Gate | Status |
|---|---|
| Product | Foundation accepted for downstream design |
| Research | Market-led baseline established; targeted validation remains optional |
| Requirements | Ready for implementation mapping |
| Domain | PASS for current first-slice scope |
| UX | Foundation accepted; broader journey not verified end-to-end |
| Architecture | PASS for current modular-monolith boundary |
| Security baseline | PASS for accepted baseline decisions |
| Current API contract | PASS for currently tested endpoints/scenarios |
| First technical slice | **PASS — CI VERIFIED for the tested commit** |
| Production identity/authentication | **OPEN — resolver seam CI-verified; latest verified code head `7c02b119c9b79f1e58f2f627bcb9edabf4c7bd39` passed run #551, but provider selection and real credential validation remain open** |
| Assignment-close authorization contract | **APPLICATION ENFORCEMENT IMPLEMENTED — deterministic fixture only; assignment-close scenarios passed runs #479/#480; latest verified code head `7c02b119c9b79f1e58f2f627bcb9edabf4c7bd39` passed run #551; production membership/policy integration remains open** |
| Production contextual membership/resource policy | **OPEN — durable membership/policy resolution not implemented** |
| Broader learning journey | **INCOMPLETE — not proven end-to-end** |
| Release/deployment | **NOT READY — production identity, privacy and deployment gates remain** |

## 9. Recommendation

Keep the verified first technical slice stable and preserve its regression guarantees. Resource-scoped fixture and regression-test commit `bf70d7b544078f9d6c4179f71894834974d78cb5` passed CI (run #474). Application-use-case enforcement, denial/no-mutation, durable idempotency/audit/outbox non-mutation on denial, replay authorization re-checks against PostgreSQL, exact-resource grant matching, indeterminate fail-closed behavior, cross-tenant rejection, and fail-closed authorizer cancellation propagation are covered by the verified suite. Latest exact-head code run #551 passed on `7c02b119c9b79f1e58f2f627bcb9edabf4c7bd39`: https://github.com/palestiny/education-platform/actions/runs/37926998814. Documentation has since been refreshed in commits `0e17129d3d16a4435d70baa0f464631de7b5a278` and `67413e05a3d5c049ada68e7c2829cf40c4b56489`; exact-head CI for those documentation commits is pending. Provider integration, durable membership/policy resolution, revocation freshness, and production implementation remain open; fixture behavior is not evidence of production authorization.

PR #2 remains a reviewable, unmerged change set; a successful CI run verifies only the tested code and scenarios, not the open gaps listed above.
