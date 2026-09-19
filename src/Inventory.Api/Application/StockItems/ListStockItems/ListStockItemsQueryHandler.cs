using Inventory.Api.Application.Availability;
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

        var items = await stockItems
            .OrderByDescending(stockItem => stockItem.UpdatedAtUtc)
            .ToListAsync(cancellationToken);

        if (items.Count == 0)
        {
            return [];
        }

        var stockItemIds = items.Select(stockItem => stockItem.Id).ToArray();
        var reservations = await dbContext.Reservations
            .AsNoTracking()
            .Where(reservation => stockItemIds.Contains(reservation.StockItemId))
            .Select(reservation => new
            {
                reservation.StockItemId,
                Input = new ReservationAvailabilityInput(
                    reservation.Quantity,
                    reservation.Status,
                    reservation.ExpiresAtUtc)
            })
            .ToArrayAsync(cancellationToken);

        var reservationsByStockItemId = reservations
            .GroupBy(reservation => reservation.StockItemId)
            .ToDictionary(
                group => group.Key,
                group => group.Select(reservation => reservation.Input).ToArray());

        var nowUtc = DateTimeOffset.UtcNow;

        return items
            .Select(stockItem =>
            {
                reservationsByStockItemId.TryGetValue(stockItem.Id, out var stockItemReservations);
                var availability = StockItemAvailabilityCalculator.Calculate(
                    stockItem.Id,
                    stockItem.OnHandQuantity,
                    stockItemReservations ?? [],
                    nowUtc);

                return stockItem.ToDto(availability.AvailableQuantity);
            })
            .ToArray();
    }
}
