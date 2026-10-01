# First Slice Durable Persistence Boundary

**Date:** 2026-10-01  
**Status:** IMPLEMENTATION IN PROGRESS

## Decision

The first slice will use a dedicated EducationPlatform.Infrastructure project for durable PostgreSQL persistence. Domain and Application remain persistence-provider independent; the API composes the infrastructure implementation.

The PostgreSQL provider is Npgsql through EF Core 10. The repository currently targets .NET 10, and the selected stable package lines are EF Core 10.0.12 and Npgsql EF Core provider 10.0.3.

## Boundary

- Domain: authoritative business objects and invariants.
- Application: use cases and store/module contracts.
- Infrastructure: EF Core mappings, PostgreSQL persistence, transactions, idempotency, audit and outbox persistence.
- API: transport/authentication composition only.

## First durable records

The initial persistence implementation starts with:
- assignments
- submissions

Reliability records are intentionally the next persistence increment:
- idempotency records
- audit records
- outbox messages

## Rules

1. No application service depends directly on EF Core.
2. Tenant ownership remains explicit on authoritative records.
3. Assignment version is mapped as an optimistic concurrency token.
4. Idempotency must become durable and unique at the database boundary before the in-memory adapter is removed.
5. Audit and outbox writes must share the authoritative transaction where required.
6. EnsureCreated is not the production migration strategy.
7. Integration tests must exercise PostgreSQL behavior rather than replacing the provider with an in-memory database.

## Verification boundary

The current Infrastructure project and DbContext establish the persistence boundary only. They do not constitute durable first-slice completion. The API still uses the temporary in-memory adapter until the transactional PostgreSQL adapter and failure-mode tests are proven in CI.
