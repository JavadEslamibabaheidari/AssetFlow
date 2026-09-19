namespace Inventory.Api.Application.Reservations;

public sealed record ReservationDto(
    Guid Id,
    Guid StockItemId,
    int Quantity,
    string Status,
    DateTimeOffset ExpiresAtUtc,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    DateTimeOffset? ReleasedAtUtc,
    DateTimeOffset? ExpiredAtUtc);
