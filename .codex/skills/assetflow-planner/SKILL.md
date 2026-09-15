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
