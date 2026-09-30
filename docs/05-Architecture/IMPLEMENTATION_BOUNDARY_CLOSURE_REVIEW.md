# Implementation Boundary Closure Review — First Technical Slice

**Date:** 2026-09-30  
**Status:** PROPOSED — IMPLEMENTATION GATE NOT PROVEN

## 1. Purpose

This document closes the three implementation-level blockers identified by the Implementation Gate Review without silently converting owner-level choices into accepted architecture.

The review is intentionally limited to the first technical slice:

**Learning Context → Assignment → Submission**

The broader learning chain remains unchanged:

**Authorized Context → Goal/Assignment → Learner Action/Submission → Assessment Result/Evidence → Teacher Decision → Next Action → Follow-up → New Evidence → Outcome**

No production implementation, migration or provider commitment is authorized by this document.

## 2. Closure Result

| Blocker | Current state | Proposed closure |
|---|---|---|
| Authentication / session boundary | BLOCKED | Provider-neutral application auth context + concrete host adapter |
| Tenant-context enforcement | BLOCKED | Server-derived tenant context enforced at application/data boundaries |
| Physical persistence mapping | NOT PROVEN | Narrow relational mapping for Context → Assignment → Submission |
| Migration strategy | NOT PROVEN | Versioned forward-compatible migrations with recovery discipline |
| Test harness | NOT PROVEN | Deterministic domain/application tests + real PostgreSQL integration boundary |

The first three items remain **PROPOSED until explicitly accepted**.

# 3. Authentication / Session Boundary

## 3.1 Required semantic boundary

Business modules must not depend directly on a specific identity provider.

The application receives an authenticated context containing, at minimum:

- Principal ID
- Authentication status
- Roles/claims relevant to authorization
- Organization/tenant memberships available to the principal
- Correlation ID
- Authentication/session assurance information where available

Authorization evaluates:

**Identity + Role + Relationship + Organization/Tenant + Policy/Consent + Learning Context + Action**

The domain does not parse JWTs, cookies, OAuth tokens or provider-specific objects.

## 3.2 Options

### Option A — ASP.NET Core host authentication + provider-neutral application context

The host authenticates the request and maps the result into an application-owned authentication context.

**Advantages**
- Keeps business/domain code provider-neutral.
- Fits the selected .NET backend direction.
- Allows development/test identities without changing application semantics.
- Leaves production identity-provider selection phaseable.

**Trade-offs**
- The production authentication provider still has to be selected later.
- Claims/role mapping must be deliberately specified.
- Session/token lifecycle remains infrastructure responsibility.

### Option B — ASP.NET Core Identity as the primary identity system

Use application-managed identity/account persistence and ASP.NET Core authentication mechanisms.

**Advantages**
- Strong application ownership of identity lifecycle.
- Fewer external dependencies for the core account model.
- Good control for a platform that may need complex role/relationship semantics.

**Trade-offs**
- More identity/security responsibility remains inside the product.
- External federation/social/enterprise identity can add complexity later.
- Identity infrastructure becomes part of the platform's operational surface.

### Option C — External OIDC/OAuth identity provider

Delegate authentication to a specialized identity provider while the platform owns application identity, membership and authorization semantics.

**Advantages**
- Strong separation between authentication and authorization.
- Easier future federation/enterprise identity scenarios.
- Provider can be changed behind an adapter if the contract remains stable.

**Trade-offs**
- External dependency and operational/vendor considerations.
- Provider selection and pricing become later decisions.
- Claims and lifecycle synchronization require explicit integration design.

## 3.3 Recommendation

**Adopt the boundary of Option A now, without selecting the production provider yet.**

That means:
- ASP.NET Core is the host authentication boundary.
- Application code consumes an internal authentication-context contract.
- Development/test environments use a deterministic test identity adapter.
- Production provider selection remains OPEN.
- Domain/application semantics never depend on provider-specific token/session objects.

**Decision status: PROPOSED — OWNER CONFIRMATION REQUIRED.**

# 4. Tenant Context Enforcement

## 4.1 Core rule

The client must never be trusted to establish tenant authority.

A request may contain a resource identifier, but authorization derives the effective tenant/organization context from authenticated membership and the protected resource/context.

A client-supplied tenant identifier may be treated as input for selection only where explicitly allowed; it is never proof of access.

## 4.2 Request flow

HTTP Request → Authentication → Principal → Tenant/Organization Membership Resolution → Resource/Learning Context Resolution → Relationship + Role + Policy Evaluation → Authorized Application Command/Query → Tenant-Scoped Persistence

## 4.3 Enforcement obligations

1. No protected command executes without an authenticated principal.
2. Tenant context is resolved server-side.
3. Membership is validated before tenant-scoped access.
4. Learning Context ownership/association is checked.
5. Cross-tenant resource access is rejected.
6. Repositories/data-access operations cannot silently execute outside the resolved tenant scope.
7. Audit records preserve tenant/context attribution.
8. Background/recovery operations carry explicit tenant context rather than inheriting ambient request state.

## 4.4 Physical isolation strategy

The first slice should implement **logical tenant isolation with defense-in-depth**.

Physical isolation options remain phaseable:
- shared database/shared schema with strict tenant keys and enforcement;
- shared database/separate schema;
- database-per-tenant for selected enterprise/regulatory cases;
- hybrid strategy later.

The application contract must not make migration between these strategies impossible.

## 4.5 Recommendation

Adopt **explicit logical tenant context + server-side authorization + persistence scoping**, with physical isolation remaining a later scale/regulatory decision.

Do not make a client header, route parameter or UI state the source of tenant authority.

**Decision status: PROPOSED — OWNER CONFIRMATION REQUIRED.**

# 5. First-Slice Physical Persistence Mapping

This mapping is intentionally narrow. It does not freeze the future platform database.

## 5.1 Minimum records

### Identity / organization boundary

**principals**
- id
- status
- created_at
- updated_at

**organization_memberships**
- id
- organization_id
- principal_id
- role/reference
- status
- version
- created_at
- updated_at

### Learning boundary

**learning_contexts**
- id
- organization_id / tenant_id
- learner_principal_id
- primary_teacher_principal_id where applicable
- status
- version
- created_at
- updated_at

**goals**
- id
- learning_context_id
- goal_reference/value
- status
- created_at
- updated_at

**assignments**
- id
- learning_context_id
- goal_id
- teacher_principal_id
- status
- work_payload
- version
- created_at
- updated_at

**submissions**
- id
- assignment_id
- learner_principal_id
- status
- submission_payload
- version
- submitted_at
- created_at
- updated_at

### Reliability / accountability boundary

**idempotency_records**
- id
- operation_scope
- actor_principal_id
- tenant/context scope
- idempotency_key
- request_fingerprint
- status
- resource_reference
- response_metadata
- created_at
- completed_at

**audit_records**
- id
- tenant/context scope
- actor_principal_id
- operation
- resource_reference
- outcome
- occurred_at
- safe_metadata

**outbox_messages**
- id
- tenant/context scope where applicable
- message_type
- aggregate/resource reference
- payload
- status
- attempts
- available_at
- created_at
- processed_at

## 5.2 Mandatory integrity direction

- Every tenant-scoped authoritative record carries an unambiguous tenant/organization ownership path.
- Foreign keys prevent references to nonexistent parent records.
- Assignment must belong to the same learning context as its goal.
- Submission must belong to its assignment and learner authorization scope.
- Assignment creation requires an authorized teacher context.
- Submission creation requires an authorized learner context.
- Accountable mutable records use an explicit version/concurrency mechanism.
- Idempotency uniqueness is scoped to the operation/actor/tenant context defined by the command contract.
- Historical attribution is preserved.
- No destructive update may erase authoritative evidence required by the accepted domain semantics.

## 5.3 Important boundary

The exact SQL types, index names, ORM mappings, database extensions and naming conventions remain implementation details to be reviewed with the chosen persistence stack.

# 6. Migration Strategy

## 6.1 Development

- Version-controlled migrations.
- Every schema change is reproducible from a clean database.
- Test database can be created from migrations.
- No manual production-like schema edits as a development shortcut.

## 6.2 Production direction

Prefer forward-compatible migrations:

Expand → Deploy compatible application → Backfill/migrate data → Switch behavior → Contract/remove obsolete structure later

A database rollback is not automatically the same thing as an application rollback.

## 6.3 Recovery

Migration recovery must distinguish:
- migration failure before data mutation;
- partial migration;
- successful migration with application incompatibility;
- data backfill failure;
- deployment rollback where schema cannot safely be rolled back.

## 6.4 First-slice requirement

Before the first production deployment, each migration must have deterministic ordering, tested application compatibility, failure visibility, recovery procedure and backup/restore validation appropriate to the environment.

**Decision status: PROPOSED — OWNER CONFIRMATION REQUIRED.**

# 7. Test Harness Strategy

The test architecture mirrors the accepted module boundaries.

## 7.1 Domain tests

Fast deterministic tests for assignment lifecycle, submission lifecycle, invalid transitions, business preconditions, version/concurrency behavior where domain-owned, and authorization-relevant domain invariants that do not require infrastructure.

## 7.2 Application tests

Verify authenticated principal propagation, tenant context, authorization decisions, idempotency, transaction orchestration, audit intent and cross-module contract behavior.

## 7.3 Persistence integration tests

Use a real PostgreSQL-compatible integration environment rather than an in-memory substitute for relational behavior that matters.

Verify PK/FK constraints, uniqueness, tenant scoping, concurrency, idempotency persistence, transactions, migrations and recovery/rebuild assumptions.

## 7.4 API contract tests

Verify request validation, response shape, error codes, authentication failures, authorization failures, tenant isolation, idempotency replay/conflict, concurrency conflict, correlation IDs and protected projections.

## 7.5 First RED suite

1. unauthenticated assignment creation is rejected;
2. authenticated but unauthorized teacher is rejected;
3. authorized teacher creates assignment;
4. same idempotency key + same request replays original result;
5. same idempotency key + different request returns conflict;
6. assignment cannot be created across tenant boundary;
7. authorized learner submits;
8. duplicate submission retry does not create a duplicate;
9. closed assignment rejects submission;
10. audit record exists for authoritative mutation.

No implementation code should be written until these RED tests are reviewed as the executable interpretation of the accepted contract.

# 8. Closure Matrix

| Area | Proposed state | Owner acceptance needed? |
|---|---|---|
| Auth application boundary | Provider-neutral application auth context | YES |
| Production identity provider | Deferred | YES, later |
| Tenant authority source | Server-derived membership/resource context | YES |
| Physical tenant isolation | Phaseable | YES, later |
| First-slice relational mapping | Proposed | YES |
| Migration strategy | Expand/compatible/contract | YES |
| Test database strategy | Real relational integration environment | YES |
| First RED suite | Proposed | YES before implementation |
| Mastery algorithm | Deferred | NO for first slice |
| Expanded evidence taxonomy | Deferred | NO for first slice |
| Advanced scheduling | Deferred | NO for first slice |
| Cloud provider | Deferred | NO for first slice |

# 9. Implementation Gate Reassessment Criteria

Implementation Gate can move from **NOT PROVEN** to **READY/PASS** only after:

- auth boundary is explicitly accepted;
- tenant-context enforcement is explicitly accepted;
- first-slice persistence mapping is explicitly accepted;
- migration strategy is accepted;
- test harness strategy is accepted;
- exact first RED suite is accepted;
- no unresolved semantic requirement is being hidden inside implementation details.

After that:

**RED → GREEN → REFACTOR → VERIFY**

No controllers, EF migrations, production UI, external identity provider or cloud provisioning should precede the gate closure.

# 10. Current Recommendation

The project is now at the point where the remaining blockers are **implementation-boundary choices**, not broad product-definition gaps.

The recommended path is:

1. accept the provider-neutral authentication boundary;
2. accept explicit server-derived tenant context enforcement;
3. accept the narrow first-slice relational mapping;
4. accept the migration/test strategy;
5. reassess Implementation Gate;
6. only then write the first RED tests.

**Current status: IMPLEMENTATION GATE NOT PROVEN.**

**No production implementation is authorized by this document.**