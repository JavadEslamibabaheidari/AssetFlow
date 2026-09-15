---
name: assetflow-tester
description: Verify AssetFlow changes with risk-based unit, integration, concurrency, and infrastructure tests.
metadata:
  short-description: Test AssetFlow changes by risk
---

# AssetFlow Tester

Use this skill to verify completed or in-progress work. Choose the smallest appropriate test level based on risk.

## Test Strategy

Use unit tests for domain logic, algorithms, transformations, validation, and isolated business rules. Do not mock everything just to label a test as a unit test.

Use integration tests when behavior depends on real infrastructure, including PostgreSQL, Kafka, blob storage, gRPC, HTTP APIs, persistence, event stores, serialization, or messaging. Prefer realistic dependencies over excessive mocking for integration behavior. Docker and containers are acceptable when available and proportionate.

Tests must be repeatable, isolated, and responsible for cleaning up their state.

## Concurrency and Messaging

For channels, consumers, workers, and event-driven components, consider race conditions, cancellation, backpressure, graceful shutdown, duplicate messages, retry behavior, and failures during processing.

Avoid unreliable tests built primarily on arbitrary delays such as `Task.Delay(5000)`. Prefer deterministic synchronization, controllable clocks, cancellation, and eventual assertions with bounded timeouts.

## Interpreting Failures

A failing test is information. Determine whether the failure indicates incorrect implementation, incorrect test expectations, incorrect requirements, flawed architecture, or an environmental problem. Do not weaken tests merely to make them pass.

When test results change the plan or architecture, update the relevant plan and send work back to implementation, prioritization, or planning as needed.
