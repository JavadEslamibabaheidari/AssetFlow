# AssetFlow Backend Implementation Reference

Open this reference for backend, persistence, API, messaging, realtime, blob,
channel, or cross-component implementation work.

## C# and ASP.NET Core

Use modern C# features when they improve the code: records, pattern matching,
nullable reference types, required members, collection expressions, cancellation
tokens, async streams, or value types where appropriate. Avoid advanced features
used only for style.

Follow established project conventions for Minimal APIs vs controllers,
dependency injection, options, middleware, authentication/authorization,
validation, exception handling, health checks, logging, and OpenTelemetry.

Never block async work with `.Result`, `.Wait()`, unnecessary `Task.Run`, or
unmanaged fire-and-forget tasks. Propagate `CancellationToken` where meaningful.

## Persistence and Data

Prefer Dapper for relational persistence when appropriate. Use explicit
parameterized SQL, async APIs, correct connection lifetimes, efficient mapping,
appropriate transactions, and focused queries. Avoid SQL injection, `SELECT *`,
N+1 queries, unnecessary round trips, and generic repository layers that hide
useful SQL.

For PostgreSQL, design schemas, constraints, indexes, transactions, concurrency
behavior, migrations, and query patterns intentionally. Think about query plans
for important paths, but do not optimize insignificant queries prematurely.

## Distributed and Realtime Features

Use Kafka only when asynchronous messaging solves a real requirement. Reason
about topic design, keys, partitions, ordering, consumer groups, retries, poison
messages, dead letters, duplicate delivery, idempotency, serialization, schema
evolution, offsets, and observability.

Separate domain events from integration events. When database state and event
publication must be consistent, evaluate a transactional outbox.

Use event sourcing only where immutable history, aggregate reconstruction,
optimistic concurrency, event versioning, replayable projections, and event
evolution are worth the complexity.

Use gRPC when strongly typed service-to-service communication has real value.
Preserve protobuf compatibility, deadlines, cancellation, status codes,
streaming semantics, and backward compatibility.

Design REST APIs with predictable resource names, validation, status codes,
pagination, filtering, versioning when necessary, and consistent error
responses.

Use SignalR for genuine realtime server-to-client communication. Keep business
logic outside hubs. Consider strongly typed hubs, groups, connection lifecycle,
reconnection, authorization, scale-out, and message contracts.

Use `System.Threading.Channels` only for real producer/consumer, buffering,
backpressure, pipeline, or async handoff needs. Choose bounded vs unbounded
capacity intentionally and handle completion, cancellation, exceptions, and
shutdown.

For blob storage, consider streaming, large files, metadata, content types,
cancellation, retries, access control, lifecycle, cleanup, and avoiding
unnecessary buffering.

## Frontend Boundary

Frontend work is secondary for this skill. Add React/TypeScript UI only when it
demonstrates backend capabilities, APIs, realtime behavior, admin/debug
workflows, or event-driven flows. Build the minimum useful interface.
