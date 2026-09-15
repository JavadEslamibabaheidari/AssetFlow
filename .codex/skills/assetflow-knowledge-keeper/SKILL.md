---
name: assetflow-knowledge-keeper
description: Maintain compact AssetFlow project knowledge in docs/knowledge so future work can start from curated architecture context while verifying against code.
metadata:
  short-description: Keep project knowledge current
---

# AssetFlow Knowledge Keeper

Use this skill before expensive repository exploration and after meaningful changes that alter architecture, contracts, persistence, messaging, APIs, realtime behavior, infrastructure, testing strategy, or important conventions.

Project knowledge is a curated cache, not a repository dump. When knowledge conflicts with actual code, actual code wins; correct the knowledge afterward.

## Location

Maintain compact Markdown in `docs/knowledge/`. Create only the files the project needs, such as:

- `overview.md`
- `architecture.md`
- `services.md`
- `domain.md`
- `data.md`
- `messaging.md`
- `apis.md`
- `realtime.md`
- `infrastructure.md`
- `testing.md`
- `decisions.md`

## What to Capture

Capture durable facts that reduce repeated discovery:

- system overview, architecture, services, bounded contexts, and responsibilities
- important directories, abstractions, domain terminology, and conventions
- database ownership, PostgreSQL schemas, important tables, migrations, and query patterns
- Kafka topics, event contracts, producers, consumers, event flows, and idempotency rules
- gRPC services, REST APIs, SignalR hubs, blob storage usage, background workers, channel pipelines
- event-sourced aggregates, projections, infrastructure, testing strategy, constraints, and ADR links

Keep entries concise, current, and actionable. Remove stale information instead of accumulating contradictions.

## Lifecycle

After meaningful changes:

1. determine whether project knowledge changed
2. update affected knowledge files
3. remove or correct stale information
4. add important architectural decisions or ADR links
5. keep documentation synchronized with code
