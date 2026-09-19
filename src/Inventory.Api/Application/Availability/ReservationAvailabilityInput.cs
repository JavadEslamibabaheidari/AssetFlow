using Inventory.Api.Domain.Entities;

namespace Inventory.Api.Application.Availability;

public sealed record ReservationAvailabilityInput(
    int Quantity,
    ReservationStatus Status,
    DateTimeOffset ExpiresAtUtc);
