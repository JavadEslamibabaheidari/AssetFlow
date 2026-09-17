# AssetFlow Codex Skills

This directory contains project-local Codex skills for AssetFlow's engineering workflow.

## Workflow

```text
Understand -> Plan -> Prioritize -> Implement -> Test -> Review -> Learn
```

For substantial work, use:

```text
Knowledge -> Understand -> Plan -> Prioritize -> Implement -> Test -> Review -> Planned-State Validation -> Knowledge Update
```

## Skills

- `$assetflow-planner`: understand requirements and create/update plans.
- `$assetflow-prioritizer`: order planned tasks by dependency, value, uncertainty, and risk.
- `$assetflow-dotnet-implementer`: implement production .NET backend changes.
- `$assetflow-tester`: verify changes with risk-based tests.
- `$assetflow-reviewer`: inspect diffs as the senior quality gate.
- `$assetflow-knowledge-keeper`: maintain compact project knowledge and enforce planned-state validation after milestones, big issues, and features.

The named agent entrypoints live in `.agents/`.
