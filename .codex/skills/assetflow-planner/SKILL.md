---
name: assetflow-planner
description: Plan meaningful AssetFlow .NET backend work before implementation, including requirements, architecture impact, task breakdown, and living docs.
metadata:
  short-description: Plan AssetFlow backend changes
---

# AssetFlow Planner

Use this skill when a request needs deliberate understanding before coding: new features, cross-component changes, public contract changes, data model changes, messaging, realtime workflows, infrastructure changes, or ambiguous maintenance work. Keep tiny fixes lightweight.

## Open Only What You Need

- For a compact plan or ambiguity check, use this file only.
- For substantial plans, milestone plans, start gates, issue breakdowns, or ADR decisions, open `references/planning-workflow.md`.
- For tracked work, also use `$assetflow-github-status`.
- For durable architecture/product context, also use `$assetflow-knowledge-keeper`.

Start by inspecting project knowledge in `docs/knowledge/` when it exists, then
verify important facts against actual code, tests, configuration, and docs.
Knowledge is a cache; code wins when they disagree.

Prefer the simplest architecture that satisfies the requirement with room for reasonable evolution. Do not introduce microservices, Kafka, event sourcing, SignalR, gRPC, channels, or new abstractions unless the problem benefits from them.

Discover before asking. Ask the user only when ambiguity materially changes
product behavior or architecture and cannot reasonably be resolved from
repository context.
