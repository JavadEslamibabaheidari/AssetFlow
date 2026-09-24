# Agentic OS Dashboard

This is the repo-local Agentic OS cockpit. It generates an interactive local HTML page from safe local repository metadata, M9 GitHub tracking when available, and curated AssetFlow workflow controls.

Run from the repository root:

```bash
./tools/agent-dashboard/generate.sh
```

The generated dashboard is written to:

```text
tools/agent-dashboard/dist/index.html
```

Open that file in a browser to review the cockpit, charts, bars, workflow graph, operating modes, approval gates, status, and controls.

To verify degraded rendering without GitHub reads:

```bash
ASSETFLOW_DASHBOARD_DISABLE_GITHUB=1 ./tools/agent-dashboard/generate.sh /tmp/assetflow-dashboard-no-github
```

## Scope

- This is local workflow tooling, not the M5+ production frontend application.
- The generator reads local Git metadata, curated repository paths, and read-only GitHub issue/milestone/PR status when `gh` is authenticated.
- Project-board status is shown as unverified when the current GitHub token cannot read project fields.
- Workflow launch points are prompts, links, local UI, and command text. The generated cockpit does not run mutating operations.
- The cockpit includes client-side interaction for live clock updates, animated bars, mode tabs, and workflow-node highlighting.
- Memory, automation, resource telemetry, research, and integration sections are local-first and Git-reviewable.
- Provider token/cost data, external connector state, and project-board columns are shown as unavailable/degraded when the repository cannot verify them.
- Secrets, raw prompts, raw transcripts, personal/private data, and generated output must not be committed.
- Generated output is ignored by Git and can be safely deleted.
