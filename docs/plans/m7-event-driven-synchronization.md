# Milestone Plan: M7 - Event-Driven Synchronization

## Goal

Publish inventory and reservation changes through a durable event boundary, process availability changes into channel synchronization work, and expose enough operations visibility for users to understand sync health, failures, and retry needs.

## Scope

In scope:

- A durable PostgreSQL outbox foundation for inventory, reservation, availability, and channel synchronization events.
- Event contracts and envelopes for stock item creation, availability changes, reservation creation, reservation release, reservation expiration, and channel sync outcomes.
- Publication from application handlers or a clear application/infrastructure boundary after successful business mutations.
- A first channel synchronization worker path that processes availability events idempotently and records pending, successful, failed, and retryable sync attempts.
- OpenAPI and frontend Operations visibility for sync status, recent failures, and backend-backed retry affordances where implemented.
- Docker Compose, CI, and tests updated enough to verify the event/sync path without live marketplace dependencies.
- M7 final validation docs, knowledge updates, GitHub synchronization, tag, and release decision.

Out of scope:

- Live marketplace provider integrations or credentials.
- Multi-service extraction into separate repositories.
- Event sourcing, audit-log rebuilds, projections as source of truth, or historical event replay beyond outbox/worker retry needs.
- Real Kafka deployment as a hard dependency for the first M7 slice. The outbox and worker contracts must be broker-ready, but the milestone starts with PostgreSQL-backed durability and can add a broker adapter only if it remains useful within the existing issue scope.
- Authentication, authorization, tenant isolation, payment, checkout, shipping, catalog enrichment, and customer identity workflows.
- M8 observability depth such as Prometheus/Grafana dashboards, distributed tracing, and full operational alerting.

## Decisions

Decided:

- M7 starts with a PostgreSQL outbox table and application-owned event contracts rather than direct in-process publication or controller-level publication.
- Business handlers remain the mutation boundary. Event records are appended only after validation, not-found, conflict, and oversell checks pass, and within the same database transaction as the state change where a transaction is required.
- Event contracts use a stable envelope with event id, event type, schema version, aggregate identifiers, occurred time, correlation fields when available, payload JSON, status, attempt count, next attempt time, last error, and processed/published timestamps.
- Stock item creation emits both a stock item event and an initial availability signal because downstream channel sync needs the channel/product/stock item identity plus on-hand and available quantities.
- Reservation create, release, and expire workflows emit reservation lifecycle events and availability-change signals for each affected stock item.
- Failed commands, validation errors, not-found results, oversell conflicts, and no-op reservation releases do not create new downstream availability work. Idempotent success responses may return existing state without duplicating events unless the implementation records an explicit no-op event for diagnostics.
- The channel sync worker uses adapter interfaces for marketplace operations. M7 test adapters must be deterministic and local; live providers remain outside scope.
- Sync state is stored in PostgreSQL so the API and frontend can show status consistently. Status values must cover pending, in progress, succeeded, failed, and retryable/next-attempt semantics.
- Retry is explicit and backend-backed. The UI must not fake success or mutate local-only state for retry operations.
- OpenAPI remains the source of truth for new sync-status and retry endpoints, and generated frontend types must be refreshed when the contract changes.

Deferred out of scope:

- Kafka as mandatory runtime infrastructure is deferred until the outbox-backed path proves the contracts and processing semantics. M7 may document Kafka topic mapping and keep code seams broker-ready, but working M7 behavior cannot depend on a live broker unless the implementation issues deliberately add it.
- Full observability dashboards are deferred to M8. M7 records enough status for users and tests but does not build metrics/traces dashboards.
- Real external marketplace adapters are deferred until a provider milestone defines credentials, API quotas, error contracts, and sandbox behavior.

Open questions:

- None.

## Deliverables

- `docs/plans/m7-event-driven-synchronization.md` and updated `docs/knowledge/overview.md` planned behavior.
- Event contract documentation, tests, and outbox persistence migration.
- Application handler changes that append event records for successful stock item and reservation mutations.
- Worker/processor code for idempotent channel synchronization using mockable adapters.
- Sync status persistence, API contract, generated frontend client updates, and Operations page sync visibility.
- Backend, frontend, and integration verification updates.
- M7 milestone report, final knowledge update, GitHub synchronization, tag, and release decision.

## Acceptance Criteria

- [x] M6 closure artifacts are verified: report, knowledge, tag, release, closed milestone, and closed M6 issues.
- [x] Event contracts are documented and covered by tests.
- [x] Event/outbox records are created only when the business mutation succeeds.
- [x] Failed commands, validation failures, not-found results, oversell conflicts, and retry failures do not lose source state or create false downstream success.
- [x] Reservation create, release, and expire workflows produce expected events after successful commits.
- [x] Stock item creation and availability recalculation produce expected downstream signals.
- [x] Channel synchronization processes relevant availability events idempotently.
- [x] Failed sync attempts are recorded with retry state while preserving the source event.
- [x] Operations users can see healthy, pending, failed, and retryable sync state.
- [x] Retry actions are backend-backed and test-covered when exposed. M7 exposes automatic backend retry scheduling; manual retry commands remain deferred because no explicit retry endpoint was added.
- [x] OpenAPI changes trigger backend, frontend, and integration checks.
- [x] Docker Compose or CI integration proves the event/sync path sufficiently for the milestone.
- [x] `docs/knowledge/` and the M7 report match the implemented final state before milestone closure.

## Task Breakdown

| Order | Priority | Task | Objective | Depends on | Verification | GitHub issue |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | Critical prerequisite | M7 start gate | Verify M6 closure, define the M7 plan, confirm follow-up issues, and update durable planned knowledge. | M6 closed and released | Docs review, GitHub milestone/issues verified, `git diff --check` | #85 |
| 2 | Critical foundation | Event/outbox foundation | Add event envelopes, event contracts, outbox persistence, and transaction-safe append behavior. | #85 | Backend unit/application tests, migration/model tests, failed-command tests | #86 |
| 3 | High implementation | Publish reservation and availability events | Emit stock item, reservation lifecycle, and availability events from successful M2/M3 mutation paths. | #86 | Backend tests for success, failure, no-op, oversell, and payload shape | #87 |
| 4 | High integration | Channel synchronization worker | Process availability events into channel sync attempts with idempotency, adapter abstraction, failure recording, and retry rules. | #86, #87 | Worker unit/integration tests, retry/failure tests, Compose or CI path where practical | #89 |
| 5 | Medium product surface | Operations sync visibility | Add contract/API/frontend visibility for sync status, recent failures, and backend-backed retry where implemented. | #86, #89 | OpenAPI generation, backend endpoint tests, frontend component tests, workflow checks | #88 |
| 6 | Critical validation | M7 validation, docs, and release closure | Verify planned behavior, update reports/knowledge, synchronize GitHub, and prepare tag/release after merge. | #86, #87, #89, #88 | Backend CI, Frontend CI, Integration CI, Docker Compose or documented CI equivalent, docs review | #90 |

## Prioritization Notes

The outbox foundation comes first because it fixes the reliability boundary and event contract language that every later slice depends on. Reservation/availability publication follows before the worker so tests can prove real event payloads from existing business workflows. The worker precedes frontend visibility because the operations page should report real backend state and retry behavior rather than a placeholder. Final validation remains last because it must compare the implementation against this plan, issues, tests, and docs.

## Event Contract Boundaries

M7 event contracts are integration events emitted from successful application use cases, not domain events raised from controllers. The first planned event families are:

- `StockItemCreated`: identifies stock item, product, channel, on-hand quantity, available quantity, and update time.
- `StockAvailabilityChanged`: identifies stock item, product, channel, on-hand quantity, reserved quantity where available, available quantity, next expiration where available, reason, and source mutation id.
- `ReservationCreated`: identifies reservation, stock item, quantity, expiration, status, and creation time.
- `ReservationReleased`: identifies reservation, stock item, quantity, release time, and resulting status.
- `ReservationExpired`: identifies reservation, stock item, quantity, expiration time, and resulting status.
- `ChannelSyncRequested`: records work derived from availability changes for a channel/stock item pair.
- `ChannelSyncSucceeded` and `ChannelSyncFailed`: record adapter outcome, attempt number, retry eligibility, and error detail safe for operations users.

The outbox schema and payload tests must make schema versioning explicit so later Kafka topics or service splits can consume the same event meanings without rewriting existing handlers.

## Data and Infrastructure Plan

- Add an `outbox_messages` table owned by `Inventory.Api` for durable event records and processing metadata.
- Add channel sync state tables only when #89/#88 need persisted status; keep them normalized around channel id, stock item id, source event id, status, attempt count, last attempted time, next attempt time, completed time, and safe error summary.
- Use EF Core migrations for new tables and indexes.
- Keep in-memory test support for application tests, but add provider-aware coverage where PostgreSQL-specific transaction behavior or SQL constraints matter.
- Extend Docker Compose only as far as needed for realistic local/CI verification. PostgreSQL-backed event persistence is required; Kafka remains optional in M7.

## API and Frontend Plan

- Add OpenAPI operations for sync status reads and retry commands only after backend state exists.
- Regenerate frontend API types when the OpenAPI contract changes.
- Expand `OperationsPage` from channel master data into a sync-health surface with loading, empty, healthy, pending, failed, retryable, and stale states.
- Keep retry controls disabled or absent until the backend retry command exists.

## Risks and Mitigations

- Risk: Event records drift from business state if publication happens outside the transaction. Mitigation: write outbox records through the same application boundary and transaction as the mutation.
- Risk: M7 grows into a full broker/provider integration milestone. Mitigation: keep broker/provider work out of scope and make the outbox/adapter seams explicit.
- Risk: Sync retries duplicate marketplace updates. Mitigation: design worker processing around source event ids, channel/stock item keys, attempt state, and idempotent adapter contracts.
- Risk: Frontend exposes controls before backend behavior is real. Mitigation: gate retry affordances on implemented API operations and test the UI states against mocked backend responses.
- Risk: In-memory tests miss PostgreSQL transaction behavior. Mitigation: keep focused unit tests fast and add provider/integration coverage for transaction, migration, and locking-sensitive behavior.

## Testing Strategy

- Unit: event envelope creation, payload mapping, availability calculation inputs, sync-status mapping, frontend sync-state rendering.
- Application/backend: successful and failed command paths for stock item create, reservation create/release/expire, no event on validation/not-found/conflict/oversell, outbox append semantics, retry state transitions.
- Integration: EF Core migration/model coverage for outbox and sync tables; worker processing with deterministic adapters; API contract tests for sync reads/retry commands.
- Contract: OpenAPI validation and generated frontend type refresh for new sync endpoints.
- Frontend: Operations page loading, empty, healthy, pending, failed, retryable, and retry-action states.
- Concurrency/failure: transaction safety around reservation changes, idempotent worker handling, failed adapter attempts, retry scheduling, and preservation of source events.
- End-to-end: Docker Compose or CI integration path that runs API, web, PostgreSQL-backed persistence, and worker processing where practical.

## GitHub Tracking

- Milestone: `M7 - Event-Driven Synchronization` is closed.
- Issues: #85 start gate, #86 event/outbox foundation, #87 reservation and availability events, #88 operations visibility, #89 channel synchronization worker, and #90 final validation/docs/release closure are closed.
- Project board: project item and column mapping is unavailable from this environment; issue project item lists are empty in `gh issue view`.
- Follow-up issue alignment: #86 through #90 matched this plan's task sequence. No scope change was required; live marketplace providers, Kafka as runtime infrastructure, observability dashboards, and manual retry commands remain deferred.

## Previous Milestone Closure

- Report: `docs/milestone-6-report.md` exists and was updated by this start gate to reflect the verified merged, tagged, released, and closed state.
- Knowledge/docs: `docs/knowledge/overview.md` captures M6 frontend parity and is updated by this start gate with M7 planned behavior.
- GitHub status: milestone `M6 - Frontend Inventory Parity` is closed with zero open issues; issues #74 through #81 are closed; release `v0.6.0 - M6 Frontend Inventory Parity` is published from tag `v0.6.0`.
- Result: previous milestone closure hook passed; the only tracked gap is unavailable project-board column mapping.

## Start Gate Result

- [x] Previous milestone report exists or was not required.
- [x] Previous milestone durable facts are captured in `docs/knowledge/`.
- [x] Plan is complete enough to implement.
- [x] No implementation-blocking decisions remain open.
- [x] GitHub tracking matches this plan.
- [x] Knowledge docs capture durable planned behavior.

Status: Start gate passed and the planned M7 implementation slices have been completed. See `docs/milestone-7-report.md` for final validation and release closure notes.
