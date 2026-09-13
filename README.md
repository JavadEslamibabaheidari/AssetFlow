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
- future event-driven architecture with Kafka, observability, and dashboards

## Current Milestone

`M0 - Repository and Hello Service`

The first milestone proves the basic delivery path:

- a minimal ASP.NET Core service
- Docker support
- GitHub Actions CI
- local Kubernetes manifests
- documentation for running the service locally

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
