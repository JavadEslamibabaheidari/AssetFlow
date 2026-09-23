---
name: assetflow-knowledge-keeper
description: Maintain compact AssetFlow project knowledge in docs/knowledge so future work can start from curated architecture context while verifying against code.
metadata:
  short-description: Keep project knowledge current
---

# AssetFlow Knowledge Keeper

Use this skill before expensive repository exploration and after meaningful changes that alter architecture, contracts, persistence, messaging, APIs, realtime behavior, infrastructure, testing strategy, important conventions, milestone status, or planned product behavior.

## Open Only What You Need

- Before meaningful work, read relevant files in `docs/knowledge/`, then verify important claims against code.
- After meaningful changes, update only the affected knowledge files.
- For milestones, large issues, start gates, closure hooks, or planned-state validation, open `references/knowledge-lifecycle.md`.

Project knowledge is a curated cache, not a repository dump. Code wins when
knowledge conflicts with implementation. Correct stale knowledge when discovered.

Maintain compact Markdown in `docs/knowledge/`. Create only the files the project needs, such as:

- `overview.md`
- `architecture.md`
- `services.md`
- `domain.md`
- `data.md`
- `messaging.md`
- `apis.md`
- `realtime.md`
- `infrastructure.md`
- `testing.md`
- `decisions.md`

Keep entries concise, current, and actionable. Remove stale information instead of accumulating contradictions.
