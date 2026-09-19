# Vision

## Product Vision

Marketplace Inventory Platform helps vendors manage inventory and reservations across multiple sales channels from one source of truth.

Example scenario:

1. A vendor has batteries available in central stock.
2. The batteries are listed on Amazon, MediaWorld, and Unieuro.
3. A customer buys or reserves one battery on Amazon.
4. The platform reduces available stock and publishes the change to all connected channels.
5. Other channels stop overselling because their availability is updated.

## Engineering Vision

The project is also a learning lab for modern AI-assisted software development.

The project should demonstrate:

- .NET backend development
- contract-first API design
- OpenAPI and JSON Schema usage
- event-driven design
- distributed system documentation
- Docker and local Kubernetes
- CI/CD with GitHub Actions
- observability through logs, metrics, traces, Prometheus, and Grafana
- an early Agentic Control Dashboard for project status, workflow launch points, agents, skills, and planning visibility
- a modern frontend application that exposes backend capabilities through clear, accessible, efficient product workflows
- workflow usage monitoring for quality, efficiency, and cost

## North Star Architecture

The long-term system may include:

- Inventory service
- Reservation service
- Channel integration service
- Pricing service
- Asset/catalog service
- Notification/synchronization workers
- Frontend web application
- PostgreSQL for relational data
- Blob storage for product/media assets
- Kafka for events
- gRPC for service-to-service calls
- REST APIs documented with OpenAPI
- Prometheus and Grafana for operational visibility
- Agentic Control Dashboard for human-plus-AI workflow visibility and control

The first implementation will stay small and build toward this vision step by step.
