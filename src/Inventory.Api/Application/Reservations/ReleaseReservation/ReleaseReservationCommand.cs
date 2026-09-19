using Inventory.Api.Application.Common;
using MediatR;

namespace Inventory.Api.Application.Reservations.ReleaseReservation;

public sealed record ReleaseReservationCommand(Guid ReservationId) : IRequest<ApplicationResult<ReservationDto>>;
