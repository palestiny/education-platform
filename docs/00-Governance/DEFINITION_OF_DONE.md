# Definition of Done

A work item is complete only when its applicable evidence exists.

## Product
- Requirement is explicit.
- User/problem context is clear.
- Acceptance criteria are defined.
- Business rules are defined.

## UX
- Happy path defined.
- Empty states defined where applicable.
- Loading states defined where applicable.
- Error states defined.
- Permission states defined.
- Accessibility considerations addressed.
- Localization considerations addressed.

## Domain
- Entities/value objects/relationships defined.
- Lifecycle and state transitions defined.
- Invariants defined.
- Ownership boundaries defined.

## Architecture
- Relevant architectural impact reviewed.
- Decision documented if required.
- Module boundaries preserved.
- Dependencies are intentional.

## Security & Privacy
- Authentication considered.
- Authorization considered.
- Data exposure reviewed.
- Sensitive data handling reviewed.
- Audit requirements reviewed where applicable.
- Abuse/safety implications reviewed where applicable.

## Data
- Data model defined.
- Constraints defined.
- Indexing considered.
- Retention/deletion considered.
- Concurrency considered.

## API
- Contract defined before implementation.
- Validation defined.
- Error contract defined.
- Idempotency considered.
- Side effects/events documented.

## Engineering
- Implementation matches approved design.
- No undocumented behavior was introduced.
- Code quality checks pass.

## Testing
- Unit tests where applicable.
- Integration tests where applicable.
- Contract tests where applicable.
- End-to-end tests where applicable.
- Negative/edge cases covered.
- Regression coverage added for defects.

## Observability
- Relevant logs/metrics/traces are defined.
- Failure diagnosis is possible.
- Audit evidence exists where required.

## Documentation
- Relevant docs updated.
- Decision records updated.
- Gaps updated.
- Checkpoint updated.

## Verification
- Test/verification evidence is recorded.
- Gate status is explicit.
- NOT PROVEN is not treated as PASS.
