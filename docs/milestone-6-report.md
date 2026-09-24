# M6 Report - Frontend Inventory Parity

## Status

M6 corrective closure is complete after the 2026-09-23 reopen. The milestone turns the M5 frontend shell into API-backed inventory and reservation workflows aligned with the M2/M3 backend contract.

## Implemented

- Expanded `InventoryApiClient` to cover create, list, detail, release, expiration, and availability operations from `contracts/openapi/inventory-api.yaml`.
- Added reusable workflow-state components for loading, empty, success, validation, conflict, not-found, and generic API feedback.
- Replaced placeholder inventory content with API-backed vendor, product, sales-channel, stock-item, and availability workflows.
- Added stock item inspection with backend-calculated on-hand, reserved, available, and next-expiration values.
- Replaced placeholder reservation content with reservation create, list, detail, release, expiration, status filter, stock filter, and availability snapshot workflows.
- Updated the Markets page to show sales-channel master data while keeping channel synchronization explicitly deferred to M7.
- Added focused component coverage for the M6 inventory, stock availability, reservation create/conflict/release/expiration, disabled-state, and operations workflow pages.
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

- `npm run format:check`
- `npm run lint`
- `npm run typecheck`
- `npm run test` - 25 frontend unit/component tests.
- `npm run build`
- `npm run test:e2e -- --project=chromium --grep-invert @backend`
- `npm run test:e2e -- --project=chromium --grep @backend` against Docker Compose API/PostgreSQL.
- `dotnet test MarketplaceInventoryPlatform.sln --configuration Release`
- `git diff --check`
- `docker compose config --quiet`
- PR #116 GitHub checks passed: Backend CI, Frontend CI, and Integration CI.

Local verification used isolated Codex worktrees so concurrent M9/M10 sessions in `/mnt/data/AssetFlow` were not disturbed. Docker-backed browser workflow coverage was split so plain Frontend CI runs app-shell Playwright checks, while Integration CI runs backend-backed Playwright workflows against the Compose stack.

## GitHub Status

- Corrective M6 implementation PR #116 is merged to `main`.
- Issue #81 is closed by PR #116.
- Issues #79, #80, and #75 are satisfied by the final validation/testing/docs pass.
- Earlier reopened M6 issues #74, #77, and #78 were already closed after their corrective slices.
- Project-board item and column mapping remains unverified from this environment.

## Known Gaps

- Channel synchronization behavior and sync health are deferred to M7.
- Authentication, authorization, tenant isolation, checkout, payment, shipment, and customer identity workflows remain out of scope.
- The frontend has focused workflow/component coverage plus backend-backed Playwright coverage for the most important create/list/reservation/marketplace paths; broader browser automation can still expand after M6 if new flows are added.

## Next

M7 should add event-driven inventory/reservation publication, channel synchronization workers, and frontend/admin visibility for synchronization status, failures, and retry needs.
