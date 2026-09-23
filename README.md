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

`M9 - Agentic OS Expansion`

M9 is ready to start from `docs/plans/m9-agentic-os-expansion.md`. The milestone expands the repo-local Agentic Control Dashboard into a local-first human-plus-AI operating layer for memory, repeatable automation, research context, and resource-aware agent workflows, while keeping secrets, raw prompts, raw transcripts, generated dashboard output, and unavailable usage guesses out of Git.

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

## Compose Run

```bash
docker compose up --build
```

Compose exposes the backend at `http://localhost:8080` and the frontend at `http://localhost:5173`. Set `ASSETFLOW_WEB_PORT=6173` before `docker compose up --build` if `5173` is already in use locally.

To include the optional local monitoring stack:

```bash
docker compose --profile monitoring up --build
```

Prometheus is exposed at `http://localhost:9090` and Grafana at `http://localhost:3000` with the local credentials `assetflow` / `assetflow`. Set `ASSETFLOW_PROMETHEUS_PORT` or `ASSETFLOW_GRAFANA_PORT` before starting Compose if those ports are already in use.

## Documentation

- [Vision](docs/vision.md)
- [Roadmap](docs/roadmap.md)
- [Architecture](docs/architecture.md)
- [AI Development Workflow](docs/ai-development-workflow.md)
- [Deployment](docs/deployment.md)
- [Project Management](docs/project-management.md)
- [M0 Status](docs/milestone-0-status.md)
