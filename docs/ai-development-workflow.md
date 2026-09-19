# AI Development Workflow

AssetFlow uses a spec-first, AI-assisted workflow. Meaningful features should move from requirement intake to spec, task breakdown, implementation, verification, review, and project knowledge updates.

Small fixes can stay lightweight, but features, public contracts, data changes, infrastructure, messaging, or milestone work must have enough written context that implementation can be checked against the plan.

## Workflow

```text
Requirement Intake -> Feature Spec -> Task Breakdown -> Implementation -> Verification -> Review -> Knowledge Update
```

For tracked GitHub work, keep one task per branch and pull request unless the user explicitly asks to combine tasks.

## Plan Change Synchronization Hook

Any roadmap, milestone, plan, scope, priority, or task breakdown change must be synchronized across local docs, the remote repository, and GitHub tracking before the change is considered complete.

When a plan changes:

- update the relevant local docs, such as `docs/roadmap.md`, `docs/project-management.md`, `docs/plans/`, milestone reports, and `docs/knowledge/`
- update GitHub milestones, issues, labels, and project board items that represent the changed plan
- create missing GitHub milestones or issues when the local plan now depends on them
- rename, reorder, reopen, close, or comment on GitHub items when the plan meaning changed
- keep the planning rule intact: detailed GitHub issues are required for the current implementation milestone, while future milestones may stay as roadmap/milestone records until their start gate
- commit and push the local docs, then open or update the pull request that carries the plan change
- verify that GitHub `main` or the active PR visibly contains the same plan state as the GitHub milestones/issues/project board
- record any intentional mismatch in the relevant plan or final response, including what remains to synchronize and why

A plan update is not done if it exists only in local files, only in a branch, or only in GitHub. The local repository, remote repository/PR, and GitHub tracking must agree, or the mismatch must be explicitly documented as a temporary blocker.

## Post-Merge Cleanup Hook

After a pull request is merged into `main`, clean up the completed branch once the merge is verified.

Before deleting a branch:

- verify the pull request is merged and CI/checks passed
- verify `main` contains the expected merge result
- verify local docs, `docs/knowledge/`, GitHub issues, milestones, and project board status are synchronized for the completed work
- verify no dependent open pull request, active task, or unmerged local-only commit still needs the branch

Then delete the remote branch and prune local stale branch/worktree state when safe.

Do not delete release branches, hotfix branches still in use, long-running integration branches explicitly kept alive, branches with open pull requests, branches needed by dependent work, or branches with unmerged local-only commits.

## Milestone Tagging Hook

After a milestone is closed, create an annotated Git tag on `main` so the completed milestone has a durable audit and deployment reference.

Before tagging:

- verify all milestone PRs are merged into `main`
- verify CI/checks passed on the final milestone state
- verify milestone docs, `docs/knowledge/`, GitHub issues, GitHub milestone status, and project board status are synchronized
- verify the local checkout is on up-to-date `main`
- verify the tag name does not already exist locally or remotely

Use annotated milestone tags named `v0.<milestone-number>.0`, such as `v0.3.0` for M3 and `v0.4.0` for M4. The annotation message should name the milestone and summarize the completed scope.

Push the tag to GitHub after creation. When container images are published, tag service images with both immutable commit tags such as `sha-<short-sha>` and milestone tags such as `v0.3.0` for deployable milestone releases.

Do not create milestone tags for incomplete milestones, failed validation, unmerged work, or commits that do not represent the synchronized milestone state.

## Milestone Start Gate

Milestone work must not start from roadmap bullets alone. Before opening implementation branches or moving milestone issues into active work, run this start gate:

- The previous milestone has a current implementation report, unless no previous milestone exists.
- The previous milestone's durable implementation facts are captured in `docs/knowledge/` and any relevant docs, unless they already exist and match the completed code.
- A milestone plan exists in `docs/plans/` and names the milestone, goal, scope, non-goals, deliverables, acceptance criteria, task breakdown, dependencies, risks, testing strategy, and GitHub tracking.
- Every product, architecture, data, API, infrastructure, rollout, and testing decision needed to start the milestone is either decided in the plan or explicitly deferred out of scope.
- The plan has no unresolved placeholders such as `TBD`, `TODO`, `unknown`, `undecided`, or open questions that affect implementation order, public contracts, persistence, concurrency, security, deployment, or verification.
- The task breakdown is small enough to execute one tracked task per branch and pull request unless a combined PR is deliberately justified.
- GitHub milestones and issues match the local plan before implementation begins.
- The GitHub project board state matches the local plan for all current-milestone issues when project-board access is available.
- `docs/knowledge/` captures any durable planned behavior that future implementation and review must compare against.

If the gate fails, stay in planning mode. Do not begin implementation until the undecided items are resolved or moved out of scope.

When the gate passes, mark the milestone as ready to start in the plan and begin with the first prioritized task.

## Previous Milestone Closure Hook

Before planning a new milestone or creating its GitHub issues, first close the previous milestone's knowledge loop.

Check whether these artifacts already exist and are current:

- a report such as `docs/milestone-<n>-report.md` that summarizes what was implemented, what changed from the plan, verification performed, known gaps, and the next milestone handoff
- `docs/knowledge/` entries that capture durable facts from the completed milestone well enough that future agents should start by reading the docs, then verify important claims against code
- local plans, roadmap/status docs, and GitHub issue/milestone state that match the completed implementation
- GitHub project board state that matches the completed milestone when project-board access is available

If all of those already exist and match the repository, record that the hook passed and continue to the new milestone plan. If any are missing, stale, or too thin to be trusted, update them before writing the new milestone plan or creating new milestone issues.

The docs are allowed to be the first read path for future work, but they are not allowed to become unquestioned truth. Future agents should read the maintained docs first, then verify important implementation facts against code, tests, configuration, and GitHub before relying on them.

## Milestone Plan Template

Use this template for each milestone before implementation starts.

```markdown
# Milestone Plan: <Mx - title>

## Goal

What must be true when this milestone is complete?

## Scope

In scope:

-

Out of scope:

-

## Decisions

Decided:

-

Deferred out of scope:

-

Open questions:

- None.

## Deliverables

-

## Acceptance Criteria

- [ ]

## Task Breakdown

| Order | Task | Objective | Depends on | Verification | GitHub issue |
| --- | --- | --- | --- | --- | --- |
| 1 |  |  |  |  |  |

## Risks and Mitigations

-

## Testing Strategy

- Unit:
- Integration:
- Contract:
- Concurrency/failure:

## GitHub Tracking

- Milestone:
- Issues:
- Project board:

## Previous Milestone Closure

- Report:
- Knowledge/docs:
- GitHub status:
- Result:

## Start Gate Result

- [ ] Previous milestone report exists or was not required.
- [ ] Previous milestone durable facts are captured in docs/knowledge.
- [ ] Plan is complete enough to implement.
- [ ] No implementation-blocking decisions remain undecided.
- [ ] GitHub tracking matches this plan.
- [ ] Knowledge docs capture durable planned behavior.

Status: Not ready / Ready to start
```

## Requirement Intake Template

Use this template before writing a spec for meaningful work.

```markdown
# Requirement Intake: <title>

## Problem

What user, operator, or system problem are we solving?

## Desired Outcome

What should be true when this work is complete?

## Users and Actors

- Primary user:
- Other actors:
- External systems:

## Scope

In scope:

-

Out of scope:

-

## Constraints

- Product constraints:
- Technical constraints:
- Operational constraints:

## Open Questions

-

## GitHub Tracking

- Issue:
- Milestone:
- Labels:
```

## Feature Spec Template

Use this template for features and other meaningful changes.

```markdown
# Feature Spec: <title>

## Summary

Short description of the planned behavior.

## Goals

-

## Non-Goals

-

## User Stories

- As a <user>, I want <capability>, so that <outcome>.

## Acceptance Criteria

- [ ]

## API Contract

- OpenAPI file:
- New endpoints:
- Changed endpoints:
- Request schemas:
- Response schemas:
- Error cases:

## Data and Persistence

- Tables or migrations:
- Query patterns:
- Transaction or concurrency concerns:

## Events and Integrations

- Published events:
- Consumed events:
- External systems:

## Observability

- Logs:
- Metrics:
- Traces:
- Alerts:

## Security and Permissions

- Authentication:
- Authorization:
- Sensitive data:

## Testing Plan

- Unit tests:
- Integration tests:
- Contract tests:
- Concurrency or failure tests:

## Rollout and Compatibility

- Migration path:
- Backward compatibility:
- Operational steps:
```

## Task Breakdown Template

Use this template to split a spec into focused GitHub tasks.

```markdown
# Task Breakdown: <feature>

## Task: <title>

Objective:

Acceptance criteria:

- [ ]

Affected files or components:

-

Dependencies:

-

Verification:

-

PR scope:

- One task per PR unless explicitly combined.
```

## Review Checklist

Use this checklist before merging meaningful work.

- The implementation matches the GitHub issue, spec, and acceptance criteria.
- The current app state matches what was planned, or deviations are documented.
- OpenAPI contracts and API behavior agree.
- Data changes are safe, reversible where practical, and tested.
- Error handling follows the project style.
- Tests cover the risk level of the change.
- End-to-end feature work has been validated through the local Docker Compose path when the feature spans deployable services, frontend/backend integration, persistence, or infrastructure.
- CI passes.
- Docs and `docs/knowledge/` are updated when durable facts changed.
- The PR is focused on one task.
- Follow-up work is tracked in GitHub.

## Model Selection Guidance

Use faster or cheaper models for:

- formatting and small documentation edits
- issue template cleanup
- simple scripted transformations
- small implementation tasks with clear examples
- summarizing already-inspected local context

Use stronger models for:

- feature planning and architecture choices
- public API contract design
- database, concurrency, reservation, or consistency logic
- security, reliability, and performance review
- debugging failures with uncertain causes
- final review before merging substantial work

Escalate model strength when the cost of a wrong answer is high, when multiple subsystems are involved, or when the task requires careful tradeoff analysis.

## Reusable Skills

When a workflow becomes repeatable, extract it into a skill. Each AssetFlow skill should document:

- purpose
- when to use it
- required inputs
- expected output
- verification expectations
- important failure modes

## Monitoring AI Work

Track AI-assisted work across:

- quality: did the output match the spec?
- efficiency: how many iterations were needed?
- consumption: which model was used, and what was the approximate cost?
- drift: did implementation diverge from the plan, and was that captured?

The project may later add productized AI monitoring for these signals.
