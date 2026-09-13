# Roadmap

## M0 - Repository and Hello Service

Goal: prove that the repository, service, Docker, CI, and local Kubernetes path work.

Deliverables:

- repository skeleton
- minimal ASP.NET Core API
- `/` service info endpoint
- `/health` endpoint
- Dockerfile
- Docker Compose file
- local Kubernetes manifests
- GitHub Actions CI workflow
- initial planning docs

## M1 - Spec-First Workflow

Goal: define the way features are planned before implementation.

Deliverables:

- OpenAPI style guide
- feature spec template
- issue templates
- task breakdown template
- review checklist
- model selection policy

## M2 - Core Inventory Domain

Goal: manage vendors, products, stock items, and sales channels.

Deliverables:

- OpenAPI contract for core inventory APIs
- PostgreSQL schema
- EF Core persistence
- basic CRUD endpoints
- unit and integration tests

## M3 - Reservations and Availability

Goal: reserve stock safely and prevent overselling.

Deliverables:

- reservation workflow
- availability calculation
- reservation expiration
- concurrency handling
- acceptance tests for stock reduction and release

## M4 - Event-Driven Synchronization

Goal: publish changes and synchronize marketplace availability through events.

Deliverables:

- Kafka integration
- stock changed events
- reservation events
- channel sync worker
- event documentation

## M5 - Observability and AI Monitoring

Goal: monitor both the software platform and the AI-assisted development workflow.

Deliverables:

- structured logs
- metrics
- traces
- Prometheus/Grafana setup
- AI prompt/model usage tracking
- dashboard for iteration count, quality, and cost signals

