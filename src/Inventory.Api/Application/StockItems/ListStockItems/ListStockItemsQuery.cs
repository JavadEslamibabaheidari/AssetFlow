using MediatR;

namespace Inventory.Api.Application.StockItems.ListStockItems;

public sealed record ListStockItemsQuery(Guid? ProductId, Guid? ChannelId)
    : IRequest<IReadOnlyList<StockItemDto>>;
