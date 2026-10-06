# Authentication & Tenant Context Boundary

## Purpose

Keep HTTP authentication provider-neutral at the application boundary while preventing request-supplied tenant identifiers from becoming authorization authority.

## Boundary

`HTTP Authentication → Application Execution Context → Tenant Context → Authorization Authority → Use Case`

The application consumes a provider-neutral `ExecutionContext` containing:

- `PrincipalId`: authenticated actor identity;
- `TenantId`: server-derived tenant context;
- `Authorities`: explicit operation authorities.

The application does not consume bearer-token strings, HTTP headers, or provider-specific identity objects.

## Rules

1. The request cannot choose its authoritative tenant.
2. Tenant context comes from the authenticated principal/membership boundary.
3. Resource tenant must match execution tenant before mutation/read-through.
4. Authorization checks operation authorities, not token names.
5. Audit actor and tenant values come from the execution context.
6. The current bearer resolver is test/development scaffolding only.
7. Production authentication remains provider-neutral and requires a real IdP adapter before production authorization is enabled.

## Current adapter

For Development/Testing, `TestBearerExecutionContextResolver` maps deterministic test credentials into execution contexts.

It is explicitly disabled outside Development/Testing. Therefore the current production posture is fail-closed rather than accepting test credentials as real authentication.

## Deferred production boundary

A future production adapter may translate an OIDC/OAuth2/JWT or enterprise identity provider into the same `ExecutionContext` contract. That provider choice is intentionally deferred and must not change application use cases.

## Verification target

The boundary is considered implemented only when CI verifies:

- authenticated principal identity reaches application mutations;
- tenant context is server-derived;
- unauthorized authorities receive 403;
- cross-tenant resource access is rejected;
- test authentication is unavailable in Production configuration;
- existing idempotency, audit, outbox, and concurrency behavior remains green.

## Status

**Implemented; CI verification pending for the current change set.**

This document does not authorize a production identity provider or claim production authentication readiness.
