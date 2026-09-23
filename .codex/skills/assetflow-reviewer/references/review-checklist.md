# AssetFlow Review Checklist

Open this reference for substantial diffs, final quality gates, architecture or
contract changes, security/performance risk, or requested reviews.

## What To Inspect

Look specifically for:

- missing requirements or acceptance criteria
- unnecessary complexity
- architectural violations
- duplicated logic
- poor naming
- inappropriate abstractions
- concurrency problems
- resource leaks
- incorrect async behavior or missing cancellation
- SQL issues, transaction problems, query performance, and migration mistakes
- Kafka reliability problems
- event consistency problems
- security issues
- insufficient tests
- breaking public contracts
- avoidable performance regressions
- observability or deployment gaps

## Feedback Loop

Classify issues by severity and route work accordingly:

- Minor: naming, cleanup, edge-case test, or localized mistake. Fix, test, and
  review again.
- Moderate: approach is wrong, several tasks need revision, or a contract needs
  adjustment. Return to implementation, update plan tasks if needed, test, and
  review again.
- Major: requirement misunderstood, architecture wrong, core assumption invalid,
  feature scope changed, or event model incorrect. Return to planning, then
  prioritize, implement, test, and review.
