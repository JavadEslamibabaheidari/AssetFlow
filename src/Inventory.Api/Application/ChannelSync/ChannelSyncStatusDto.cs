namespace Inventory.Api.Application.ChannelSync;

public sealed record ChannelSyncStatusDto(
    Guid Id,
    Guid ChannelId,
    Guid StockItemId,
    Guid SourceEventId,
    int AvailableQuantity,
    string Status,
    int AttemptCount,
    DateTimeOffset? LastAttemptedAtUtc,
    DateTimeOffset? NextAttemptAtUtc,
    DateTimeOffset? LastSucceededAtUtc,
    string? LastError,
    DateTimeOffset UpdatedAtUtc);
