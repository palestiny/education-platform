# First Slice TDD RED Specification

**Date:** 2026-09-30  
**Status:** PROPOSED — RED SUITE READY FOR REVIEW  
**Gate:** Implementation Gate PASS — TDD RED Entry

## 1. Purpose

Define the first executable RED suite before any GREEN production implementation.

The suite must fail because the required behavior is not implemented, not because the test encodes an invented semantic rule.

## 2. Scope

```
Authenticated Principal
    ↓
Tenant Membership
    ↓
Learning Context
    ↓
Goal
    ↓
Assignment
    ↓
Submission
```

Cross-cutting behavior:
- authorization
- tenant isolation
- assignment lifecycle
- submission lifecycle
- idempotency
- auditability

## 3. Test Matrix

| ID | Scenario | Expected result | Contract source |
|---|---|---|---|
| RED-001 | Unauthenticated teacher creates assignment | 401 AUTHENTICATION_REQUIRED; no mutation | API §12 / Security |
| RED-002 | Authenticated teacher without authority creates assignment | 403 FORBIDDEN; no mutation | API §1 / Authorization |
| RED-003 | Authorized teacher creates assignment in own tenant/context | 201; ACTIVE assignment; version initialized | API §3 / Domain |
| RED-004 | Same idempotency key + semantically identical request | Original result replayed; no duplicate assignment | API §1 / Reliability |
| RED-005 | Same idempotency key + materially different request | 409 IDEMPOTENCY_CONFLICT; no second mutation | API §12 / Reliability |
| RED-006 | Authorized actor attempts assignment across tenant boundary | 403 or policy-approved non-disclosure; no cross-tenant mutation | Security baseline |
| RED-007 | Authorized learner submits active assignment | 201; SUBMITTED submission; correct tenant/context ownership | API §4 / Domain |
| RED-008 | Same submission request is retried | Original result replayed; exactly one authoritative submission | Reliability |
| RED-009 | Submission against closed assignment | 422 BUSINESS_RULE_VIOLATION; no submission mutation | Domain lifecycle |
| RED-010 | Successful authoritative assignment/submission mutation | Required audit record exists with actor/context/correlation | Audit contract |

## 4. Test Identity Model

Tests must use an explicit application authentication context rather than a production identity provider.

Minimum test identity attributes:
- principal ID
- active role/authority
- organization/tenant membership
- relationship/context authority where required

The test harness must be able to create:
- authorized teacher
- unauthorized teacher
- authorized learner
- cross-tenant actor
- unauthenticated request

## 5. Tenant Isolation Test Fixture

At minimum, create two independent tenant contexts:

```
Tenant A
  Teacher A
  Learner A
  Learning Context A

Tenant B
  Teacher B
  Learner B
  Learning Context B
```

RED-006 must prove that valid identity in Tenant A does not grant authority over Tenant B resources.

The test must not rely on a client-supplied tenant ID as the authority source.

## 6. Idempotency Assertions

For RED-004 and RED-008:
- reuse the same idempotency key;
- use the same semantic request;
- assert equivalent original response/resource identity;
- assert exactly one authoritative record.

For RED-005:
- reuse the same key with a materially different request;
- assert `409 IDEMPOTENCY_CONFLICT`;
- assert no second authoritative record.

## 7. Lifecycle Assertions

Assignment:
```
Draft → Active → Closed
```

The RED suite only needs the behavior required to prove that a closed assignment cannot accept a new submission.

Submission:
```
Created → Submitted
```

No assessment/result behavior is introduced in this RED suite.

## 8. Audit Assertions

Audit records are accountability records, not the business source of truth.

The assertion must verify at least:
- actor/principal
- tenant/context
- operation
- correlation identifier
- resulting operation status

Sensitive learner data must not be required merely to prove auditability.

## 9. Failure-Reason Discipline

Each RED test must fail for one primary intended reason.

Examples:
- missing authentication must not fail because the database is unavailable;
- forbidden tenant access must not fail because an unrelated schema constraint is missing;
- idempotency replay must not fail because the response serializer is incomplete.

If a test fails for an unrelated reason, fix the test harness/design before entering GREEN.

## 10. Traceability

```
RED-001 → Authentication boundary / API error contract
RED-002 → Authorization contract
RED-003 → Assignment domain + API contract
RED-004 → Idempotency contract
RED-005 → Idempotency conflict contract
RED-006 → Tenant isolation/security baseline
RED-007 → Submission domain + API contract
RED-008 → Submission idempotency
RED-009 → Assignment lifecycle/domain invariant
RED-010 → Audit/observability contract
```

## 11. RED Review Checklist

- [ ] Every test maps to an accepted requirement/contract.
- [ ] No test introduces deferred product semantics.
- [ ] No test chooses a production identity provider.
- [ ] No test requires physical tenant isolation.
- [ ] No test assumes an unapproved evidence taxonomy.
- [ ] No test assumes a mastery/progress algorithm.
- [ ] No test assumes advanced scheduling/recurrence.
- [ ] Each failure has an intended primary cause.
- [ ] Duplicate/retry behavior is asserted by authoritative record count.
- [ ] Cross-tenant access is tested server-side.
- [ ] Audit assertions do not turn logs into business truth.

## 12. Entry / Exit

**RED entry:** authorized by Implementation Gate PASS.

**RED exit:** all ten tests exist in the test project, fail for their intended reasons, and receive review.

Only after RED exit may GREEN implementation begin.

**Current status: RED SUITE SPECIFICATION READY FOR REVIEW.**
