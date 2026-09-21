# M6 Report - Frontend Inventory Parity

## Status

M6 is complete, merged, tagged, released, and closed. The milestone turns the M5 frontend shell into API-backed inventory and reservation workflows aligned with the M2/M3 backend contract.

## Implemented

- Expanded `InventoryApiClient` to cover create, list, detail, release, expiration, and availability operations from `contracts/openapi/inventory-api.yaml`.
- Added reusable workflow-state components for loading, empty, success, validation, conflict, not-found, and generic API feedback.
- Replaced placeholder inventory content with API-backed vendor, product, sales-channel, stock-item, and availability workflows.
- Added stock item inspection with backend-calculated on-hand, reserved, available, and next-expiration values.
- Replaced placeholder reservation content with reservation create, list, detail, release, expiration, status filter, stock filter, and availability snapshot workflows.
- Updated the Markets page to show sales-channel master data while keeping channel synchronization explicitly deferred to M7.
- Added focused component coverage for the M6 inventory and reservation workflow pages.
- Split GitHub Actions into Backend CI, Frontend CI, and Integration CI with OpenAPI changes covered by frontend plus backend/integration paths.
- Updated frontend docs, root README, milestone plan, and repo knowledge for the M6 state.

## Planned-State Validation

- The frontend can create/list/inspect vendors, products, sales channels, and stock items through the typed API client.
- Product and stock item forms depend on vendor, product, and channel selections from backend data.
- Stock item availability reads use `getStockItemAvailability` and present on-hand, reserved, available, and next-expiration values together.
- Reservation workflows can create, list, inspect, release, and expire reservations through backend APIs.
- Oversell and other `409 Conflict` responses map through shared API feedback as inventory business feedback.
- Empty, loading, validation, not-found, conflict, disabled, and success states use shared UI conventions.
- CI is split without dropping OpenAPI coverage: contract changes run frontend checks and backend/integration checks.
- Event-driven channel synchronization UI remains deferred to M7.

## Verification

- `PATH=/home/javad/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/bin:$PATH ./node_modules/.bin/prettier --check .`
- `PATH=/home/javad/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/bin:$PATH ./node_modules/.bin/tsc -p tsconfig.json --pretty false`
- `PATH=/home/javad/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/bin:$PATH ./node_modules/.bin/eslint .`
- `PATH=/home/javad/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/bin:$PATH ./node_modules/.bin/vitest run`
- `PATH=/home/javad/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/bin:$PATH ./node_modules/.bin/vite build`
- `PATH=/home/javad/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/bin:$PATH ./node_modules/.bin/playwright test --project=chromium`
- `dotnet test MarketplaceInventoryPlatform.sln --configuration Release`
- `git diff --check`
- `docker compose config --quiet`

The local shell still does not expose `node` or `npm` on `PATH`; frontend verification used the bundled Codex Node runtime and locally installed workspace dependencies. Local Docker Compose build/run verification could not complete because this host has no Docker daemon socket at `/var/run/docker.sock`; the new Integration CI workflow owns Compose build/run verification for the PR.

## GitHub Status

- M6 tracking issues: #74 through #81 are closed.
- M6 PRs #82, #83, and #84 are merged.
- Tag and release `v0.6.0 - M6 Frontend Inventory Parity` exist.
- Project-board item and column mapping remains unverified from this environment.

## Known Gaps

- Channel synchronization behavior and sync health are deferred to M7.
- Authentication, authorization, tenant isolation, checkout, payment, shipment, and customer identity workflows remain out of scope.
- The frontend has focused workflow/component coverage and app-shell Playwright smoke coverage; broader browser automation around seeded backend data can be expanded after M6 if needed.

## Next

M7 should add event-driven inventory/reservation publication, channel synchronization workers, and frontend/admin visibility for synchronization status, failures, and retry needs.
