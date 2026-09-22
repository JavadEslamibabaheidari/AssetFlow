# M7 Event Contracts

M7 uses integration events persisted through the `outbox_messages` table before any external broker or channel worker processes them. Events are emitted from application use cases after successful business validation and state changes; controllers do not publish events.

## Envelope

Every outbox record stores:

- `id`: unique outbox message id.
- `event_type`: stable event name.
- `schema_version`: positive integer schema version.
- `aggregate_type`: source aggregate family, such as `stockItem`.
- `aggregate_id`: source aggregate id.
- `occurred_at_utc`: business occurrence time.
- `payload_json`: event payload serialized as JSON.
- `status`: processing state, initially `Pending`.
- `attempt_count`: processing attempt count, initially `0`.
- `next_attempt_at_utc`: optional retry schedule.
- `last_error`: safe processing error summary.
- `processed_at_utc`: processing completion time.
- `created_at_utc`: outbox record creation time.

## Initial Events

`StockItemCreated` version 1:

- `stockItemId`
- `productId`
- `channelId`
- `onHandQuantity`
- `availableQuantity`
- `updatedAtUtc`

`StockAvailabilityChanged` version 1:

- `stockItemId`
- `productId`
- `channelId`
- `onHandQuantity`
- `reservedQuantity`
- `availableQuantity`
- `nextExpirationAtUtc`
- `reason`
- `sourceMutationId`

The M7 plan reserves names for `ReservationCreated`, `ReservationReleased`, `ReservationExpired`, `ChannelSyncRequested`, `ChannelSyncSucceeded`, and `ChannelSyncFailed`. Their typed payload records exist in code so follow-up slices can add producers and processors without changing the outbox foundation.

## Publication Rules

- Successful stock item creation writes `StockItemCreated` and an initial `StockAvailabilityChanged` event in the same persistence unit as the new stock item.
- Validation failures, missing references, duplicate conflicts, and oversell conflicts must not create outbox records.
- Outbox records are broker-ready integration events. Kafka or another broker can be added later by processing pending records without moving publication into controllers or rewriting domain behavior.
