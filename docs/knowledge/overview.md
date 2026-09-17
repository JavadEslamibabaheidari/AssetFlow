# AssetFlow Knowledge Overview

This file is the starting point for AssetFlow repo knowledge. Treat it as a maintained engineering reference: read it before broad code inspection, verify important facts against code and GitHub, and update it whenever meaningful work changes reality.

## Current Shape

AssetFlow is a marketplace inventory platform in early foundation work. The repository currently contains one ASP.NET Core service, `Inventory.Api`, plus tests, Docker, local Kubernetes manifests, GitHub Actions, and planning documentation.

The first service is intentionally minimal. It exposes:

- `GET /`: returns service info for `Inventory API`
- `GET /health`: returns a healthy status, service name, and `checkedAtUtc`

The test project `Inventory.Api.Tests` verifies both endpoints through `WebApplicationFactory<Program>`.

## Milestone State

M0, Repository and Hello Service, is mostly complete: repository skeleton, minimal API, Docker support, local Kubernetes manifests, CI, issue templates, and planning docs exist. The remaining known M0 gap is local Kubernetes verification with `kind` or `minikube`.

M1, Spec-First Workflow, is the active workflow milestone. Its work should define how features are specified, reviewed, tracked, and implemented before the core inventory domain is built.

## Architecture Direction

The current implementation is a single minimal ASP.NET Core API. The target direction is an inventory platform with clear service ownership, OpenAPI-documented APIs, PostgreSQL-backed business state, and event-driven synchronization when the product need justifies it.

Kafka, gRPC, SignalR, event sourcing, projections, and channel workers are future options, not current commitments. Add them only when a milestone or issue creates a concrete need.

## Workflow Invariants

Repo knowledge must stay aligned with code. For milestones, big issues, and features, completion requires planned-state validation: compare the original plan and acceptance criteria with the actual code, tests, docs, and GitHub status before calling the work done.

When this knowledge conflicts with code, code wins and this knowledge must be corrected immediately.
