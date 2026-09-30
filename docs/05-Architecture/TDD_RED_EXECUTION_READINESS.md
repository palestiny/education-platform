# TDD RED Execution Readiness

**Date:** 2026-09-30  
**Status:** ACCEPTED — TEST PROJECT CREATION AUTHORIZED

## Current finding

The repository now contains the minimum test-project structure required to begin executable RED work.

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

The test framework has been explicitly selected.

Selected: xUnit

NUnit remains an alternative but is not selected for this project.

**Decision status: ACCEPTED — 2026-09-30.**

The executable harness uses xUnit v3 (`xunit.v3` 4.0.1) with the Visual Studio adapter 4.0.0 and Microsoft.NET.Test.Sdk 18.10.0.

### Test project layout

Accepted minimum:

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

The minimum solution/test-project skeleton has been created. The next step is to translate RED-001 through RED-010 into executable tests.

Until then:

**RED specification = ready**  
**Test project skeleton = created**  
**Executable RED tests = not yet created**  
**GREEN implementation = blocked**

## Framework Baseline Verification — 2026-09-30

The current .NET LTS release is .NET 10, and the first test projects target `net10.0`. Microsoft lists .NET 10 as active LTS through November 14, 2028.

The test harness package baseline was refreshed against current NuGet listings:
- `xunit.v3` 4.0.1
- `xunit.runner.visualstudio` 4.0.0
- `Microsoft.NET.Test.Sdk` 18.10.0

NuGet currently identifies xUnit v2 2.9.3 as legacy/deprecated, so the harness uses xUnit v3 instead.


## 2026-09-30 Execution Boundary Note

xUnit has now been explicitly accepted (DEC-0019).

Before translating the ten RED scenarios into executable C# tests, the repository still needs a minimal .NET solution/application boundary to compile against. Creating tests that fail because types/projects are absent would be a harness failure, not valid RED evidence.

Therefore the correct next implementation step is:

1. establish the minimal solution and application/test project boundaries;
2. establish the provider-neutral application interfaces required by the accepted contract;
3. write RED tests against those boundaries;
4. run the tests and verify each failure is behavioral and intentional;
5. only then enter GREEN.

The application boundary must remain minimal and must not implement the requested behavior merely to satisfy the tests.
