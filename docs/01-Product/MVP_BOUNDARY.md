# MVP Boundary

Date: 2026-09-25
Status: Boundary Proposal — PROPOSED
Gate: Product Foundation / Competitive Intelligence — NOT PROVEN
Implementation authorization: None

## Purpose

Prevent scope explosion while preserving enough competitive parity to build a coherent first product.

The MVP is not "the smallest number of features." It is the smallest **complete learning workflow** for one validated initial segment.

## Boundary Principle

**One segment + one coherent learning journey + required parity + trustworthy execution.**

We do not build the entire global education ecosystem in MVP.

## MVP Candidate Shape

### Must Exist in the First Coherent Product

Subject to initial-segment validation:

- identity and authentication;
- required roles and relationships;
- basic organization/tenant context if the segment needs it;
- learner context/profile;
- learning content appropriate to the segment;
- learning activity/session;
- assignments/homework where relevant;
- practice where relevant;
- assessments where relevant;
- evidence-backed progress;
- teacher workflow;
- required parent visibility;
- communication;
- notifications;
- scheduling where required;
- attendance where required;
- required payment/fee flow if monetization depends on it;
- privacy/permissions;
- auditability for important actions;
- Arabic/RTL + localization foundation for the initial context;
- reliability/recovery for critical flows;
- support/recovery path.

These are candidate capabilities, not final requirements.

## Explicitly Not MVP by Default

Unless evidence makes one essential for the chosen segment:

- full marketplace;
- white-label platform;
- advanced community;
- large-scale gamification;
- certificate ecosystem;
- advanced content marketplace;
- enterprise integrations;
- advanced BI/data warehouse;
- sophisticated adaptive learning;
- broad AI agent system;
- multi-provider AI orchestration;
- global payment complexity;
- every possible class modality;
- every organization model;
- large social graph.

## Conditional MVP Capabilities

These depend on the validated beachhead:

| Capability | Include when |
|---|---|
| Live classes | Live teaching is core to initial segment |
| Offline/in-person classes | Segment relies on physical tutoring/classes |
| Parent portal | Parent is a real actor in the validated workflow |
| Organization management | Center/school/academy is the beachhead |
| Payments | Paid transaction is part of the first business workflow |
| Attendance | Attendance affects the selected learning workflow |
| AI assistance | A validated workflow has a measurable benefit and safe human boundary |
| Intervention lifecycle | Direct research validates recurring material coordination friction |

## MVP Quality Gates

The MVP is not complete if the happy path works but critical failure behavior is undefined.

For each critical capability, define:

- authorization;
- validation;
- duplicate/replay handling;
- timeout/external failure;
- partial failure;
- retry;
- recovery;
- observability;
- audit where needed;
- regression tests.

## MVP Success Evidence

Do not use feature count as the primary success measure.

Candidate evidence:

- learners can complete the intended learning journey;
- teachers can act without excessive reconstruction/admin work;
- parents can understand relevant status without surveillance overload;
- organizations can operate required workflows;
- critical failures recover predictably;
- users can explain why the next action is being presented;
- outcome evidence can be collected for the chosen workflow.

Exact metrics remain OPEN until the initial segment and business model are validated.

## Scope Gate

Before implementation starts, we must have:

1. validated initial segment;
2. Product Requirements Map;
3. committed MVP capabilities;
4. explicit non-MVP list;
5. key business rules;
6. privacy/consent boundaries;
7. initial domain boundaries;
8. architecture decision;
9. critical API/data contracts;
10. test strategy and Definition of Done.

No code is authorized by this document.
