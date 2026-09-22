namespace Inventory.Api.Infrastructure.ChannelSync;

public sealed class ChannelSyncState
{
    public ChannelSyncState(
        Guid id,
        Guid channelId,
        Guid stockItemId,
        Guid sourceEventId,
        int availableQuantity,
        DateTimeOffset updatedAtUtc)
    {
        Id = id;
        ChannelId = channelId;
        StockItemId = stockItemId;
        SourceEventId = sourceEventId;
        AvailableQuantity = availableQuantity;
        Status = ChannelSyncStatus.Pending;
        UpdatedAtUtc = updatedAtUtc;
    }

    private ChannelSyncState()
    {
    }

    public Guid Id { get; private set; }

    public Guid ChannelId { get; private set; }

    public Guid StockItemId { get; private set; }

    public Guid SourceEventId { get; private set; }

    public int AvailableQuantity { get; private set; }

    public ChannelSyncStatus Status { get; private set; }

    public int AttemptCount { get; private set; }

    public DateTimeOffset? LastAttemptedAtUtc { get; private set; }

    public DateTimeOffset? NextAttemptAtUtc { get; private set; }

    public DateTimeOffset? LastSucceededAtUtc { get; private set; }

    public string? LastError { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public void StartAttempt(Guid sourceEventId, int availableQuantity, DateTimeOffset attemptedAtUtc)
    {
        SourceEventId = sourceEventId;
        AvailableQuantity = availableQuantity;
        Status = ChannelSyncStatus.InProgress;
        AttemptCount++;
        LastAttemptedAtUtc = attemptedAtUtc;
        NextAttemptAtUtc = null;
        LastError = null;
        UpdatedAtUtc = attemptedAtUtc;
    }

    public void MarkSucceeded(DateTimeOffset succeededAtUtc)
    {
        Status = ChannelSyncStatus.Succeeded;
        NextAttemptAtUtc = null;
        LastSucceededAtUtc = succeededAtUtc;
        LastError = null;
        UpdatedAtUtc = succeededAtUtc;
    }

    public void MarkFailed(string errorSummary, DateTimeOffset attemptedAtUtc, DateTimeOffset? nextAttemptAtUtc)
    {
        Status = ChannelSyncStatus.Failed;
        LastError = errorSummary;
        NextAttemptAtUtc = nextAttemptAtUtc;
        UpdatedAtUtc = attemptedAtUtc;
    }
}
