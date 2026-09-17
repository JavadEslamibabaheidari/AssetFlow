# Architecture

## Initial Architecture

The project starts with one minimal ASP.NET Core API service.

This keeps Milestone 0 focused on repository setup, delivery pipeline, Docker, and local Kubernetes.

## Target Architecture Direction

The product domain naturally fits an event-driven distributed system because stock and reservation changes must be propagated to multiple sales channels.

The target direction is:

- services own their data
- service APIs are documented with OpenAPI
- API endpoints delegate application behavior to CQRS command/query handlers through MediatR
- EF Core is used behind application handlers for relational persistence when the core inventory domain starts
- internal service communication may use gRPC where useful
- business events are published through Kafka
- read models and dashboards consume event streams where useful
- metrics, logs, and traces are available from the beginning

## Application Architecture Rule

When AssetFlow introduces core inventory persistence, use CQRS with MediatR as the application boundary.

- Commands represent state-changing use cases such as creating vendors, products, channels, stock items, and reservations.
- Queries represent read use cases such as listing or getting vendors, products, channels, stock items, and availability.
- HTTP endpoints should validate transport concerns and dispatch commands or queries through MediatR; they should not contain domain or persistence logic.
- EF Core `DbContext` usage belongs behind command/query handlers and supporting persistence abstractions, not in controllers or minimal API route bodies.
- Transaction boundaries for state-changing use cases should be explicit in command handlers.

Use this same shape for M3 reservation workflows so concurrency-sensitive behavior is isolated in command handlers and tested at the application boundary.

For M4 event-driven synchronization, publish domain/integration events from application handlers or a clear outbox-style boundary after successful state changes. Do not publish marketplace synchronization events directly from controllers.

## Event-Driven vs Event Sourcing

Event-driven architecture fits the project early because many components need to react to changes such as stock updates, reservations, and channel synchronization.

Event sourcing may become useful later for auditability and rebuilding state from historical events, but it should not be the first architectural commitment. The first version should model business state clearly in PostgreSQL, publish domain events, and add event sourcing only if the learning or product value justifies the complexity.

## First Service

`Inventory.Api` is the first service. During M0, it only exposes:

- `GET /`
- `GET /health`

Later milestones will turn it into the core inventory API.
