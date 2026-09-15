---
name: assetflow-reviewer
description: Review AssetFlow diffs as a senior quality gate for requirements, architecture, correctness, security, performance, observability, tests, and maintainability.
metadata:
  short-description: Review diffs and quality gates
---

# AssetFlow Reviewer

Use this skill for final review, requested review, or when implementation needs a senior quality gate. Inspect the actual diff and relevant surrounding code.

Review against:

- original request, requirements, acceptance criteria, and plan
- architecture, coding standards, public contracts, and repository conventions
- tests, security, performance, observability, maintainability, and deployment impact

Look specifically for missing requirements, unnecessary complexity, architectural violations, duplicated logic, poor naming, inappropriate abstractions, concurrency problems, resource leaks, incorrect async behavior, missing cancellation, SQL issues, transaction problems, Kafka reliability problems, event consistency problems, security issues, insufficient tests, breaking contracts, and avoidable performance regressions.

## Feedback Loop

Classify issues by severity and route work accordingly:

- Minor: naming, cleanup, edge-case test, or localized mistake. Fix, test, and review again.
- Moderate: approach is wrong, several tasks need revision, or a contract needs adjustment. Return to implementation, update plan tasks if needed, test, and review again.
- Major: requirement misunderstood, architecture wrong, core assumption invalid, feature scope changed, or event model incorrect. Return to planning, then prioritize, implement, test, and review.

Never preserve a bad implementation just because it matches an outdated plan. The product need wins over the plan document.

When the user asks for a code review, lead with findings ordered by severity and include file and line references where available.
