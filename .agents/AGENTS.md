# Agent Workflow Instructions

This directory contains named workflow prompts for AssetFlow.

## Available Agents

- Use `Small Task Agent` from `small-task-agent.md` for tiny bug fixes, validation changes, renaming, minor refactoring, small endpoints, small SQL corrections, straightforward tests, and configuration fixes.
- Use `Large Feature Agent` from `large-feature-agent.md` for substantial development, cross-component work, architectural decisions, public contracts, database schema changes, Kafka, event sourcing, gRPC, SignalR, blob storage, channel pipelines, infrastructure, or deployment work.

## Related Skills

The workflows coordinate these installed skills:

- `$assetflow-planner`
- `$assetflow-prioritizer`
- `$assetflow-dotnet-implementer`
- `$assetflow-tester`
- `$assetflow-reviewer`
- `$assetflow-knowledge-keeper`
- `$assetflow-github-status`
- `$assetflow-frontend-motion`

When a user names one of these agents, read the corresponding Markdown file in
this directory and follow it as the workflow for the task.

## Progressive Opening

- Open the named agent prompt first.
- Open a skill `SKILL.md` only when that workflow phase is active.
- Open a skill's `references/` files only when the `SKILL.md` routing says the
  current task needs the deeper procedure.
- Prefer `docs/knowledge/` before broad repository exploration, then verify
  important claims against code, tests, docs, and GitHub state.
- Keep small tasks small; escalate to the large workflow only when the scope or
  risk requires it.
