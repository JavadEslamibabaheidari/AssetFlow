namespace Inventory.Api.Infrastructure.ChannelSync;

public sealed record ChannelAvailabilitySyncRequest(
    Guid SourceEventId,
    Guid ChannelId,
    Guid StockItemId,
    int AvailableQuantity);
