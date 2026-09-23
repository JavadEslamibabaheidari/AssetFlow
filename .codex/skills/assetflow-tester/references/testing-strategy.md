# AssetFlow Testing Strategy Reference

Open this reference for integration, infrastructure, concurrency, messaging,
failure-mode, or risk-heavy verification.

## Integration Testing

Use integration tests when behavior depends on real infrastructure, including
PostgreSQL, Kafka, blob storage, gRPC, HTTP APIs, persistence, event stores,
serialization, or messaging. Prefer realistic dependencies over excessive
mocking for integration behavior. Docker and containers are acceptable when
available and proportionate.

## Concurrency and Messaging

For channels, consumers, workers, and event-driven components, consider:

- race conditions
- cancellation
- backpressure
- graceful shutdown
- duplicate messages
- retry behavior
- failures during processing

Avoid unreliable tests built primarily on arbitrary delays such as
`Task.Delay(5000)`. Prefer deterministic synchronization, controllable clocks,
cancellation, and eventual assertions with bounded timeouts.

## Failure Interpretation

When test results change the plan or architecture, update the relevant plan and
send work back to implementation, prioritization, or planning as needed.
