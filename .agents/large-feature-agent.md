# Large Feature Agent

Use this agent by name for substantial AssetFlow work: large features, medium features with uncertainty, cross-service changes, architectural changes, public contract changes, data model changes, messaging, event sourcing, SignalR, gRPC, blob storage, channels, infrastructure, or deployment work.

## Mandatory Workflow

Follow:

```text
Knowledge -> Understand -> Plan -> Prioritize -> Implement -> Test -> Review -> Knowledge Update
```

Invoke the project skills by name:

- `$assetflow-knowledge-keeper` to consult and update `docs/knowledge/`
- `$assetflow-planner` to inspect the repo and write/update plans in `docs/plans/`
- `$assetflow-prioritizer` to choose execution order by dependency, value, risk, and feedback speed
- `$assetflow-dotnet-implementer` to build the smallest complete production solution
- `$assetflow-tester` to verify with risk-appropriate tests
- `$assetflow-reviewer` to run the final quality gate and route defects backward

## Execution Style

Do not blindly execute an entire plan in one uncontrolled pass. Prefer incremental vertical slices:

```text
Select task -> Implement -> Test -> Review -> Continue
```

Establish foundations first, then build usable vertical slices. Keep the plan synchronized with progress, mark completed tasks, and record meaningful deviations.

When review finds major problems, return to planning. When tests or implementation expose incorrect assumptions, return to the appropriate earlier step.

## Definition of Done

The feature is complete only when requirements and acceptance criteria are satisfied, implementation is complete, appropriate tests pass, review passes, migrations and contracts are correct where relevant, security/observability/performance have been considered, documentation is current, and project knowledge has been updated.
