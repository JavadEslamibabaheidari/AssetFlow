# AssetFlow Knowledge Overview

This file is the starting point for AssetFlow repo knowledge. Treat it as a maintained engineering reference: read it before broad code inspection, verify important facts against code and GitHub, and update it whenever meaningful work changes reality.

## Current Shape

AssetFlow is a marketplace inventory platform in early foundation work. The repository currently contains one ASP.NET Core service, `Inventory.Api`, plus tests, Docker, local Kubernetes manifests, GitHub Actions, and planning documentation.

The first service is intentionally minimal. It exposes:

- `GET /`: returns service info for `Inventory API`
- `GET /health`: returns a healthy status, service name, and `checkedAtUtc`

The test project `Inventory.Api.Tests` verifies both endpoints through `WebApplicationFactory<Program>`.

## Milestone State

M0, Repository and Hello Service, is complete: repository skeleton, minimal API, Docker support, local Kubernetes manifests, CI, issue templates, planning docs, GitHub work tracking, and local Kubernetes verification with `kind` exist.

M1, Spec-First Workflow, is complete: GitHub status workflow, repo knowledge enforcement, OpenAPI style guidance, AI feature workflow guidance, the first inventory OpenAPI contract, and the M1 report exist.

M2, Core Inventory Domain, is complete. It started from the reviewed inventory contract and spec-first workflow, and its milestone plan is `docs/plans/m2-core-inventory-domain.md`.

M2 must introduce EF Core through a CQRS application layer using MediatR. API endpoints should dispatch commands and queries; EF Core access belongs behind handlers, not inside route bodies. M3 reservation work should use the same MediatR/CQRS shape for concurrency-sensitive reservation and availability behavior. M7 event-driven synchronization should publish events from application handlers or an outbox-style boundary after successful state changes, not directly from controllers.

For M2, keep the implementation in the existing `Inventory.Api` project with clear `Domain`, `Application`, `Infrastructure`, and `Api` folders. Use EF Core with the Npgsql PostgreSQL provider, MediatR for commands and queries, Minimal API route groups for contract endpoints, and `ProblemDetails` for validation, not-found, and conflict responses. `availableQuantity` equals `onHandQuantity` until M3 adds reservation-aware availability.

M2 foundation work has started: `Inventory.Api` now references MediatR, EF Core Design, and `Npgsql.EntityFrameworkCore.PostgreSQL`; registers MediatR and inventory persistence at startup; defines domain entities for vendors, products, sales channels, and stock items; adds `InventoryDbContext`; and includes an initial PostgreSQL migration for the inventory schema. A classic `MarketplaceInventoryPlatform.sln` exists for CI compatibility alongside the existing `.slnx`.

Vendor endpoints are implemented as the first M2 vertical slice: `POST /vendors`, `GET /vendors`, and `GET /vendors/{vendorId}` dispatch MediatR requests, use EF Core through handlers, return contract-shaped `{ vendor: ... }` and `{ items: ... }` responses, and map validation, duplicate-name conflict, and not-found results to `ProblemDetails`.

Product endpoints are implemented as the second M2 vertical slice: `POST /products`, `GET /products`, and `GET /products/{productId}` dispatch MediatR requests, validate vendor references, enforce duplicate SKU conflicts per vendor, support optional `vendorId` list filtering, and map validation, vendor/product not-found, and conflict outcomes to `ProblemDetails`.

Channel endpoints are implemented as the third M2 vertical slice: `POST /channels` and `GET /channels` dispatch MediatR requests, enforce duplicate channel-code conflicts, return contract-shaped channel responses, and map validation and conflict outcomes to `ProblemDetails`.

Stock item endpoints are implemented as the fourth M2 vertical slice: `POST /stock-items`, `GET /stock-items`, and `GET /stock-items/{stockItemId}` dispatch MediatR requests, validate product and channel references, enforce one stock item per product/channel pair, support optional `productId` and `channelId` list filters, keep `availableQuantity` equal to `onHandQuantity`, and map validation, not-found, and conflict outcomes to `ProblemDetails`.

M3, Reservations and Availability, is complete. Its milestone plan is `docs/plans/m3-reservations-and-availability.md`, and its final report is `docs/milestone-3-report.md`. M3 builds on the M2 MediatR/CQRS application shape and adds reservation-aware availability, release, explicit expiration, and oversell conflict behavior.

M3 behavior: reservations belong to a stock item, have a positive quantity, and move through `Active`, `Released`, and `Expired` states. Active, unexpired reservations reduce availability; released and expired reservations do not. Availability is calculated as `stockItem.OnHandQuantity - activeUnexpiredReservedQuantity`, and reservation creation is rejected with `409 Conflict` when it would oversell. M3 uses explicit API commands for release and expiration; automatic background expiration and event publication are deferred.

The M3 contract in `contracts/openapi/inventory-api.yaml` defines reservation create/list/get/release/expire operations, a stock item availability read operation, reservation and availability schemas, and validation/not-found/conflict response expectations.

The M3 persistence foundation includes a `Reservation` domain entity and `ReservationStatus` enum, application DTO/mapping types, a `StockItemAvailabilityCalculator`, EF Core reservation mapping, and a PostgreSQL migration for the `reservations` table. The table stores stock item reference, positive quantity, string status, expiration/created/updated timestamps, optional release/expiration timestamps, a restricted stock item foreign key, check constraints for status/timestamp consistency, and indexes for stock item availability lookups plus due-expiration scans.

Reservation create/read endpoints are implemented for the M3 create/read slice: `POST /reservations` validates stock item id, positive quantity, and future expiration, returns `404` for missing stock items, calculates active unexpired reserved quantity from reservation rows, and returns `409` when a create request would exceed current availability. `GET /reservations` supports optional `stockItemId` and `status` filters, and `GET /reservations/{reservationId}` returns a reservation or `404`. Endpoints dispatch MediatR requests; EF Core access remains inside handlers.

Reservation release is implemented: `POST /reservations/{reservationId}/release` dispatches a MediatR command, releases active reservations, returns already released reservations idempotently, returns `404` for missing reservations, and returns `409` for expired reservations. Released reservations no longer count against availability because create availability checks only active, unexpired reservation rows.

Reservation expiration is implemented through `POST /reservations/expire`. The endpoint dispatches a MediatR command that expires active reservations whose `ExpiresAtUtc` is at or before an optional cutoff, or server current time when omitted. Not-due, released, and already expired reservations are skipped; the response reports `expiredCount` and the expired reservation items. Expired reservations no longer count against availability because create availability checks only active, unexpired reservation rows.

Stock item availability reads are implemented through `GET /stock-items/{stockItemId}/availability`, which returns on-hand, reserved, available, and next-expiration values calculated from active, unexpired reservations. `GET /stock-items/{stockItemId}` and `GET /stock-items` return reservation-aware `availableQuantity` values. Reservation create, release, and expiration handlers recalculate the stored `stock_items.available_quantity` compatibility field after state changes.

When the EF provider is Npgsql, reservation create/release/expire handlers run inside a transaction and lock affected stock item rows with `FOR UPDATE` before mutating reservation state and recalculating stored availability. The lock helper intentionally no-ops for the EF Core InMemory test provider.

After M3, the roadmap pulls an early control surface forward as M4, Agentic Control Dashboard MVP. M4 should give the user a repo-local dashboard for current milestone state, GitHub/local tracking links, reachable agents and skills, common workflow launch points, and lightweight AI-assisted activity or usage signals. It is deliberately narrower than both the later frontend application foundation and the later Agentic OS expansion: production frontend architecture and inventory screens are deferred to M5/M6, while memory-system selection, external workspace automation, research notebooks, and deep per-skill/per-agent resource accounting are deferred to M9.

M4 started from `docs/plans/m4-agentic-control-dashboard-mvp.md` and stayed under local tooling rather than defining the M5 frontend app shell. The dashboard reads from safe local sources (`git`, docs, `.agents/`, `.codex/skills/`) and GitHub CLI/API status when authenticated, shows degraded status when GitHub or project-board access is unavailable, and separates read-only status from mutating workflow launch points. The dashboard MVP spec and data contract live in `docs/specs/m4-agentic-control-dashboard-mvp.md`. M4 GitHub tracking is issues #49 through #53 under the `M4 - Agentic Control Dashboard MVP` milestone.

M4 is complete. Its final report is `docs/milestone-4-report.md`. The initial M4 dashboard foundation lives in `tools/agent-dashboard/`. Run `./tools/agent-dashboard/generate.sh` from the repo root to generate `tools/agent-dashboard/dist/index.html`, a local static dashboard with overview, tracking, docs, agents, skills, workflow launch point, and activity/gap sections. The generator reads local Git metadata, curated repo links, and read-only GitHub milestone/issues/recent PR status through `gh` when authenticated; set `ASSETFLOW_DASHBOARD_DISABLE_GITHUB=1` to verify degraded mode. Project-board state remains explicitly unverified/not mapped. Workflow launch points are static prompts, links, and command text; the dashboard does not run mutating operations. The generated `dist/` output is ignored by Git.

M5, Frontend Application Foundation, is complete. Its plan is `docs/plans/m5-frontend-application-foundation.md`, its final report is `docs/milestone-5-report.md`, and its GitHub tracking is issues #60 through #66. The frontend workspace is `src/AssetFlow.Web` using React, TypeScript, Vite, React Router, TanStack Query, npm, Vitest, Testing Library, Playwright, ESLint, and Prettier. The frontend remains in the same repository so backend contracts, CI, Docker Compose, and docs can evolve together.

M5 established the app shell, routing, responsive navigation, error/loading states, design tokens, starter UI primitives, OpenAPI-aligned typed API access from `contracts/openapi/inventory-api.yaml`, frontend quality gates, Docker Compose build/run path, and a small Playwright smoke-test foundation. Full inventory, reservation, and availability workflows remain M6. Runtime frontend configuration uses explicit environment variables, and no secrets or machine-specific credentials are committed.

M5 issue #61 started the frontend workspace at `src/AssetFlow.Web`. It contains the Vite/React/TypeScript project shape, local setup documentation, environment-based API base URL configuration, a responsive app shell, React Router routes for overview, inventory, reservations, operations, and settings, global error/loading states, and placeholder product areas that deliberately leave full API-backed workflows for M6.

M5 issue #62 added the starter frontend design-system foundation under `src/AssetFlow.Web/src/design-system/`. The foundation defines named CSS tokens for AssetFlow colors, spacing, type, radii, shadows, and focus states; documents `lucide-react` as the icon strategy; and provides reusable primitives for page headers, metric cards, informational panels, status badges, and definition-list settings.

M5 issue #63 added the frontend API-client foundation under `src/AssetFlow.Web/src/api/`. Contract types are generated from `contracts/openapi/inventory-api.yaml` with `npm run generate:api`, producing `src/api/generated/inventory-api.ts`. `ApiHttpClient` handles base URL configuration, JSON requests, query parameters, network errors, and Problem Details mapping; `InventoryApiClient` exposes typed read methods for vendors, products, channels, stock items, availability, and reservations.

M5 issue #64 added frontend quality gates for `src/AssetFlow.Web`: Prettier format checks, ESLint, TypeScript type checks, Vitest/Testing Library unit and component tests, and Playwright smoke tests. CI installs Node 20, runs the frontend checks, builds the frontend, installs Chromium for Playwright, and runs the app-shell smoke tests before Docker image build/publish steps.

M5 issue #65 added the frontend Docker Compose verification path. `src/AssetFlow.Web/Dockerfile` builds the Vite app with Node 20 and serves static output with Nginx on container port 8080. `docker-compose.yml` now runs `inventory-api` on host port 8080 and `assetflow-web` on host port 5173, with the frontend build configured to call the browser-facing API base URL `http://localhost:8080`.

M5 final validation confirmed the plan acceptance criteria against the implemented workspace, documentation, CI, and Docker Compose path. Local backend tests and Compose smoke checks passed; frontend package-script checks were verified by GitHub CI because the local validation shell exposed Node but no `npm` executable. Project-board item/column mapping remains unverified from this environment, while GitHub issues, PRs, CI, and milestone state were verified through the available GitHub CLI/API path.

M6, Frontend Inventory Parity, is complete. Its plan is `docs/plans/m6-frontend-inventory-parity.md`, its report is `docs/milestone-6-report.md`, and its GitHub milestone `M6 - Frontend Inventory Parity` is closed. The final M6 state is tagged and released as `v0.6.0 - M6 Frontend Inventory Parity`; issues #74 through #81 are closed, and PRs #82, #83, and #84 are merged.

M6 brings the frontend product surface to the backend state reached at the end of M3. It covers vendors, products, sales channels, stock items, reservations, and reservation-aware availability, including loading, empty, validation, conflict, not-found, disabled, success, responsive, and containerized frontend/backend verification states. The UI remains a production operational inventory tool: planning text, milestone status, and task guidance belong in docs and issues, not in the visible app.

M6 frontend workflow state: the M5 app shell remains, Assets, Reservations, and Markets are API-backed work surfaces, stock item detail shows on-hand/reserved/available state, and oversell prevention is presented as normal business feedback. Assets owns vendor, product, channel, stock item, and availability workflows. Reservations owns create, list, detail, release, expiration, filters, and availability snapshot workflows. Markets shows sales-channel master data and keeps synchronization status deferred to M7.

M6 splits CI into three focused workflows: Backend CI for .NET/API tests/backend Docker image, Frontend CI for `src/AssetFlow.Web/**` lint/type/test/build/Playwright/frontend Docker image, and Integration CI for Compose, workflows, OpenAPI contracts, cross-cutting root config/docs, and manual dispatch. OpenAPI contract changes run frontend checks as well as backend/integration checks so generated frontend types and backend behavior stay aligned.

M6 issue #74 established the frontend UX/API foundation for implementation slices: `InventoryApiClient` now covers M2/M3 create, list, detail, release, expiration, and availability operations with generated request/response types; `src/shared/workflowStates.tsx` provides reusable loading, empty, success, validation, conflict, not-found, and generic error feedback components; and `src/AssetFlow.Web/README.md` records route ownership for Assets, Reservations, and Markets.

The final M6 implementation replaces placeholder frontend panels with backend-backed product workflows. `InventoryPage` creates/lists vendors, products, sales channels, and stock items and inspects stock item availability. `ReservationsPage` creates reservations, lists and filters reservations, inspects reservation detail, releases active reservations, triggers expiration, and refreshes availability. `OperationsPage` lists channel master data while deferring synchronization to M7. Component tests in `src/pages/M6WorkflowPages.test.tsx` cover inventory creation/listing and reservation release wiring.

M7, Event-Driven Synchronization, is ready to start after the start-gate PR merges. Its plan is `docs/plans/m7-event-driven-synchronization.md`, and its GitHub tracking is issues #85 through #90 under the open `M7 - Event-Driven Synchronization` milestone. M7 must not begin implementation from roadmap bullets alone; implementation slices should start from the M7 plan and keep one issue per focused branch/PR unless the user explicitly combines them.

M7 planned behavior starts with a PostgreSQL-backed outbox instead of direct in-process publication, controller-level publication, or a mandatory Kafka runtime dependency. Application handlers or a clear application/infrastructure boundary append integration events only after successful business mutations. Failed validation, not-found, conflict, oversell, and failed retry paths must not create false downstream success events. The outbox/event contracts must be broker-ready so Kafka can be added later without rewriting domain behavior.

The first M7 event families are planned as `StockItemCreated`, `StockAvailabilityChanged`, `ReservationCreated`, `ReservationReleased`, `ReservationExpired`, `ChannelSyncRequested`, `ChannelSyncSucceeded`, and `ChannelSyncFailed`. Events need stable envelopes with explicit schema versions, identifiers, occurrence times, payload JSON, processing status, attempt metadata, and safe error details. Stock item creation produces both stock item and initial availability signals; reservation create/release/expire workflows produce reservation lifecycle events and availability signals for affected stock items.

M7 channel synchronization is planned as an idempotent worker path using mockable marketplace adapter interfaces, persisted sync status, explicit retry state, and no live provider credentials. The Operations page should evolve from M6 channel master data into backend-backed sync visibility showing healthy, pending, failed, retryable, and stale states. Retry controls must exist only where backend retry behavior is implemented and covered by tests.

M7 verification should include event contract tests, application tests for successful and failed mutation paths, outbox persistence/migration coverage, worker idempotency and retry/failure tests, OpenAPI and generated frontend type checks for sync endpoints, Operations page state tests, and Docker Compose or CI integration that proves the event/sync path sufficiently for milestone closure.

M7 issue #86 adds the event/outbox foundation. `Inventory.Api` now has application-level integration event contracts under `Application/Events`, an EF-backed `IIntegrationEventOutbox`, and an `outbox_messages` table mapped through `InventoryDbContext`. The outbox envelope records event type, schema version, aggregate type/id, occurrence time, payload JSON, processing status, attempt metadata, safe error text, processed time, and creation time. Stock item creation now enqueues `StockItemCreated` and an initial `StockAvailabilityChanged` record in the same `SaveChangesAsync` unit as the created stock item. Failed stock item validation, missing references, duplicate conflicts, and other non-success paths must not append outbox records. `docs/specs/m7-event-contracts.md` documents the initial contract surface; reservation lifecycle producers are intentionally left for #87.

M7 issue #87 publishes reservation and availability events from M3 workflows. Successful reservation creation enqueues `ReservationCreated` plus `StockAvailabilityChanged`; successful active reservation release enqueues `ReservationReleased` plus `StockAvailabilityChanged`; successful expiration enqueues one `ReservationExpired` per expired reservation and one `StockAvailabilityChanged` per affected stock item. Idempotent release of an already released reservation, validation failures, missing stock items or reservations, oversell conflicts, expired-release conflicts, and not-due expiration scans must not create new outbox records. `StockItemAvailabilityStore.RecalculateAsync` now returns the recalculated availability snapshot so handlers can publish downstream availability signals without duplicating calculation logic.

## Architecture Direction

The current implementation is a single minimal ASP.NET Core API. The target direction is an inventory platform with clear service ownership, OpenAPI-documented APIs, PostgreSQL-backed business state, dependency-injected application boundaries inside each service, and event-driven synchronization when the product need justifies it.

AssetFlow should evolve toward multiple deployable services when bounded contexts and operational needs justify the split. Candidate future services include inventory, reservation, channel integration, pricing, catalog/assets, notification/synchronization workers, and frontend-facing surfaces. New services may live in separate repositories or workspaces while still belonging to the same overall app and Kubernetes deployment landscape.

Docker Compose is the local end-to-end verification path for meaningful feature work once the feature spans deployable services, frontend/backend integration, persistence, or infrastructure. As frontend and new services appear, keep Compose aligned with the deployable app shape so integrated behavior can be tested locally before completion.

Kafka, gRPC, SignalR, event sourcing, projections, and channel workers are future options, not current commitments. Add them only when a milestone or issue creates a concrete need.

## Workflow Invariants

Repo knowledge must stay aligned with code. For milestones, big issues, and features, completion requires planned-state validation: compare the original plan and acceptance criteria with the actual code, tests, docs, and GitHub status before calling the work done.

When this knowledge conflicts with code, code wins and this knowledge must be corrected immediately.
