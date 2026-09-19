using Inventory.Api.Application.Availability;
using Inventory.Api.Application.Common;
using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.StockItems.GetStockItemAvailability;

public sealed class GetStockItemAvailabilityQueryHandler(InventoryDbContext dbContext)
    : IRequestHandler<GetStockItemAvailabilityQuery, ApplicationResult<StockItemAvailabilityDto>>
{
    public async Task<ApplicationResult<StockItemAvailabilityDto>> Handle(
        GetStockItemAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        var stockItem = await dbContext.StockItems
            .AsNoTracking()
            .SingleOrDefaultAsync(stockItem => stockItem.Id == request.StockItemId, cancellationToken);

        if (stockItem is null)
        {
            return ApplicationResult<StockItemAvailabilityDto>.NotFound(
                "stockItem.notFound",
                "Stock item was not found.");
        }

        var reservations = await dbContext.Reservations
            .AsNoTracking()
            .Where(reservation => reservation.StockItemId == request.StockItemId)
            .Select(reservation => new ReservationAvailabilityInput(
                reservation.Quantity,
                reservation.Status,
                reservation.ExpiresAtUtc))
            .ToArrayAsync(cancellationToken);

        var availability = StockItemAvailabilityCalculator.Calculate(
            stockItem.Id,
            stockItem.OnHandQuantity,
            reservations,
            DateTimeOffset.UtcNow);

        return ApplicationResult<StockItemAvailabilityDto>.Success(availability);
    }
}
