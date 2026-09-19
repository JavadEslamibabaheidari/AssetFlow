# Project Management

AssetFlow uses a milestone-first workflow.

## Planning Rule

Keep the full roadmap visible, but only create detailed GitHub issues for the current milestone.

This keeps the project focused while still showing the future direction.

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
