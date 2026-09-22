namespace Inventory.Api.Infrastructure.ChannelSync;

public sealed record ChannelAvailabilitySyncResult(
    bool Succeeded,
    bool Retryable,
    string? ErrorSummary)
{
    public static ChannelAvailabilitySyncResult Success() => new(true, false, null);

    public static ChannelAvailabilitySyncResult Failure(bool retryable, string errorSummary)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorSummary);

        return new ChannelAvailabilitySyncResult(false, retryable, errorSummary);
    }
}
