using Inventory.Api.Application.Common;
using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Reservations.GetReservation;

public sealed class GetReservationQueryHandler(InventoryDbContext dbContext)
    : IRequestHandler<GetReservationQuery, ApplicationResult<ReservationDto>>
{
    public async Task<ApplicationResult<ReservationDto>> Handle(
        GetReservationQuery request,
        CancellationToken cancellationToken)
    {
        var reservation = await dbContext.Reservations
            .AsNoTracking()
            .Where(reservation => reservation.Id == request.ReservationId)
            .Select(reservation => reservation.ToDto())
            .SingleOrDefaultAsync(cancellationToken);

        return reservation is null
            ? ApplicationResult<ReservationDto>.NotFound(
                "reservation.notFound",
                "Reservation was not found.")
            : ApplicationResult<ReservationDto>.Success(reservation);
    }
}
