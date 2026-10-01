# TDD RED Execution Readiness

**Date:** 2026-09-30  
**Status:** EXECUTABLE RED SUITE CREATED — EXECUTION/VERIFICATION PENDING

## Current finding

The repository now contains the minimum test-project structure required to begin executable RED work.

The test-project boundary exists. The next step is to add executable RED tests only after the application boundary types they exercise are defined without inventing deferred semantics.

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


## Test Project Skeleton — 2026-10-01

The branch now contains the minimum xUnit test-project boundaries:

- `tests/EducationPlatform.UnitTests/`
- `tests/EducationPlatform.ApplicationTests/`
- `tests/EducationPlatform.IntegrationTests/`

The projects intentionally contain no fake implementation and no artificial failing assertions.

Executable RED tests remain the next task because the application boundary they will exercise does not yet exist.

## Execution Update — 2026-10-01

The repository already contained the first .NET solution/test-project skeleton, so no duplicate project structure was created.

The test harness was aligned with the accepted xUnit decision using the current xUnit.net v3 line. The application test project was also aligned with the ASP.NET Core Web SDK requirement for `WebApplicationFactory`.

### Executable RED

RED-001 through RED-009 are now represented as executable HTTP contract tests.

RED-010 is deliberately not represented through a fabricated `/test-observability/audit` endpoint. That would introduce an API contract that was never approved.

RED-010 therefore remains bounded to the persistence observation seam: once the first-slice audit persistence mapping exists, the test will verify the persisted audit record directly through the test-owned persistence boundary.

### Verification

GitHub Actions run **#36** is currently executing the test workflow for the latest commit.

**Current result: IN PROGRESS — no PASS/FAIL conclusion yet.**



## Execution Root-Cause Correction — 2026-10-01

The first executable RED CI attempt did **not** reach test execution.

GitHub Actions run #41 successfully completed checkout, .NET 10 setup, and restore, but the test command failed before discovery because .NET 10 was invoking the legacy VSTest path against Microsoft.Testing.Platform-based xUnit v3 projects.

The exact failure was:

> Testing with VSTest target is no longer supported by Microsoft.Testing.Platform on .NET 10 SDK and later.

This was a **test infrastructure configuration failure**, not valid RED evidence.

### Corrective action

The harness was corrected to use the native .NET 10 Microsoft Testing Platform path:

- added root `global.json` selecting `Microsoft.Testing.Platform`;
- configured all three xUnit test projects as executable test applications;
- enabled the xUnit Microsoft Testing Platform runner;
- removed the VSTest-specific TRX logger argument from the CI `dotnet test` invocation.

This follows the .NET 10 testing model and xUnit v3 guidance.

### Current verification status

The correction has been committed to the working branch, but a new GitHub Actions run has not yet completed.

Therefore:

- Test harness infrastructure: **CORRECTED**
- Restore: **previously verified PASS**
- RED behavioral execution: **PENDING NEW CI RUN**
- RED-001..009: **defined and executable**
- RED-010: **deferred to the real persistence/audit seam; no fake API**
- GREEN: **BLOCKED**


## Behavioral RED Verification — 2026-10-01

GitHub Actions run **#61** on commit `137824498d95c54e4cb0daf40aa9586d8e094b85` reached actual xUnit execution.

The previous `StreamContent` disposal failure was confirmed as a test-harness lifetime bug and was corrected by awaiting `HttpClient.SendAsync` before disposing each request.

### Result

- ApplicationTests: **9 RED scenarios executed**
- Behavioral failures: **8**
- Behavioral passes: **1**
- Harness/runtime crash: **none in ApplicationTests**
- Restore/build/discovery: **PASS**
- UnitTests and IntegrationTests currently contain zero executable tests; this is separate from the first-slice RED suite.

The observed failures are intentional evidence that the production API boundary is not implemented yet. The current API returns **404 Not Found** because `Program.cs` does not yet register the first-slice endpoints.

Observed:
- RED-001 expected 401, actual 404
- RED-002 expected 403, actual 404
- RED-003 expected 201, actual 404
- RED-004 expected 201, actual 404
- RED-005 expected 201, actual 404
- RED-006 expected 403/404, actual 404
- RED-007 expected 201, actual 404
- RED-008 expected 201, actual 404
- RED-009 expected 422, actual 404

RED-006 therefore currently passes because the accepted contract permits 404 as a non-disclosing cross-tenant response.

### Gate interpretation

The executable RED suite has now crossed the important boundary from **test-harness failure** to **behavioral failure against missing implementation**.

Therefore:

**RED execution infrastructure = PASS**  
**RED behavioral evidence = PASS**  
**RED-001..009 = executable**  
**RED-010 = intentionally deferred to real audit persistence seam**  
**GREEN implementation = UNBLOCKED**

The next phase is GREEN, beginning with the smallest coherent vertical slice: authentication/authorization boundary → learning context/goal/assignment creation → submission, while preserving idempotency, tenant authority, audit and outbox boundaries. No fake endpoint or shortcut implementation should be introduced.
