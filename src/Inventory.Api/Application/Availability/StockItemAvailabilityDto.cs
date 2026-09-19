namespace Inventory.Api.Application.Availability;

public sealed record StockItemAvailabilityDto(
    Guid StockItemId,
    int OnHandQuantity,
    int ReservedQuantity,
    int AvailableQuantity,
    DateTimeOffset? NextExpirationUtc);
