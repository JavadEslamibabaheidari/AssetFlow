---
name: assetflow-prioritizer
description: Prioritize AssetFlow implementation tasks by dependency, risk, feedback speed, and business value before execution.
metadata:
  short-description: Order planned work safely
---

# AssetFlow Prioritizer

Use this skill after planning substantial work, when tasks need an execution order. Do not spend much time prioritizing trivial fixes.

## Prioritization Criteria

Do not simply execute tasks in the order they were written. Build an order that accounts for:

- dependencies and blockers
- architectural foundations
- business value and user-visible progress
- uncertainty and technical risk
- feedback speed and testing ability
- integration, migration, and infrastructure requirements

Prefer early work that reduces uncertainty. For example, prove a risky PostgreSQL design, Kafka interaction, SignalR flow, or integration boundary before building a large feature around it.

## Output

Produce an executable order. When useful, classify tasks by priority:

- Critical
- High
- Medium
- Low

Also classify task type when it clarifies sequencing:

- prerequisite
- implementation
- integration
- verification
- cleanup

Record priority decisions in the relevant plan under `docs/plans/` when a plan document exists.
