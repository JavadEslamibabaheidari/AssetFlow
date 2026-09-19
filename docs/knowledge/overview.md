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

M2 must introduce EF Core through a CQRS application layer using MediatR. API endpoints should dispatch commands and queries; EF Core access belongs behind handlers, not inside route bodies. M3 reservation work should use the same MediatR/CQRS shape for concurrency-sensitive reservation and availability behavior. M4 event-driven synchronization should publish events from application handlers or an outbox-style boundary after successful state changes, not directly from controllers.

For M2, keep the implementation in the existing `Inventory.Api` project with clear `Domain`, `Application`, `Infrastructure`, and `Api` folders. Use EF Core with the Npgsql PostgreSQL provider, MediatR for commands and queries, Minimal API route groups for contract endpoints, and `ProblemDetails` for validation, not-found, and conflict responses. `availableQuantity` equals `onHandQuantity` until M3 adds reservation-aware availability.

M2 foundation work has started: `Inventory.Api` now references MediatR, EF Core Design, and `Npgsql.EntityFrameworkCore.PostgreSQL`; registers MediatR and inventory persistence at startup; defines domain entities for vendors, products, sales channels, and stock items; adds `InventoryDbContext`; and includes an initial PostgreSQL migration for the inventory schema. A classic `MarketplaceInventoryPlatform.sln` exists for CI compatibility alongside the existing `.slnx`.

Vendor endpoints are implemented as the first M2 vertical slice: `POST /vendors`, `GET /vendors`, and `GET /vendors/{vendorId}` dispatch MediatR requests, use EF Core through handlers, return contract-shaped `{ vendor: ... }` and `{ items: ... }` responses, and map validation, duplicate-name conflict, and not-found results to `ProblemDetails`.

Product endpoints are implemented as the second M2 vertical slice: `POST /products`, `GET /products`, and `GET /products/{productId}` dispatch MediatR requests, validate vendor references, enforce duplicate SKU conflicts per vendor, support optional `vendorId` list filtering, and map validation, vendor/product not-found, and conflict outcomes to `ProblemDetails`.

Channel endpoints are implemented as the third M2 vertical slice: `POST /channels` and `GET /channels` dispatch MediatR requests, enforce duplicate channel-code conflicts, return contract-shaped channel responses, and map validation and conflict outcomes to `ProblemDetails`.

Stock item endpoints are implemented as the fourth M2 vertical slice: `POST /stock-items`, `GET /stock-items`, and `GET /stock-items/{stockItemId}` dispatch MediatR requests, validate product and channel references, enforce one stock item per product/channel pair, support optional `productId` and `channelId` list filters, keep `availableQuantity` equal to `onHandQuantity`, and map validation, not-found, and conflict outcomes to `ProblemDetails`.

M3, Reservations and Availability, is the next milestone. It should build on the same MediatR/CQRS application shape and introduce reservation-aware availability, expiration, and concurrency behavior.

## Architecture Direction

The current implementation is a single minimal ASP.NET Core API. The target direction is an inventory platform with clear service ownership, OpenAPI-documented APIs, PostgreSQL-backed business state, and event-driven synchronization when the product need justifies it.

Kafka, gRPC, SignalR, event sourcing, projections, and channel workers are future options, not current commitments. Add them only when a milestone or issue creates a concrete need.

## Workflow Invariants

Repo knowledge must stay aligned with code. For milestones, big issues, and features, completion requires planned-state validation: compare the original plan and acceptance criteria with the actual code, tests, docs, and GitHub status before calling the work done.

When this knowledge conflicts with code, code wins and this knowledge must be corrected immediately.
