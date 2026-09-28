# Direct Evidence to Segment Decision Protocol

**Status:** Research Decision Protocol — PROPOSED  
**Gate:** Product Foundation — NOT PROVEN  
**Implementation:** Not authorized  
**Date:** 2026-09-28

## 1. Purpose

Define how real educational cases are converted into an explicit initial-segment decision without inventing evidence, scoring participants, or silently turning research interpretation into product requirements.

**Decision sequence:** Real Cases → Evidence Normalization → Cross-Case Synthesis → Segment Decision → Committed Core Journey → Exact Foundation Requirements → Product Foundation Gate

## 2. Decision Boundary

This protocol may support:
- comparing candidate segments using documented evidence;
- identifying recurring workflow structures and meaningful differences;
- identifying contradictions and unresolved questions;
- selecting an initial segment when evidence is sufficient;
- deriving a candidate Core Journey from observed workflow evidence.

This protocol does not authorize MVP commitment, domain/bounded-context design, database schema, API contracts, UX implementation, architecture/technology selection, AI provider selection, pricing/free-paid commitment, or prevalence claims from a small qualitative sample.

## 3. Evidence Classes

| Class | Meaning |
|---|---|
| OBSERVED FACT | Directly observed behavior or artifact evidence |
| PARTICIPANT REPORT | Participant statement about what happened |
| RECONSTRUCTED WORKFLOW | Structured reconstruction of a real case |
| MARKET EVIDENCE | Public product/research evidence |
| RESEARCH INTERPRETATION | Our synthesis of evidence |
| HYPOTHESIS | Proposition still requiring validation |

A participant report is not silently upgraded to observed fact. A reconstruction is not silently upgraded to prevalence.

## 4. Case Inclusion

A case is eligible when it concerns a real recent educational workflow, the segment/context and relevant roles are identifiable, the trigger and workflow can be reconstructed usefully, unknowns remain explicit, and source/consent boundaries are recorded.

Exclude hypothetical scenarios, feature opinions without a real case, invented examples, generic complaints without reconstructable context, and duplicate records that add no material evidence.

## 5. Case Normalization

Preserve:

**Context → Trigger → Evidence → Interpretation → Decision Owner → Action Owner → Action → Communication → Due/Follow-up → Outcome Evidence → Closure**

Also preserve tools/workarounds, cost/friction, failure/recovery, privacy/consent, AI/human accountability, contradictions, and unknowns.

Remove unnecessary identifiers without removing decision-relevant context.

## 6. Cross-Case Synthesis

For each candidate segment ask:
- What workflows actually recur?
- Which roles repeatedly participate?
- Where does evidence originate?
- Where is evidence fragmented or lost?
- Who interprets evidence?
- Who owns the decision?
- Who performs the action?
- Is follow-up explicit?
- Is closure visible?
- What proves the outcome?
- What workarounds are used?
- What material friction/cost is reported or observed?
- What failures require recovery?
- What privacy/consent boundaries matter?
- What alternatives already solve the problem?
- What evidence contradicts the emerging interpretation?

Synthesis is qualitative and evidence-traceable. Do not convert these questions into a numeric score.

## 7. Contradiction Handling

When cases disagree:
1. Preserve both records.
2. Identify whether the difference comes from role, segment, workflow type, context, recency, or evidence quality.
3. Do not average away disagreement.
4. State what remains uncertain.
5. Collect targeted additional cases only when the contradiction could change the segment decision or Core Journey.

## 8. No Evidence vs Negative Evidence

**NO EVIDENCE:** the question was not sufficiently observed or asked.

**NEGATIVE EVIDENCE:** a real case or participant experience provides evidence against the hypothesis.

Never record “not mentioned” as evidence that a problem does not exist.

## 9. Segment Decision Readiness

The research record should answer, with traceable evidence:
1. What real workflow is being served?
2. Who are the recurring roles?
3. What is the meaningful trigger/goal?
4. What evidence drives action?
5. Where does material friction occur?
6. Who owns decisions and actions?
7. What follow-up/closure behavior exists?
8. What outcome evidence exists?
9. Which alternatives/workarounds are used?
10. Why is this segment coherent enough for a first product journey?
11. What important evidence contradicts the current interpretation?
12. What remains unknown and could materially change the decision?

No arbitrary minimum case count is sufficient by itself. More evidence is required when cases remain contradictory, shallow, or materially different.

## 10. Core Journey Derivation

After segment readiness, derive the Core Journey from actual cases.

It must identify starting context, user goal/trigger, learning action, evidence creation/collection, interpretation, progress state, next useful action, responsible human role, communication/context handoff, follow-up, outcome evidence, and closure/escalation where actually observed.

If a proposed step has no supporting evidence, mark it OPEN/HYPOTHESIS rather than presenting it as committed.

## 11. Evidence Quality

For each important conclusion record supporting case IDs, source class, evidence recency, directness, contradictions, unresolved unknowns, and a qualitative confidence statement. Do not manufacture precision.

## 12. Decision Record

### Decision
Selected initial segment: ______

### Evidence basis
- recurring workflows:
- material friction/cost:
- role/ownership structure:
- evidence/decision/action chain:
- follow-up/closure:
- outcome evidence:
- existing alternatives:
- privacy/consent:
- failure/recovery:
- contradictions:

### Why this segment is coherent
Document the evidence-based rationale without numeric ranking or hidden criteria.

### Rejected / deferred alternatives
For each alternative, document the evidence boundary and what remains unknown. Do not describe alternatives as inferior in general.

### Open questions
List unresolved questions that must remain open.

### Consequences
List what becomes segment-specific and what remains product-level.

### Gate status
PASS / GAP / NOT PROVEN / REJECT-PIVOT

## 13. Evidence Ledger Update Rules

After each real case:
1. Add the case to the case ledger.
2. Preserve its original evidence classification.
3. Update relevant segment evidence.
4. Record contradictions.
5. Update decision-readiness state.
6. Do not rewrite earlier cases to fit the emerging narrative.
7. Update the checkpoint only when the state actually changes.

## 14. Research Stop Rule

Stop a line of evidence when the decision-relevant question is adequately answered, additional cases no longer change the interpretation, contradictions have been investigated sufficiently, and remaining uncertainty is explicitly accepted or deferred.

Continue when a contradiction could materially change the segment or Core Journey.

## 15. Anti-Bias Controls

Researchers must ask for real recent cases, avoid feature-selling, probe failures and workarounds, ask what happened when the process worked and failed, actively seek counterexamples, and preserve participant language separately from researcher interpretation.

## 16. Output Contract

Before the Product Foundation Gate, the repository should contain:
- direct case records;
- updated direct evidence ledger;
- contradiction evidence;
- updated segment decision framework;
- explicit segment decision record;
- candidate Core Journey;
- unresolved questions and gaps;
- updated checkpoint.

**Current state remains:** Initial Segment NOT SELECTED, Direct Cases 0, Product Foundation NOT PROVEN, Domain NOT PROVEN, Architecture NOT PROVEN, Implementation NOT AUTHORIZED.
