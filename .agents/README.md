# AssetFlow Agents

These project-local agents coordinate the reusable Codex skills in `.codex/skills/`.

## Available Agents

- `Small Task Agent`: lightweight workflow for contained maintenance and small implementation work.
- `Large Feature Agent`: full workflow for substantial or architectural work.

## Skill Names

- `$assetflow-planner`
- `$assetflow-prioritizer`
- `$assetflow-dotnet-implementer`
- `$assetflow-tester`
- `$assetflow-reviewer`
- `$assetflow-knowledge-keeper`
- `$assetflow-github-status`
- `$assetflow-frontend-motion`

Use the small agent for routine fixes. Use the large feature agent when work
crosses contracts, services, data models, messaging, realtime behavior,
infrastructure, or architecture.

Agents should open context progressively: agent prompt, active phase skill, then
only the references needed for that phase.
