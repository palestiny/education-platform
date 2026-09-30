# TDD RED Execution Readiness

**Date:** 2026-09-30  
**Status:** READY TO CREATE TEST PROJECT — TEST FRAMEWORK DECISION OPEN

## Current finding

The repository currently contains the approved product, domain, UX, architecture, security, data, API and implementation-gate documentation, but no application/test project structure is present on the implementation branch.

Therefore the RED specification can be translated into executable tests only after establishing the minimum test-project boundary.

## What is already fixed

- First technical slice: Learning Context → Goal → Assignment → Submission.
- Authentication boundary: provider-neutral application auth context behind ASP.NET Core host authentication.
- Tenant authority: server-derived from authenticated membership and protected resource/context.
- Logical tenant isolation for the first slice.
- Relational persistence boundary.
- Idempotency and optimistic concurrency semantics.
- Auditability.
- Ten RED scenarios are defined in FIRST_SLICE_TDD_RED_SPECIFICATION.md.

## What remains an implementation-level choice

### Test framework

A concrete .NET test framework/runner has not been explicitly selected in the repository.

Candidate: xUnit

Alternative: NUnit

Recommendation: xUnit because it is a conventional .NET test framework and keeps the first test harness small.

**Status: PROPOSED — not an owner-level product/architecture decision.**

### Test project layout

Proposed minimum:

```text
tests/
  EducationPlatform.UnitTests/
  EducationPlatform.ApplicationTests/
  EducationPlatform.IntegrationTests/
```

The first RED suite should begin at the application boundary and add persistence integration coverage where the scenario requires real relational constraints.

### Test database

Use a real PostgreSQL-compatible database for persistence integration tests.

The harness should isolate test data and never depend on developer-local production data.

## No implementation shortcut

We must not create fake production behavior merely to make RED tests pass.

RED must initially fail because the application behavior is absent.

## Next executable step

After the test-framework choice is explicitly accepted, create the minimum solution/test-project skeleton and translate RED-001 through RED-010 into executable tests.

Until then:

**RED specification = ready**  
**Executable RED tests = not yet created**  
**GREEN implementation = blocked**