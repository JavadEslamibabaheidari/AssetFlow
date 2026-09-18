namespace Inventory.Api.Application.StockItems;

public sealed record StockItemDto(
    Guid Id,
    Guid ProductId,
    Guid ChannelId,
    int OnHandQuantity,
    int AvailableQuantity,
    DateTimeOffset UpdatedAtUtc);
