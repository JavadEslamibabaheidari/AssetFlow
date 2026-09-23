---
name: assetflow-tester
description: Verify AssetFlow changes with risk-based unit, integration, concurrency, and infrastructure tests.
metadata:
  short-description: Test AssetFlow changes by risk
---

# AssetFlow Tester

Use this skill to verify completed or in-progress work. Choose the smallest appropriate test level based on risk.

## Open Only What You Need

- For small localized changes, choose and run the closest relevant tests using this file only.
- For integration, infrastructure, concurrency, messaging, or failure-mode testing, open `references/testing-strategy.md`.
- If test results change the plan or architecture, route work back to planning, prioritization, or implementation.

Use unit tests for domain logic, algorithms, transformations, validation, and
isolated business rules. Do not mock everything just to label a test as a unit
test.

Use integration tests when behavior depends on real infrastructure, including
PostgreSQL, Kafka, blob storage, gRPC, HTTP APIs, persistence, event stores,
serialization, or messaging.

Tests must be repeatable, isolated, and responsible for cleaning up their state.

## Interpreting Failures

A failing test is information. Determine whether the failure indicates incorrect implementation, incorrect test expectations, incorrect requirements, flawed architecture, or an environmental problem. Do not weaken tests merely to make them pass.
