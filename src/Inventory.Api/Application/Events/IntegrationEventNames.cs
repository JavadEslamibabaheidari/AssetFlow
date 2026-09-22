namespace Inventory.Api.Application.Events;

public static class IntegrationEventNames
{
    public const string StockItemCreated = "StockItemCreated";
    public const string StockAvailabilityChanged = "StockAvailabilityChanged";
    public const string ReservationCreated = "ReservationCreated";
    public const string ReservationReleased = "ReservationReleased";
    public const string ReservationExpired = "ReservationExpired";
    public const string ChannelSyncRequested = "ChannelSyncRequested";
    public const string ChannelSyncSucceeded = "ChannelSyncSucceeded";
    public const string ChannelSyncFailed = "ChannelSyncFailed";
}
