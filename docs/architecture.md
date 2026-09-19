# Architecture

## Initial Architecture

The project starts with one minimal ASP.NET Core API service.

This keeps Milestone 0 focused on repository setup, delivery pipeline, Docker, and local Kubernetes.

## Target Architecture Direction

The product domain naturally fits an event-driven distributed system because stock and reservation changes must be propagated to multiple sales channels.

The target direction is:

- services own their data
- services are split by real bounded contexts and operational ownership rather than by premature technical layering
- dependency injection keeps application behavior explicit and testable inside each service
- service APIs are documented with OpenAPI
- API endpoints delegate application behavior to CQRS command/query handlers through MediatR
- EF Core is used behind application handlers for relational persistence when the core inventory domain starts
- internal service communication may use gRPC where useful
- business events are published through Kafka
- read models and dashboards consume event streams where useful
- metrics, logs, and traces are available from the beginning

AssetFlow should not permanently concentrate all business behavior in one service. The first repository and `Inventory.Api` service are a controlled starting point. As the product reaches clearer bounded contexts, new deployable services can be introduced in separate repositories or workspaces and deployed together through Kubernetes, with each service owning its API, data, dependency-injection composition root, tests, and runtime configuration.

Early service splits should be justified by ownership, scaling, data boundaries, integration needs, or operational independence. Candidate future services include reservation, channel integration, pricing, catalog/assets, notification/synchronization workers, and frontend/backend-for-frontend surfaces if the product needs them.

## Application Architecture Rule

When AssetFlow introduces core inventory persistence, use CQRS with MediatR as the application boundary.

- Commands represent state-changing use cases such as creating vendors, products, channels, stock items, and reservations.
- Queries represent read use cases such as listing or getting vendors, products, channels, stock items, and availability.
- HTTP endpoints should validate transport concerns and dispatch commands or queries through MediatR; they should not contain domain or persistence logic.
- EF Core `DbContext` usage belongs behind command/query handlers and supporting persistence abstractions, not in controllers or minimal API route bodies.
- Transaction boundaries for state-changing use cases should be explicit in command handlers.

Use this same shape for M3 reservation workflows so concurrency-sensitive behavior is isolated in command handlers and tested at the application boundary.

For M7 event-driven synchronization, publish domain/integration events from application handlers or a clear outbox-style boundary after successful state changes. Do not publish marketplace synchronization events directly from controllers.

## Frontend Architecture Direction

M5 introduces the frontend application foundation after the M4 dashboard MVP. The frontend should use a modern, strongly typed, component-driven architecture with clear boundaries between route/page composition, reusable UI components, API clients, and workflow-specific view models.

Frontend work must stay contract-aligned with backend services through OpenAPI-generated or OpenAPI-validated clients where practical. Backend services remain the source of truth for business rules; the frontend should focus on user workflows, accessibility, state presentation, and clear handling of backend validation, conflict, not-found, and success responses.

M6 brings the frontend to parity with the backend state reached by M3, covering vendors, products, sales channels, stock items, reservations, and reservation-aware availability. Browser end-to-end coverage should start small and high-value, then expand as product workflows stabilize.

## Event-Driven vs Event Sourcing

Event-driven architecture fits the project early because many components need to react to changes such as stock updates, reservations, and channel synchronization.

Event sourcing may become useful later for auditability and rebuilding state from historical events, but it should not be the first architectural commitment. The first version should model business state clearly in PostgreSQL, publish domain events, and add event sourcing only if the learning or product value justifies the complexity.

## First Service

`Inventory.Api` is the first service. During M0, it only exposes:

- `GET /`
- `GET /health`

Later milestones will turn it into the core inventory API.
