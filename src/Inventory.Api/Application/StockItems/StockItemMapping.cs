using Inventory.Api.Domain.Entities;

namespace Inventory.Api.Application.StockItems;

public static class StockItemMapping
{
    public static StockItemDto ToDto(this StockItem stockItem) =>
        new(
            stockItem.Id,
            stockItem.ProductId,
            stockItem.ChannelId,
            stockItem.OnHandQuantity,
            stockItem.AvailableQuantity,
            stockItem.UpdatedAtUtc);
}
