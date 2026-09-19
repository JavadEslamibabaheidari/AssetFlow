using Inventory.Api.Application.Common;
using MediatR;

namespace Inventory.Api.Application.Reservations.GetReservation;

public sealed record GetReservationQuery(Guid ReservationId) : IRequest<ApplicationResult<ReservationDto>>;
