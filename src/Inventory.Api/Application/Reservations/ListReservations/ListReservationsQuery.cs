using Inventory.Api.Application.Common;
using MediatR;

namespace Inventory.Api.Application.Reservations.ListReservations;

public sealed record ListReservationsQuery(Guid? StockItemId, string? Status)
    : IRequest<ApplicationResult<IReadOnlyList<ReservationDto>>>;
