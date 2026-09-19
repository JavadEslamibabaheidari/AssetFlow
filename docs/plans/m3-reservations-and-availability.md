# Milestone Plan: M3 - Reservations and Availability

## Goal

Add reservation-aware availability so AssetFlow can safely reserve stock, release or expire holds, and prevent overselling through concurrency-safe application commands.

## Scope

In scope:

- Extend the Inventory API contract with reservation and availability endpoints.
- Add a reservation domain model and PostgreSQL-backed EF Core persistence.
- Calculate stock availability as on-hand quantity minus active, unexpired reservation quantities.
- Create reservations only when enough stock is available.
- Release active reservations and expire reservations whose expiration time has passed.
- Expose availability for a stock item through a MediatR query.
- Add concurrency and failure tests around oversell prevention, release, and expiration.

Out of scope:

- Marketplace event publication, Kafka, outbox, and channel synchronization. These start in M4.
- Payment, checkout, order fulfillment, shipment, or customer identity workflows.
- Authentication, authorization, tenant isolation, and reservation ownership permissions.
- Partial reservation fulfillment or allocation across multiple stock items.
- Automatic background expiration workers. M3 adds an explicit expiration command/API; scheduled or worker-based expiration can follow later.
- Updating on-hand stock after stock item creation. M3 focuses on reservation holds over the M2 stock item state.

## Decisions

Decided:

- Keep M3 in the existing `Inventory.Api` project and use the M2 folder pattern: `Domain`, `Application`, `Infrastructure`, and `Api`.
- Use MediatR commands and queries for reservation creation, release, expiration, listing, getting, and availability reads.
- Add a `Reservation` aggregate/entity with `Id`, `StockItemId`, `Quantity`, `Status`, `ExpiresAtUtc`, `CreatedAtUtc`, `UpdatedAtUtc`, and optional `ReleasedAtUtc`/`ExpiredAtUtc` timestamps.
- Use reservation statuses `Active`, `Released`, and `Expired`.
- Treat active reservations whose `ExpiresAtUtc` is greater than the command/query time as reserved quantity.
- Calculate `availableQuantity = stockItem.OnHandQuantity - activeUnexpiredReservedQuantity`.
- Store stock item `available_quantity` as the latest calculated availability after reservation state changes for contract compatibility, while availability queries recalculate from reservation rows.
- Reject reservation creation with `400` for non-positive quantity or expiration that is not in the future.
- Reject reservation creation with `404` when the stock item does not exist.
- Reject reservation creation with `409` when requested quantity exceeds current availability.
- Prevent overselling with a database transaction and row-level lock on the target stock item for reservation create, release, and expiration commands when using PostgreSQL.
- Keep endpoint tests on the existing EF Core InMemory test harness for HTTP behavior, and add focused application/persistence tests around availability math and conflict cases where practical.
- Use UTC timestamps supplied by application handlers.
- Add explicit `POST /reservations/expire` to expire all due active reservations up to a supplied or server-current timestamp.
- Make `POST /reservations/{reservationId}/release` idempotent for already released reservations and return `409` for already expired reservations.

Deferred out of scope:

- Event publication and channel sync after reservation changes.
- Background expiration scheduling.
- Reservation ownership, checkout identity, and authorization.
- Distributed locks beyond the PostgreSQL transaction boundary.
- Cross-stock-item reservations or bundle availability.

Open questions:

- None.

## Deliverables

- Updated OpenAPI contract for reservation and availability endpoints.
- Reservation domain entity, EF Core mapping, constraints, indexes, and migration.
- MediatR commands/queries and handlers for reservation create/list/get/release/expire.
- Availability query and endpoint for stock items.
- Updated stock item responses so `availableQuantity` reflects active, unexpired reservations.
- Endpoint and application tests for success, validation, not-found, conflict, release, expiration, and availability behavior.
- Updated project knowledge and milestone report.

## Acceptance Criteria

- [ ] `POST /reservations` creates an active reservation when enough stock is available.
- [ ] `POST /reservations` returns `409` without creating a reservation when the request would oversell a stock item.
- [ ] `GET /reservations` lists reservations and supports optional `stockItemId` and `status` filters.
- [ ] `GET /reservations/{reservationId}` returns a reservation or `404`.
- [ ] `POST /reservations/{reservationId}/release` releases active reservations and restores availability.
- [ ] `POST /reservations/expire` expires due active reservations and restores availability.
- [ ] `GET /stock-items/{stockItemId}/availability` returns on-hand, reserved, available, and next-expiration values.
- [ ] Stock item get/list responses return reservation-aware `availableQuantity`.
- [ ] Reservation commands use MediatR and keep EF Core access inside handlers.
- [ ] PostgreSQL persistence includes constraints/indexes that support reservation lookup and conflict handling.
- [ ] Tests cover validation, missing stock items/reservations, oversell conflicts, release, expiration, and availability calculations.
- [ ] Full build, test, review, docs, and GitHub status checks pass.

## Task Breakdown

| Order | Priority | Task | Objective | Depends on | Verification | GitHub issue |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | Critical | Extend reservation and availability contract | Add OpenAPI paths/schemas/errors for reservation and availability behavior before implementation. | M2 contract | OpenAPI parse/review and contract diff | #34 |
| 2 | Critical | Add reservation persistence foundation | Add reservation entity, EF Core mapping, constraints, migration, DTOs, and availability calculation helpers. | Task 1 | `dotnet build`, migration review, focused availability tests | #39 |
| 3 | High | Implement reservation create and read endpoints | Add create/list/get commands, queries, handlers, endpoints, and tests. | Tasks 1-2 | Endpoint tests for create/list/get/validation/not-found/oversell | #40 |
| 4 | High | Implement reservation release | Add release command/endpoint and update availability after release. | Tasks 2-3 | Endpoint tests for active release, idempotent released reservation, expired conflict | #38 |
| 5 | High | Implement reservation expiration | Add explicit expiration command/endpoint and expiration-aware availability behavior. | Tasks 2-4 | Endpoint tests for due/not-due expiration and restored availability | #37 |
| 6 | High | Implement availability reads and stock item availability updates | Add stock item availability endpoint and make stock item get/list return reservation-aware availability. | Tasks 2-5 | Endpoint tests for availability before/after create/release/expire | #36 |
| 7 | Medium | Complete M3 validation and docs | Run planned-state validation, update knowledge/docs, and confirm GitHub status. | Tasks 1-6 | Full `dotnet test`, contract review, status update | #35 |

## Risks and Mitigations

- Oversell prevention depends on concurrency behavior that EF Core InMemory cannot prove. Mitigate by isolating transaction/lock behavior in handlers and documenting PostgreSQL verification expectations; add PostgreSQL-backed tests when Docker is available.
- Expiration can be interpreted as automatic background behavior. Mitigate by making M3's explicit expiration endpoint and background-worker deferral clear in the API plan and docs.
- Stored `available_quantity` can drift from reservation rows if updates are missed. Mitigate by recalculating availability in query handlers and updating the stored value in reservation state-changing commands.
- Contract drift is likely because M3 adds new resources. Mitigate by updating OpenAPI first and reviewing endpoint response shapes against the contract in every implementation slice.

## Testing Strategy

- Unit: availability calculation helper behavior, validation edge cases, and status transitions.
- Integration/endpoint: reservation create/list/get/release/expire and stock item availability responses through `WebApplicationFactory`.
- Contract: review OpenAPI paths, schemas, operation IDs, response codes, and examples against `docs/openapi-style-guide.md`.
- Concurrency/failure: oversell conflict tests at the application boundary; PostgreSQL row-lock verification when local Docker or CI infrastructure supports it.

## Priority Decisions

- Critical prerequisite: update the contract first so public behavior is decided before code.
- Critical foundation: persistence and availability calculation shape before endpoint slices.
- High value vertical slices: reservation create/read first, then release, expiration, and availability reads.
- Medium cleanup: final milestone validation and docs after behavior is complete.

## Progress

- Planning started on branch `codex/m3-reservations-planning`.
- Issue #34 contract slice implemented on branch `codex/34-reservation-availability-contract`: `contracts/openapi/inventory-api.yaml` now defines reservation create/list/get/release/expire operations, stock item availability reads, M3 reservation/availability schemas, examples, and validation/not-found/conflict responses.
- Issue #39 persistence foundation implemented on branch `codex/39-reservation-persistence-foundation`: reservation domain/status model, DTO/mapping, availability calculation helper, EF Core mapping, PostgreSQL migration, and focused availability/status-transition tests are in place.
- Issue #40 create/read slice implemented on branch `codex/40-reservation-create-read`: reservation create/list/get MediatR requests, handlers, endpoints, validation, not-found, oversell conflict behavior, and endpoint tests are in place.
- Issue #38 release slice implemented on branch `codex/38-reservation-release`: reservation release command/endpoint, idempotent already-released behavior, expired conflict handling, missing reservation handling, and availability reuse after release are covered by endpoint tests.

## GitHub Tracking

- Milestone: `M3 - Reservations and Availability` (GitHub milestone number 4)
- Issues: #34, #39, #40, #38, #37, #36, #35.
- Project board: keep each active task isolated to one branch and pull request unless explicitly combined.

## Previous Milestone Closure

- Report: `docs/milestone-2-report.md` exists and records M2 implementation, validation, verification, known gaps, and M3 handoff.
- Knowledge/docs: `docs/knowledge/overview.md`, `docs/plans/m2-core-inventory-domain.md`, README, and roadmap/project docs capture M2's durable implementation state and M3 handoff.
- GitHub status: M2 issues are closed and the M3 milestone issues are created from this plan.
- Result: Passed. No duplicate M2 report was needed.

## Start Gate Result

- [x] Previous milestone report exists or was not required.
- [x] Previous milestone durable facts are captured in docs/knowledge.
- [x] Plan is complete enough to implement.
- [x] No implementation-blocking decisions remain undecided.
- [x] GitHub tracking matches this plan.
- [x] Knowledge docs capture durable planned behavior.

Status: Ready to start
