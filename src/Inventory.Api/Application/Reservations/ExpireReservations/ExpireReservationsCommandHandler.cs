using Inventory.Api.Domain.Entities;
using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Reservations.ExpireReservations;

public sealed class ExpireReservationsCommandHandler(InventoryDbContext dbContext)
    : IRequestHandler<ExpireReservationsCommand, ExpireReservationsDto>
{
    public async Task<ExpireReservationsDto> Handle(
        ExpireReservationsCommand request,
        CancellationToken cancellationToken)
    {
        var cutoffUtc = request.ExpiresBeforeUtc ?? DateTimeOffset.UtcNow;

        var dueReservations = await dbContext.Reservations
            .Where(reservation =>
                reservation.Status == ReservationStatus.Active &&
                reservation.ExpiresAtUtc <= cutoffUtc)
            .OrderBy(reservation => reservation.ExpiresAtUtc)
            .ToArrayAsync(cancellationToken);

        foreach (var reservation in dueReservations)
        {
            reservation.Expire(cutoffUtc);
        }

        if (dueReservations.Length > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return new ExpireReservationsDto(
            dueReservations.Length,
            dueReservations.Select(reservation => reservation.ToDto()).ToArray());
    }
}
