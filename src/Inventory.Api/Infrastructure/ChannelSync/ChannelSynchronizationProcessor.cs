using System.Text.Json;
using Inventory.Api.Application.Events;
using Inventory.Api.Infrastructure.Observability;
using Inventory.Api.Infrastructure.Persistence;
using Inventory.Api.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Infrastructure.ChannelSync;

public sealed class ChannelSynchronizationProcessor(
    InventoryDbContext dbContext,
    IChannelAvailabilitySyncAdapter adapter)
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private static readonly TimeSpan RetryDelay = TimeSpan.FromMinutes(5);

    public async Task<int> ProcessDueAsync(
        int batchSize,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken)
    {
        using var activity = InventoryDiagnostics.ActivitySource.StartActivity("channel.sync.process_due");
        activity?.SetTag("assetflow.channel_sync.batch_size", batchSize);

        if (batchSize <= 0)
        {
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, "Batch size must be greater than zero.");
            throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be greater than zero.");
        }

        var messages = await dbContext.OutboxMessages
            .Where(message =>
                message.EventType == IntegrationEventNames.StockAvailabilityChanged &&
                (message.Status == OutboxMessageStatus.Pending ||
                    (message.Status == OutboxMessageStatus.Failed &&
                        message.NextAttemptAtUtc != null &&
                        message.NextAttemptAtUtc <= nowUtc)))
            .OrderBy(message => message.CreatedAtUtc)
            .Take(batchSize)
            .ToArrayAsync(cancellationToken);

        foreach (var message in messages)
        {
            await ProcessMessageAsync(message, nowUtc, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        activity?.SetTag("assetflow.channel_sync.processed_count", messages.Length);
        activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);

        return messages.Length;
    }

    private async Task ProcessMessageAsync(
        OutboxMessage message,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken)
    {
        using var activity = InventoryDiagnostics.ActivitySource.StartActivity("channel.sync.process_message");
        activity?.SetTag("assetflow.outbox.event_type", message.EventType);
        activity?.SetTag("assetflow.outbox.status", message.Status.ToString());

        var payload = JsonSerializer.Deserialize<StockAvailabilityChangedPayload>(
            message.PayloadJson,
            SerializerOptions);

        if (payload is null)
        {
            message.MarkFailed(nowUtc, "Stock availability payload could not be deserialized.", null);
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, "Payload could not be deserialized.");
            return;
        }

        var syncState = await dbContext.ChannelSyncStates
            .SingleOrDefaultAsync(
                state => state.ChannelId == payload.ChannelId && state.StockItemId == payload.StockItemId,
                cancellationToken);
        if (syncState is null)
        {
            syncState = new ChannelSyncState(
                Guid.NewGuid(),
                payload.ChannelId,
                payload.StockItemId,
                message.Id,
                payload.AvailableQuantity,
                nowUtc);
            dbContext.ChannelSyncStates.Add(syncState);
        }

        message.MarkProcessing(nowUtc);
        syncState.StartAttempt(message.Id, payload.AvailableQuantity, nowUtc);

        ChannelAvailabilitySyncResult result;
        try
        {
            result = await adapter.SyncAvailabilityAsync(
                new ChannelAvailabilitySyncRequest(
                    message.Id,
                    payload.ChannelId,
                    payload.StockItemId,
                    payload.AvailableQuantity),
                cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            result = ChannelAvailabilitySyncResult.Failure(retryable: true, exception.Message);
        }

        if (result.Succeeded)
        {
            message.MarkPublished(nowUtc);
            syncState.MarkSucceeded(nowUtc);
            activity?.SetTag("assetflow.channel_sync.outcome", "Succeeded");
            activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);
            return;
        }

        var errorSummary = result.ErrorSummary ?? "Channel availability sync failed.";
        DateTimeOffset? nextAttemptAtUtc = result.Retryable ? nowUtc.Add(RetryDelay) : null;
        message.MarkFailed(nowUtc, errorSummary, nextAttemptAtUtc);
        syncState.MarkFailed(errorSummary, nowUtc, nextAttemptAtUtc);
        activity?.SetTag("assetflow.channel_sync.outcome", "Failed");
        activity?.SetTag("assetflow.channel_sync.retryable", result.Retryable);
        activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, errorSummary);
    }
}
