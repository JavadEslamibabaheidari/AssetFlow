namespace Inventory.Api.Infrastructure.ChannelSync;

public sealed class NoOpChannelAvailabilitySyncAdapter : IChannelAvailabilitySyncAdapter
{
    public Task<ChannelAvailabilitySyncResult> SyncAvailabilityAsync(
        ChannelAvailabilitySyncRequest request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(ChannelAvailabilitySyncResult.Success());
    }
}
