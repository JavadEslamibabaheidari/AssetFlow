using Inventory.Api.Application.Common;
using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.StockItems.GetStockItem;

public sealed class GetStockItemQueryHandler(InventoryDbContext dbContext)
    : IRequestHandler<GetStockItemQuery, ApplicationResult<StockItemDto>>
{
    public async Task<ApplicationResult<StockItemDto>> Handle(
        GetStockItemQuery request,
        CancellationToken cancellationToken)
    {
        var stockItem = await dbContext.StockItems
            .AsNoTracking()
            .Where(stockItem => stockItem.Id == request.StockItemId)
            .Select(stockItem => stockItem.ToDto())
            .SingleOrDefaultAsync(cancellationToken);

        return stockItem is null
            ? ApplicationResult<StockItemDto>.NotFound(
                "stockItem.notFound",
                "Stock item was not found.")
            : ApplicationResult<StockItemDto>.Success(stockItem);
    }
}
