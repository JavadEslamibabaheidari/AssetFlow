using Inventory.Api.Application.Reservations;

namespace Inventory.Api.Api.Reservations;

public sealed record CreateReservationRequest(
    Guid StockItemId,
    int Quantity,
    DateTimeOffset ExpiresAtUtc);

public sealed record ReservationResponse(ReservationDto Reservation);

public sealed record ReservationListResponse(IReadOnlyList<ReservationDto> Items);
