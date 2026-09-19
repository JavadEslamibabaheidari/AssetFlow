# Marketplace Inventory Platform

A portfolio project for learning and demonstrating spec-first, AI-assisted backend development with .NET.

The long-term product goal is a platform that helps vendors manage product assets, stock, reservations, and availability across multiple sales channels such as Amazon, MediaWorld, and Unieuro. When a customer buys or temporarily reserves an item in one channel, the available quantity should be reduced and synchronized across the other channels.

The engineering goal is to build the platform through a disciplined AI development workflow:

- spec-first design with OpenAPI and clear acceptance criteria
- GitHub Issues and Projects for planning and execution
- small vertical slices implemented and reviewed with AI assistance
- Docker-based local development
- GitHub Actions CI/CD
- local Kubernetes deployment for learning
- future event-driven architecture with Kafka, observability, and an early Agentic Control Dashboard

## Current Milestone

`M4 - Event-Driven Synchronization`

M3 is complete. The next milestone will add event-driven synchronization on top of the reservation-aware inventory domain:

- event publication after inventory and reservation changes
- event contracts for stock and reservation changes
- outbox/channel synchronization decisions
- marketplace availability update flow

## Local Run

```bash
dotnet run --project src/Inventory.Api
```

Then call:

```bash
curl http://localhost:5000/health
```

## Documentation

- [Vision](docs/vision.md)
- [Roadmap](docs/roadmap.md)
- [Architecture](docs/architecture.md)
- [AI Development Workflow](docs/ai-development-workflow.md)
- [Deployment](docs/deployment.md)
- [Project Management](docs/project-management.md)
- [M0 Status](docs/milestone-0-status.md)
