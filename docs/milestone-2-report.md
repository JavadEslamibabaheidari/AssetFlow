# M2 Report - Core Inventory Domain

## Status

M2 is complete. The milestone turned the Inventory API from a minimal service into a production-shaped core inventory domain with PostgreSQL persistence, EF Core migrations, MediatR-based CQRS handlers, and contract-backed endpoints.

## Implemented

- Added a milestone start gate so future milestones cannot begin until plans, decisions, and GitHub tracking are complete.
- Added the M2 milestone plan in `docs/plans/m2-core-inventory-domain.md`.
- Added EF Core with the Npgsql PostgreSQL provider and an initial inventory schema migration.
- Added MediatR and organized the existing API project into `Domain`, `Application`, `Infrastructure`, and `Api` folders.
- Added domain entities for vendors, products, sales channels, and stock items.
- Implemented vendor endpoints: create, list, and get by id.
- Implemented product endpoints: create, list with optional vendor filter, and get by id.
- Implemented channel endpoints: create and list.
- Implemented stock item endpoints: create, list with optional product/channel filters, and get by id.
- Added contract-shaped responses and `ProblemDetails` mapping for validation, not-found, and conflict outcomes.
- Added endpoint tests using `WebApplicationFactory` and an EF Core InMemory test provider override.
- Added a classic `MarketplaceInventoryPlatform.sln` and updated CI to target it explicitly.

## Planned-State Validation

- The implemented endpoints match the M1 OpenAPI contract operations for vendors, products, channels, and stock items.
- M2 deliberately keeps update/delete operations, authentication, tenant isolation, reservations, availability holds, event publication, and marketplace synchronization out of scope.
- `availableQuantity` intentionally equals `onHandQuantity` until M3 introduces reservation-aware availability.
- API endpoint delegates dispatch MediatR commands and queries; EF Core access is inside application handlers.
- Duplicate vendor names, product SKUs per vendor, channel codes, and stock item product/channel pairs return `409`.
- Missing referenced vendors, products, channels, and stock items return `404`.
- Invalid names, missing identifiers, and negative stock quantities return `400`.

## Verification

- `dotnet build MarketplaceInventoryPlatform.sln --configuration Release`
- `dotnet test MarketplaceInventoryPlatform.sln --configuration Release --no-build`
- GitHub CI passed on each implementation PR before merge.
- Final local test count: 31 passing tests.

## Known Gaps

- Docker build could not be rerun locally during implementation because this environment has no Docker daemon socket.
- PostgreSQL container-backed integration tests are not yet present; M2 uses handler checks, EF model/migration review, and endpoint tests with an in-memory provider.
- MediatR logs a development/test license warning from its package at runtime; it does not fail build or tests.

## Next

M3 can start reservations and availability planning from the completed M2 domain, persistence model, and CQRS endpoint structure.
