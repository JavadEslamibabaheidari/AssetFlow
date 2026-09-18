using Inventory.Api.Application.Common;
using Inventory.Api.Domain.Entities;
using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.StockItems.CreateStockItem;

public sealed class CreateStockItemCommandHandler(InventoryDbContext dbContext)
    : IRequestHandler<CreateStockItemCommand, ApplicationResult<StockItemDto>>
{
    public async Task<ApplicationResult<StockItemDto>> Handle(
        CreateStockItemCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ProductId == Guid.Empty)
        {
            return ApplicationResult<StockItemDto>.Validation(
                "stockItem.productIdRequired",
                "Product id is required.");
        }

        if (request.ChannelId == Guid.Empty)
        {
            return ApplicationResult<StockItemDto>.Validation(
                "stockItem.channelIdRequired",
                "Channel id is required.");
        }

        if (request.OnHandQuantity < 0)
        {
            return ApplicationResult<StockItemDto>.Validation(
                "stockItem.onHandQuantityNegative",
                "On-hand quantity must be zero or greater.");
        }

        var productExists = await dbContext.Products
            .AnyAsync(product => product.Id == request.ProductId, cancellationToken);

        if (!productExists)
        {
            return ApplicationResult<StockItemDto>.NotFound(
                "product.notFound",
                "Product was not found.");
        }

        var channelExists = await dbContext.Channels
            .AnyAsync(channel => channel.Id == request.ChannelId, cancellationToken);

        if (!channelExists)
        {
            return ApplicationResult<StockItemDto>.NotFound(
                "channel.notFound",
                "Channel was not found.");
        }

        var duplicateExists = await dbContext.StockItems
            .AnyAsync(
                stockItem => stockItem.ProductId == request.ProductId
                    && stockItem.ChannelId == request.ChannelId,
                cancellationToken);

        if (duplicateExists)
        {
            return ApplicationResult<StockItemDto>.Conflict(
                "stockItem.duplicateProductChannel",
                "A stock item already exists for this product and channel.");
        }

        var stockItem = new StockItem(
            Guid.NewGuid(),
            request.ProductId,
            request.ChannelId,
            request.OnHandQuantity,
            DateTimeOffset.UtcNow);

        dbContext.StockItems.Add(stockItem);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ApplicationResult<StockItemDto>.Success(stockItem.ToDto());
    }
}
