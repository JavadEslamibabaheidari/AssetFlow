using Inventory.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Availability;

public static class StockItemAvailabilityStore
{
    public static async Task<StockItemAvailabilitySnapshot?> RecalculateAsync(
        InventoryDbContext dbContext,
        Guid stockItemId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken)
    {
        var stockItem = await dbContext.StockItems
            .SingleOrDefaultAsync(stockItem => stockItem.Id == stockItemId, cancellationToken);

        if (stockItem is null)
        {
            return null;
        }

        var reservations = await dbContext.Reservations
            .AsNoTracking()
            .Where(reservation => reservation.StockItemId == stockItemId)
            .Select(reservation => new ReservationAvailabilityInput(
                reservation.Quantity,
                reservation.Status,
                reservation.ExpiresAtUtc))
            .ToArrayAsync(cancellationToken);

        var availability = StockItemAvailabilityCalculator.Calculate(
            stockItem.Id,
            stockItem.OnHandQuantity,
            reservations,
            nowUtc);

        stockItem.UpdateAvailableQuantity(availability.AvailableQuantity, nowUtc);

        return new StockItemAvailabilitySnapshot(
            stockItem.Id,
            stockItem.ProductId,
            stockItem.ChannelId,
            stockItem.OnHandQuantity,
            availability.ReservedQuantity,
            availability.AvailableQuantity,
            availability.NextExpirationUtc);
    }
}

public sealed record StockItemAvailabilitySnapshot(
    Guid StockItemId,
    Guid ProductId,
    Guid ChannelId,
    int OnHandQuantity,
    int ReservedQuantity,
    int AvailableQuantity,
    DateTimeOffset? NextExpirationUtc);
