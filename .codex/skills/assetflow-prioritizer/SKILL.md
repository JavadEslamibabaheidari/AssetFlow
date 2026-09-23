---
name: assetflow-prioritizer
description: Prioritize AssetFlow implementation tasks by dependency, risk, feedback speed, and business value before execution.
metadata:
  short-description: Order planned work safely
---

# AssetFlow Prioritizer

Use this skill after planning substantial work, when tasks need an execution order. Do not spend much time prioritizing trivial fixes.

## Open Only What You Need

- For a short task list, use this file only and produce an executable order.
- For milestone or multi-phase sequencing, open `references/prioritization.md`.
- Record priority decisions in the relevant `docs/plans/` file when one exists.

Do not simply execute tasks in the order they were written. Build an order that
accounts for dependencies, foundations, value, uncertainty, feedback speed, and
integration risk.

Prefer early work that reduces uncertainty. For example, prove a risky PostgreSQL design, Kafka interaction, SignalR flow, or integration boundary before building a large feature around it.

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
