# Global Education Feature Baseline

Date: 2026-09-28
Status: Research Working Baseline
Gate: Product Foundation / Competitive Intelligence — NOT PROVEN
Implementation authorization: None

## Purpose

Establish a current global feature baseline from established education products so the project can distinguish:

1. conventional market parity;
2. mature advanced capabilities;
3. current AI/new-product capabilities;
4. operational and trust expectations;
5. genuine differentiation hypotheses.

This document is a market baseline, not an implementation backlog and not a competitor ranking.

## Evidence Rule

Current product documentation demonstrates that a capability exists or is being offered. It does not by itself prove effectiveness, adoption, superiority, or user pain prevalence.

Direct workflow research remains targeted to:
- uncertain or segment-specific requirements;
- materially different workflows;
- proposed differentiators;
- prevalence/severity/cost claims;
- high-impact semantics that cannot safely be inferred from market evidence.

## Market Baseline — 2026

### 1. Core / Established

| Capability | Current market signal | Product implication |
|---|---|---|
| Identity, classes, rosters | Mature | Parity foundation |
| Course/content delivery | Mature | Parity foundation |
| Video/on-demand learning | Mature | Parity where relevant |
| Assignments/homework | Mature | Parity foundation |
| Practice | Mature | Parity foundation |
| Quizzes/exams/question banks | Mature | Parity foundation |
| Grading/feedback/rubrics | Mature | Parity foundation |
| Progress/gradebook | Mature | Parity foundation |
| Teacher dashboard/insights | Mature | Parity foundation |
| Communication/notifications | Mature | Parity foundation |
| Scheduling/calendar | Mature | Segment-dependent parity |
| Attendance | Mature in managed learning | Segment-dependent parity |
| Parent visibility | Established in K-12 products | Segment-dependent parity |
| Mobile + web | Mature | Expected for broad platform ambition |
| Localization / multilingual | Established | Global architecture requirement |
| Integrations | Mature platforms increasingly support them | Scale/ecosystem requirement |

Google Classroom currently documents assignments, grading, progress, notifications, rubrics, multiple classes, mobile apps, communication and SIS integrations. Khan Academy documents assignments, progress, mastery and teacher insights. These establish the maturity of the conventional baseline. [1][2]

### 2. Mature Advanced

| Capability | Current market signal | Product implication |
|---|---|---|
| Mastery/skill progression | Strong | Quality/parity benchmark |
| Adaptive/personalized assessment | Strong | Mature advanced capability |
| AI-assisted teacher planning | Strong | Expected advanced capability |
| AI-generated assessments | Commercially exposed | Mature advanced capability |
| Interactive video | Commercially exposed | Advanced capability |
| Human teacher escalation | Established | Important recovery/help pattern |
| Analytics and actionable insights | Mature | Progress should lead to action |
| Curriculum alignment | Strong in K-12 | Configurable domain capability |
| Multi-role / school operations | Mature | Required for organization-oriented products |
| Payments/fees | Mature in operations products | Required when business workflow depends on it |

Khan Academy's current teacher experience combines mastery levels, assignments, progress insights and AI teacher tools. Classera exposes adaptive exams, AI-assisted exam generation, interactive video, chatbot and teaching-assistant capabilities. [1][3]

### 3. Current AI / New Capability Baseline

| Capability | Current signal | Product implication |
|---|---|---|
| AI tutoring | Commercially exposed | Baseline AI category |
| AI homework help | Commercially exposed | Baseline AI category |
| AI practice-test generation | Commercially exposed | Baseline AI category |
| AI study-guide generation | Commercially exposed | Baseline AI category |
| AI summarization | Commercially exposed | Commodity capability |
| AI flashcard generation | Commercially exposed | Commodity capability |
| AI lesson planning | Commercially exposed | Teacher productivity capability |
| AI grading/feedback assistance | Commercially exposed | Human-accountable workflow |
| AI conversational practice | Commercially exposed | Newer learner-experience category |
| AI roleplay/simulation | Commercially exposed | Newer learner-experience category |
| AI voice/hands-free interaction | Emerging commercial capability | UX/operations opportunity |
| AI-grounded/RAG assistance | Emerging ecosystem capability | Trust/context opportunity |
| AI provider abstraction | Emerging platform capability | Architecture concern, not product differentiator by itself |
| AI/MCP action interfaces | Emerging 2026 ecosystem signal | Security/audit boundary required |

Quizlet currently exposes AI-generated practice tests, study guides, PDF summarization, flashcard generation and AI homework help. Duolingo exposes AI conversational Video Call and Roleplay with post-conversation feedback. Moodle's 2026 ecosystem exposes multiple AI providers, RAG-grounded assistance and controlled AI integrations. [4][5][6]

### 4. Operational / Trust Baseline

The product should treat the following as first-class capabilities rather than technical afterthoughts:

- authentication/session continuity;
- resumable learning;
- duplicate/replay protection;
- reliable submission and assessment state;
- recoverable external-provider failures;
- clear delivery status for communication;
- permission-aware visibility;
- auditability of important actions;
- privacy and consent;
- accessibility;
- localization without business-logic contamination;
- support/recovery context;
- observability.

These are quality requirements around parity capabilities, not optional polish.

## What Is Actually New?

The market is no longer differentiated simply by adding another LMS feature or another AI chatbot.

The more current product frontier is moving toward:

**Context → AI assistance → evidence → personalized action → feedback → updated state**

Examples include:
- Khan Academy connecting mastery state to teacher insights and next support actions. [1]
- Classera connecting interaction data, adaptive exams, AI-generated assessments and teacher assistance. [3]
- Quizlet turning learner material into multiple practice artifacts. [4]
- Duolingo turning conversational practice into an adaptive feedback loop. [5]
- Moodle extending AI through configurable providers, RAG and controlled action interfaces. [6]

The product opportunity is therefore not to claim that these capabilities are unique. The open question is whether we can make the complete cross-role learning workflow substantially simpler, more reliable, explainable and coherent.

## Initial Feature Classification

### PARITY FOUNDATION
Identity, content, learning activity, assignments, practice, assessments, feedback, progress, teacher workflow, communication, notifications, mobile/web, localization, privacy, reliability.

### ADVANCED PARITY
Mastery, actionable teacher insights, adaptive assessment, curriculum alignment, parent visibility, scheduling/attendance/payments where the segment requires them, integrations.

### AI BASELINE
Tutoring, homework help, practice generation, study-guide generation, summarization, flashcards, teacher planning, feedback assistance.

### CURRENT AI / UX FRONTIER
Conversational practice, roleplay/simulation, multimodal input, voice/hands-free interaction, grounded/RAG assistance, controlled action-taking assistants.

### SCALE / ECOSYSTEM
Multi-tenancy, white-label, marketplace, enterprise integrations, advanced analytics, content ecosystem, community, certificates, large-scale gamification.

## Product Consequence

The working strategy remains:

**Parity → Reliability → Simplicity → Workflow Quality → Evidence-backed Differentiation.**

We should not block conventional product planning because no direct user case exists for a standard feature.

We should also not promote a novel workflow to a committed requirement merely because a competitor markets a similar feature.

## Sources

[1] Khan Academy — Teachers / educator experience: https://www.khanacademy.org/teachers
[2] Google for Education — Classroom features: https://edu.google.com/workspace-for-education/products/classroom/editions/
[3] Classera — LMS: https://classera.com/en/products/classera-lms/
[4] Quizlet — AI study tools: https://quizlet.com/features/ai-study-tools
[5] Duolingo — Duolingo Max: https://blog.duolingo.com/duolingo-max/
[6] Moodle Marketplace — AI ecosystem/providers: https://marketplace.moodle.com/

## Gate Status

**NOT PROVEN**

This baseline is sufficient to continue competitive-parity and requirements work. It does not select the beachhead, finalize MVP scope, approve architecture, or authorize implementation.
