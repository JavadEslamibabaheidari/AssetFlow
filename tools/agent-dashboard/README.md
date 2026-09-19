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

## Scope

- This is local workflow tooling, not the M5 production frontend application.
- The generator reads local Git metadata and curated repository paths.
- Later M4 tasks will add richer GitHub/local status views and workflow launch behavior.
- Generated output is ignored by Git and can be safely deleted.
