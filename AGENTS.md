# Repository Instructions

- The canonical workspace for this project is `/mnt/data/AssetFlow`.
- Run Git, build, test, Docker, and workflow commands from `/mnt/data/AssetFlow` unless the user explicitly says otherwise.
- Do not use `/home/javad/Documents/ChatGPT/AssetFlow` for this project; that path may contain stale copies from earlier workspace setup.

## AssetFlow Codex Workflows

- Active reusable skills are installed under `~/.codex/skills/` and source copies live in `.codex/skills/`.
- Invoke skills by name when the phase matters:
  - `$assetflow-planner`
  - `$assetflow-prioritizer`
  - `$assetflow-dotnet-implementer`
  - `$assetflow-tester`
  - `$assetflow-reviewer`
  - `$assetflow-knowledge-keeper`
  - `$assetflow-github-status`
- Named workflow prompts live in `.agents/`:
  - `Small Task Agent` for contained fixes and simple maintenance.
  - `Large Feature Agent` for substantial features, contract changes, data changes, messaging, realtime, infrastructure, or architecture work.
- For large work, use the loop `GitHub Status -> Knowledge -> Understand -> Plan -> Prioritize -> Implement -> Test -> Review -> Knowledge Update -> GitHub Status`.
- For small work, keep the loop light: `Understand -> Implement -> Test -> Review`.
- For any tracked GitHub work, inspect the relevant issue/task/milestone before starting and update GitHub plus local status docs before calling the work complete.
- Do tracked tasks one by one in separate branches and separate PRs unless the user explicitly asks to combine them.

## Frontend Motion

- Use `$assetflow-frontend-motion` for AssetFlow frontend implementation, review, or polish work that touches animated UI behavior in `src/AssetFlow.Web`.
- Prefer the maintained Motion package for React animation. Framer Motion is now distributed upstream as Motion; import React APIs from `motion/react`.
