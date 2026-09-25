# Competitive Parity Backlog

Date: 2026-09-25
Status: Planning Artifact — PROPOSED
Gate: Product Foundation / Competitive Intelligence — NOT PROVEN
Implementation authorization: None

## Purpose

Translate the target product scenario into a capability baseline so the first product is not accidentally weaker than established alternatives.

This is a planning backlog, not a commitment to implement every capability in the first release.

## Policy

For every capability, record:

- Expected market baseline
- Initial-segment relevance
- Minimum acceptable behavior
- Reliability/recovery expectation
- Cross-role connection
- Evidence status
- MVP disposition

Priority vocabulary:

- **PARITY-MUST** — expected for the selected segment; omission would create a material baseline gap.
- **QUALITY-MUST** — baseline capability with reliability/trust requirements.
- **WORKFLOW-CANDIDATE** — candidate for deeper differentiation; requires direct validation.
- **LATER** — valuable but not required for the first coherent product.
- **OPEN** — cannot be decided until evidence/segment constraints are known.

## Capability Baseline

| Capability | Baseline | Quality bar | Cross-role relevance | MVP disposition | Status |
|---|---|---|---|---|---|
| Identity & authentication | PARITY-MUST | Recovery, session continuity, safe retries | All roles | Must | PROPOSED |
| Multi-role identity | PARITY-MUST | Explicit role/permission boundaries | All roles | Must | PROPOSED |
| Organization/tenant basics | PARITY-MUST for B2B | Isolation, auditability | Org + users | Segment-dependent | OPEN |
| Student profile/context | PARITY-MUST | Current context, provenance | Student/teacher/parent | Must | PROPOSED |
| Parent-child relationship | PARITY-MUST where parent segment applies | Consent + visibility rules | Parent/student | Segment-dependent | OPEN |
| Learning content | PARITY-MUST | Fast, accessible, resumable | Student/teacher | Must | PROPOSED |
| Recorded/video learning | PARITY-MUST where relevant | Resume/recovery | Student/teacher | Segment-dependent | OPEN |
| Live learning | PARITY-MUST where relevant | Join/rejoin/recovery | Student/teacher | Segment-dependent | OPEN |
| Offline/in-person class support | PARITY-MUST for relevant segment | Attendance/context continuity | Student/teacher/org | OPEN | OPEN |
| Assignments/homework | PARITY-MUST | Submission state + retry/recovery | Student/teacher/parent | Must | PROPOSED |
| Practice | PARITY-MUST | Feedback + progress continuity | Student/teacher | Must | PROPOSED |
| Assessments/quizzes | PARITY-MUST | Attempt integrity + clear results | Student/teacher/parent | Must | PROPOSED |
| Progress | PARITY-MUST | Evidence-backed, not vanity metrics | All roles | Must | PROPOSED |
| Teacher insights | PARITY-MUST | Action-oriented, low cognitive load | Teacher/org | Must | PROPOSED |
| Parent visibility | PARITY-MUST where applicable | Confidence, not surveillance | Parent/student/teacher | Segment-dependent | OPEN |
| Human teacher help | PARITY-MUST | Context preserved | Student/teacher | Must | PROPOSED |
| Scheduling/calendar | PARITY-MUST | Time-zone/locale aware | All roles | Segment-dependent | OPEN |
| Attendance | PARITY-MUST for managed learning | Correctness + offline tolerance where needed | Student/teacher/org/parent | Segment-dependent | OPEN |
| Communication | PARITY-MUST | Context + delivery status | All roles | Must | PROPOSED |
| Notifications | PARITY-MUST | Preference, dedupe, recovery | All roles | Must | PROPOSED |
| Payments/fees | PARITY-MUST for paid organizational workflows | Idempotency + reconciliation | Parent/org | Later/segment | OPEN |
| Arabic/RTL | QUALITY-MUST for initial MENA context | Complete RTL, not translation-only | All roles | Must | PROPOSED |
| Localization | QUALITY-MUST | Locale/currency/time-zone separation | All roles | Must | PROPOSED |
| Privacy/permissions | QUALITY-MUST | Least privilege + explicit visibility | All roles | Must | PROPOSED |
| Auditability | QUALITY-MUST | Trace important actions | Org/support/security | Must | PROPOSED |
| Search | PARITY-MUST at scale | Relevant, permission-aware | All roles | Later | OPEN |
| Support/help | QUALITY-MUST | Context-preserving support path | All roles | Must | PROPOSED |
| Offline/low-connectivity recovery | QUALITY-MUST where justified | Explicit sync/retry semantics | Student/teacher/org | Segment-dependent | OPEN |
| Gamification | LATER | Educational purpose required | Student | Later | OPEN |
| Community/social | LATER | Safety/moderation required | Student/teacher | Later | OPEN |
| Certificates | LATER | Credential purpose must be defined | Student/org | Later | OPEN |
| White-label | LATER | Tenant isolation + branding | Organizations | Later | OPEN |
| Marketplace | LATER | Commerce/trust/moderation | Platform | Later | OPEN |
| Advanced AI | WORKFLOW-CANDIDATE | Human accountability + evaluation | All roles | Later | OPEN |
| Intervention lifecycle | WORKFLOW-CANDIDATE | Ownership, due state, outcome, escalation | Student/teacher/parent/org | Research first | NOT PROVEN |

## Reliability Policy

Every MVP capability must define, where applicable:

1. validation failure,
2. permission failure,
3. duplicate/replay behavior,
4. timeout/external failure,
5. partial failure,
6. retry semantics,
7. user-visible recovery,
8. audit/observability,
9. regression tests.

## Exit Criteria for This Backlog

Before Architecture Gate:

- initial segment selected from evidence,
- MVP dispositions converted from OPEN to explicit decisions or documented uncertainty,
- required capabilities have acceptance-level behavior defined,
- critical recovery paths identified,
- research gaps linked to evidence or open questions.

No code is authorized by this document.
