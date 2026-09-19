# Milestone Plan: M4 - Agentic Control Dashboard MVP

## Goal

Create an early control center that lets the user see and launch the AssetFlow AI-assisted workflow from one place after M3 is complete.

## Scope

In scope:

- Show current milestone, active branch/task, and linked local planning docs.
- Surface GitHub milestone and issue status for the active and upcoming work.
- Provide quick access to roadmap, plans, project knowledge, agents, and reusable skills.
- Provide command launch points for common workflow steps: status check, planning, implementation, testing, review, knowledge update, and GitHub sync.
- Show lightweight AI-assisted activity and usage signals when available from local logs, GitHub activity, or Codex-accessible metadata.

Out of scope:

- Replacing GitHub as the source of truth for tracked issues, milestones, and board status.
- Deep memory-system implementation or migration to Obsidian, Google Drive, Notion, or another external memory store.
- Google Workspace, NotebookLM, email, calendar, or research automation.
- Per-skill, per-agent, or per-automation resource accounting beyond a lightweight MVP summary.
- Frontend application foundation and inventory workflow parity; those are planned for M5 and M6.
- Event-driven marketplace synchronization, Kafka, outbox, and channel workers; those remain in M7.
- Full Agentic OS expansion; that is reserved for M9.

## Decisions

Decided:

- M4 is a focused dashboard MVP, not the full frontend application or the full Agentic OS.
- GitHub remains the tracking source of truth; the dashboard reads or links to GitHub status instead of becoming a competing tracker.
- Local docs in `docs/`, `docs/plans/`, and `docs/knowledge/` remain durable context and must be visible from the dashboard.
- The first dashboard should prioritize control and visibility over automation depth.
- M4 planning starts after M3 closure so the dashboard can use the real active workflow state.

Deferred out of scope:

- Memory product selection and security review.
- External workspace and research integrations.
- Deep resource accounting by skill, agent, model, workflow, or automation.
- Automated background orchestration beyond explicit user-triggered workflow commands.
- Frontend application buildout and inventory screens.

Open questions:

- None blocking roadmap sequencing. Product and implementation details will be decided when M4 planning starts after M3 closure.

## Deliverables

- A dashboard MVP plan with final UX, data-source, security, and implementation decisions.
- A first usable dashboard surface for current project state and workflow control.
- Links or controls for local docs, GitHub milestones/issues, agents, skills, and workflow commands.
- Lightweight activity or usage summary where practical.
- Updated knowledge docs and GitHub tracking for M4 completion.

## Acceptance Criteria

- [ ] The user can open one dashboard and understand the current milestone, active task, and next planned work.
- [ ] The dashboard links to the relevant roadmap, milestone plan, knowledge overview, GitHub milestone, and active issues.
- [ ] The dashboard exposes common workflow actions or launch points without requiring the user to remember every command or skill name.
- [ ] The dashboard clearly distinguishes read-only status from actions that mutate GitHub, docs, code, or project state.
- [ ] The dashboard avoids storing sensitive external credentials or personal workspace data in the repository.
- [ ] The MVP leaves full frontend application buildout, memory, external automations, and deep resource accounting as explicit later follow-ups.

## Task Breakdown

| Order | Task | Objective | Depends on | Verification | GitHub issue |
| --- | --- | --- | --- | --- | --- |
| 1 | Define dashboard MVP spec | Decide UX, local data sources, GitHub data sources, commands, and security boundaries. | M3 closure | Reviewed spec and start gate update | To create after M3 |
| 2 | Build dashboard foundation | Implement the first dashboard surface and local navigation model. | Task 1 | Local run and UI review | To create after M3 |
| 3 | Add GitHub and local status views | Surface milestone, issue, branch, and docs status from approved sources. | Task 2 | Status comparison against GitHub and local docs | To create after M3 |
| 4 | Add workflow launch points | Add explicit controls for status, planning, implementation, testing, review, knowledge update, and GitHub sync workflows. | Tasks 2-3 | Manual workflow checks | To create after M3 |
| 5 | Complete M4 validation and docs | Verify dashboard behavior, update knowledge, and synchronize GitHub/local state. | Tasks 1-4 | Full validation report | To create after M3 |

## Risks and Mitigations

- Risk: The dashboard becomes too broad and delays inventory platform work. Mitigation: keep M4 limited to visibility and explicit workflow launch points.
- Risk: Local docs and GitHub drift. Mitigation: keep GitHub as tracking source of truth and make sync status visible in the dashboard.
- Risk: External memory or workspace integrations introduce security concerns. Mitigation: defer those integrations to M9 with a security review.
- Risk: Workflow buttons run mutating operations accidentally. Mitigation: separate read-only status actions from write actions and require explicit user intent for mutations.

## Testing Strategy

- Unit: dashboard data-source adapters and status mapping logic where applicable.
- Integration: GitHub/local status read paths when implemented.
- UI: dashboard layout, navigation, and action-state checks.
- Security: verify no external credentials or personal workspace data are stored in repo files.

## GitHub Tracking

- Milestone: `M4 - Agentic Control Dashboard MVP`
- Issues: create after M3 closure and detailed M4 planning.
- Project board: keep each active task isolated to one branch and pull request unless explicitly combined.

## Previous Milestone Closure

- Report: M3 report must exist before M4 implementation starts.
- Knowledge/docs: M3 durable facts must be captured in `docs/knowledge/`.
- GitHub status: M3 issues and milestone state must match the completed implementation.
- Result: Pending M3 completion.

## Start Gate Result

- [ ] Previous milestone report exists or was not required.
- [ ] Previous milestone durable facts are captured in docs/knowledge.
- [ ] Plan is complete enough to implement.
- [ ] No implementation-blocking decisions remain undecided.
- [ ] GitHub tracking matches this plan.
- [ ] Knowledge docs capture durable planned behavior.

Status: Not ready
