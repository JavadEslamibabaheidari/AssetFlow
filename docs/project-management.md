# Project Management

AssetFlow uses a milestone-first workflow.

## Planning Rule

Keep the full roadmap visible, but only create detailed GitHub issues for the current milestone.

This keeps the project focused while still showing the future direction.

## Plan Synchronization Rule

Every meaningful plan change must be synchronized in all three places before it is called done:

- local repository docs
- remote repository state through a pushed branch, pull request, or merged `main`
- GitHub tracking, including milestones, current-milestone issues, labels, and project board state when accessible

Examples of meaningful plan changes include adding, renaming, reordering, splitting, or removing milestones; changing milestone scope; changing task breakdowns; changing dependencies; or changing the definition of done for a feature.

Future milestones may have GitHub milestones without detailed issues until their start gate. The current implementation milestone should have detailed GitHub issues and project board status aligned with the local plan.

If GitHub access or project board access is unavailable, record the exact sync gap in the plan or final status instead of treating the update as complete.

## Post-Merge Cleanup Rule

After a task branch is merged into `main`, delete the merged remote branch and prune stale local branch/worktree state once the merge is verified and the work is complete.

Before cleanup, confirm:

- the PR is merged into `main`
- CI/checks passed
- docs, knowledge, issues, milestones, and project board status are synchronized
- no open PR, active task, dependency, or unmerged local-only commit still needs the branch

Keep release branches, active hotfix branches, explicitly approved long-running integration branches, branches with open PRs, and branches needed by dependent work.

## Previous Milestone Closure Rule

Before planning a new milestone or creating new milestone issues, verify that the previous milestone is documented well enough to be trusted as the first read path for future work.

If the previous milestone already has a current implementation report, synchronized `docs/knowledge/` entries, and matching GitHub status, do not duplicate those artifacts. Record that the closure check passed and continue.

If any of those artifacts are missing, stale, or too thin, update them before creating the new milestone plan or issues.

## Milestone Start Rule

A milestone may start only after its plan passes the start gate in `docs/ai-development-workflow.md`.

Before implementation begins:

- confirm the previous milestone closure rule has passed
- create or update the milestone plan in `docs/plans/`
- decide every implementation-blocking product, API, data, infrastructure, testing, rollout, and GitHub-tracking question
- move any deliberately deferred questions out of scope
- verify there are no unresolved `TBD`, `TODO`, `unknown`, `undecided`, or blocking open-question entries
- align GitHub milestone/issues/project status with the local plan
- verify the plan synchronization rule has passed for the milestone

If any blocking decision remains unresolved, keep the milestone in planning. Implementation starts only after the plan says `Status: Ready to start`.

## GitHub Structure

Recommended GitHub setup:

- GitHub repository: `AssetFlow`
- GitHub Project: `AssetFlow`
- Issues for current milestone work
- Milestones for major phases
- Labels for work type and workflow state

## Project Board Columns

- Backlog
- Spec Needed
- Ready
- In Progress
- Review
- Done

## Initial Milestones

- `M0 - Repository and Hello Service`
- `M1 - Spec-First Workflow`
- `M2 - Core Inventory Domain`
- `M3 - Reservations and Availability`
- `M4 - Agentic Control Dashboard MVP`
- `M5 - Frontend Application Foundation`
- `M6 - Frontend Inventory Parity`
- `M7 - Event-Driven Synchronization`
- `M8 - Observability and Monitoring`
- `M9 - Agentic OS Expansion`

## Label Set

- `feature`
- `task`
- `docs`
- `infra`
- `ci-cd`
- `kubernetes`
- `openapi`
- `ai-workflow`
- `spec-needed`
- `ready`
- `review`
