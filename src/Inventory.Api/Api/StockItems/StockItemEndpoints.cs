using Inventory.Api.Api.Common;
using Inventory.Api.Application.StockItems.CreateStockItem;
using Inventory.Api.Application.StockItems.GetStockItem;
using Inventory.Api.Application.StockItems.ListStockItems;
using MediatR;

namespace Inventory.Api.Api.StockItems;

public static class StockItemEndpoints
{
    public static IEndpointRouteBuilder MapStockItemEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/stock-items")
            .WithTags("StockItems");

        group.MapGet("/", async (
            Guid? productId,
            Guid? channelId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var stockItems = await sender.Send(new ListStockItemsQuery(productId, channelId), cancellationToken);

            return Results.Ok(new StockItemListResponse(stockItems));
        })
        .WithName("ListStockItems");

        group.MapPost("/", async (
            CreateStockItemRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new CreateStockItemCommand(request.ProductId, request.ChannelId, request.OnHandQuantity),
                cancellationToken);

            return result.ToHttpResult(stockItem =>
                Results.Created($"/stock-items/{stockItem.Id}", new StockItemResponse(stockItem)));
        })
        .WithName("CreateStockItem");

        group.MapGet("/{stockItemId:guid}", async (
            Guid stockItemId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetStockItemQuery(stockItemId), cancellationToken);

            return result.ToHttpResult(stockItem => Results.Ok(new StockItemResponse(stockItem)));
        })
        .WithName("GetStockItem");

        return endpoints;
    }
}
