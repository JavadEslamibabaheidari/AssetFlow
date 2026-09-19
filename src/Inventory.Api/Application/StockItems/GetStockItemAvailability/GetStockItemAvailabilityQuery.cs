using Inventory.Api.Application.Availability;
using Inventory.Api.Application.Common;
using MediatR;

namespace Inventory.Api.Application.StockItems.GetStockItemAvailability;

public sealed record GetStockItemAvailabilityQuery(Guid StockItemId)
    : IRequest<ApplicationResult<StockItemAvailabilityDto>>;
