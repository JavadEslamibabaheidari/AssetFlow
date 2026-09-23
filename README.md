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

`M7 - Event-Driven Synchronization`

M7 is implemented, tagged, released, and closed. The platform now publishes inventory and reservation changes through a PostgreSQL-backed outbox, processes availability changes through an idempotent channel synchronization worker, and exposes backend-backed synchronization health in the frontend Operations surface:

- durable integration event envelopes and `outbox_messages` persistence
- stock item, reservation lifecycle, availability, and channel sync event contracts
- channel synchronization processing with persisted success, failure, retry, and attempt state
- `GET /channel-sync/status` plus generated frontend client support
- Operations page visibility for no-sync, pending/in-progress, succeeded, failed, and retryable states
- backend, frontend, and integration CI coverage for contract and workflow changes

The next milestone is M8, Observability and Monitoring, which should add deeper logs, metrics, traces, dashboards, and operational alerting around the synchronized product surface.

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

## Documentation

- [Vision](docs/vision.md)
- [Roadmap](docs/roadmap.md)
- [Architecture](docs/architecture.md)
- [AI Development Workflow](docs/ai-development-workflow.md)
- [Deployment](docs/deployment.md)
- [Project Management](docs/project-management.md)
- [M0 Status](docs/milestone-0-status.md)
