# M8 Report - Observability and Monitoring

## Status

M8 implementation is complete on branch `codex/m8-observability-monitoring` and ready for review. The milestone adds dependency-light observability for local operators and future agents: structured request logs, trace/correlation identifiers, JSON health and metric summaries, Prometheus-compatible metrics, optional Prometheus/Grafana Compose monitoring, and Operations page visibility for key service signals.

## Implemented

- Added request observability middleware that propagates `X-Correlation-ID`, returns the correlation id header, creates request activities, and logs method, path, status, elapsed time, trace id, and correlation id without request or response payloads.
- Added built-in .NET `ActivitySource` diagnostics around channel synchronization batch and message processing.
- Added `/observability/health`, `/observability/metrics`, and `/observability/prometheus`.
- Added aggregate outbox and channel synchronization metrics derived from `outbox_messages` and `channel_sync_states`.
- Added Prometheus text exposition for service health, outbox counts by status/event type, channel sync status counts, retryable failures, and next retry timestamp.
- Added OpenAPI contracts, generated frontend types, and typed frontend client methods for the JSON observability endpoints.
- Expanded the Operations page with a service observability panel for health, outbox backlog, sync failures, next retry, trace id, correlation id, and last checked time.
- Added optional Docker Compose `monitoring` profile with Prometheus scrape configuration and Grafana provisioning for an AssetFlow overview dashboard.
- Updated README, M8 plan, and durable project knowledge.

## Planned-State Validation

- M8 stayed within the planned dependency-light approach: no mandatory hosted monitoring, collector, exporter, or provider-specific APM integration was added.
- `/health` remains the simple compatibility endpoint; detailed observability lives under `/observability/*`.
- Metrics are aggregate and low-cardinality; no payloads, credentials, customer data, or high-cardinality identifiers are exposed as Prometheus labels.
- Prometheus/Grafana are optional Compose profile services and do not affect normal local startup.
- Operations UI summarizes operational state and does not try to replace Grafana as a time-series dashboard.

## Verification

- `dotnet test MarketplaceInventoryPlatform.sln --configuration Release -v minimal`
- `dotnet format MarketplaceInventoryPlatform.sln --verify-no-changes --no-restore`
- `npm run typecheck`
- `npm run test`
- `npm run lint`
- `npm run build`
- `npm run format:check`
- `docker compose config --quiet`
- `git diff --check`

## GitHub Status

- M8 milestone exists as `M8 - Observability and Monitoring`.
- M8 tracking issues exist: #100 start gate, #98 backend observability, #101 local monitoring path, #99 Operations UI, and #102 validation/docs/release closure.
- This branch implements the planned M8 issues in one integrated milestone branch at the user's request.
- Project-board item and column mapping remains unavailable from this environment.

## Known Gaps

- OTLP exporter, collector deployment, alert rules, and notification routing remain deferred until deployment topology and real operational thresholds are known.
- Workflow usage and AI cost signals remain deferred to M9.
- Live provider-specific marketplace telemetry remains deferred until real adapters and credentials exist.
- GitHub issues and the M8 milestone should be closed after this branch is reviewed, merged, tagged as `v0.8.0`, and released.

## Next

After merge, close the M8 issues, tag `main` as `v0.8.0`, create the M8 release, and then begin M9 planning from the completed observability baseline.
