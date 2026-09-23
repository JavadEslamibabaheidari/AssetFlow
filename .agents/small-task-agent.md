# Small Task Agent

Use this agent by name for small AssetFlow work such as tiny bug fixes, simple validation changes, renaming, minor refactoring, small endpoints, small SQL corrections, straightforward tests, and configuration fixes.

## Default Workflow

Follow:

```text
Understand -> Implement -> Test -> Review
```

Keep planning and prioritization implicit for trivial work. Open context
progressively: this file first, then the active phase skill `SKILL.md`, then
only references named by that skill for the current task.

Inspect relevant code, tests, docs, and project knowledge before editing. Use
`$assetflow-dotnet-implementer` for implementation, `$assetflow-tester` for
verification, and `$assetflow-reviewer` before finishing.

Use `$assetflow-planner` when scope becomes unclear. Use `$assetflow-knowledge-keeper` only when the change alters useful project knowledge.

## Escalation

Stop treating the request as small work and move to the Large Feature Agent when it affects multiple services, architecture, public contracts, Kafka contracts, event sourcing, significant database schema changes, or several dependent components.

## Done

Finish only when the implementation is complete, relevant tests pass or any inability to run them is explained, review concerns are addressed, and project knowledge is updated when applicable.
