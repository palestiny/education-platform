# Stage B — Provider, Membership & Revocation Decision Brief

**Status:** DECISION BRIEF — OWNER APPROVAL REQUIRED  
**Date:** 2026-10-16  
**Scope:** Decision preparation only; no provider or production behavior is selected by this document.

## 1. Current verified baseline

- The provider-neutral resolver and its Application tests pass on PR #4 head `de50eb0b9dad3f5dc536dde8d389e1a8896a3e7c`.
- GitHub Actions run `38947654272` completed successfully: build, EF migration validation, migration-script generation/upload, and 74 tests passed.
- The resolver is not wired into API dependency injection. Production authentication remains fail-closed.
- This brief does not authorize a provider integration or a merge.

## 2. Decision A — identity service operating model

| Option | Advantages | Costs / risks | Best fit |
|---|---|---|---|
| Managed OIDC/OAuth identity service | Less identity infrastructure to operate; provider handles credential security and protocol details; faster initial delivery | Recurring cost, vendor dependency, plan/region limits, migration and export constraints must be checked | Small team prioritizing delivery and operational simplicity |
| Self-hosted identity service (for example, a standards-based OIDC server) | More control over deployment and data location; potential flexibility | Team owns upgrades, availability, backups, incident response, key rotation, hardening and monitoring | Team with explicit operational capacity and hosting requirements |
| Build authentication in the product | Maximum implementation control | Highest security and maintenance burden; easy to get recovery, sessions, MFA, abuse controls and token lifecycle wrong | Not recommended for this project without a compelling requirement |

### Recommendation to evaluate, not a final vendor decision

Prefer a standards-based OIDC/OAuth boundary and keep provider SDKs in the API/Infrastructure layer. Compare at least two managed services against one self-hosted option using current prices, supported regions, social/email login, MFA, account recovery, mobile/web support, organization support, export/deletion, SLA, and migration cost. Do not select a vendor from feature marketing alone.

## 3. Decision B — platform person and external identity mapping

Recommended invariant:
- Internal `PersonId` is stable and provider-neutral.
- External identity is uniquely identified by the pair `(Issuer, Subject)`; email alone is not a safe permanent identity key.
- One person may have multiple external login identities only through an explicit, authenticated account-linking flow.
- Unknown external identities do not implicitly create tenant membership, role, guardian relationship, or privileged account.
- Account creation, invitations, recovery, and account linking are separate product flows with audit events and abuse protections.

Owner decisions needed:
1. Is initial registration open, invitation-only, or mixed by role/organization?
2. Can a person join multiple schools/centers/independent learning spaces?
3. Who may invite a teacher, learner, guardian, or administrator, and what evidence is required?
4. Is social login required for the first pilot, or can email verification be sufficient?

## 4. Decision C — membership and context selection

Recommended invariants:
- Membership belongs to a person and a specific tenant/organization; learning-context access is scoped separately where required.
- A user with one eligible context may use it only after the server validates it for the requested action/resource.
- A user with multiple eligible contexts must select a context through a server-validated flow; the client selection is a request, not authority.
- No membership, ambiguous membership, inactive membership, or unavailable membership data fails closed.
- Platform-owner access is separate from tenant administrator access; tenant administration never implies global platform access.

Before schema design, define eligible membership rules for these operations individually:
- teacher creates/edits/closes an assignment;
- learner views/submits work;
- guardian views learner progress;
- teacher views learner records;
- organization admin manages members and learning contexts;
- platform operator handles support incidents.

## 5. Decision D — lifecycle and revocation freshness

Possible operating models:

| Option | Behavior | Trade-off |
|---|---|---|
| Resolve membership/policy on every protected request | Fastest revocation propagation if the source is current | More database/policy traffic; dependency outages must fail closed |
| Short-lived cached membership/policy | Lower load and latency | Revocation may remain stale for the cache TTL plus propagation time |
| Token-embedded roles/memberships | Fast local checks | Stale privileges until token expiry; unsuitable as sole source for sensitive contextual authorization |

### Recommendation

Do not put durable tenant memberships or sensitive contextual permissions solely in long-lived identity tokens. Resolve them from an authoritative platform source, with caching only after a maximum revocation-staleness objective is agreed and tested. For the first production-capable slice, prefer no membership cache unless measured performance requires one.

Owner must approve a measurable maximum revocation window (for example, immediate next-request enforcement versus a bounded number of seconds) before implementation; the example is illustrative, not a selected SLA.

## 6. Decision E — policy and relationship authorization

Use contextual policy inputs rather than global role-only checks:
`principal + tenant + membership + learning context + relationship + resource + action`.

Examples:
- A guardian relationship is evidence for a policy decision, not automatic unrestricted access.
- A teacher role does not permit actions in every class or center.
- A tenant administrator cannot administer another tenant or platform-wide accounts by implication.
- Learner submission must bind the authenticated person to the learner identity expected by the assignment.

Policies should return explicit Allowed, Denied, or Indeterminate. Indeterminate must never be treated as allow.

## 7. Decision F — authorization versus mutation

A revocation can race with a protected write. Before declaring this boundary production-ready, choose and test a consistency model:
- re-check authorization immediately before mutation;
- use a transaction/locking/version strategy where membership and protected mutation share a database;
- or document the bounded race and enforce the approved revocation objective.

The correct mechanism depends on where membership and learning data are stored. Do not claim atomicity if they live in different systems.

Protected idempotency replay must re-check current authorization before returning any sensitive replay result.

## 8. Proposed implementation sequence after owner decisions

1. Approve the provider operating model and choose a short-listed provider using current commercial/security evidence.
2. Approve person identity, onboarding, invitation, account-linking and recovery rules.
3. Approve membership/context model and operation-specific eligibility predicates.
4. Approve revocation freshness objective and authorization/mutation consistency model.
5. Write API-level RED tests for invalid credentials, no membership, multiple contexts, cross-tenant access, guardian relationship without permission, revocation, indeterminate dependencies, and denied idempotency replay.
6. Implement the provider adapter and durable sources behind the existing provider-neutral ports.
7. Verify the exact candidate commit in CI and keep Production fail-closed until all acceptance criteria pass.

## 9. Explicit non-decisions

This document does not choose a specific identity vendor, database schema, token lifetime, cache TTL, legal consent rule, guardian verification procedure, or production deployment. Those decisions require owner approval and, where applicable, legal/privacy review.

**Gate remains OPEN.** Passing the provider-neutral orchestration tests is not production identity readiness.
