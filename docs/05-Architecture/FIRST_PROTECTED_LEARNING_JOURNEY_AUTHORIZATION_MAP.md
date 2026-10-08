# First Protected Learning Journey — Authorization Map

**Project:** Education Platform  
**Status:** DESIGN REVIEW ARTIFACT — NO POLICY OR IMPLEMENTATION AUTHORIZED  
**Date:** 2026-10-09  
**Branch:** `chore/architecture-gate-preparation`  
**Related:** `STAGE_B_IDENTITY_MEMBERSHIP_CONTRACT_TEST_PLAN.md`, `PROVIDER_NEUTRAL_IDENTITY_MEMBERSHIP_CONTRACT_PROPOSAL.md`

## 1. Journey under review

The smallest current protected journey is:

1. A teacher creates an assignment inside a learning context.
2. The assigned learner submits work.
3. An authorized teacher closes the assignment.

This is a useful first journey because it crosses context selection, tenant isolation, actor-to-resource authorization, and a state-changing operation. It is not a complete school/guardian policy model.

## 2. Current API behavior observed in source

| Operation | Current checks | Explicitly not established |
|---|---|---|
| Create assignment: `POST /api/v1/learning-contexts/{contextId}/assignments` | A trusted execution context exists; `assignment:create` authority; context ID is exactly `context-a`; tenant is exactly `tenant-a`. | A persisted membership grants this teacher access to this context; the learner belongs to that context; a server-side resolver selected the context. |
| Submit: `POST /api/v1/assignments/{assignmentId}/submissions` | A trusted execution context exists; `submission:create` authority; assignment exists in the same tenant; actor principal ID equals the assignment learner ID. | The learner has an active membership in the assignment's learning context; learner lifecycle/revocation is checked by a membership service. |
| Close: `POST /api/v1/assignments/{assignmentId}/close` | A trusted execution context exists; `assignment:close` authority; assignment exists in the same tenant. | Assignment ownership, context membership, organization-level policy, or resource-specific permission is established. |

The development/testing bearer resolver supplies trusted context fixtures; this is not production identity or persisted membership resolution. The hard-coded context/tenant boundary is a deliberate first-slice restriction, not a substitute for the future membership resolver.

## 3. Contract outcomes needed before runtime implementation

| Decision point | Safe outcome to specify | Evidence required later |
|---|---|---|
| External identity cannot be mapped to a platform Person | No trusted execution context; no implicit account or membership creation | Provider-neutral contract test; later real-provider integration test |
| Person is disabled or revoked | Deny before protected operation | Lifecycle contract test and an approved revocation freshness budget |
| No eligible membership for the requested context/action | Deny; do not infer membership from a client-supplied context ID | Resolver contract test plus API integration test |
| Exactly one eligible membership | Server selects it and establishes a bounded trusted context | Contract test proving client tenant/context input cannot override resolution |
| More than one eligible membership | Return an explicit ambiguous outcome; no first/default membership | Contract test; selection UX and binding semantics remain product decisions |
| Membership exists but action/resource policy denies | Deny; membership/relationship is not a universal grant | Policy contract test for the approved first-slice rules |
| Resolver unavailable or state is indeterminate | Fail closed; no test-credential or anonymous fallback | Failure contract test and API-level enforcement test |
| Teacher closes an assignment in the same tenant | Outcome depends on the still-unapproved close policy below | Regression tests only after owner accepts the policy |

## 4. Assignment-close policy options

### Option A — Tenant-wide authority is sufficient for this first slice
Any authenticated principal with the trusted `assignment:close` authority may close any assignment within the resolved tenant.

- **Benefit:** simple operational model; appropriate if closing is intentionally an organization-wide capability.
- **Cost/risk:** no assignment-owner or context-specific boundary; authority configuration becomes security-critical.
- **Required guardrail:** authority must be issued by trusted server-side policy, never accepted from client input or unvalidated external claims.

### Option B — Require membership in the assignment's learning context
The actor must have an eligible membership in the assignment's context and a policy grant for closing.

- **Benefit:** aligns access with the context where the assignment exists; supports multiple contexts and least privilege.
- **Cost/risk:** requires a defined membership model and resource/action policy; membership alone must not automatically imply close permission.

### Option C — Require assignment creator/owner plus policy override
The creator/owner may close the assignment; separately authorized context/tenant roles may override.

- **Benefit:** narrow default authority and an explicit administrative path.
- **Cost/risk:** requires ownership semantics, creator transfer/deletion rules, and override auditing. The current domain/store contracts do not establish these rules.

**Design recommendation, not an owner decision:** Option B is the stronger long-term default for a multi-tenant education platform, with any tenant-wide override explicitly modeled as a separate policy. Do not implement it until the owner confirms the first-slice rule and the model can represent it. Option C may be preferable if assignment ownership is a product requirement; that cannot be inferred from the current code.

## 5. Minimum acceptance decisions for Stage B

Before creating source contracts or executable contract RED tests, the owner/security review must decide:

1. Whether unknown identities are rejected and onboarded only through a separate invitation/provisioning flow.
2. Which of Options A/B/C governs assignment close in the first slice.
3. Whether the first slice requires teacher membership in the learning context at creation and close, and learner membership at submission.
4. How zero and multiple eligible memberships behave (recommended: deny / explicit ambiguity, respectively).
5. Which system is authoritative for Person disable/revocation and the maximum acceptable stale-access interval.
6. Whether the first slice has any guardian/student relationship behavior. If not, explicitly defer it rather than implying it is covered.

Provider choice, account linking, consent/legal policy, membership schema, physical tenant isolation, and production authentication remain separate decisions.

## 6. TDD sequence after decisions are accepted

1. Define provider-neutral outcome and policy contracts without provider SDK types or raw claims in Domain/Application.
2. Add deterministic contract tests for unknown/disabled identity, no/one/multiple memberships, policy denial, resolver outage, and trusted context establishment.
3. Run those tests and classify RED failures as behavior gaps rather than harness/build failures.
4. Add API integration tests proving the selected decisions are enforced on create, submit, and close endpoints.
5. Only after separate implementation authorization, implement the minimum GREEN behavior and verify at the exact PR head.

## 7. Current conclusion

- Current source enforces a test-context credential boundary, coarse authority checks, tenant equality, and learner-ID matching on submission.
- Current source does not establish persisted identity, membership resolution, or a resource-specific close policy.
- This document maps those gaps and compares close-policy options. It adds no source contracts, runtime behavior, provider, schema, ownership field, or accepted product policy.
- PR #2 must remain open and unmerged pending the normal owner decision and review.
