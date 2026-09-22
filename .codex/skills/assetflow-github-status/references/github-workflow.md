# AssetFlow GitHub Workflow Reference

Open this reference for plan changes, substantial work start/finish, full status
sync, post-merge cleanup, milestone tagging, or release decisions.

## Access

Use the best available GitHub access path in the current environment:

- a GitHub connector or plugin, if available
- the `gh` CLI, if authenticated
- direct GitHub web/API access, if explicitly available

If no authenticated GitHub access is available, say so clearly, record what
would need to be updated, and do not claim that project tracking is complete.

## Before Work

- Inspect the relevant GitHub milestone, issue, task, labels, and project board
  status.
- Confirm the requested work maps to an existing issue or identify that a new
  issue/task is needed.
- Check dependencies, acceptance criteria, and current board column before
  changing code.
- Note any mismatch between GitHub and local docs.
- Handle tracked tasks one by one, each isolated to its own branch and pull
  request unless the user explicitly requests a combined PR.

For tiny local-only fixes, keep this lightweight, but still mention when no
GitHub issue was found or updated.

## During Work

When scope changes, dependencies appear, or acceptance criteria need
clarification:

- update the issue/task description or comments when GitHub access is available
- adjust labels, milestone, or board status when the meaning of the work changes
- update GitHub milestone titles/descriptions when roadmap milestone names,
  order, or goals change
- keep implementation plans in `docs/plans/` synchronized for substantial work

Do not create unrelated GitHub churn. Update only the items touched by the
current work.

## Branches and Pull Requests

For tracked GitHub work, prefer one issue/task per branch and one pull request
per issue/task. Keep each PR focused on the acceptance criteria for that single
task, and leave dependent or follow-up tasks for separate PRs.

Use the repository's branch naming convention, normally the `codex/` prefix, and
include a short task identifier when one exists. Reference the GitHub issue in
the PR body, include verification performed, and avoid mixing unrelated cleanup
into the PR.

## After Work

Before calling work complete:

- update the relevant GitHub issue/task with what changed, verification
  performed, and remaining follow-ups
- move the board item to the appropriate state, such as Review or Done
- close the issue only when its acceptance criteria are actually met
- update milestone status if the work changes milestone progress
- update local status docs when they are now stale
- confirm the remote branch/PR or merged `main` contains the same plan docs as
  GitHub tracking

If verification could not run, record that clearly in GitHub and in the final
response.

## Plan Change Synchronization Hook

Whenever roadmap, milestone, plan, scope, priority, or task breakdown changes
are made, synchronize all affected tracking surfaces before calling the work
complete:

- local repo docs: roadmap, project-management docs, milestone plans, milestone
  reports, and `docs/knowledge/`
- remote repo state: pushed branch, pull request, or merged `main` containing
  the same docs
- GitHub tracking: milestones, current-milestone issues, labels, and project
  board fields/items when accessible

Create or rename GitHub milestones when local roadmap milestones change. Create
or update detailed GitHub issues for the current implementation milestone.
Future milestones may remain milestone records without detailed issues until
their start gate, but their titles/order must still match the roadmap.

If project-board access is unavailable, record that as a sync gap rather than
claiming the board is synchronized. If GitHub access is unavailable, clearly
state exactly what local changes still need remote/GitHub synchronization.

## Post-Merge Cleanup Hook

After a pull request is merged, clean up its branch when the work is genuinely
complete:

- confirm the PR is merged into `main`
- confirm required checks passed
- confirm `main` contains the expected merge result
- confirm issues, milestones, project board status, local docs, and
  `docs/knowledge/` are synchronized
- confirm no dependent open PR, active task, or unmerged local-only commit still
  needs the branch
- delete the merged remote branch and prune local stale branch/worktree state
  when safe

Do not delete release branches, active hotfix branches, explicitly retained
integration branches, branches with open pull requests, branches needed by
dependent work, or branches with unmerged local-only commits.

## Milestone Tags And Releases

After a milestone is closed and synchronized, create an annotated Git tag on
`main` only after verifying all milestone PRs are merged, checks passed, docs and
tracking are synchronized, checkout is on up-to-date `main`, and the tag does
not already exist locally or remotely. Use `v0.<milestone-number>.0`.

After tagging, decide whether to create a GitHub Release. Create a release for
meaningful product capability, backend/frontend/infrastructure milestones,
deployment or demo baselines, external handoffs, artifact boundaries, or
user-requested release records. Use tag-only for internal documentation
checkpoints, tiny maintenance markers, intermediate technical cleanup, or
experimental snapshots.

Release notes should include the milestone name/tag, completed capability,
verification, known gaps/deferred work, and deployment/artifact notes when
relevant.

## Status Requests

When asked for project status:

- read GitHub milestones, open/closed issues, and board state when access is
  available
- compare GitHub state with local docs and recent commits
- report the current milestone, completed work, active/in-progress work, blocked
  work, verification status, and tracking gaps
- update stale local status docs or GitHub tracking if the user asked for
  maintenance, not just a read-only report

Ask the user before destructive or surprising GitHub changes.
