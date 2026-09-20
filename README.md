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

`M5 - Frontend Application Foundation`

M4 is complete. The current milestone establishes the production frontend foundation before full inventory workflows:

- React, TypeScript, and Vite frontend workspace
- app shell with routing, navigation, loading, and error states
- design-system foundation
- OpenAPI-aligned frontend API client strategy
- frontend quality gates and smoke-test foundation
- Docker Compose frontend/backend verification path

## Local Run

```bash
dotnet run --project src/Inventory.Api
```

Then call:

```bash
curl http://localhost:5000/health
```

## Frontend Run

```bash
cd src/AssetFlow.Web
npm install
npm run dev
```

The frontend defaults to `http://localhost:5173` and reads the backend API base URL from `VITE_ASSETFLOW_API_BASE_URL`, falling back to `http://localhost:8080`.

## Documentation

- [Vision](docs/vision.md)
- [Roadmap](docs/roadmap.md)
- [Architecture](docs/architecture.md)
- [AI Development Workflow](docs/ai-development-workflow.md)
- [Deployment](docs/deployment.md)
- [Project Management](docs/project-management.md)
- [M0 Status](docs/milestone-0-status.md)
