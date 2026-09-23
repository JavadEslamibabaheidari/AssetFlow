# M9 External Workspace And Research Integration Plan

## Decision

M9 plans external workspace and research integrations but does not require them for milestone completion. The local repository remains the source of truth for AssetFlow engineering memory, plans, reports, and dashboard controls.

## Candidate Integrations

| Integration | Useful For | M9 Scope | Boundary |
| --- | --- | --- | --- |
| Google Drive / Docs | Sharing research summaries and planning docs | Plan only | Requires connector installation and explicit user approval |
| Google Calendar | Milestone reminders and recurring review rituals | Plan only | Use Codex automations only when explicitly requested |
| Research notebooks | Longer investigations and literature/product notes | Plan only | Summarize repo-safe conclusions into Markdown |
| Slack / Teams | Team handoff and notifications | Out of scope | No notification routing in M9 |
| Notion | Alternative workspace memory | Out of scope | Avoid duplicating `docs/knowledge/` |

## Local-First Fallback

Every integration must have a repository fallback:

- Workspace docs -> Markdown specs/reports in `docs/`
- Research notebook -> `docs/specs/` summary with source links
- Calendar/reminder -> human-readable runbook text unless the user explicitly creates an automation
- Connector status -> dashboard degraded state
- External artifact -> repo-safe summary, not raw export

## Connector Requirements

Before using an external connector:

- Confirm the connector is installed and authorized.
- Confirm the user explicitly wants that service used.
- Identify what data will be read or written.
- Avoid copying credentials, raw transcripts, private notes, or sensitive exports into Git.
- Record degraded fallback behavior when access is unavailable.

## Approval Points

Explicit user approval is required for:

- installing or enabling plugins/connectors
- writing to an external workspace
- creating recurring automations or reminders
- importing external files into the repository
- publishing or sharing repo-derived content externally

## Research Artifact Shape

Research summaries should include:

- question or decision being researched
- sources or links
- concise findings
- recommendation or decision
- security/data-boundary notes
- local follow-up issue or plan link

Raw notebooks, web clippings, or exported documents should not be committed unless they are intentionally sanitized and small enough to review.
