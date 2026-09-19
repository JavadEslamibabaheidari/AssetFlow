using Inventory.Api.Domain.Entities;

namespace Inventory.Api.Application.Availability;

public static class StockItemAvailabilityCalculator
{
    public static StockItemAvailabilityDto Calculate(
        Guid stockItemId,
        int onHandQuantity,
        IEnumerable<ReservationAvailabilityInput> reservations,
        DateTimeOffset nowUtc)
    {
        var activeUnexpiredReservations = reservations
            .Where(reservation =>
                reservation.Status == ReservationStatus.Active &&
                reservation.ExpiresAtUtc > nowUtc)
            .ToArray();

        var reservedQuantity = activeUnexpiredReservations.Sum(reservation => reservation.Quantity);
        DateTimeOffset? nextExpirationUtc = activeUnexpiredReservations.Length == 0
            ? null
            : activeUnexpiredReservations.Min(reservation => reservation.ExpiresAtUtc);

        return new StockItemAvailabilityDto(
            stockItemId,
            onHandQuantity,
            reservedQuantity,
            onHandQuantity - reservedQuantity,
            nextExpirationUtc);
    }
}
