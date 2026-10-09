# TDD RED Test Project Boundary

**Date:** 2026-10-01
**Status:** PROPOSED — PROJECT SKELETON DESIGN READY

## 1. Objective

Create the minimum .NET solution/test boundary required to execute the accepted RED suite without inventing production behavior.

## 2. Proposed solution boundary

```text
src/
  EducationPlatform.Api/
  EducationPlatform.Application/
  EducationPlatform.Domain/
  EducationPlatform.Infrastructure/

tests/
  EducationPlatform.UnitTests/
  EducationPlatform.ApplicationTests/
  EducationPlatform.IntegrationTests/
```

## 3. Responsibility

- Domain: pure business rules and lifecycle invariants.
- Application: commands, authorization inputs, idempotency orchestration and application contracts.
- Infrastructure: persistence and external adapters.
- API: HTTP authentication boundary and contract adapter.
- UnitTests: domain invariants without infrastructure.
- ApplicationTests: authorization/idempotency/application behavior.
- IntegrationTests: PostgreSQL-backed persistence and HTTP/application boundary where required.

## 4. Dependency direction

```text
Api → Application → Domain
          ↓
     Infrastructure

Tests reference the layer under test.
```

Domain must not depend on Infrastructure.
Application must not depend on a concrete production identity provider.

## 5. RED rule

The initial RED tests may reference interfaces/contracts that do not yet have implementations. They must not contain fake production implementations that satisfy the behavior.

## 6. First RED placement

- RED-001/002/003/004/005/006: ApplicationTests.
- RED-007/008/009: ApplicationTests, with persistence integration coverage where durable uniqueness/lifecycle behavior is required.
- RED-010: IntegrationTests because audit persistence is an authoritative accountability side effect.

## 7. Infrastructure constraint

No production database migration or cloud resource is required to create the RED test skeleton.
The integration harness may define its test-database contract, but implementation of database infrastructure belongs to GREEN.

## 8. Acceptance

This document intentionally does not choose package versions, database container technology, hosting provider, ORM, authentication provider, or production deployment topology.

**Next:** create the solution/project skeleton and the executable RED tests.