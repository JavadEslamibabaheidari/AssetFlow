# Milestone Plan: M9 - Agentic OS Expansion

## Goal

Expand the repo-local Agentic Control Dashboard into a broader, safe human-plus-AI operating layer for project memory, repeatable automation, research context, and resource-aware agent workflows.

## Scope

In scope:

- Evaluate and decide the M9 memory layer before adopting Obsidian, repo-local notes, vector stores, external drives, or other memory tooling.
- Define security, privacy, retention, and access boundaries for memory, automation, research, and usage telemetry artifacts.
- Add a repo-local automation and skillpack foundation for repeated AssetFlow project, workspace, and research workflows.
- Define a resource usage telemetry model for skills, agents, workflow runs, automation runs, and known unavailable cost/token fields.
- Expand the static Agentic Control Dashboard with M9 memory, automation, resource, research, and degraded-source controls.
- Define the external workspace and research notebook integration plan, including connector/plugin prerequisites and local-first fallbacks.
- Keep generated dashboard output, secrets, credentials, raw prompts, raw transcripts, and personal/private data out of Git.
- Update docs, knowledge, GitHub tracking, and release closure records.

Out of scope:

- Installing or requiring external workspace connectors, plugins, Obsidian, Google Workspace, or research notebook services.
- Storing credentials, personal/private notes, raw AI transcripts, or prompt logs in this repository.
- Building a production multi-user automation platform, scheduler service, permissions system, or hosted memory service.
- Deep token/cost accounting from unavailable provider APIs; M9 records unavailable fields explicitly instead of inventing numbers.
- Replacing `docs/knowledge/` as the authoritative curated repo knowledge cache.
- Making the static dashboard run mutating operations directly.
- Authentication, authorization, tenant isolation, or production deployment hardening.

## Decisions

Decided:

- M9 stays local-first and Git-reviewable. New durable artifacts must live in docs, specs, or repo-local tooling with clear schemas and no secrets.
- `docs/knowledge/` remains the curated engineering knowledge cache. Any broader memory layer must complement it rather than duplicate or contradict it.
- Memory adoption starts with evaluation and a security model. External or personal knowledge tools are not adopted until their data boundaries and fallback behavior are documented.
- Automation is represented as a catalog of read-only checks, suggested prompts, runbooks, and explicit command text. Mutating actions require user approval through existing Codex/GitHub workflows.
- Resource telemetry starts from trustworthy low-risk sources: GitHub issues/PRs, local dashboard metadata, workflow docs, verification commands, and optional user-provided exports. Unavailable token/cost data is recorded as unavailable.
- The Agentic Control Dashboard remains a generated static repo-local artifact. It may display controls and command text, but it must not perform writes by itself.
- External workspace and research notebook integrations are planned with connector requirements and local-first fallbacks; M9 does not depend on unavailable connector access.

Deferred out of scope:

- Hosted memory, vector search infrastructure, remote note synchronization, and external workspace automation writes.
- Provider-specific token/cost integrations and billing dashboards.
- Alerting or scheduled notification routing for agent workflows.
- Production access control and multi-user governance.

Open questions:

- None.

## Deliverables

- `docs/plans/m9-agentic-os-expansion.md` and updated `docs/knowledge/overview.md`.
- Memory layer evaluation and security model.
- Repo-local automation catalog and skillpack/workflow guidance.
- Resource usage telemetry schema, source mapping, and degraded-data behavior.
- Expanded Agentic Control Dashboard controls for memory, automation, resource telemetry, research, tracking, and degraded states.
- External workspace and research notebook integration plan with connector prerequisites and local-first fallback.
- M9 report, final planned-state validation, and release closure updates.

## Acceptance Criteria

- [x] M8 closure artifacts are verified: report, knowledge, closed milestone/issues, tag, and release.
- [x] M9 GitHub milestone and issue set match this plan.
- [x] Memory-layer options are evaluated and a concrete M9 decision is recorded with security/privacy boundaries.
- [x] Automation catalog distinguishes read-only status, suggested prompts, and mutating actions that require explicit approval.
- [x] Resource telemetry schema records trustworthy workflow/agent/skill signals and marks unavailable cost/token fields honestly.
- [x] Dashboard expansion surfaces memory, automation, resource, research, tracking, and degraded-source sections without committing generated output.
- [x] External workspace and research notebook integration plan records value, connector needs, security boundaries, and local-first fallbacks.
- [x] Tests or review checks cover changed tooling and dashboard behavior.
- [x] Docs, knowledge, GitHub tracking, and final M9 report match implemented behavior before closure.

## Task Breakdown

| Order | Priority | Task | Objective | Depends on | Verification | GitHub issue |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | Critical prerequisite | M9 start gate | Verify M8 closure, define the M9 plan and safety boundaries, create matching issues, and update durable planned knowledge. | M8 closed and released | Docs review, GitHub milestone/issues verified, `git diff --check` | #104 |
| 2 | Critical foundation | Memory layer and security model | Evaluate memory options, decide the M9 memory approach, and define security/privacy/retention/access rules. | #104 | Docs/spec review, security checklist review | #105 |
| 3 | High foundation | Workflow automation and skillpack foundation | Define repo-local automation catalog, recurring workflow runbooks, skillpack guidance, and approval boundaries. | #104, #105 | Docs/spec review, dashboard/tooling tests if behavior changes | #106 |
| 4 | High foundation | Resource usage telemetry model | Define safe workflow/agent/skill telemetry schema, sources, aggregation, and unavailable-field behavior. | #104, #105 | Schema/spec review, tooling tests if code is added | #107 |
| 5 | High product surface | Agentic OS dashboard controls | Expand the dashboard with memory, automation, resource, research, tracking, and degraded-source controls. | #105, #106, #107 | Normal/degraded dashboard generation and HTML review | #108 |
| 6 | Medium integration | External workspace and research notebook plan | Define integration value, connector prerequisites, security boundaries, and local-first research fallbacks. | #105, #106 | Docs/spec review and connector fallback review | #109 |
| 7 | Critical validation | M9 validation, docs, and release closure | Validate final implementation against plan, update reports and knowledge, synchronize GitHub, and prepare tag/release closure. | #105, #106, #107, #108, #109 | Relevant tests/checks, dashboard generation if changed, docs review | #110 |

## Prioritized Execution

1. Critical prerequisite: #104 records the plan and start gate so implementation does not begin from roadmap bullets.
2. Critical foundation: #105 decides memory and security boundaries before automation or dashboard work stores or displays new context.
3. High foundation: #106 defines repeatable automation and skillpack workflows after the memory boundaries are known.
4. High foundation: #107 defines resource telemetry early so the dashboard does not invent usage or cost signals later.
5. High product surface: #108 expands the Agentic Control Dashboard after the memory, automation, and telemetry contracts exist.
6. Medium integration: #109 plans external workspace and research notebook integration without blocking local-first Agentic OS value.
7. Critical validation: #110 verifies implementation, docs, knowledge, GitHub status, and release closure.

## Risks and Mitigations

- Risk: Agentic OS scope expands into a full external automation platform. Mitigation: keep M9 local-first, static-dashboard-oriented, and Git-reviewable.
- Risk: memory artifacts leak secrets, private notes, or raw transcripts. Mitigation: require a security model before adoption and explicitly ban credentials, raw prompts, raw transcripts, and personal/private data in Git.
- Risk: usage telemetry creates false precision. Mitigation: record only trustworthy sources and show unavailable token/cost fields as unavailable.
- Risk: dashboard controls imply mutating actions that are not safe. Mitigation: keep mutating operations as prompts, links, or command text requiring explicit approval.
- Risk: external connectors are unavailable. Mitigation: define connector prerequisites and local-first fallbacks; do not make M9 depend on connector installation.
- Risk: project-board state remains inaccessible. Mitigation: continue recording issue/milestone truth and show project-board mapping as unavailable when not readable.

## Testing Strategy

- Docs/spec: memory security model, automation catalog, telemetry schema, integration plan, and milestone plan review.
- Tooling: dashboard generator checks in normal mode and degraded mode when dashboard behavior changes.
- Static: `git diff --check` for all branches; relevant format/lint checks for touched frontend/tooling code.
- Frontend/backend: only run affected tests when M9 changes production app or API code.
- Failure/degraded: missing GitHub, project-board, connector, and usage-source states must render as explicit unavailable/degraded states.

## GitHub Tracking

- Milestone: `M9 - Agentic OS Expansion` is open.
- Issues: #104 start gate, #105 memory/security, #106 automation/skillpack foundation, #107 resource telemetry, #108 dashboard controls, #109 external workspace/research plan, and #110 validation/docs/release closure.
- Project board: project-board item and column mapping remains unavailable from this environment; GitHub issue/milestone tracking is available.

## Previous Milestone Closure

- Report: `docs/milestone-8-report.md` exists and records M8 as complete.
- Knowledge/docs: `docs/knowledge/overview.md` captures M8 request observability, correlation IDs, JSON and Prometheus observability endpoints, optional monitoring profile, and Operations page observability behavior.
- GitHub status: M8 issues #98 through #102 are closed; PR #103 is merged; tag and release `v0.8.0 - M8 Observability and Monitoring` exist; the M8 milestone is closed.
- Result: previous milestone closure hook passed; the only tracked gap is unavailable project-board column mapping.

## Start Gate Result

- [x] Previous milestone report exists or was not required.
- [x] Previous milestone durable facts are captured in `docs/knowledge/`.
- [x] Plan is complete enough to implement.
- [x] No implementation-blocking decisions remain open.
- [x] GitHub tracking matches this plan.
- [x] Knowledge docs capture durable planned behavior.

Status: Implementation complete and ready for review.
