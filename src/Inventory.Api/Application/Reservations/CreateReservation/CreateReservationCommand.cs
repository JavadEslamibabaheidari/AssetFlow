using Inventory.Api.Application.Common;
using MediatR;

namespace Inventory.Api.Application.Reservations.CreateReservation;

public sealed record CreateReservationCommand(
    Guid StockItemId,
    int Quantity,
    DateTimeOffset ExpiresAtUtc) : IRequest<ApplicationResult<ReservationDto>>;
