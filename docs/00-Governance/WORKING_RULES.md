# Working Rules

## 1. Purpose

These rules govern product discovery, design, implementation, testing and verification.

## 2. Source of Truth

The repository is the persistent source of truth. Chat is a collaboration surface, not the final record of project decisions.

Approved decisions, requirements, assumptions, gaps and verification evidence must be persisted in the repository.

## 3. No Undefined Behavior

If behavior is not defined, it must not be implemented. Identify the ambiguity, record it, explain why it matters, present options where appropriate, recommend a path when useful, and record the required decision.

## 4. No Hidden Decisions

Do not silently turn assumptions into requirements. Every important decision needs context, alternatives, trade-offs, rationale, consequences and status.

## 5. No Surprise Implementation

Code is an implementation of approved requirements and design. Code must not become the place where business behavior, architecture, API contracts, security rules or product policy are invented.

## 6. State Classification

Project knowledge should be distinguishable as DEFINED, ASSUMED, OPEN or BLOCKED. An ASSUMED item must not silently become a final decision.

## 7. Design Before Implementation

Relevant Design Gates must pass before implementation begins. Design Complete is not the same as Implementation Complete.

## 8. TDD / Verification Discipline

Where practical: RED → GREEN → REFACTOR → VERIFY.

A passing test proves only the behavior covered by that test. Important requirements require explicit verification evidence.

## 9. Edge Cases Are First-Class

Every important feature should consider validation failures, authentication failures, authorization failures, conflicts, duplicate requests, concurrency, external failures, timeouts, retries, partial failures, unexpected failures, audit behavior and recovery behavior.

## 10. API Contract Before API Implementation

Before implementation define, as applicable: purpose, authorization, request, response, validation, business rules, errors, idempotency, concurrency, side effects, events, audit and observability.

## 11. Data Design Before Schema

Before schema implementation define, as applicable: entity purpose, ownership, lifecycle, relationships, constraints, indexes, uniqueness, deletion behavior, audit, retention and concurrency.

## 12. Domain Source of Truth

State transitions must define valid states, triggers, preconditions, forbidden transitions, emitted events and failure behavior.

## 13. Root Cause Before Closure

Symptom → Reproduce → Evidence → Root Cause → Fix → Regression Test → Verification → Close.

A workaround must be explicitly classified as a workaround.

## 14. No Silent Architecture Changes

Major changes to architecture, domain model, API, security, data strategy or provider strategy require a documented decision.

## 15. Traceability

Where applicable maintain: Business Objective → Requirement → Feature → Design → ADR → API → Code → Test → Verification.

## 16. Change Control

New Evidence → Impact Analysis → Change Proposal → Affected Artifacts → Decision → Implementation → Verification.

## 17. Checkpoints

Every meaningful milestone should update the checkpoint with current stage, completed work, in-progress work, open questions, gaps, risks, next gate and verification status.

## 18. Quality Doctrine

- If not defined, not implemented.
- If not tested, not proven.
- If not documented, not an accepted decision.
- Silent behavior changes are defects.
- Root cause before declaring fix.
- Important assumptions must be visible.
- Important decisions require rationale.
- Completed milestones require verification.
- NOT PROVEN never equals PASS.
