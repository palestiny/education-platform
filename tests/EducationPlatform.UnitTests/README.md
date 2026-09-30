# EducationPlatform.UnitTests

The first technical slice uses the repository's existing xUnit v3 test-harness skeleton.

Executable RED tests must exercise accepted application behavior. Placeholder tests that fail by assertion alone are not considered valid RED evidence.

Scope:

```
Authenticated Principal
  -> Tenant Membership
  -> Learning Context
  -> Goal
  -> Assignment
  -> Submission
```

See:
- `docs/05-Architecture/FIRST_SLICE_TDD_RED_SPECIFICATION.md`
- `docs/05-Architecture/TDD_RED_EXECUTION_READINESS.md`

The test harness must remain provider-neutral and must not introduce deferred product semantics.
