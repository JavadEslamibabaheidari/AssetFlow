---
name: assetflow-planner
description: Plan meaningful AssetFlow .NET backend work before implementation, including requirements, architecture impact, task breakdown, and living docs.
metadata:
  short-description: Plan AssetFlow backend changes
---

# AssetFlow Planner

Use this skill when a request needs deliberate understanding before coding: new features, cross-component changes, public contract changes, data model changes, messaging, realtime workflows, infrastructure changes, or ambiguous maintenance work. Keep tiny fixes lightweight.

## Workflow

Start by inspecting project knowledge in `docs/knowledge/` when it exists, then verify important facts against actual code, tests, configuration, and docs. Knowledge is a cache; code wins when they disagree.

Discover before asking. Ask the user only when ambiguity materially changes product behavior or architecture and cannot reasonably be resolved from repository context.

Identify:

- user intent, business requirements, functional requirements, and non-functional requirements
- assumptions, constraints, dependencies, risks, unknowns, and affected components
- existing conventions, contracts, architecture boundaries, tests, and deployment impact
- whether the work touches APIs, domain model, PostgreSQL, Dapper SQL, Kafka, event sourcing, projections, gRPC, SignalR, blob storage, background services, channels, configuration, infrastructure, deployment, or frontend

Prefer the simplest architecture that satisfies the requirement with room for reasonable evolution. Do not introduce microservices, Kafka, event sourcing, SignalR, gRPC, channels, or new abstractions unless the problem benefits from them.

## Plans

Store substantial plans in `docs/plans/`. Plans are living documents and should be updated when implementation or review changes reality.

Before planning a new milestone or creating its GitHub issues, run the previous milestone closure hook from `docs/ai-development-workflow.md`. Check whether the previous milestone already has a current implementation report, synchronized `docs/knowledge/`, and matching GitHub status. If those artifacts already exist and match reality, record that the hook passed and continue. If they are missing, stale, or too thin to be trusted as the first read path, update them before writing the new milestone plan or issues.

Milestone plans must pass the start gate before implementation begins. For every milestone, make sure the plan states the goal, scope, non-goals, deliverables, acceptance criteria, task breakdown, dependencies, risks, testing strategy, GitHub tracking, and durable knowledge updates.

Before marking a milestone ready, resolve every implementation-blocking product, architecture, data, API, infrastructure, rollout, and testing decision. The plan must not contain unresolved placeholders such as `TBD`, `TODO`, `unknown`, `undecided`, or open questions that affect implementation. If a question will not be decided in the milestone, move it explicitly to out of scope or deferred decisions.

For substantial work, use only as much hierarchy as the task needs:

```text
Epic -> Feature -> User Story -> Task
```

For each concrete task, include the useful subset of:

- ID, title, objective, rationale, affected components, dependencies
- acceptance criteria, implementation notes, testing expectations
- risk or complexity, status

For small fixes, produce a compact plan in the response or current task notes rather than manufacturing epics and stories.

## Architecture Decisions

Create lightweight ADRs in `docs/decisions/` for important architectural choices such as Kafka adoption, REST vs gRPC, event sourcing, Dapper persistence strategy, channels, SignalR, or service boundaries. Include context, decision, alternatives, and consequences. Do not create ADRs for trivial implementation details.
