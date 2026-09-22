namespace Inventory.Api.Infrastructure.ChannelSync;

public interface IChannelAvailabilitySyncAdapter
{
    Task<ChannelAvailabilitySyncResult> SyncAvailabilityAsync(
        ChannelAvailabilitySyncRequest request,
        CancellationToken cancellationToken);
}
