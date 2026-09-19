using Inventory.Api.Application.Common;
using Inventory.Api.Domain.Entities;
using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Reservations.ReleaseReservation;

public sealed class ReleaseReservationCommandHandler(InventoryDbContext dbContext)
    : IRequestHandler<ReleaseReservationCommand, ApplicationResult<ReservationDto>>
{
    public async Task<ApplicationResult<ReservationDto>> Handle(
        ReleaseReservationCommand request,
        CancellationToken cancellationToken)
    {
        var reservation = await dbContext.Reservations
            .SingleOrDefaultAsync(reservation => reservation.Id == request.ReservationId, cancellationToken);

        if (reservation is null)
        {
            return ApplicationResult<ReservationDto>.NotFound(
                "reservation.notFound",
                "Reservation was not found.");
        }

        if (reservation.Status == ReservationStatus.Expired)
        {
            return ApplicationResult<ReservationDto>.Conflict(
                "reservation.alreadyExpired",
                "Expired reservations cannot be released.");
        }

        var changed = reservation.Release(DateTimeOffset.UtcNow);

        if (changed)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return ApplicationResult<ReservationDto>.Success(reservation.ToDto());
    }
}
