# Feature Spec: M4 Agentic Control Dashboard MVP

## Summary

The M4 dashboard is a repo-local operator surface for AssetFlow's AI-assisted development workflow. It gives the user one place to inspect current milestone status, local planning knowledge, GitHub tracking, available agents and skills, and common workflow launch points.

The MVP is not the production frontend foundation. It should live under local tooling and serve the development workflow until M5 creates the real frontend application architecture.

## Users

- Primary user: the project owner using Codex to plan, implement, verify, and review AssetFlow work.
- Secondary actor: Codex agents that need a compact source of dashboard requirements before implementation.
- External systems: Git, GitHub issues/milestones/PRs through `gh`, local repo docs, `.agents/`, and `.codex/skills/`.

## Goals

- Make the current milestone, active issue, branch, and next work visible without reading several docs manually.
- Link directly to the roadmap, M4 plan, knowledge overview, milestone reports, GitHub milestone, and active M4 issues.
- Surface available workflow agents and reusable skills.
- Provide explicit launch points for common workflow steps.
- Distinguish read-only status from actions that may mutate GitHub, docs, code, branches, or project state.
- Handle missing GitHub authentication or missing project-board scope as a visible degraded state.

## Non-Goals

- Do not replace GitHub as the source of truth for issues, milestones, or board state.
- Do not build the M5 production frontend app shell, design system, routing, API client strategy, or inventory workflow screens.
- Do not implement external workspace automation for Google Workspace, Notion, email, calendar, research notebooks, or similar tools.
- Do not select, migrate, or synchronize a long-term memory system.
- Do not perform deep resource accounting by skill, agent, model, automation, or workflow.
- Do not run mutating workflow operations automatically on page load.

## Dashboard Views

### Overview

Purpose: answer "where are we, what is active, and what should happen next?"

Required content:

- repository name and local checkout path
- current Git branch and clean/dirty status
- current milestone name and status
- active issue or task when detectable from branch name or explicit dashboard metadata
- next planned issue from the M4 task order
- links to roadmap, M4 plan, knowledge overview, M3 report, M4 GitHub milestone, and open M4 issues
- degraded-state notices for unavailable GitHub or project-board data

### Tracking

Purpose: show local and GitHub tracking without becoming a second tracker.

Required content:

- M4 milestone open/closed issue counts when GitHub data is available
- open M4 issues with number, title, labels, URL, and local task order
- recent merged PRs or commits as lightweight activity signals
- project-board state only when project-board access is available
- explicit "not verified" state when the token lacks `read:project` or similar scopes

### Docs

Purpose: make durable project context reachable.

Required links:

- `docs/roadmap.md`
- `docs/project-management.md`
- `docs/ai-development-workflow.md`
- `docs/knowledge/overview.md`
- `docs/plans/m4-agentic-control-dashboard-mvp.md`
- `docs/specs/m4-agentic-control-dashboard-mvp.md`
- `docs/milestone-3-report.md`

### Agents And Skills

Purpose: show workflow entry points the user can invoke in Codex.

Required content:

- `Small Task Agent` with a link to `.agents/small-task-agent.md`
- `Large Feature Agent` with a link to `.agents/large-feature-agent.md`
- project skill names from `.codex/skills/README.md`
- short use guidance copied from the local agent/skill docs, not invented at runtime

### Workflow Launch Points

Purpose: provide safe, explicit starts for repeatable workflow steps.

Required launch points:

- status check: inspect current GitHub/local status
- planning: use `$assetflow-planner`
- prioritization: use `$assetflow-prioritizer`
- implementation: use `$assetflow-dotnet-implementer`
- testing: use `$assetflow-tester`
- review: use `$assetflow-reviewer`
- knowledge update: use `$assetflow-knowledge-keeper`
- GitHub sync: use `$assetflow-github-status`
- post-merge cleanup: follow `docs/ai-development-workflow.md` and `docs/project-management.md`

Read-only launch points may show commands or links directly. Mutating launch points must be marked as write actions and require explicit user initiation.

## Information Hierarchy

The first screen should prioritize:

1. current milestone and active task
2. Git branch and sync state
3. M4 issue progress and next task
4. workflow launch points
5. docs, agents, skills, and recent activity
6. degraded-state warnings and sync gaps

Warnings about missing GitHub auth, missing project-board scope, dirty local state, or stale generated data should be visible near the affected section and summarized on the overview.

## Data Sources

Approved local sources:

- `git status --short --branch`
- `git branch --show-current`
- recent local commits from `git log`
- `docs/roadmap.md`
- `docs/project-management.md`
- `docs/ai-development-workflow.md`
- `docs/knowledge/overview.md`
- `docs/plans/m4-agentic-control-dashboard-mvp.md`
- `docs/specs/m4-agentic-control-dashboard-mvp.md`
- milestone reports in `docs/`
- `.agents/README.md`
- `.agents/AGENTS.md`
- `.agents/small-task-agent.md`
- `.agents/large-feature-agent.md`
- `.codex/skills/README.md`
- project-local skill directories under `.codex/skills/`

Approved GitHub sources when authenticated:

- `gh issue list` for M4 issues
- `gh issue view` for active issue details
- `gh pr list` and `gh pr view` for recent pull request status
- `gh api repos/:owner/:repo/milestones/<number>` for milestone counts and state
- GitHub project APIs only when the token has the required project scopes

Generated dashboard artifacts must not persist GitHub tokens, API responses containing secrets, personal workspace data, or unrelated local machine metadata.

## Degraded States

The dashboard must still render when:

- GitHub CLI is missing
- `gh auth status` fails
- network access to GitHub fails
- GitHub issue or milestone API calls fail
- project-board access is unavailable because the token lacks `read:project`
- local files listed in the spec are missing or renamed
- the repository is in a detached or dirty local state

Each degraded state should show:

- affected data source
- impact on dashboard accuracy
- suggested next check or command
- whether implementation may continue safely

Missing project-board access is non-blocking for M4 implementation, but the dashboard must label board state as unverified.

## Data Contract

The implementation should produce or consume a dashboard data model equivalent to this shape. Field names may change only if the implementation updates this spec or records the compatible mapping.

```json
{
  "generatedAtUtc": "2026-09-19T00:00:00Z",
  "repository": {
    "name": "AssetFlow",
    "path": "/mnt/data/AssetFlow",
    "branch": "codex/49-dashboard-spec",
    "isDirty": false,
    "headSha": "string",
    "remoteUrl": "https://github.com/JavadEslamibabaheidari/AssetFlow"
  },
  "milestone": {
    "title": "M4 - Agentic Control Dashboard MVP",
    "url": "https://github.com/JavadEslamibabaheidari/AssetFlow/milestone/5",
    "state": "open",
    "openIssues": 5,
    "closedIssues": 0,
    "projectBoardState": "unverified"
  },
  "activeTask": {
    "issueNumber": 49,
    "title": "Define dashboard MVP spec and data contract",
    "url": "https://github.com/JavadEslamibabaheidari/AssetFlow/issues/49",
    "status": "in-progress"
  },
  "issues": [
    {
      "order": 1,
      "number": 49,
      "title": "Define dashboard MVP spec and data contract",
      "url": "https://github.com/JavadEslamibabaheidari/AssetFlow/issues/49",
      "state": "open",
      "labels": ["feature", "docs", "ai-workflow", "ready"]
    }
  ],
  "documents": [
    {
      "title": "M4 Plan",
      "path": "docs/plans/m4-agentic-control-dashboard-mvp.md",
      "required": true,
      "exists": true
    }
  ],
  "agents": [
    {
      "name": "Small Task Agent",
      "path": ".agents/small-task-agent.md",
      "recommendedFor": "contained fixes and simple maintenance"
    }
  ],
  "skills": [
    {
      "name": "$assetflow-planner",
      "path": ".codex/skills/assetflow-planner/SKILL.md",
      "category": "planning"
    }
  ],
  "workflowActions": [
    {
      "id": "github-sync",
      "label": "GitHub Sync",
      "kind": "write",
      "commandOrPrompt": "$assetflow-github-status",
      "requiresConfirmation": true
    }
  ],
  "activity": [
    {
      "kind": "pull-request",
      "title": "Start M4 dashboard planning",
      "url": "https://github.com/JavadEslamibabaheidari/AssetFlow/pull/54",
      "status": "merged"
    }
  ],
  "degradedStates": [
    {
      "source": "github-project-board",
      "status": "unverified",
      "reason": "GitHub token lacks read:project scope",
      "impact": "Issue and milestone status are available; project-board column status is not verified."
    }
  ]
}
```

## Security And Privacy

- Never write GitHub tokens, environment secrets, OAuth credentials, browser cookies, or personal workspace data into repo files or generated static artifacts.
- Treat GitHub issue, PR, milestone, and project-board data as public-to-repo metadata, not as secrets.
- Do not shell out to mutating commands from the dashboard without explicit user initiation.
- Prefer links and prepared commands/prompts over automatic writes for mutating workflows.
- If the dashboard caches data, the cache must be clearly generated and safe to delete.

## Verification Plan

For issue #49:

- Review this spec against `docs/plans/m4-agentic-control-dashboard-mvp.md`.
- Confirm every #49 acceptance criterion is represented.
- Confirm no implementation-blocking open question remains.

For later M4 implementation issues:

- Verify the generated or served dashboard renders with GitHub data available.
- Verify degraded rendering without GitHub data.
- Verify project-board state is shown as unverified when project scope is missing.
- Verify page load performs no mutating GitHub, Git, or filesystem operation beyond reading allowed sources or writing an explicit generated artifact.
- Verify links and workflow launch points match current local docs.

## Open Questions

- None blocking implementation.

UI composition may iterate in issue #50 as long as this spec's scope, source, security, and action-boundary requirements remain intact.
