namespace Inventory.Api.Application.Reservations.ExpireReservations;

public sealed record ExpireReservationsDto(
    int ExpiredCount,
    IReadOnlyList<ReservationDto> Items);
