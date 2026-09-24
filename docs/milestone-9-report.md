# M9 Report - Agentic OS Expansion

## Status

M9 was reopened on 2026-09-23 after product feedback that the milestone should deliver a richer AI-side Agentic OS cockpit rather than a simple static reporter. The cockpit adoption was accepted and M9 was closed again on 2026-09-24. The original `codex/m9-finalize-agentic-os` work remains the baseline for local-first memory, automation, research, resource telemetry, tracking, and degraded-source visibility; `codex/m9-agentic-os-cockpit` adds the cockpit adoption.

## Implemented

- Added the M9 memory and security model in `docs/specs/m9-agentic-os-memory-security.md`.
- Added the M9 automation catalog, skillpack surface, daily runbooks, resource telemetry schema, privacy rules, and degraded-source behavior in `docs/specs/m9-agentic-os-automation-telemetry.md`.
- Added the M9 external workspace and research integration plan in `docs/specs/m9-agentic-os-integrations.md`.
- Reworked `tools/agent-dashboard/generate.sh` from the M4 dashboard into the Agentic OS dashboard.
- Dashboard now surfaces M9 overview, GitHub tracking, project-board degraded state, memory, automation, resource telemetry, research/integration, workflow launch points, recent activity, and unavailable/degraded sources.
- Cockpit adoption upgrades the dashboard into an Agentic OS cockpit with a command-model summary, animated bars, runtime mix chart, workflow graph, live clock, operating-mode tabs, human approval queue, evaluation/tracing cues, and safer degraded-source visibility.
- Kept generated dashboard output under the ignored `tools/agent-dashboard/dist/` path.
- Updated `tools/agent-dashboard/README.md`, `README.md`, roadmap, M9 plan, and durable project knowledge.

## Planned-State Validation

- M9 remains local-first and Git-reviewable.
- `docs/knowledge/` remains the curated engineering knowledge cache.
- Obsidian, vector stores, external drive sync, personal notebook sync, hosted memory infrastructure, and external connector dependencies are deferred.
- Dashboard controls remain safe local UI, prompts, links, and command text; the dashboard does not run mutating operations. Client-side interaction is allowed for live-feeling cockpit views such as tabs, clocks, bars, and workflow highlighting.
- Resource telemetry records only trustworthy sources and marks unavailable token/cost fields as unavailable.
- External workspace and research integrations have local Markdown fallbacks and explicit approval boundaries.
- Generated dashboard output, secrets, raw prompts, raw transcripts, personal/private data, and guessed usage/cost values are not committed.

## Verification

- `./tools/agent-dashboard/generate.sh /tmp/assetflow-agentic-os-dashboard`
- `ASSETFLOW_DASHBOARD_DISABLE_GITHUB=1 ./tools/agent-dashboard/generate.sh /tmp/assetflow-agentic-os-dashboard-no-github`
- `rg -n "AssetFlow Agentic OS Dashboard|Memory Layer|Resource Telemetry|Research And Workspace Integrations|Unavailable Or Degraded Sources" /tmp/assetflow-agentic-os-dashboard/index.html`
- `rg -n "GitHub reads disabled|M9 issue sequence|Memory Layer|Resource Telemetry" /tmp/assetflow-agentic-os-dashboard-no-github/index.html`
- `bash -n tools/agent-dashboard/generate.sh`
- `git diff --check`

## GitHub Status

- M9 milestone exists as `M9 - Agentic OS Expansion`.
- M9 tracking issues exist: #104 start gate, #105 memory/security, #106 automation/skillpack, #107 resource telemetry, #108 dashboard controls, #109 external workspace/research plan, and #110 validation/docs/release closure.
- #104 through #107 and #109 remain closed from the original M9 foundation.
- #108 and #110 were reopened for cockpit adoption and validation, then closed as completed on 2026-09-24.
- The GitHub milestone was reopened with the explicit goal of adopting orchestration, memory, workflow graph, human approvals, evaluation/tracing, governance, and live visual telemetry beyond a static reporter, then closed after acceptance.
- Project-board item and column mapping remains unavailable from this environment.

## Known Gaps

- External connectors are planned but not installed or required.
- Token/cost telemetry remains unavailable unless the user supplies sanitized exports in a later milestone.
- Project-board column mapping remains unavailable from this environment.
- Hosted memory, vector search, alerting, scheduler services, and production access control remain deferred.

## Next

M9 cockpit adoption is accepted. The remaining release question is whether to create a corrective `v0.9.1` tag/release or document the cockpit adoption as a post-`v0.9.0` milestone correction.
