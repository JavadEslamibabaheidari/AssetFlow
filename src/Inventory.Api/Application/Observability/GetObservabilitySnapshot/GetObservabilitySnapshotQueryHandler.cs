using Inventory.Api.Infrastructure.ChannelSync;
using Inventory.Api.Infrastructure.Persistence;
using Inventory.Api.Infrastructure.Persistence.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Observability.GetObservabilitySnapshot;

public sealed class GetObservabilitySnapshotQueryHandler(InventoryDbContext dbContext)
    : IRequestHandler<GetObservabilitySnapshotQuery, ObservabilitySnapshotDto>
{
    public async Task<ObservabilitySnapshotDto> Handle(
        GetObservabilitySnapshotQuery request,
        CancellationToken cancellationToken)
    {
        var outboxStatusCounts = await dbContext.OutboxMessages
            .AsNoTracking()
            .GroupBy(message => message.Status)
            .Select(group => new OutboxStatusMetric(group.Key, group.Count()))
            .ToArrayAsync(cancellationToken);

        var outboxEventCounts = await dbContext.OutboxMessages
            .AsNoTracking()
            .GroupBy(message => new { message.EventType, message.Status })
            .Select(group => new OutboxEventMetric(
                group.Key.EventType,
                group.Key.Status,
                group.Count()))
            .ToArrayAsync(cancellationToken);
        var outboxByStatus = outboxStatusCounts
            .Select(metric => new OutboxStatusMetricDto(metric.Status.ToString(), metric.Count))
            .OrderBy(metric => metric.Status)
            .ToArray();
        var outboxByEventType = outboxEventCounts
            .Select(metric => new OutboxEventMetricDto(
                metric.EventType,
                metric.Status.ToString(),
                metric.Count))
            .OrderBy(metric => metric.EventType)
            .ThenBy(metric => metric.Status)
            .ToArray();

        var oldestPendingAtUtc = await dbContext.OutboxMessages
            .AsNoTracking()
            .Where(message => message.Status == OutboxMessageStatus.Pending)
            .MinAsync(message => (DateTimeOffset?)message.CreatedAtUtc, cancellationToken);

        var nextOutboxAttemptAtUtc = await dbContext.OutboxMessages
            .AsNoTracking()
            .Where(message => message.Status == OutboxMessageStatus.Failed &&
                message.NextAttemptAtUtc != null)
            .MinAsync(message => message.NextAttemptAtUtc, cancellationToken);

        var syncByStatus = await dbContext.ChannelSyncStates
            .AsNoTracking()
            .GroupBy(state => state.Status)
            .Select(group => new ChannelSyncStatusMetric(group.Key, group.Count()))
            .ToArrayAsync(cancellationToken);

        var nextSyncRetryAtUtc = await dbContext.ChannelSyncStates
            .AsNoTracking()
            .Where(state => state.Status == ChannelSyncStatus.Failed &&
                state.NextAttemptAtUtc != null)
            .MinAsync(state => state.NextAttemptAtUtc, cancellationToken);

        var lastSyncFailureAtUtc = await dbContext.ChannelSyncStates
            .AsNoTracking()
            .Where(state => state.Status == ChannelSyncStatus.Failed)
            .MaxAsync(state => (DateTimeOffset?)state.UpdatedAtUtc, cancellationToken);

        var outbox = new OutboxObservabilityDto(
            TotalMessages: outboxByStatus.Sum(metric => metric.Count),
            PendingMessages: CountStatus(outboxByStatus, OutboxMessageStatus.Pending.ToString()),
            ProcessingMessages: CountStatus(outboxByStatus, OutboxMessageStatus.Processing.ToString()),
            PublishedMessages: CountStatus(outboxByStatus, OutboxMessageStatus.Published.ToString()),
            FailedMessages: CountStatus(outboxByStatus, OutboxMessageStatus.Failed.ToString()),
            OldestPendingAtUtc: oldestPendingAtUtc,
            NextAttemptAtUtc: nextOutboxAttemptAtUtc,
            ByStatus: outboxByStatus.OrderBy(metric => metric.Status).ToArray(),
            ByEventType: outboxByEventType);

        var channelSync = new ChannelSyncObservabilityDto(
            TotalStates: syncByStatus.Sum(metric => metric.Count),
            PendingStates: CountStatus(syncByStatus, ChannelSyncStatus.Pending),
            InProgressStates: CountStatus(syncByStatus, ChannelSyncStatus.InProgress),
            SucceededStates: CountStatus(syncByStatus, ChannelSyncStatus.Succeeded),
            FailedStates: CountStatus(syncByStatus, ChannelSyncStatus.Failed),
            RetryableFailures: await dbContext.ChannelSyncStates
                .AsNoTracking()
                .CountAsync(state => state.Status == ChannelSyncStatus.Failed &&
                    state.NextAttemptAtUtc != null, cancellationToken),
            NextRetryAtUtc: nextSyncRetryAtUtc,
            LastFailureAtUtc: lastSyncFailureAtUtc);

        return new ObservabilitySnapshotDto(
            DetermineStatus(outbox, channelSync),
            request.CheckedAtUtc,
            outbox,
            channelSync);
    }

    private static int CountStatus(
        IEnumerable<OutboxStatusMetricDto> metrics,
        string status)
    {
        return metrics.FirstOrDefault(metric => metric.Status == status)?.Count ?? 0;
    }

    private static int CountStatus(
        IEnumerable<ChannelSyncStatusMetric> metrics,
        ChannelSyncStatus status)
    {
        return metrics.FirstOrDefault(metric => metric.Status == status)?.Count ?? 0;
    }

    private static string DetermineStatus(
        OutboxObservabilityDto outbox,
        ChannelSyncObservabilityDto channelSync)
    {
        if (outbox.FailedMessages > 0 || channelSync.FailedStates > 0)
        {
            return "Degraded";
        }

        if (outbox.PendingMessages > 0 ||
            outbox.ProcessingMessages > 0 ||
            channelSync.PendingStates > 0 ||
            channelSync.InProgressStates > 0)
        {
            return "Busy";
        }

        return "Healthy";
    }

    private sealed record ChannelSyncStatusMetric(ChannelSyncStatus Status, int Count);

    private sealed record OutboxStatusMetric(OutboxMessageStatus Status, int Count);

    private sealed record OutboxEventMetric(string EventType, OutboxMessageStatus Status, int Count);
}
