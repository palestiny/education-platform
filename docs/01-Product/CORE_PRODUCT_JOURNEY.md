# Core Product Journey

Date: 2026-09-28
Status: Product Synthesis Artifact — PROPOSED
Gate: Product Foundation — NOT PROVEN
Implementation authorization: None

## Purpose

Define the platform's first coherent product journey without prematurely freezing the commercial beachhead, MVP feature list, domain boundaries, UX screens, or architecture.

The platform is one connected system. Role-specific experiences are views and actions over shared context, evidence, permissions, communication, and workflow state.

## Core Product Promise

The platform should answer, for every relevant actor:

- **Student:** What should I do now, and how am I progressing?
- **Parent:** What changed, why does it matter, and what—if anything—should I do?
- **Teacher:** Who needs attention, what evidence supports that, and what action should I take?
- **Center/School:** What needs management, who owns it, and whether it reached an outcome?
- **Platform:** Can the system preserve context, evidence, accountability, and reliable recovery across the journey?

## Canonical Journey

**1. Context → 2. Goal → 3. Plan → 4. Learn / Practice → 5. Assess → 6. Evidence → 7. Understand → 8. Progress → 9. Next Useful Action → 10. Feedback / Follow-up → 11. New Evidence → 12. Outcome**

This is a product journey, not a literal mandatory screen sequence.

## Journey Stages

### 1. Context

Establish the learner, role, course/subject, class/group, organization where relevant, permissions, locale, and current learning state.

**Rule:** The user should not need to reconstruct context manually when the platform already knows it.

### 2. Goal

Identify the intended outcome:
- learn a concept;
- complete assigned work;
- prepare for assessment;
- improve a demonstrated skill;
- recover from a demonstrated gap.

Goal semantics remain open until the product scope is committed.

### 3. Plan

Translate the goal into the relevant learning activity or sequence.

Possible plan sources:
- teacher assignment;
- curriculum;
- learner choice;
- organization plan;
- system suggestion;
- AI-assisted suggestion subject to governance.

A recommendation is not automatically a committed plan.

### 4. Learn / Practice

Deliver the appropriate learning experience:
- recorded;
- live;
- in-person/offline;
- reading/content;
- practice;
- assignment;
- human help.

The platform should preserve the same learner context across modalities.

### 5. Assess

Capture meaningful evidence through:
- quiz/test;
- assignment;
- practice result;
- teacher observation;
- learner self-report;
- other authorized evidence sources.

Assessment is one evidence source, not the only possible source.

### 6. Evidence

Normalize the meaningful result with:
- source;
- timestamp;
- context;
- subject/skill/objective where known;
- observed value;
- provenance;
- uncertainty where relevant;
- actor/system;
- visibility permissions.

### 7. Understand

Separate observed evidence from interpretation.

The system may explain:
- what happened;
- what it may mean;
- how confident the interpretation is;
- what evidence is missing.

AI can assist interpretation, but must not silently turn uncertain inference into authoritative state.

### 8. Progress

Represent change relative to the relevant goal/context.

Progress should not collapse into:
- screen completion;
- video watch time;
- login counts;
- attendance alone.

Exact progress/mastery calculation remains OPEN.

### 9. Next Useful Action

Present the most useful next step supported by available evidence.

Examples:
- continue;
- practice;
- retry;
- review prerequisite;
- ask teacher;
- complete assignment;
- reassess;
- communicate;
- wait for more evidence.

The platform should explain enough context for the user to understand why the action is suggested.

### 10. Feedback / Follow-up

After action, preserve what remains unresolved.

Follow-up may be lightweight. It should not automatically create a formal intervention case.

Candidate states:
- not required;
- planned;
- due;
- completed;
- overdue;
- cancelled;
- superseded;
- escalated.

### 11. New Evidence

The next meaningful evidence point is linked to the preceding goal/action when appropriate.

This is how the platform avoids ending at “notification sent” or “assignment completed.”

### 12. Outcome

Determine whether the intended outcome:
- improved;
- partially improved;
- did not demonstrably improve;
- produced contradictory evidence;
- remains unevaluable.

Outcome does not require certainty where evidence is insufficient.

## Cross-Role Continuity

### Student → Teacher

Student activity/evidence becomes teacher-readable context with appropriate permissions.

### Teacher → Student

Teacher feedback/action becomes a student-understandable next step.

### Teacher → Parent

Relevant evidence and action status become understandable parent context without exposing inappropriate internal data.

### Parent → Teacher / Organization

Parent communication preserves the child/context relationship and does not become detached chat history.

### Organization → All

Scheduling, enrollment, attendance, payment, communication, and policy state remain connected to the correct learner/relationship/context.

## Product Rules

1. **One source of learning truth:** evidence/state should not be recreated independently in each role experience.
2. **Role-specific presentation, shared semantics:** dashboards may differ; underlying meaning should not.
3. **Evidence before inference:** do not present interpretation as fact.
4. **Progress before activity metrics:** activity can support progress but is not equivalent to it.
5. **Action before dashboard overload:** show what matters next.
6. **Communication preserves context:** messages should remain attached to relevant learning/operational context where appropriate.
7. **Follow-up has lifecycle:** important unresolved work must not disappear after communication.
8. **AI remains governed:** assistance, provenance, permissions, human accountability, and safe failure are required.
9. **Failure is part of the journey:** retries, duplicates, timeouts, partial failures, and recovery must preserve state.
10. **Complexity stays inside the platform:** users should not have to understand the domain model to use the product.

## MVP Implication

The first release should implement a **complete slice of this journey**, not disconnected feature clusters.

A capability belongs in the first slice only when it is required to complete the selected journey with acceptable trust, privacy, reliability, and business viability.

Therefore:

**Core Journey ≠ MVP feature checklist.**

The MVP boundary is derived after the journey and commercial beachhead are explicitly decided.

## Open Decisions

- initial commercial beachhead;
- exact learning mode(s);
- role set;
- parent involvement;
- organization depth;
- goal model;
- progress/mastery semantics;
- payment/business transaction;
- AI first-release scope;
- formal follow-up/intervention semantics;
- measurable product success criteria.

## Gate Position

**NOT PROVEN**

This artifact establishes a coherent product journey candidate. It does not authorize domain, UX, architecture, API, data, or implementation decisions.

Next:

**Core Journey → Product Foundation Gate → Requirements consolidation → Domain Gate preparation**
