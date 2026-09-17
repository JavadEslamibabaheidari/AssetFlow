# AI Development Workflow

AssetFlow uses a spec-first, AI-assisted workflow. Meaningful features should move from requirement intake to spec, task breakdown, implementation, verification, review, and project knowledge updates.

Small fixes can stay lightweight, but features, public contracts, data changes, infrastructure, messaging, or milestone work must have enough written context that implementation can be checked against the plan.

## Workflow

```text
Requirement Intake -> Feature Spec -> Task Breakdown -> Implementation -> Verification -> Review -> Knowledge Update
```

For tracked GitHub work, keep one task per branch and pull request unless the user explicitly asks to combine tasks.

## Requirement Intake Template

Use this template before writing a spec for meaningful work.

```markdown
# Requirement Intake: <title>

## Problem

What user, operator, or system problem are we solving?

## Desired Outcome

What should be true when this work is complete?

## Users and Actors

- Primary user:
- Other actors:
- External systems:

## Scope

In scope:

- 

Out of scope:

- 

## Constraints

- Product constraints:
- Technical constraints:
- Operational constraints:

## Open Questions

- 

## GitHub Tracking

- Issue:
- Milestone:
- Labels:
```

## Feature Spec Template

Use this template for features and other meaningful changes.

```markdown
# Feature Spec: <title>

## Summary

Short description of the planned behavior.

## Goals

- 

## Non-Goals

- 

## User Stories

- As a <user>, I want <capability>, so that <outcome>.

## Acceptance Criteria

- [ ] 

## API Contract

- OpenAPI file:
- New endpoints:
- Changed endpoints:
- Request schemas:
- Response schemas:
- Error cases:

## Data and Persistence

- Tables or migrations:
- Query patterns:
- Transaction or concurrency concerns:

## Events and Integrations

- Published events:
- Consumed events:
- External systems:

## Observability

- Logs:
- Metrics:
- Traces:
- Alerts:

## Security and Permissions

- Authentication:
- Authorization:
- Sensitive data:

## Testing Plan

- Unit tests:
- Integration tests:
- Contract tests:
- Concurrency or failure tests:

## Rollout and Compatibility

- Migration path:
- Backward compatibility:
- Operational steps:
```

## Task Breakdown Template

Use this template to split a spec into focused GitHub tasks.

```markdown
# Task Breakdown: <feature>

## Task: <title>

Objective:

Acceptance criteria:

- [ ] 

Affected files or components:

- 

Dependencies:

- 

Verification:

- 

PR scope:

- One task per PR unless explicitly combined.
```

## Review Checklist

Use this checklist before merging meaningful work.

- The implementation matches the GitHub issue, spec, and acceptance criteria.
- The current app state matches what was planned, or deviations are documented.
- OpenAPI contracts and API behavior agree.
- Data changes are safe, reversible where practical, and tested.
- Error handling follows the project style.
- Tests cover the risk level of the change.
- CI passes.
- Docs and `docs/knowledge/` are updated when durable facts changed.
- The PR is focused on one task.
- Follow-up work is tracked in GitHub.

## Model Selection Guidance

Use faster or cheaper models for:

- formatting and small documentation edits
- issue template cleanup
- simple scripted transformations
- small implementation tasks with clear examples
- summarizing already-inspected local context

Use stronger models for:

- feature planning and architecture choices
- public API contract design
- database, concurrency, reservation, or consistency logic
- security, reliability, and performance review
- debugging failures with uncertain causes
- final review before merging substantial work

Escalate model strength when the cost of a wrong answer is high, when multiple subsystems are involved, or when the task requires careful tradeoff analysis.

## Reusable Skills

When a workflow becomes repeatable, extract it into a skill. Each AssetFlow skill should document:

- purpose
- when to use it
- required inputs
- expected output
- verification expectations
- important failure modes

## Monitoring AI Work

Track AI-assisted work across:

- quality: did the output match the spec?
- efficiency: how many iterations were needed?
- consumption: which model was used, and what was the approximate cost?
- drift: did implementation diverge from the plan, and was that captured?

The project may later add productized AI monitoring for these signals.
