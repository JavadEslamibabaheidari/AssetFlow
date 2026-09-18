using Inventory.Api.Application.StockItems;

namespace Inventory.Api.Api.StockItems;

public sealed record CreateStockItemRequest(Guid ProductId, Guid ChannelId, int OnHandQuantity);

public sealed record StockItemResponse(StockItemDto StockItem);

public sealed record StockItemListResponse(IReadOnlyList<StockItemDto> Items);
