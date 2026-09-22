---
name: assetflow-github-status
description: Keep AssetFlow GitHub issues, tasks, milestones, and local project status synchronized before and after project work.
metadata:
  short-description: Sync AssetFlow work with GitHub
---

# AssetFlow GitHub Status

Use this skill whenever AssetFlow work is planned, started, reprioritized, completed, reviewed, or status is requested. The goal is to keep GitHub issues, tasks, milestones, and project board state aligned with the actual repository and local status docs.

## Open Only What You Need

- For a quick status check, inspect the relevant GitHub item and local docs using this file only.
- For plan changes, post-merge cleanup, milestone tagging, releases, or full sync, open `references/github-workflow.md`.
- For implementation tasks tracked in GitHub, inspect the relevant issue/task/milestone before starting.

GitHub is the work-tracking source of truth for active issues, tasks, milestones,
labels, and project board state. Local docs such as `docs/milestone-0-status.md`,
`docs/roadmap.md`, `docs/project-management.md`, `docs/github-backlog.md`,
`docs/plans/`, and `docs/knowledge/` are supporting records and should be
updated when they diverge from reality.

When GitHub and local docs disagree, inspect the repository and GitHub history
before deciding which is stale.

Do not invent issue numbers, milestone IDs, board fields, or GitHub state. If
access is missing, distinguish confirmed local repository status from unverified
GitHub status.
