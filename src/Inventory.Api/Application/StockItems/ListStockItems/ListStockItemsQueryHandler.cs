using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.StockItems.ListStockItems;

public sealed class ListStockItemsQueryHandler(InventoryDbContext dbContext)
    : IRequestHandler<ListStockItemsQuery, IReadOnlyList<StockItemDto>>
{
    public async Task<IReadOnlyList<StockItemDto>> Handle(
        ListStockItemsQuery request,
        CancellationToken cancellationToken)
    {
        var stockItems = dbContext.StockItems.AsNoTracking();

        if (request.ProductId is { } productId)
        {
            stockItems = stockItems.Where(stockItem => stockItem.ProductId == productId);
        }

        if (request.ChannelId is { } channelId)
        {
            stockItems = stockItems.Where(stockItem => stockItem.ChannelId == channelId);
        }

        return await stockItems
            .OrderByDescending(stockItem => stockItem.UpdatedAtUtc)
            .Select(stockItem => stockItem.ToDto())
            .ToListAsync(cancellationToken);
    }
}
