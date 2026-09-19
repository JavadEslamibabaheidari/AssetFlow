# M3 Report - Reservations and Availability

## Status

M3 is complete. The milestone added reservation-aware availability to the Inventory API, including contract-first reservation endpoints, PostgreSQL-backed reservation persistence, MediatR command/query handlers, explicit release and expiration workflows, availability reads, and tests for validation and conflict behavior.

## Implemented

- Extended `contracts/openapi/inventory-api.yaml` with reservation create/list/get/release/expire operations and stock item availability reads.
- Added a `Reservation` domain entity and `ReservationStatus` lifecycle with `Active`, `Released`, and `Expired` states.
- Added reservation DTO/mapping types, availability calculation DTOs, and a shared stock item availability calculator.
- Added EF Core reservation mapping and a PostgreSQL migration for the `reservations` table.
- Added constraints for positive reservation quantity, valid status values, release/expiration timestamp consistency, and expiration after creation.
- Added indexes for stock item lookup, stock item/status/expiration lookup, and due-expiration scanning.
- Implemented `POST /reservations`, `GET /reservations`, and `GET /reservations/{reservationId}` through MediatR handlers.
- Implemented oversell prevention for reservation creation by calculating active, unexpired reservation quantity before insert.
- Implemented `POST /reservations/{reservationId}/release`, including idempotent already-released behavior and `409` for expired reservations.
- Implemented `POST /reservations/expire`, including optional cutoff time, server-current fallback, due active expiration, and response counts.
- Implemented `GET /stock-items/{stockItemId}/availability`.
- Updated stock item get/list responses to return reservation-aware `availableQuantity`.
- Recalculated stored `stock_items.available_quantity` after reservation create, release, and expiration.
- Added Npgsql-only stock item row locks using `FOR UPDATE` around reservation state changes; the lock helper no-ops for the EF Core InMemory test provider.
- Updated `docs/knowledge/overview.md` and the M3 plan with final implementation behavior.

## Planned-State Validation

- `POST /reservations` creates active reservations only when enough stock is available.
- Oversell attempts return `409` and do not create another reservation.
- `GET /reservations` supports optional `stockItemId` and `status` filters.
- `GET /reservations/{reservationId}` returns a reservation or `404`.
- `POST /reservations/{reservationId}/release` releases active reservations, is idempotent for already released reservations, returns `404` for missing reservations, and returns `409` for expired reservations.
- `POST /reservations/expire` expires due active reservations, skips not-due/released/already-expired reservations, supports omitted request bodies, and reports `expiredCount`.
- `GET /stock-items/{stockItemId}/availability` returns on-hand, reserved, available, and next-expiration values.
- Stock item get/list responses return reservation-aware `availableQuantity`.
- Reservation endpoints dispatch MediatR requests; EF Core access remains inside handlers.
- PostgreSQL persistence includes constraints and indexes for reservation lookup, status filtering, due expiration, and data consistency.
- Npgsql execution uses a transaction plus stock item row-level `FOR UPDATE` locks around reservation create/release/expire state changes.

## Verification

- OpenAPI YAML parse and internal `$ref` check passed for `contracts/openapi/inventory-api.yaml`.
- `dotnet build MarketplaceInventoryPlatform.sln --configuration Release`
- `dotnet test MarketplaceInventoryPlatform.sln --configuration Release --no-build`
- Final local test count: 59 passing tests.
- GitHub CI passed for each implementation PR before merge.

## Known Gaps

- Docker build and PostgreSQL container-backed concurrency tests were not rerun locally because this environment does not provide a Docker daemon socket.
- Row-level lock behavior is implemented for Npgsql, but verified by code review and build rather than a live PostgreSQL concurrency integration test.
- Automatic background expiration remains out of scope; M3 uses the explicit `POST /reservations/expire` API.
- Authentication, authorization, tenant isolation, reservation ownership, event publication, marketplace channel sync, and outbox behavior remain deferred.

## GitHub Status

- M3 milestone issues #34, #39, #40, #38, #37, and #36 are closed.
- Issue #35 closes with this final validation/docs slice.
- After #35 merges, the GitHub milestone `M3 - Reservations and Availability` can be closed.

## Next

M4 can start from the completed reservation-aware inventory API and should focus on event-driven synchronization: event contracts, publication boundaries, outbox/channel synchronization decisions, and marketplace availability update flow.
