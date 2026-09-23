# Milestone Plan: M8 - Observability and Monitoring

## Goal

Make AssetFlow observable enough for local operators and future agents to diagnose API health, outbox backlog, channel synchronization failures, retry delay, and request correlation without adding mandatory production monitoring infrastructure.

## Scope

In scope:

- Structured request logging with trace/correlation identifiers and safe, non-sensitive fields.
- Backend health, JSON metric summary, and Prometheus-compatible metric endpoints for service, outbox, and synchronization state.
- Lightweight trace hooks around channel synchronization processing using built-in .NET diagnostics.
- Optional Docker Compose monitoring profile with Prometheus and Grafana provisioning.
- Frontend Operations visibility for service health, outbox backlog, synchronization failure pressure, retry delay, and trace/correlation hints.
- OpenAPI, generated frontend types, tests, docs, and durable knowledge updates.

Out of scope:

- Mandatory hosted monitoring, alert routing, paging, or production incident policy.
- Full OpenTelemetry collector deployment, OTLP export, or vendor-specific APM integration.
- Authentication, authorization, tenant isolation, or secret management.
- Live marketplace provider observability beyond the M7 adapter boundary.
- Deep per-agent/per-skill cost accounting and workspace automation telemetry; that remains M9.

## Decisions

Decided:

- M8 starts with dependency-light observability using ASP.NET Core logging, `ActivitySource`, JSON operational summaries, and a Prometheus text endpoint instead of adding a required observability package stack.
- `/health` remains the simple compatibility endpoint. New observability detail lives under `/observability/*`.
- Prometheus and Grafana run through an optional Compose profile so normal local development remains fast and does not require monitoring containers.
- Metrics are derived from persisted state that already exists after M7: `outbox_messages` and `channel_sync_states`.
- Metric names use the `assetflow_` prefix and expose low-cardinality labels only, such as event type and status.
- Operations UI displays summarized operational signals, not a full dashboard editor.
- Trace/correlation identifiers are safe to expose to operators and are useful for copying into logs or distributed traces later.

Deferred out of scope:

- OTLP exporters and collector wiring are deferred until a deployment milestone chooses a collector topology.
- Alert rules and notification channels are deferred until thresholds are validated with real workload data.
- Workflow usage and AI cost signals remain roadmap scope for M9 unless an existing local source is cheap and reliable during final validation.

Open questions:

- None.

## Deliverables

- `docs/plans/m8-observability-and-monitoring.md` and updated `docs/knowledge/overview.md`.
- Backend observability contracts and endpoints under `/observability`.
- Structured request logging middleware and channel synchronization tracing hooks.
- Prometheus scrape configuration and Grafana datasource/dashboard provisioning under `deploy/monitoring/`.
- Docker Compose monitoring profile.
- OpenAPI and generated frontend client updates.
- Operations page observability panel with tests.
- M8 report and final GitHub synchronization after validation.

## Acceptance Criteria

- [x] M7 closure artifacts are verified: report, knowledge, closed GitHub issues/milestone, tag, and release.
- [x] M8 GitHub milestone and issue set match this plan.
- [x] API requests log method, path, status, elapsed time, trace id, and correlation id without sensitive payloads.
- [x] Channel synchronization processing creates diagnostic activity spans with low-cardinality tags for batch size, processed count, and outcome.
- [x] `GET /observability/health` returns service health plus outbox and synchronization summary.
- [x] `GET /observability/metrics` returns JSON operational metrics for frontend and agent consumption.
- [x] `GET /observability/prometheus` returns Prometheus-compatible text metrics for local scraping.
- [x] OpenAPI and generated frontend types include the new observability surface.
- [x] Operations page shows health, backlog, failure, retry, and trace/correlation summaries with loading/error states.
- [x] Docker Compose validates with optional Prometheus/Grafana monitoring services.
- [x] Tests cover backend observability endpoints, metric formatting, processor diagnostics, frontend client wiring, and UI states.
- [x] M8 report and knowledge updates match the implemented final state before closure.

## Task Breakdown

| Order | Priority | Task | Objective | Depends on | Verification | GitHub issue |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | Critical prerequisite | M8 start gate | Verify M7 closure, define the M8 plan, create matching issues, and update durable planned knowledge. | M7 closed and released | Docs review, GitHub milestone/issues verified, `git diff --check` | #100 |
| 2 | Critical foundation | API observability endpoints and instrumentation | Add request logging, trace/correlation handling, backend health details, JSON metrics, Prometheus metrics, and channel sync trace hooks. | #100 | Backend endpoint tests, processor diagnostics tests, OpenAPI validation | #98 |
| 3 | High infrastructure | Prometheus and Grafana local monitoring path | Add optional Compose monitoring services, Prometheus scrape config, Grafana provisioning, and docs. | #98 | `docker compose config --quiet`, Prometheus config review, docs review | #101 |
| 4 | High product surface | Operations observability UI | Add typed frontend client methods and an Operations panel for health, backlog, failures, retry delay, and trace/correlation hints. | #98 | Frontend unit/component tests, typecheck, generated API diff review | #99 |
| 5 | Critical validation | M8 validation, docs, and release closure | Verify planned behavior, update reports and knowledge, synchronize GitHub, and prepare tag/release after merge. | #98, #101, #99 | Backend CI, Frontend CI, Integration CI or local equivalents, docs review | #102 |

## Risks and Mitigations

- Risk: Observability grows into a full monitoring platform. Mitigation: keep M8 focused on first useful signals and optional local monitoring containers.
- Risk: Metrics endpoints expose high-cardinality or sensitive data. Mitigation: use aggregate counts and low-cardinality status/event labels only.
- Risk: Frontend dashboards duplicate Grafana. Mitigation: Operations shows concise product health summaries; Grafana owns charts and time-series views.
- Risk: Tracing without exporters looks incomplete. Mitigation: add `ActivitySource` hooks and request correlation now, then defer exporter topology until deployment needs it.
- Risk: Monitoring containers slow normal local development. Mitigation: place Prometheus and Grafana behind a Compose profile.

## Testing Strategy

- Unit: metric text formatting, health classification, trace/correlation id handling, frontend health summary rendering.
- Application/backend: observability endpoint responses against seeded outbox and sync state, request logging middleware behavior where practical, channel synchronization diagnostics around success and failure.
- Contract: OpenAPI operations and generated frontend types for `/observability/*`.
- Frontend: Operations page loading, healthy, degraded, failed, and API-error states.
- Infrastructure: `docker compose config --quiet` and monitoring configuration review.
- Failure: failed outbox/sync state must degrade health and expose retry delay without leaking unsafe error details.

## GitHub Tracking

- Milestone: `M8 - Observability and Monitoring` is open.
- Issues: #100 start gate, #98 API observability endpoints and instrumentation, #101 Prometheus/Grafana local monitoring path, #99 Operations observability UI, and #102 validation/docs/release closure.
- Project board: project-board item and column mapping remains unavailable from this environment; GitHub issue/milestone tracking is available.

## Previous Milestone Closure

- Report: `docs/milestone-7-report.md` exists and records M7 as complete, merged, tagged, released, and closed.
- Knowledge/docs: `docs/knowledge/overview.md` captures M7 outbox, event publication, channel synchronization, sync status API, Operations UI, and known gaps.
- GitHub status: M7 issues #85 through #90 are closed; PRs #91 through #96 are merged; tag and release `v0.7.0 - M7 Event-Driven Synchronization` exist; the M7 milestone is closed.
- Result: previous milestone closure hook passed; the only tracked gap is unavailable project-board column mapping.

## Start Gate Result

- [x] Previous milestone report exists or was not required.
- [x] Previous milestone durable facts are captured in `docs/knowledge/`.
- [x] Plan is complete enough to implement.
- [x] No implementation-blocking decisions remain open.
- [x] GitHub tracking matches this plan.
- [x] Knowledge docs capture durable planned behavior.

Status: Implementation complete and ready for review.
