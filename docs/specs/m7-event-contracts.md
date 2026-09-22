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

`ReservationCreated` version 1:

- `reservationId`
- `stockItemId`
- `quantity`
- `expiresAtUtc`
- `status`
- `createdAtUtc`

`ReservationReleased` version 1:

- `reservationId`
- `stockItemId`
- `quantity`
- `releasedAtUtc`
- `status`

`ReservationExpired` version 1:

- `reservationId`
- `stockItemId`
- `quantity`
- `expiredAtUtc`
- `status`

The M7 plan reserves names for `ChannelSyncRequested`, `ChannelSyncSucceeded`, and `ChannelSyncFailed`. Their typed payload records exist in code so follow-up slices can add processors without changing the outbox foundation.

## Publication Rules

- Successful stock item creation writes `StockItemCreated` and an initial `StockAvailabilityChanged` event in the same persistence unit as the new stock item.
- Successful reservation creation writes `ReservationCreated` and `StockAvailabilityChanged` events.
- Successful reservation release writes `ReservationReleased` and `StockAvailabilityChanged` events. Idempotent release of an already released reservation does not duplicate events.
- Successful reservation expiration writes one `ReservationExpired` event per expired reservation and one `StockAvailabilityChanged` event per affected stock item.
- Validation failures, missing references, duplicate conflicts, oversell conflicts, and not-due expiration scans must not create outbox records.
- Outbox records are broker-ready integration events. Kafka or another broker can be added later by processing pending records without moving publication into controllers or rewriting domain behavior.
