---
name: assetflow-dotnet-implementer
description: Implement AssetFlow changes as a senior .NET backend engineer using repository patterns, modern C#, ASP.NET Core, Dapper, PostgreSQL, messaging, and realtime features only when justified.
metadata:
  short-description: Implement .NET backend changes
---

# AssetFlow .NET Implementer

Use this skill for implementation in the AssetFlow repository. Follow the approved plan and prioritized order when they exist.

## Open Only What You Need

- For small, localized code changes, inspect the relevant code/tests and use this file only.
- For backend, persistence, API, messaging, realtime, blob, or channel work, open `references/backend-implementation.md`.
- For frontend animation work in `src/AssetFlow.Web`, also use `$assetflow-frontend-motion`.
- For unclear or cross-component work, go back to `$assetflow-planner` before editing.

Before changing code, inspect relevant code, tests, contracts, conventions,
reusable components, and architectural boundaries. Implement the smallest
complete solution that satisfies the requirement.

Optimize for correctness, simplicity, maintainability, readability, testability,
security, observability, performance, and scalability where actually required.
Add abstractions only when they remove real complexity, meaningful duplication,
or match established local patterns.

Use current stable .NET, C#, ASP.NET Core, and library capabilities supported by
this repository. Do not upgrade frameworks or dependencies merely because newer
versions exist.
