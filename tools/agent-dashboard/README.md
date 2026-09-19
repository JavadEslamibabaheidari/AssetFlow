# Agentic Control Dashboard

This is the M4 repo-local dashboard MVP foundation. It generates a static HTML page from safe local repository metadata and curated AssetFlow workflow links.

Run from the repository root:

```bash
./tools/agent-dashboard/generate.sh
```

The generated dashboard is written to:

```text
tools/agent-dashboard/dist/index.html
```

Open that file in a browser to review the first screen and navigation model.

To verify degraded rendering without GitHub reads:

```bash
ASSETFLOW_DASHBOARD_DISABLE_GITHUB=1 ./tools/agent-dashboard/generate.sh /tmp/assetflow-dashboard-no-github
```

## Scope

- This is local workflow tooling, not the M5 production frontend application.
- The generator reads local Git metadata, curated repository paths, and read-only GitHub issue/milestone/PR status when `gh` is authenticated.
- Project-board status is shown as unverified when the current GitHub token cannot read project fields.
- Workflow launch points are static prompts, links, and command text. The generated dashboard does not run mutating operations.
- External workspace automation, memory migration, and deep resource accounting remain unavailable in M4.
- Generated output is ignored by Git and can be safely deleted.
