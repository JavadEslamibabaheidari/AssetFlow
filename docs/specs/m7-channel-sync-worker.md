# M7 Channel Synchronization Worker

M7 introduces a deterministic channel synchronization processor for `StockAvailabilityChanged` outbox messages. The processor is intentionally adapter-based so tests and local development do not require live marketplace providers.

## Processing Path

`ChannelSynchronizationProcessor` reads due `StockAvailabilityChanged` messages from `outbox_messages`.

Due messages are:

- `Pending` availability messages.
- `Failed` availability messages whose `next_attempt_at_utc` is due.

For each due message, the processor:

1. Deserializes the `StockAvailabilityChanged` payload.
2. Upserts `channel_sync_states` for the channel/stock item pair.
3. Calls `IChannelAvailabilitySyncAdapter`.
4. Marks the outbox message `Published` and sync state `Succeeded` on success.
5. Marks the outbox message and sync state `Failed` on adapter failure.
6. Schedules retry for retryable failures with `next_attempt_at_utc`.

The default `NoOpChannelAvailabilitySyncAdapter` succeeds without calling an external provider. Real marketplace adapters must implement `IChannelAvailabilitySyncAdapter` and remain deterministic under tests.

## State

`channel_sync_states` stores the latest sync state per channel and stock item:

- `channel_id`
- `stock_item_id`
- `source_event_id`
- `available_quantity`
- `status`
- `attempt_count`
- `last_attempted_at_utc`
- `next_attempt_at_utc`
- `last_succeeded_at_utc`
- `last_error`
- `updated_at_utc`

This table is the backend foundation for the #88 Operations visibility slice.

## Idempotency and Retry

- Published source messages are ignored by later processor runs.
- Retryable failures keep the source event and become due only after `next_attempt_at_utc`.
- Non-retryable failures keep the source event and do not schedule another attempt.
- The adapter receives source event id, channel id, stock item id, and available quantity.
