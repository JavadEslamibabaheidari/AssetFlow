---
name: assetflow-github-status
description: Keep AssetFlow GitHub issues, tasks, milestones, and local project status synchronized before and after project work.
metadata:
  short-description: Sync AssetFlow work with GitHub
---

# AssetFlow GitHub Status

Use this skill whenever AssetFlow work is planned, started, reprioritized, completed, reviewed, or status is requested. The goal is to keep GitHub issues, tasks, milestones, and project board state aligned with the actual repository and local status docs.

## Source of Truth

GitHub is the work-tracking source of truth for active issues, tasks, milestones, labels, and project board state. Local docs such as `docs/milestone-0-status.md`, `docs/roadmap.md`, `docs/project-management.md`, `docs/github-backlog.md`, `docs/plans/`, and `docs/knowledge/` are supporting records and should be updated when they diverge from reality.

When GitHub and local docs disagree, inspect the repository and GitHub history before deciding which is stale. Do not silently prefer a local checklist over an open GitHub issue.

## Plan Change Synchronization Hook

Whenever roadmap, milestone, plan, scope, priority, or task breakdown changes are made, synchronize all affected tracking surfaces before calling the work complete:

- local repo docs: roadmap, project-management docs, milestone plans, milestone reports, and `docs/knowledge/`
- remote repo state: pushed branch, pull request, or merged `main` containing the same docs
- GitHub tracking: milestones, current-milestone issues, labels, and project board fields/items when accessible

Create or rename GitHub milestones when local roadmap milestones change. Create or update detailed GitHub issues for the current implementation milestone. Future milestones may remain milestone records without detailed issues until their start gate, but their titles/order must still match the roadmap.

If project-board access is unavailable, record that as a sync gap rather than claiming the board is synchronized. If GitHub access is unavailable, clearly state exactly what local changes still need remote/GitHub synchronization.

## Post-Merge Cleanup Hook

After a pull request is merged, clean up its branch when the work is genuinely complete:

- confirm the PR is merged into `main`
- confirm required checks passed
- confirm `main` contains the expected merge result
- confirm issues, milestones, project board status, local docs, and `docs/knowledge/` are synchronized
- confirm no dependent open PR, active task, or unmerged local-only commit still needs the branch
- delete the merged remote branch and prune local stale branch/worktree state when safe

Do not delete release branches, active hotfix branches, explicitly retained integration branches, branches with open pull requests, branches needed by dependent work, or branches with unmerged local-only commits.

## Milestone Tagging Hook

After a milestone is closed and synchronized, create an annotated Git tag on `main`:

- verify all milestone PRs are merged
- verify CI/checks passed on the final milestone state
- verify local docs, `docs/knowledge/`, GitHub issues, milestones, and project board status are synchronized
- verify the checkout is on up-to-date `main`
- verify the tag does not already exist locally or remotely
- create an annotated tag named `v0.<milestone-number>.0`
- push the tag to GitHub

Git tags identify source commits. When service images are published, use matching milestone image tags plus immutable commit tags such as `sha-<short-sha>`.

Do not tag incomplete milestones, unsynchronized milestone states, or branches that have not been merged to `main`.

## Release Decision Hook

After creating a milestone tag, decide whether to create a GitHub Release:

- create a release for meaningful product capability, backend/frontend/infrastructure milestones, deployment or demo baselines, external handoffs, artifact boundaries, or user-requested release records
- create a release when service images or other artifacts are published from the tag
- use tag-only for internal workflow/documentation checkpoints, tiny maintenance markers, intermediate technical cleanup, or experimental snapshots

Release notes should include the milestone name/tag, completed capability, verification, known gaps/deferred work, and deployment/artifact notes when relevant.

If skipping the release, record `tag-only; no GitHub Release` in the milestone report, relevant issue/comment, or final response.

## Access

Use the best available GitHub access path in the current environment:

- a GitHub connector or plugin, if available
- the `gh` CLI, if authenticated
- direct GitHub web/API access, if explicitly available

If no authenticated GitHub access is available, say so clearly, record what would need to be updated, and do not claim that project tracking is complete.

## Before Work

Before substantial implementation or planning work:

- inspect the relevant GitHub milestone, issue, task, labels, and project board status
- confirm the requested work maps to an existing issue or identify that a new issue/task is needed
- check dependencies, acceptance criteria, and current board column before changing code
- note any mismatch between GitHub and local docs
- for plan changes, identify the affected milestones/issues/project items that must be updated before completion
- plan to handle tasks one by one, with each tracked task isolated to its own branch and pull request unless the user explicitly requests a combined PR

For tiny local-only fixes, keep this lightweight, but still mention when no GitHub issue was found or updated.

## Branches and Pull Requests

For tracked GitHub work, prefer one issue/task per branch and one pull request per issue/task. Keep each PR focused on the acceptance criteria for that single task, and leave dependent or follow-up tasks for separate PRs.

Use the repository's branch naming convention, normally the `codex/` prefix, and include a short task identifier when one exists. Reference the GitHub issue in the PR body, include verification performed, and avoid mixing unrelated cleanup into the PR.

## During Work

When scope changes, dependencies appear, or acceptance criteria need clarification:

- update the issue/task description or comments when GitHub access is available
- adjust labels, milestone, or board status when the meaning of the work changes
- update GitHub milestone titles/descriptions when roadmap milestone names, order, or goals change
- keep implementation plans in `docs/plans/` synchronized for substantial work

Do not create unrelated GitHub churn. Update only the items touched by the current work.

## After Work

Before calling work complete:

- update the relevant GitHub issue/task with what changed, verification performed, and remaining follow-ups
- move the board item to the appropriate state, such as Review or Done
- close the issue only when its acceptance criteria are actually met
- update milestone status if the work changes milestone progress
- update local status docs when they are now stale
- confirm the remote branch/PR or merged `main` contains the same plan docs as GitHub tracking

If verification could not run, record that clearly in GitHub and in the final response.

## Status Requests

When asked for project status:

- read GitHub milestones, open/closed issues, and board state when access is available
- compare GitHub state with local docs and recent commits
- report the current milestone, completed work, active/in-progress work, blocked work, verification status, and tracking gaps
- update stale local status docs or GitHub tracking if the user asked for maintenance, not just a read-only report

## Boundaries

Do not invent issue numbers, milestone IDs, board fields, or GitHub state. If access is missing, distinguish confirmed local repository status from unverified GitHub status.

Do not use this skill as authorization to publish, close, delete, or broadly rewrite GitHub items unrelated to the current task. Ask the user before destructive or surprising GitHub changes.
