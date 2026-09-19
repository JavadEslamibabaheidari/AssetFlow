using Inventory.Api.Application.Common;
using Inventory.Api.Domain.Entities;
using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Reservations.ListReservations;

public sealed class ListReservationsQueryHandler(InventoryDbContext dbContext)
    : IRequestHandler<ListReservationsQuery, ApplicationResult<IReadOnlyList<ReservationDto>>>
{
    public async Task<ApplicationResult<IReadOnlyList<ReservationDto>>> Handle(
        ListReservationsQuery request,
        CancellationToken cancellationToken)
    {
        ReservationStatus? status = null;

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<ReservationStatus>(request.Status, ignoreCase: true, out var parsedStatus))
            {
                return ApplicationResult<IReadOnlyList<ReservationDto>>.Validation(
                    "reservation.statusInvalid",
                    "Reservation status must be Active, Released, or Expired.");
            }

            status = parsedStatus;
        }

        var reservations = dbContext.Reservations.AsNoTracking();

        if (request.StockItemId is not null)
        {
            reservations = reservations.Where(reservation => reservation.StockItemId == request.StockItemId);
        }

        if (status is not null)
        {
            reservations = reservations.Where(reservation => reservation.Status == status);
        }

        var items = await reservations
            .OrderByDescending(reservation => reservation.CreatedAtUtc)
            .Select(reservation => reservation.ToDto())
            .ToArrayAsync(cancellationToken);

        return ApplicationResult<IReadOnlyList<ReservationDto>>.Success(items);
    }
}
