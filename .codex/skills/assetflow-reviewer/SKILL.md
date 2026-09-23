---
name: assetflow-reviewer
description: Review AssetFlow diffs as a senior quality gate for requirements, architecture, correctness, security, performance, observability, tests, and maintainability.
metadata:
  short-description: Review diffs and quality gates
---

# AssetFlow Reviewer

Use this skill for final review, requested review, or when implementation needs a senior quality gate. Inspect the actual diff and relevant surrounding code.

## Open Only What You Need

- For small diffs, inspect the diff, touched code, and tests using this file only.
- For substantial diffs, architecture, contracts, security/performance risk, or final quality gates, open `references/review-checklist.md`.
- When the user asks for a code review, lead with findings ordered by severity and include file/line references.

Review against the original request, requirements, acceptance criteria, plan,
architecture, conventions, public contracts, tests, security, performance,
observability, maintainability, and deployment impact.

Never preserve a bad implementation just because it matches an outdated plan.
The product need wins over the plan document.
