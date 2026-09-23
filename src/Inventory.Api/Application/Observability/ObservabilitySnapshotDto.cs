namespace Inventory.Api.Application.Observability;

public sealed record ObservabilitySnapshotDto(
    string Status,
    DateTimeOffset CheckedAtUtc,
    OutboxObservabilityDto Outbox,
    ChannelSyncObservabilityDto ChannelSync);

public sealed record OutboxObservabilityDto(
    int TotalMessages,
    int PendingMessages,
    int ProcessingMessages,
    int PublishedMessages,
    int FailedMessages,
    DateTimeOffset? OldestPendingAtUtc,
    DateTimeOffset? NextAttemptAtUtc,
    IReadOnlyList<OutboxStatusMetricDto> ByStatus,
    IReadOnlyList<OutboxEventMetricDto> ByEventType);

public sealed record OutboxStatusMetricDto(
    string Status,
    int Count);

public sealed record OutboxEventMetricDto(
    string EventType,
    string Status,
    int Count);

public sealed record ChannelSyncObservabilityDto(
    int TotalStates,
    int PendingStates,
    int InProgressStates,
    int SucceededStates,
    int FailedStates,
    int RetryableFailures,
    DateTimeOffset? NextRetryAtUtc,
    DateTimeOffset? LastFailureAtUtc);
