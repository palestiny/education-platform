# Stage B — API Integration Gap Review

**Project:** Education Platform  
**Status:** REVIEW EVIDENCE — NO PRODUCTION INTEGRATION AUTHORIZED  
**Date:** 2026-10-16  
**Reviewed against:** `chore/architecture-gate-preparation` API source and PR #4 contract orchestration  
**Purpose:** Record what the current API actually does and what remains unproven before connecting the new identity resolver.

## 1. Verified current API behavior

### Request execution context

`Program.cs` registers `TestBearerExecutionContextResolver` only in Development/Testing and `FailClosedExecutionContextResolver` otherwise. The middleware invokes `IExecutionContextResolver.ResolveAsync` for each request and stores a non-null result in `RequestExecutionContextAccessor`.

The production resolver currently returns no execution context, even if an Authorization header contains a known test fixture bearer value. This is a fail-closed default, not a production sign-in implementation.

### Assignment creation

`POST /api/v1/learning-contexts/{contextId}/assignments` requires a context, checks the `assignment:create` authority, then restricts this first slice to `context-a` and `tenant-a`. The request body includes `learnerId`; this endpoint does not currently invoke the new `IdentityExecutionContextResolver` or an approved durable membership/policy source.

### Learner submission

`POST /api/v1/assignments/{assignmentId}/submissions` requires a context and `submission:create`, loads the assignment using the resolved tenant, verifies tenant equality and that the principal matches the assignment learner, then calls the application service. These are useful first-slice checks, but the execution context is not yet backed by a real identity provider or durable membership resolver in production.

### Assignment close

`POST /api/v1/assignments/{assignmentId}/close` checks `assignment:close` and delegates to `CloseAssignmentAsync`. The separate `IAssignmentCloseAuthorizer` uses `TestFixtureAssignmentCloseAuthorizer` in Development/Testing and `FailClosedAssignmentCloseAuthorizer` in other environments. The production close authorizer returns `Indeterminate`; this is intentionally safe until a real membership/resource-policy adapter is approved.

## 2. What PR #4 adds — and does not add

PR #4 introduces provider-neutral orchestration and deterministic Application tests for the sequence:

`ValidatedExternalIdentity → Person mapping → lifecycle → contextual membership → resource/action policy → ExecutionContext`

It does not register this resolver in the API dependency-injection graph, validate credentials, select a provider, persist external-identity mappings or memberships, or replace the assignment-close authorization boundary. Therefore PR #4 passing its Application tests is not evidence of end-to-end API identity integration.

## 3. Integration gaps to close in order

1. **Identity provider and credential validation:** choose and validate the production provider, issuer/audience/signature/expiry rules, web/mobile flow, and safe authentication error mapping.
2. **Platform person mapping:** approve stable internal Person identity and account-linking/onboarding rules; unknown identities must not create accounts implicitly.
3. **Lifecycle source and revocation freshness:** define the authoritative source for active/disabled/revoked status and a measurable maximum stale-authorization window.
4. **Membership/context source:** design persistence and isolation, eligible membership rules per operation, and how users with multiple eligible contexts select one without client-controlled tenant authority.
5. **Policy integration:** map each endpoint action/resource/context into the approved policy. A broad authority string alone is not a substitute for contextual resource authorization.
6. **Mutation and idempotency boundary:** prove authorization is re-evaluated before mutation and before returning protected replay data; explicitly resolve the authorization/mutation race with membership revocation.
7. **API integration tests:** test missing/invalid credentials, authenticated-but-denied, wrong tenant, wrong learning context, learner mismatch, relationship without permission, indeterminate dependencies, revocation, and no mutation on every denied path.
8. **Audit and operations:** verify actor, tenant, context, action, correlation, outcome, and failure observability without logging credential material.

## 4. Required acceptance evidence

Do not declare production identity/authorization complete until CI proves, on the exact candidate commit:

- a real provider adapter validates credentials using approved configuration;
- only server-resolved person, tenant, membership, and context values enter the trusted execution context;
- assignment create, learner submit, and assignment close are each authorized against their specific resource/action and contextual membership;
- denied/indeterminate outcomes cause no mutation or protected idempotency replay;
- cross-tenant and unauthorized relationship access are rejected;
- lifecycle revocation behavior meets the approved freshness requirement;
- durable integration tests prove tenant isolation and auditability;
- the full build, tests, EF migration validation, and migration-script generation pass.

## 5. Gate conclusion

**Current API integration: NOT COMPLETE.**  
**PR #4 contract orchestration: verified by Application tests only; not end-to-end identity integration.**  
**Production default: keep fail-closed.**  
**Next authorized engineering step:** close provider, lifecycle, membership, policy, and revocation decisions, then write failing API integration tests before connecting a production adapter.

No PR is merged by this review.
