# Production Identity Provider Strategy Gate

**Project:** Education Platform  
**Status:** DECISION PROPOSAL — IMPLEMENTATION NOT AUTHORIZED  
**Date:** 2026-10-07

## 1. Purpose

Select the production identity-provider strategy without coupling the Education Platform application/domain to a provider SDK, token type, claim shape, or provider-specific user model.

This gate follows the closed **Production Identity & Authorization Design Gate**.

## 2. Product requirements

The identity strategy must support:
- global B2C + B2B usage;
- students, parents/guardians, teachers, TAs, organizations and platform operators;
- contextual organization membership and multiple organizations per person;
- enterprise SSO for schools/organizations;
- consumer-friendly sign-up/sign-in;
- MFA and phishing-resistant authentication;
- account recovery and lifecycle controls;
- web and future mobile clients;
- provider-neutral application authorization;
- auditability without persisting raw credentials;
- practical .NET/ASP.NET Core integration;
- a credible migration path if the provider is replaced.

## 3. Non-negotiable architectural rule

The provider is an **identity infrastructure dependency**, not the application's identity/domain model.

Production flow remains:

**Provider Authentication → Provider Adapter → Platform Identity Mapping → ExecutionContext → Membership/Authorization Policy → Use Case**

The application must not consume provider SDK objects, provider-specific claims, refresh tokens, raw access tokens, or provider user IDs as domain concepts.

The platform owns its stable internal Person identity mapping and organization memberships.

## 4. Strategy options

### Option A — Managed CIAM: Microsoft Entra External ID

**Current recommendation: preferred candidate.**

Strengths:
- Explicit CIAM support for consumer and business customers.
- OIDC/SAML application integration.
- Social federation including Google, Apple and Facebook.
- Custom OIDC and SAML/WS-Fed federation.
- Email/password and email OTP flows.
- MFA and passkeys.
- Strong fit with the existing ASP.NET Core/.NET direction.
- Enterprise identity federation is important for future schools and organizations.

Current Microsoft documentation confirms External ID is designed for consumer and business customer applications and supports customized sign-up/sign-in, federation, OIDC/SAML integration, and passkey/MFA capabilities. citeturn0search3turn0search5turn0search8

Trade-offs:
- Microsoft platform dependency.
- Exact commercial cost at projected scale must be validated before production commitment.
- Authentication-mode differences exist between native and browser-delegated flows and matter for future mobile UX. citeturn0search9
- Provider-specific lifecycle/tenant semantics must remain outside the application domain.

### Option B — Auth0 / Okta Customer Identity

Strengths:
- Mature CIAM positioning.
- Strong ecosystem around OIDC/OAuth, SAML, MFA, passkeys and enterprise federation.
- Good fit for B2C/B2B SaaS patterns.

Trade-offs:
- Commercial cost and pricing complexity must be modeled at projected scale.
- Additional vendor dependency.
- Organization/authorization capabilities must be deliberately mapped to the platform's own membership model.

### Option C — Amazon Cognito

Strengths:
- Managed AWS-native identity service.
- Appropriate for applications already deeply committed to AWS.
- Standard federation patterns.

Trade-offs:
- AWS coupling.
- Product/UX and organization-management requirements need more application-owned work.
- Less attractive if the platform is intended to remain cloud-provider neutral.

### Option D — Keycloak / self-hosted identity

Strengths:
- Maximum control and portability.
- Open-source and provider-independent.
- Strong standards support and extensibility.
- Can be operated in the platform's own infrastructure.

Trade-offs:
- We would own HA, upgrades, security operations, abuse protection, recovery, incident response and operational reliability.
- Identity infrastructure becomes a substantial platform responsibility.
- This conflicts with the current priority of spending engineering capacity on education-domain value rather than rebuilding commodity identity infrastructure.

Keycloak continues to add enterprise identity capabilities, but self-hosting transfers the operational burden to us. citeturn0search12

## 5. Decision matrix

| Criterion | Entra External ID | Auth0/Okta CIAM | Cognito | Keycloak |
|---|---|---|---|---|
| B2C | Strong | Strong | Strong | Strong |
| B2B / enterprise federation | Strong | Strong | Good | Strong |
| OIDC/SAML | Strong | Strong | Strong | Strong |
| Social login | Strong | Strong | Strong | Strong |
| MFA/passkeys | Strong | Strong | Validate exact roadmap/UX | Strong |
| .NET integration | Excellent | Excellent | Good | Good |
| Mobile path | Good, validate chosen flow | Strong | Strong | Good |
| Operational ownership | Low | Low | Low | High |
| Cloud portability | Medium | High | Low | High |
| Lock-in risk | Medium | Medium | High | Low |
| Engineering burden | Low/Medium | Low/Medium | Medium | High |
| Fit for current architecture | **High** | **High** | Medium | Medium |

This table is an architectural assessment, not a pricing benchmark.

## 6. Recommended strategy

Adopt:

> **Managed CIAM with a provider-neutral adapter, with Microsoft Entra External ID as the preferred production provider candidate.**

This is intentionally two-layered:

1. **Architecture decision:** managed CIAM + provider-neutral adapter.
2. **Provider candidate:** Microsoft Entra External ID.

The first is the durable architectural decision. The second remains subject to final commercial and operational validation before production credentials and deployment configuration are introduced.

## 7. Why this is the preferred direction

The Education Platform is not an identity product. Its differentiation is the educational operating model, evidence/progress semantics, relationships, workflows, safety and AI-assisted learning operations.

Therefore we should buy commodity identity infrastructure unless a concrete requirement proves that self-hosting creates sufficient strategic value.

Entra External ID currently aligns particularly well with the planned B2C+B2B shape and future enterprise federation while preserving a clean provider boundary. Its current documentation explicitly covers consumer/business CIAM, OIDC/SAML federation, social identity providers, MFA and passkeys. citeturn0search3turn0search5

## 8. Required final validation before provider commitment

Before production provider implementation is authorized, verify:

1. projected monthly active-user cost at MVP, 100k MAU and 1M MAU;
2. Egypt/global availability and relevant data-residency constraints;
3. terms and privacy implications for minors/guardian accounts;
4. organization onboarding and enterprise SSO workflow;
5. account deletion/export requirements;
6. disabled/revoked-user propagation;
7. recovery and support/admin-access model;
8. web + Android + iOS authentication flow;
9. passkey UX and fallback behavior;
10. provider outage/degraded-mode behavior;
11. export/migration feasibility;
12. audit and security-event availability;
13. required custom domains/branding;
14. rate limits and administrative API limits.

No production provider contract should be considered closed until these checks are documented.

## 9. Provider adapter contract

The future adapter should expose only provider-neutral operations such as:
- authenticate request/session;
- validate credential/token according to provider configuration;
- resolve external identity;
- map external identity to platform Person;
- establish tenant/membership context;
- detect disabled/revoked identity;
- expose safe authentication outcome;
- never expose raw provider credential material to Application/Domain.

## 10. Explicitly deferred

This gate does not decide:
- exact token/session implementation;
- refresh-token storage;
- MFA policy by role;
- passkey enrollment UX;
- guardian/child account policy;
- age verification;
- consent/jurisdiction rules;
- organization invitation workflows;
- emergency/support access;
- exact provider pricing tier;
- production secrets/configuration.

## 11. Gate result

**Production Identity Provider Strategy: CONDITIONAL PASS**

- Architecture strategy: **ACCEPTED — managed CIAM + provider-neutral adapter**
- Preferred provider candidate: **Microsoft Entra External ID**
- Final provider commitment: **PENDING validation items in Section 8**
- Production implementation authorization: **NOT YET GRANTED**
- Next engineering step: write executable RED tests against the provider-neutral adapter contract; provider SDK integration remains outside Application/Domain boundaries.

## 12. Evidence

Current Microsoft documentation:
- External ID CIAM overview and consumer/business customer positioning. citeturn0search3
- Supported identity providers and federation options. citeturn0search5
- OIDC/SAML and application integration guidance. citeturn0search8
- Passkey support. citeturn0search0
