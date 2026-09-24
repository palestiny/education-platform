# Direct Validation Gating Checklist

Status: Research Instrument  
Current Gate: Product Foundation / Competitive Intelligence — NOT PROVEN  
Implementation authorization: None

## Purpose

Define the evidence review required before moving from direct workflow research into product requirements. This is a qualitative decision framework; it intentionally does not assign a numeric score.

## Evidence completeness

Review whether the research contains:

- [ ] Real recent cases, not only hypothetical opinions.
- [ ] More than one relevant role where the hypothesis requires cross-role coordination.
- [ ] A reconstructable trigger.
- [ ] Evidence and evidence quality.
- [ ] Interpretation and uncertainty.
- [ ] Decision ownership.
- [ ] Action ownership.
- [ ] Intervention details.
- [ ] Communication/context handoff.
- [ ] Due state and follow-up.
- [ ] Outcome evidence.
- [ ] Closure behavior.
- [ ] Existing workaround.
- [ ] Material cost or friction.
- [ ] Failure/recovery behavior.
- [ ] Privacy/consent boundaries.
- [ ] AI/human accountability where relevant.
- [ ] Contradictory evidence.

## Product Foundation decision tests

### PASS

Use only when the evidence supports all of the following:

- The workflow is recurring within a clearly identified segment.
- The problem is materially costly or frustrating rather than merely inconvenient.
- Current alternatives leave an important coordination gap.
- Evidence/context/ownership/follow-up/outcome loss is demonstrated rather than assumed.
- A meaningful outcome can be measured.
- Privacy and role boundaries can be defined responsibly.
- The problem remains after considering contradictory evidence.
- A product intervention can be described without inventing requirements that research did not establish.

### GAP

Use when the workflow appears meaningful but one or more critical dimensions remain insufficiently evidenced, such as frequency, cost, outcome, ownership, segment fit, or privacy.

The next research action must name the missing evidence.

### NOT PROVEN

Use when available evidence is insufficient to establish a meaningful unmet need. This includes cases where evidence is mostly market capability inventory, opinions, hypothetical scenarios, or isolated reports.

### REJECT / PIVOT

Use when direct evidence contradicts the hypothesis strongly enough that the current problem framing should be abandoned or materially changed.

Record the observed problem that replaces it, if any.

## Gate review record

Date: TBD

### Evidence reviewed

- Case records:
- Participant roles:
- Segment:
- Supporting artifacts:
- Contradictory evidence:

### Findings

- Recurrence:
- Cost/friction:
- Fragmentation:
- Ownership:
- Follow-up:
- Closure:
- Outcome:
- Workaround:
- Failure/recovery:
- Privacy/consent:
- AI/human accountability:

### Decision

- Gate outcome: NOT PROVEN
- Rationale:
- Evidence gaps:
- Next research action:
- Implementation authorization: NO

## Guardrail

Passing this research gate does not automatically approve architecture or implementation. It only authorizes progression into the next product/requirements gate. Architecture, domain, UX, API, data and security decisions remain subject to their own gates.
