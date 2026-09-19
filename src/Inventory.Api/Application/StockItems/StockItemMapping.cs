using Inventory.Api.Domain.Entities;

namespace Inventory.Api.Application.StockItems;

public static class StockItemMapping
{
    public static StockItemDto ToDto(this StockItem stockItem) =>
        stockItem.ToDto(stockItem.AvailableQuantity);

    public static StockItemDto ToDto(this StockItem stockItem, int availableQuantity) =>
        new(
            stockItem.Id,
            stockItem.ProductId,
            stockItem.ChannelId,
            stockItem.OnHandQuantity,
            availableQuantity,
            stockItem.UpdatedAtUtc);
}
