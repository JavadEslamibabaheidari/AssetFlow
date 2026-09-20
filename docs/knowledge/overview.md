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

M5, Frontend Application Foundation, starts from `docs/plans/m5-frontend-application-foundation.md` and GitHub issues #60 through #66. Its planned frontend workspace is `src/AssetFlow.Web` using React, TypeScript, Vite, React Router, TanStack Query, npm, Vitest, Testing Library, Playwright, ESLint, and Prettier. The frontend remains in the same repository so backend contracts, CI, Docker Compose, and docs can evolve together.

M5 should establish the app shell, routing, responsive navigation, error/loading states, design tokens, starter UI primitives, OpenAPI-aligned typed API access from `contracts/openapi/inventory-api.yaml`, frontend quality gates, Docker Compose build/run path, and a small Playwright smoke-test foundation. Full inventory, reservation, and availability workflows remain M6. Runtime frontend configuration should use explicit environment variables, and no secrets or machine-specific credentials should be committed.

M5 issue #61 started the frontend workspace at `src/AssetFlow.Web`. It contains the Vite/React/TypeScript project shape, local setup documentation, environment-based API base URL configuration, a responsive app shell, React Router routes for overview, inventory, reservations, operations, and settings, global error/loading states, and placeholder product areas that deliberately leave full API-backed workflows for M6.

M6, Frontend Inventory Parity, should bring the frontend product surface to the backend state reached at the end of M3. It should cover vendors, products, sales channels, stock items, reservations, and reservation-aware availability, including loading, empty, validation, conflict, not-found, success, responsive, and containerized frontend/backend verification states. Browser end-to-end tests should expand around these critical workflows in M6 rather than becoming a large suite before the frontend exists.

## Architecture Direction

The current implementation is a single minimal ASP.NET Core API. The target direction is an inventory platform with clear service ownership, OpenAPI-documented APIs, PostgreSQL-backed business state, dependency-injected application boundaries inside each service, and event-driven synchronization when the product need justifies it.

AssetFlow should evolve toward multiple deployable services when bounded contexts and operational needs justify the split. Candidate future services include inventory, reservation, channel integration, pricing, catalog/assets, notification/synchronization workers, and frontend-facing surfaces. New services may live in separate repositories or workspaces while still belonging to the same overall app and Kubernetes deployment landscape.

Docker Compose is the local end-to-end verification path for meaningful feature work once the feature spans deployable services, frontend/backend integration, persistence, or infrastructure. As frontend and new services appear, keep Compose aligned with the deployable app shape so integrated behavior can be tested locally before completion.

Kafka, gRPC, SignalR, event sourcing, projections, and channel workers are future options, not current commitments. Add them only when a milestone or issue creates a concrete need.

## Workflow Invariants

Repo knowledge must stay aligned with code. For milestones, big issues, and features, completion requires planned-state validation: compare the original plan and acceptance criteria with the actual code, tests, docs, and GitHub status before calling the work done.

When this knowledge conflicts with code, code wins and this knowledge must be corrected immediately.
