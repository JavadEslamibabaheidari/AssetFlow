using System.Text.Json;
using Inventory.Api.Application.Events;
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
        if (batchSize <= 0)
        {
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

        return messages.Length;
    }

    private async Task ProcessMessageAsync(
        OutboxMessage message,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Deserialize<StockAvailabilityChangedPayload>(
            message.PayloadJson,
            SerializerOptions);

        if (payload is null)
        {
            message.MarkFailed(nowUtc, "Stock availability payload could not be deserialized.", null);
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
            return;
        }

        var errorSummary = result.ErrorSummary ?? "Channel availability sync failed.";
        DateTimeOffset? nextAttemptAtUtc = result.Retryable ? nowUtc.Add(RetryDelay) : null;
        message.MarkFailed(nowUtc, errorSummary, nextAttemptAtUtc);
        syncState.MarkFailed(errorSummary, nowUtc, nextAttemptAtUtc);
    }
}
