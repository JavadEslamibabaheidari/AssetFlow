using Inventory.Api.Application.Common;
using MediatR;

namespace Inventory.Api.Application.StockItems.GetStockItem;

public sealed record GetStockItemQuery(Guid StockItemId) : IRequest<ApplicationResult<StockItemDto>>;
