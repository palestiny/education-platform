# Segment Dependency Decision Matrix

**Status:** Planning / Decision-Support Artifact — PROPOSED  
**Gate:** Product Foundation / Competitive Intelligence — NOT PROVEN  
**Implementation authorization:** None  
**Date:** 2026-09-26

## Purpose

Make segment-dependent product decisions explicit without selecting a beachhead segment by assumption.

Candidate segments:
- Private tutoring teacher + parent + student
- Tutoring center teacher/coordinator + parent + student
- School teacher/coordinator + parent + student

This matrix separates what can be decided from existing evidence from what requires direct workflow evidence.

## Decision discipline

- A segment-dependent requirement must not become a committed requirement merely because it is common in the market.
- Public competitor capability proves that a capability exists; it does not prove that the capability is required for our first segment or that an unmet need exists.
- Direct participant evidence must reconstruct recent real cases rather than hypothetical opinions.
- No overall segment score or winner is produced here.
- Unknown remains unknown.
- Segment selection authorizes product-requirement discovery only; it does not by itself authorize architecture or implementation.

## Dependency matrix

| Requirement / capability | Private tutor | Tutoring center | School | Current decision | Required direct evidence | Downstream impact |
|---|---|---|---|---|---|---|
| PR-001 Multi-role identity | Relevant | Relevant | Relevant | PROPOSED | Confirm actual role combinations and account boundaries | Identity / authorization |
| PR-002 Context: organization, curriculum, locale, permissions | Conditional | Relevant | Relevant | CONDITIONAL | Reconstruct real contexts used in cases | Context / authz / tenancy |
| PR-003 Multiple learning modes | Conditional | Conditional | Conditional | CONDITIONAL | Identify actual first-mode mix | Learning experience |
| PR-004 Meaningful learning evidence | Relevant | Relevant | Relevant | PROPOSED | Identify evidence actually used in decisions | Evidence model |
| PR-005 Evidence quality / uncertainty | Relevant | Relevant | Relevant | PROPOSED | Observe stale, missing, conflicting or weak evidence | Evidence provenance / interpretation |
| PR-006 Evidence-backed progress | Relevant | Relevant | Relevant | PROPOSED | Identify how progress is currently established | Learner state |
| PR-007 Clear next useful action | Relevant | Relevant | Relevant | PROPOSED | Reconstruct how next actions are currently chosen | UX / recommendation boundary |
| PR-008 Teacher signal → evidence → decision → action | Relevant | Relevant | Relevant | PROPOSED | Recent teacher intervention cases | Teacher workflow |
| PR-009 Parent visibility/action | Relevant | Relevant | Relevant | CONDITIONAL | Confirm parent involvement and information boundaries | Relationship / privacy / communication |
| PR-010 Organization unresolved work | Usually low/conditional | Relevant | Relevant | CONDITIONAL | Cases involving coordinator/owner follow-up | Work management / org |
| PR-011 Context-preserving communication | Relevant | Relevant | Relevant | PROPOSED | Trace a real communication handoff | Communication |
| PR-012 Due / follow-up state | Relevant | Relevant | Relevant | PROPOSED | Identify overdue/open follow-up cases | Work-item lifecycle |
| PR-013 Outcome evidence before closure | Relevant | Relevant | Relevant | PROPOSED | Determine how participants know an action worked | Evidence / lifecycle |
| PR-014 Intervention lifecycle | Hypothesis | Hypothesis | Hypothesis | NOT PROVEN | Cross-role recurring cases with material friction | Potential case/workflow domain |
| PR-015 Reliability / recovery | Relevant | Relevant | Relevant | PROPOSED | Observe actual failures and recovery workarounds | Cross-cutting architecture |
| PR-016 Offline / low connectivity | Conditional | Conditional | Conditional | OPEN | Determine whether first workflow materially depends on offline operation | Sync / data consistency |
| PR-017 AI assistance | Conditional | Conditional | Conditional | OPEN | Identify a concrete assistive task and human owner | AI boundary |
| PR-018 AI governance | Relevant if AI used | Relevant if AI used | Relevant if AI used | PROPOSED | Define approval, fallback and audit from actual workflow | Security / audit / AI |
| PR-019 Privacy / consent | Relevant | Relevant | Relevant | PROPOSED | Identify sensitive evidence and access boundaries | Security / relationships |
| PR-020 Localization | Relevant | Relevant | Relevant | PROPOSED | Confirm first-market language/locale requirements | i18n / UX |
| PR-021 Tenant isolation | Conditional | Relevant | Relevant | OPEN | Confirm organization ownership and isolation needs | Architecture / data |
| PR-022 Payments | Conditional | Relevant | Relevant | OPEN | Identify first actual transaction and payer | Commerce |
| PR-023 Support / recovery context | Relevant | Relevant | Relevant | PROPOSED | Reconstruct failures requiring support/manual recovery | Support / observability |
| PR-024 Audit | Relevant | Relevant | Relevant | PROPOSED | Identify consequential actions requiring traceability | Audit / security |
| PR-025 Permission-aware search | Conditional | Conditional | Conditional | OPEN | Identify actual search tasks and sensitive results | Search / authorization |

## What is sufficiently supported now

### Can remain product-level candidates across segments

The following are sufficiently justified as planning candidates without claiming that every detail is final:

- Multi-role identity rather than a single user-type model.
- Meaningful learning evidence rather than activity counts alone.
- Evidence provenance/quality as a first-class concern.
- Evidence-backed progress.
- Teacher decision/action visibility where teachers are part of the workflow.
- Context-preserving communication.
- Follow-up and outcome visibility.
- Reliability and recovery as product quality requirements.
- Privacy, permissions, consent and audit as cross-cutting constraints.
- Global-ready localization foundations.

These remain PROPOSED until the selected segment and direct evidence determine exact behavior.

## What cannot be finalized before segment selection

The following must remain OPEN or CONDITIONAL:

1. Exact role set and role relationships.
2. Whether an organization/tenant is part of the first product boundary.
3. Parent portal and parent authorization model.
4. Attendance and scheduling depth.
5. Offline synchronization requirements.
6. Payments and first commercial transaction.
7. Organization-level unresolved-work management.
8. Intervention Case as a domain concept.
9. Exact AI use cases and approval boundaries.
10. Exact learning modes required in MVP.
11. Exact tenancy/isolation strategy.
12. Exact API/data contracts and aggregates.

## Segment-specific evidence questions

### Private tutoring

Required evidence:
- Does a tutor repeatedly coordinate with a parent around a learner issue?
- How is evidence collected across homework, assessment, attendance and communication?
- Who decides the intervention?
- Who performs it?
- How is follow-up remembered?
- How is success determined?
- What happens when the tutor or parent misses the handoff?
- What current tools are used, and where does reconstruction happen?

Architecture implications if validated:
- Person/relationship model may dominate over organization complexity.
- Scheduling/payment may become first-class if they are part of the real transaction.
- Organization/tenant can remain simpler if no center is involved.

### Tutoring center

Required evidence:
- What cases require teacher → coordinator → parent handoff?
- Who owns unresolved learner issues?
- How are teacher changes, branch changes or coordinator escalation handled?
- Are payments/attendance/scheduling materially coupled to learning workflows?
- What operational work is currently split across systems?

Architecture implications if validated:
- Organization/branch context becomes more likely to be foundational.
- Work ownership, permissions and audit become more important.
- Tenant/organization isolation requirements need earlier resolution.

### School

Required evidence:
- What is the actual teacher → coordinator/admin → parent workflow?
- Which decisions require school-level authorization?
- What curriculum, class/group and academic-calendar context must persist?
- What privacy/consent constraints apply to learner evidence?
- Which existing SIS/LMS/communication systems must be integrated rather than replaced?

Architecture implications if validated:
- Organization hierarchy, authorization, academic context and integration boundaries become more significant.
- Data governance and audit requirements may materially affect architecture.
- MVP scope may need tighter limits to avoid attempting a full school information system.

## Required gate evidence

Before selecting the first segment, collect enough real recent cases to establish:

- recurrence;
- material cost/friction;
- cross-role dependency;
- evidence fragmentation;
- decision ownership;
- action ownership;
- follow-up burden;
- closure visibility;
- outcome evidence;
- current workaround;
- failure/recovery behavior;
- privacy/consent boundaries;
- human accountability;
- segment-specific value hypothesis.

No numeric threshold is assumed in advance. Contradictory evidence must be preserved.

## Current conclusion

**Initial segment: NOT SELECTED.**

The planning work now has a clearer dependency boundary:

**Stable product principles → segment-dependent requirements → direct workflow evidence → segment decision → committed MVP → domain confirmation → architecture gate.**

The project must not reverse this order by selecting architecture first and forcing the product into it.

## Next gate

The immediate blocker remains **real recent workflow cases from actual participants**. The repository is collection-ready; public competitive research is no longer an acceptable substitute for those cases.

Until that evidence exists:
- Product Foundation = NOT PROVEN
- Domain Gate = NOT PROVEN
- Architecture Gate = NOT PROVEN
- Implementation = NOT AUTHORIZED
