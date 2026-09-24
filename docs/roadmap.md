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
- EF Core persistence behind CQRS command/query handlers
- MediatR-based application layer
- basic CRUD endpoints that dispatch commands and queries
- unit and integration tests

## M3 - Reservations and Availability

Goal: reserve stock safely and prevent overselling.

Deliverables:

- reservation workflow
- availability calculation
- reservation expiration
- concurrency handling
- MediatR command/query handlers for reservation and availability use cases
- acceptance tests for stock reduction and release

## M4 - Agentic Control Dashboard MVP

Goal: give the user an early control center for project status, AI-assisted workflow entry points, and planning visibility before deeper automation is added.

Deliverables:

- dashboard surface for current milestone, active branch/task, local docs, and GitHub tracking links
- quick access to roadmap, plans, project knowledge, agents, and reusable skills
- buttons or command launch points for common workflows such as status check, planning, implementation, testing, review, knowledge update, and GitHub sync
- lightweight activity and usage summary for AI-assisted work when available
- clear boundaries that keep memory systems, external workspace automation, and deep resource accounting for later milestones

## M5 - Frontend Application Foundation

Goal: establish the modern frontend application repository, architecture, design system foundation, and delivery pipeline before building product workflows.

Deliverables:

- frontend repository or workspace with a production-grade web app stack selected during milestone planning
- app shell with routing, layout, navigation, error boundaries, and environment configuration
- design system foundation with tokens, reusable components, icons, accessibility rules, and responsive layout conventions
- OpenAPI-based API client strategy that keeps frontend contracts aligned with backend APIs
- frontend quality gates including linting, formatting, unit/component testing, visual checks where practical, and a small Playwright smoke-test foundation
- Docker Compose build/run path for frontend and relevant backend containers
- clear documentation for frontend architecture, coding conventions, local development, and backend integration

## M6 - Frontend Inventory Parity

Goal: bring the frontend to the same functional product surface as the backend reaches at the end of M3.

Deliverables:

- user-facing screens and workflows for vendors, products, sales channels, stock items, reservations, and availability
- API-backed list/detail/create flows that match the M2 and M3 OpenAPI contracts
- clear loading, empty, validation, conflict, not-found, and success states for inventory and reservation workflows
- reservation-aware availability views that make oversell-prevention behavior understandable to the user
- frontend integration tests and focused Playwright coverage for the most important inventory and reservation paths
- Docker Compose end-to-end verification with frontend and backend containers running together
- updated docs that describe how frontend workflows map to backend contracts and milestone behavior

## M7 - Event-Driven Synchronization

Goal: publish changes and synchronize marketplace availability through events.

Deliverables:

- PostgreSQL-backed outbox foundation with broker-ready event contracts
- stock item and availability changed events
- reservation events
- event publication from application handlers or an outbox-style boundary
- idempotent channel sync worker with adapter seam and persisted attempt/retry state
- frontend/admin visibility for synchronization status, failures, and automatic retry state
- event documentation

## M8 - Observability and Monitoring

Goal: monitor the software platform and development workflow health.

Deliverables:

- structured logs
- metrics
- traces
- Prometheus/Grafana setup
- workflow usage and cost signals where available
- frontend or observability dashboard surfaces for service health, delivery quality, and operational signals

## M9 - Agentic OS Expansion

Goal: expand the dashboard into a broader human-plus-AI operating layer for memory, automation, research, resource-aware agent workflows, and visible AI orchestration.

Status: closed after cockpit adoption.

Deliverables:

- evaluated memory layer with security, privacy, retention, and access boundaries before adopting tools such as Obsidian, repo-local notes, vector stores, external drives, or alternatives
- local-first skillpack and automation catalog for project, workspace, and research activity, with mutating actions requiring explicit approval
- resource usage telemetry model for skill, agent, workflow, and automation activity that marks unavailable token/cost fields honestly
- integration plan for external tools such as Google Workspace and research notebooks where they add clear value, including connector prerequisites and local-first fallbacks
- expanded Agentic Control Dashboard controls for memory, automation, resource telemetry, research links, workflow launch points, and degraded-source visibility
