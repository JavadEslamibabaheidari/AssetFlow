using Inventory.Api.Application.Common;
using MediatR;

namespace Inventory.Api.Application.StockItems.CreateStockItem;

public sealed record CreateStockItemCommand(Guid ProductId, Guid ChannelId, int OnHandQuantity)
    : IRequest<ApplicationResult<StockItemDto>>;
