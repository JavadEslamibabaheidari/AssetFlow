# M9 Agentic OS Automation And Resource Telemetry

## Automation Model

M9 defines a repo-local automation catalog, not a hosted scheduler or autonomous write system.

Automation entries fall into three groups:

| Group | Examples | Write Boundary |
| --- | --- | --- |
| Read-only checks | Git status, GitHub milestone state, recent merged PRs, docs availability, dashboard generation | Safe to run when requested |
| Suggested prompts | Planning, implementation, testing, review, knowledge update, GitHub sync | User chooses when to send |
| Approval-required actions | Issue edits, PR creation/merge, branch deletion, tag/release creation, connector writes, scheduled reminders | Must use explicit user/tool approval |

The static dashboard may display launch points, prompts, links, and command text. It must not run mutating operations by itself.

## Skillpack Surface

The M9 dashboard groups the existing AssetFlow skills around the large-feature workflow:

- `$assetflow-github-status`: inspect and synchronize GitHub state
- `$assetflow-knowledge-keeper`: maintain durable project knowledge
- `$assetflow-planner`: plan milestone and feature work
- `$assetflow-prioritizer`: order work by dependency, risk, and value
- `$assetflow-dotnet-implementer`: implement scoped changes
- `$assetflow-tester`: verify by risk
- `$assetflow-reviewer`: run the quality gate
- `$assetflow-frontend-motion`: apply motion guidance when frontend animation changes

## Daily Workflow Runbooks

Daily or recurring workflow prompts should be stored as human-readable runbook text, not raw scheduler directives.

Recommended runbooks:

- Morning status: inspect active milestone issues, open PRs, CI state, and local branch cleanliness.
- Planning refresh: compare roadmap, current milestone plan, GitHub issues, and `docs/knowledge/`.
- Work completion: run relevant checks, update reports/knowledge, synchronize GitHub, and verify no generated artifacts are committed.
- Research intake: summarize external findings into repo-safe Markdown with source links and data-boundary notes.

## Resource Telemetry Schema

M9 records resource usage using trustworthy repo-local and GitHub-derived signals.

| Field | Meaning | Source | Required |
| --- | --- | --- | --- |
| `workflow` | Named workflow or milestone task | Plan/report/issue | Yes |
| `agent_or_skill` | Skill, agent, or workflow used | Plan/report/dashboard catalog | Yes |
| `github_issue` | Tracked issue number | GitHub | When tracked |
| `pull_request` | PR number | GitHub | When available |
| `verification` | Commands/checks run | PR/report | Yes |
| `elapsed_time` | Wall-clock duration if explicitly known | Optional user/tool export | No |
| `token_usage` | Provider token count | Optional export only | No |
| `cost` | Provider cost | Optional export only | No |
| `data_quality` | Complete, partial, unavailable | Derived | Yes |

Unavailable token/cost fields must be shown as unavailable. They must not be estimated from elapsed time, issue count, file count, or subjective effort.

## Privacy Rules

- Do not ingest raw prompts, raw transcripts, personal notes, credentials, billing exports, or customer data.
- Do not use high-cardinality personal identifiers as telemetry labels.
- Optional user-provided exports must be sanitized before becoming repo memory.
- Dashboard summaries should explain partial data instead of hiding gaps.

## Degraded States

The dashboard must show explicit degraded states for:

- missing `gh`
- GitHub reads disabled
- project-board access unavailable
- connector/plugin access unavailable
- token/cost data unavailable
- missing optional specs or reports
