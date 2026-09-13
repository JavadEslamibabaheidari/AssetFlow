# Architecture

## Initial Architecture

The project starts with one minimal ASP.NET Core API service.

This keeps Milestone 0 focused on repository setup, delivery pipeline, Docker, and local Kubernetes.

## Target Architecture Direction

The product domain naturally fits an event-driven distributed system because stock and reservation changes must be propagated to multiple sales channels.

The target direction is:

- services own their data
- service APIs are documented with OpenAPI
- internal service communication may use gRPC where useful
- business events are published through Kafka
- read models and dashboards consume event streams where useful
- metrics, logs, and traces are available from the beginning

## Event-Driven vs Event Sourcing

Event-driven architecture fits the project early because many components need to react to changes such as stock updates, reservations, and channel synchronization.

Event sourcing may become useful later for auditability and rebuilding state from historical events, but it should not be the first architectural commitment. The first version should model business state clearly in PostgreSQL, publish domain events, and add event sourcing only if the learning or product value justifies the complexity.

## First Service

`Inventory.Api` is the first service. During M0, it only exposes:

- `GET /`
- `GET /health`

Later milestones will turn it into the core inventory API.

