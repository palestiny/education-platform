# Project Charter — v0.1

Status: Draft for Foundation Review  
Date: 2026-09-24  
Product Stage: Pre-Discovery / Foundation  
Implementation: Not Started  
Architecture: Not Started  
Primary Principle: Simple on the surface, powerful underneath.

---

## 1. Executive Definition

The product is a global-ready Educational Operating Platform / Student Success Platform that manages and improves the complete learning journey.

It is not merely a video platform, course marketplace, or traditional LMS.

The platform is intended to make the learning journey:
- clear for students,
- trustworthy for parents,
- productive for teachers,
- manageable for educational organizations,
- scalable for the platform operator.

Foundational Product Statement:

> A global-ready educational operating platform that makes the learning journey clear for students, trustworthy for parents, productive for teachers, manageable for educational organizations, and scalable for the platform operator.

## 2. Vision

Build an education platform where the complexity of education is handled by the platform rather than placed on the user.

The user should experience a simple, comfortable and trustworthy product while the underlying platform can support sophisticated learning, assessment, analytics, orchestration, organizations, commerce and AI capabilities.

## 3. Mission

Help each participant understand:
- where they are,
- where they need to go,
- what evidence exists,
- what should happen next,
- what action they can take.

## 4. Product Philosophy

The platform should feel like the user's "إيده ورجله".

The product should proactively organize the journey without becoming confusing or overwhelming.

Core user questions:
- Student: What should I do now?
- Parent: ابني عامل إيه؟ Why? What should I do?
- Teacher: Who needs me? Why? What evidence supports that?
- Organization: What needs management, attention or intervention?
- Platform Operator: Is the ecosystem healthy, useful, trusted and scalable?

## 5. Product Principles

P01 — Student First: Student outcomes and learning clarity take priority over vanity activity metrics.

P02 — Evidence Before Recommendations: Recommendations should be grounded in available evidence and expose appropriate reasoning/context.

P03 — Progress Over Activity: Learning progress is more important than screen time, clicks, video counts or superficial engagement.

P04 — Parents Need Confidence, Not Surveillance: Parents should receive meaningful visibility without turning the product into unnecessary monitoring.

P05 — Teachers Need More Teaching Time: Automation and tooling should reduce administrative burden rather than create another management system to maintain.

P06 — AI Assists; Humans Remain Accountable: AI may assist analysis, recommendations, content and workflows, but accountability boundaries remain explicit.

P07 — Global by Architecture, Local by Configuration: Localization, language, RTL/LTR, currencies, time zones, academic calendars, curricula, payments and regulatory differences should be configurable rather than hard-coded around one market.

P08 — Trust Is a Product Feature: Privacy, security, transparency, reliability and predictable behavior are product requirements.

P09 — Privacy and Safety by Design: Privacy, child safety, access control, data minimization and abuse prevention are foundational concerns.

P10 — No Hidden Complexity: Complexity may exist internally, but it should not unexpectedly appear in user workflows or implementation behavior.

## 6. Target Ecosystem

Potential participants include Students, Parents, Teachers, Teaching Assistants, Center Managers, Branch Managers, Accountants, Content Managers, Moderators, Support Agents, Organization Administrators, Platform Administrators, Content Creators, Schools, Centers / Academies, Enterprises and Marketplace participants.

This is a candidate ecosystem, not a final permission model.

## 7. Identity Model

The system must not depend on a single permanent UserType enum.

Conceptual model:
Identity + Roles + Organizations + Permissions + Relationships.

A person may hold multiple roles simultaneously, such as Parent + Teacher or Teacher + Center Owner.

Final role and permission design is a Domain and Security Gate decision.

## 8. Multi-Tenancy Vision

Potential tenant types include Individual Teacher, Center, Academy, School, Enterprise and Marketplace / Global Platform.

A tenant may own or manage users, teachers, students, courses, content, branding, domains, pricing, subscriptions, policies, reports and communication.

Tenant isolation is a foundational architectural concern.

Exact tenancy strategy is OPEN until Architecture/Data/Security Gates.

## 9. Learning Model

Course != Video.

A learning experience may include Video, Live Class, Offline Class, Hybrid Class, Assignment, Assessment, Practice, Workshop and Private Session.

Reusable learning objects may include concepts, explanations, examples, questions, exercises, videos, documents, interactive material and assessments.

The final learning domain model remains subject to the Domain Gate.

## 10. Student Learning Profile

The conceptual profile may include curriculum position, skills, estimated mastery, confidence, attempts, misconceptions, strengths, attention areas, pace, attendance, engagement, assessment history, goals, learning history and intervention history.

The product should avoid permanent labels such as "Weak Student" or "Strong Student". Student state should be evidence-based and time-varying.

Example: Algebra — estimated mastery 64%, trend improving, evidence from four assessments.

This example is illustrative, not an approved scoring formula.

## 11. Core Learning Loop

Candidate product loop:
Goal → Plan → Learn → Practice → Assess → Understand → Improve → Repeat

Candidate learning orchestration:
Student → Current State → Goal → Curriculum / Skills → Learning History → Assessment → Evidence → Recommendation → Next Best Action → New Evidence

The exact algorithms and rules are not defined yet.

## 12. Parent Experience

The parent experience should answer:
1. What is happening?
2. Why?
3. What should I do?

The platform should provide meaningful progress and intervention visibility without overwhelming parents with raw telemetry.

Exact parent data visibility and consent boundaries require Product, UX, Privacy and Security decisions.

## 13. Teacher Experience

The teacher experience should answer:
1. Who needs me?
2. Why?
3. What evidence supports that?
4. What action can I take?

The platform should prioritize teaching and intervention over administrative overhead.

## 14. Business Model — Initial Hypothesis

Potential revenue models: Free Core, Student Premium, Teacher SaaS, Center SaaS, Enterprise, Marketplace Commission, B2B and B2B2C.

This is a business-model hypothesis, not a final pricing decision.

## 15. Free Access Principle

The free product should provide genuine utility rather than functioning only as a demo or artificial teaser.

The exact boundary between free and paid capabilities remains OPEN.

## 16. Initial Technical Direction — Candidate Only

Candidate technologies:
- Backend: ASP.NET Core / .NET LTS
- Database: PostgreSQL
- Cache: Redis
- Web: Next.js + TypeScript
- Mobile: React Native
- Object storage: S3-compatible storage
- Video: external video infrastructure initially
- Observability: OpenTelemetry-compatible approach
- CI/CD: GitHub Actions
- Infrastructure: cloud + Infrastructure as Code

These are candidates, not approved architecture.

## 17. Architecture Principle — Candidate

Current architectural direction under consideration:
Modular Monolith + strong module boundaries + explicit contracts + domain events where justified + outbox where required.

The objective is to preserve strong boundaries and future evolution without prematurely accepting the operational complexity of distributed microservices.

This is a candidate direction only. Final architecture requires an Architecture Gate and ADR.

## 18. Initial Domain Candidates

Candidate modules:
Identity & Access; Organizations; Education; Curriculum; Content; Learning; Assessment; Student Profile; Learning Orchestration; Communication; Commerce; Video; Live Learning; Trust & Safety; Analytics; AI; Notifications; Search; Platform Administration.

These are discovery inputs, not final bounded contexts.

## 19. Non-Goals at This Stage

The project is not currently authorized to:
- begin feature implementation,
- finalize microservices,
- finalize database schema,
- finalize APIs,
- finalize pricing,
- finalize AI behavior,
- build proprietary video infrastructure,
- assume Egypt-only product rules,
- invent requirements during coding.

## 20. Quality Doctrine

1. If not defined, not implemented.
2. If not tested, not proven.
3. If not documented, not an accepted decision.
4. Silent behavior changes are defects.
5. Root cause before declaring a fix.
6. A workaround is not a root-cause fix unless explicitly classified.
7. Important assumptions must be visible.
8. Important decisions have an owner and rationale.
9. Completed milestones require verification.
10. No implementation before the relevant Design Gate passes.

## 21. Evidence Classification

- VERIFIED FACT — supported by a reliable source or direct project evidence.
- USER REPORT — reported experience not independently established.
- REPEATED PATTERN — pattern supported by multiple credible/relevant observations.
- INFERENCE — reasoned interpretation from evidence.
- PRODUCT OPPORTUNITY — proposed opportunity derived from evidence.
- OPEN QUESTION — unresolved point requiring evidence or decision.

Interpretation based on evidence must not be presented as verified fact.

## 22. Gap Management

Unresolved issues are recorded in the Gap Register. A gap must not be silently converted into implementation behavior.

Minimum fields: Gap ID, Area, Description, Impact, Severity, Owner, Required Decision, Status, Resolution, Verification.

Statuses: OPEN, IN PROGRESS, BLOCKED, RESOLVED, NOT PROVEN.

## 23. Decision Management

Important decisions must be recorded with context, problem, options, trade-offs, decision, rationale, consequences, evidence, status, owner and date.

Architecture decisions receive individual ADR documents.

## 24. Change Control

New Evidence → Impact Analysis → Change Proposal → Affected Requirements / Architecture / APIs / Data / Tests → Decision → Implementation → Verification.

No major design change should be introduced silently during implementation.

## 25. Definition of Done

A feature is not complete until applicable items are verified: requirement, business rules, UX, architecture, security, data, API contract, edge cases, tests, implementation, observability, documentation, regression verification and acceptance evidence.

## 26. Design Gate Model

Candidate sequence:
1. Product Gate
2. Research Gate
3. Requirements Gate
4. Domain Gate
5. UX Gate
6. Architecture Gate
7. Security Gate
8. Data Gate
9. API Contract Gate
10. Implementation Gate
11. Testing Gate
12. Release Gate
13. Verification Gate

Gate statuses: PASS, GAP, NOT PROVEN, BLOCKED.

NOT PROVEN never equals PASS.

## 27. Root-Cause Bug Workflow

Symptom → Reproduce → Evidence → Root Cause → Fix → Regression Test → Verification → Close.

If a defect recurs, investigate why the previous control failed and strengthen the system rather than repeatedly applying local workarounds.

## 28. M0 — Foundation

Expected outcomes: Project Charter, Working Rules, Definition of Done, Decision Register, Gap Register, Risk Register, Checkpoints, Competitive Intelligence plan, evidence classification and research traceability.

No product implementation is required for M0.

## 29. Current Status

Stage: Pre-Discovery / Foundation.  
Completed: Initial product framing and governance doctrine documented.  
In Progress: Repository foundation and Competitive Intelligence preparation.  
Implementation: Not started.  
Architecture: Not started.  
Next Gate: Product Foundation / Competitive Intelligence Gate.

## 30. Immediate Next Work

Competitive Intelligence should compare relevant Egypt/MENA and global products across business model, target users, geography, pricing, free/paid boundaries, student/parent/teacher/organization journeys, content, video/live/offline learning, assignments, assessment, progress, AI/recommendations, communication, payments, marketplace, trust/safety/privacy, notifications, search, mobile, performance, accessibility, internationalization, evidence-backed weaknesses, repeated complaints, unserved needs and opportunities.

Research must distinguish verified facts from user reports, repeated patterns, inference and opportunity.

## 31. Charter Status

Version: 0.1  
Status: Draft  
Next Gate: Product Foundation / Competitive Intelligence Gate  
Implementation Authorization: Not granted
