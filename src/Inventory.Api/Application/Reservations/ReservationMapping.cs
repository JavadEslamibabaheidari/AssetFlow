using Inventory.Api.Domain.Entities;

namespace Inventory.Api.Application.Reservations;

public static class ReservationMapping
{
    public static ReservationDto ToDto(this Reservation reservation) =>
        new(
            reservation.Id,
            reservation.StockItemId,
            reservation.Quantity,
            reservation.Status.ToString(),
            reservation.ExpiresAtUtc,
            reservation.CreatedAtUtc,
            reservation.UpdatedAtUtc,
            reservation.ReleasedAtUtc,
            reservation.ExpiredAtUtc);
}
