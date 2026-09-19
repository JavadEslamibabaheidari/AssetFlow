# Milestone Plan: M4 - Agentic Control Dashboard MVP

## Goal

Create an early repo-local control center that lets the user see AssetFlow project status and launch AI-assisted workflow entry points from one place after M3 is complete.

## Scope

In scope:

- Build a repository-local dashboard MVP under a tools area, not a production inventory frontend.
- Generate or serve a local dashboard from approved local sources: Git state, docs, `.agents/`, `.codex/skills/`, and GitHub CLI/API output when authenticated.
- Show current milestone, active branch/task, local planning docs, and GitHub milestone/issue links.
- Surface M4 issue status and upcoming milestone links without replacing GitHub as the tracker.
- Provide quick access to roadmap, plans, project knowledge, agents, and reusable skills.
- Provide explicit command launch points for common workflow steps: status check, planning, implementation, testing, review, knowledge update, GitHub sync, and post-merge cleanup.
- Show lightweight AI-assisted activity and usage signals from safe available metadata, such as recent merged PRs, issues, branch state, and local workflow docs.
- Document how to run, verify, and maintain the dashboard.

Out of scope:

- Replacing GitHub as the source of truth for tracked issues, milestones, and board status.
- Building the M5 production frontend app shell, design system, routing foundation, API client strategy, or inventory screens.
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
- The MVP should be repo-local and should not require external workspace plugins or credentials beyond the user's existing GitHub CLI authentication.
- The dashboard may generate static HTML or run a small local-only tool, but it must not establish the production frontend architecture reserved for M5.
- Mutating workflow actions must be presented as explicit launch points or copied commands/prompts, with read-only status clearly separated from write actions.
- Dashboard data sources are local repository files, `git`, GitHub issue/milestone data available through `gh`, and Codex-visible task metadata when available.
- The dashboard must tolerate missing GitHub/project-board access and show a clear degraded status instead of failing the whole page.
- M3 closure passed on 2026-09-19: the M3 report exists, durable behavior is captured in `docs/knowledge/overview.md`, all M3 issues are closed, and the M3 GitHub milestone is closed.

Deferred out of scope:

- Memory product selection and security review.
- External workspace and research integrations.
- Deep resource accounting by skill, agent, model, workflow, or automation.
- Automated background orchestration beyond explicit user-triggered workflow commands.
- Frontend application buildout and inventory screens.
- GitHub project-board mutation beyond reporting/sync guidance, unless a later task obtains reliable project-board access.

Open questions:

- None blocking implementation start. UI details may iterate inside the dashboard foundation task as long as the scope, sources, and security boundaries above remain intact.

## Deliverables

- A first usable dashboard surface for current project state and workflow control.
- Links or controls for local docs, GitHub milestones/issues, agents, skills, and workflow commands.
- Read-only status views for M4 milestone/issues, local branch state, key docs, and recent completed work.
- Lightweight activity or usage summary where practical.
- Run and verification documentation for the local dashboard.
- Updated knowledge docs and GitHub tracking for M4 completion.

## Acceptance Criteria

- [ ] The user can open one dashboard and understand the current milestone, active task, and next planned work.
- [ ] The dashboard links to the relevant roadmap, milestone plan, knowledge overview, GitHub milestone, and active issues.
- [ ] The dashboard exposes common workflow actions or launch points without requiring the user to remember every command or skill name.
- [ ] The dashboard clearly distinguishes read-only status from actions that mutate GitHub, docs, code, or project state.
- [ ] The dashboard handles missing GitHub authentication or project-board access with an explicit degraded state.
- [ ] The dashboard avoids storing sensitive external credentials or personal workspace data in the repository.
- [ ] The MVP leaves full frontend application buildout, memory, external automations, and deep resource accounting as explicit later follow-ups.
- [ ] The dashboard can be run and verified locally from documented commands.

## Task Breakdown

| Order | Task | Objective | Depends on | Verification | GitHub issue |
| --- | --- | --- | --- | --- | --- |
| 1 | Define dashboard MVP spec and data contract | Write the focused UX/data-source/security spec and dashboard data model before implementation. | M3 closure | Spec review against this plan and issue acceptance criteria | #49 |
| 2 | Build repo-local dashboard foundation | Implement the local dashboard surface, navigation model, and run command without creating the M5 frontend foundation. | Task 1 | Local run, build/lint where applicable, and UI review | #50 |
| 3 | Add GitHub and local status views | Surface milestone, issue, branch, docs, and recent activity status from approved sources with degraded states. | Task 2 | Compare dashboard output against `git`, local docs, and GitHub milestone/issues | #51 |
| 4 | Add workflow launch points | Add explicit read/write-separated controls for status, planning, implementation, testing, review, knowledge update, GitHub sync, and post-merge cleanup workflows. | Tasks 2-3 | Manual workflow checks and no accidental mutations from page load | #52 |
| 5 | Complete M4 validation and docs | Verify dashboard behavior, update run docs and knowledge, and synchronize GitHub/local state. | Tasks 1-4 | Full validation report, knowledge update, and GitHub tracking check | #53 |

## Risks and Mitigations

- Risk: The dashboard becomes too broad and delays inventory platform work. Mitigation: keep M4 limited to visibility and explicit workflow launch points.
- Risk: Local docs and GitHub drift. Mitigation: keep GitHub as tracking source of truth and make sync status visible in the dashboard.
- Risk: External memory or workspace integrations introduce security concerns. Mitigation: defer those integrations to M9 with a security review.
- Risk: Workflow buttons run mutating operations accidentally. Mitigation: separate read-only status actions from write actions and require explicit user intent for mutations.
- Risk: M4 accidentally establishes production frontend architecture before M5. Mitigation: keep implementation under local tooling, document that it is an operator/workflow dashboard, and avoid inventory product screens.
- Risk: GitHub project-board access is unavailable. Mitigation: report milestone and issue status from the GitHub APIs that are available and show project-board sync as unavailable rather than synchronized.

## Testing Strategy

- Unit: dashboard data-source adapters, status mapping, and command metadata where applicable.
- Integration: GitHub/local status read paths when implemented, including degraded mode without GitHub data.
- UI: dashboard layout, navigation, action-state checks, and link correctness.
- Security: verify no external credentials, tokens, or personal workspace data are stored in repo files or generated dashboard artifacts.
- Documentation: verify run commands and workflow launch points match the actual implementation.

## GitHub Tracking

- Milestone: `M4 - Agentic Control Dashboard MVP`
- Issues: #49, #50, #51, #52, #53.
- Project board: project-board access is not verified from this environment; keep each active task isolated to one branch and pull request unless explicitly combined, and record board sync gaps until access is available.

## Previous Milestone Closure

- Report: `docs/milestone-3-report.md` exists.
- Knowledge/docs: M3 durable facts are captured in `docs/knowledge/overview.md`.
- GitHub status: M3 issues #34, #35, #36, #37, #38, #39, and #40 are closed; the M3 GitHub milestone is closed.
- Result: Passed on 2026-09-19.

## Start Gate Result

- [x] Previous milestone report exists or was not required.
- [x] Previous milestone durable facts are captured in docs/knowledge.
- [x] Plan is complete enough to implement.
- [x] No implementation-blocking decisions remain undecided.
- [x] GitHub tracking matches this plan.
- [x] Knowledge docs capture durable planned behavior.

Status: Ready to start
