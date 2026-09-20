# M5 Report - Frontend Application Foundation

## Status

M5 is complete pending the merge of the final validation/docs slice. The milestone established the first production frontend foundation for AssetFlow without implementing the full inventory and reservation workflows reserved for M6.

## Implemented

- Added the M5 frontend architecture plan in `docs/plans/m5-frontend-application-foundation.md`.
- Added the first-party frontend workspace at `src/AssetFlow.Web` using React, TypeScript, Vite, React Router, TanStack Query, npm, Vitest, Testing Library, Playwright, ESLint, and Prettier.
- Added a responsive app shell with routes for overview, inventory, reservations, operations, and settings.
- Added global loading and error views, environment-based API base URL configuration, and product-facing placeholder panels for future workflows.
- Added design-system tokens and starter primitives for page headers, metric cards, informational panels, status badges, and definition-list settings.
- Documented `lucide-react` as the icon strategy and captured starter accessibility conventions.
- Added OpenAPI-generated TypeScript contract types from `contracts/openapi/inventory-api.yaml`.
- Added a typed frontend API foundation with `ApiHttpClient`, Problem Details/error mapping, query-parameter support, and `InventoryApiClient` read methods.
- Added frontend quality gates: Prettier checks, ESLint, TypeScript checks, Vitest/Testing Library tests, and Playwright smoke tests.
- Extended GitHub Actions CI so frontend checks, build, Playwright smoke tests, Docker Compose build, and Compose smoke checks run before backend Docker publish steps.
- Added a frontend Dockerfile that builds the Vite app with Node 20 and serves the static output through Nginx on container port 8080.
- Extended `docker-compose.yml` with `assetflow-web`, defaulting the host frontend port to `5173` and allowing local override through `ASSETFLOW_WEB_PORT`.
- Documented local frontend and Compose run paths in the root README and `src/AssetFlow.Web/README.md`.
- Updated `docs/knowledge/overview.md` with durable M5 implementation facts and the M6 handoff.

## Planned-State Validation

- A new developer has documented local commands for backend, frontend, and Compose runs.
- The app shell renders as a real product surface with operational panels for assets, markets/channels, warehouses/stock, reservations, and settings instead of exposing milestone planning text in the UI.
- Routing, navigation, global loading, and global error states are present.
- Shared UI primitives and design tokens exist and are documented.
- Frontend API access is typed and traceable to `contracts/openapi/inventory-api.yaml`.
- Lint, format, type-check, unit/component test, and Playwright smoke-test commands exist and run in CI.
- Docker Compose can build and run the backend and frontend foundation together.
- No secrets or machine-specific credentials are committed.
- Knowledge docs describe the frontend foundation and preserve M6 boundaries.

## Verification

- GitHub CI passed for the merged M5 implementation PRs #67 through #72.
- `dotnet test MarketplaceInventoryPlatform.sln --configuration Release --no-build`
- `docker compose build`
- `ASSETFLOW_WEB_PORT=6173 docker compose up -d`
- `curl http://127.0.0.1:8080/health`
- `curl http://127.0.0.1:6173/health`
- `curl http://127.0.0.1:6173/ | rg "AssetFlow"`
- `ASSETFLOW_WEB_PORT=6173 docker compose config --quiet`
- `git diff --check`

Frontend package-script checks were verified by GitHub CI. The local shell used for the final validation did not have an `npm` executable on `PATH`; the bundled desktop runtime exposed Node but no npm shim.

## Known Gaps

- Full vendor, product, sales-channel, stock-item, reservation, and availability workflows are not implemented in M5; they are M6 scope.
- The frontend API client currently provides the typed foundation and read-oriented client methods. Mutation workflows will be added with M6 screens.
- The M5 UI is a foundation shell with realistic operational panels, not live inventory management.
- Project-board item and column mapping remains unverified from this environment; GitHub issue, milestone, PR, and CI status were verified through the available GitHub CLI/API path.
- The frontend Docker image uses `npm install` in its build stage because the frontend workspace currently has no committed lockfile.

## GitHub Status

- M5 milestone issues #60, #61, #62, #63, #64, and #65 are closed.
- Issue #66 closes with this final validation/docs slice.
- After #66 merges, the GitHub milestone `M5 - Frontend Application Foundation` can be closed.

## Next

M6 should start from the completed frontend foundation and build API-backed inventory parity: vendors, products, sales channels, stock items, reservations, reservation-aware availability, workflow state handling, and focused end-to-end coverage through Docker Compose.
