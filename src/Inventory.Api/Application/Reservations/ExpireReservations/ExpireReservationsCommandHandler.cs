using Inventory.Api.Application.Availability;
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

        await using var stockItemLock = await StockItemLockTransaction.BeginAsync(
            dbContext,
            dueReservations.Select(reservation => reservation.StockItemId),
            cancellationToken);

        foreach (var reservation in dueReservations)
        {
            reservation.Expire(cutoffUtc);
        }

        if (dueReservations.Length > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);

            foreach (var stockItemId in dueReservations.Select(reservation => reservation.StockItemId).Distinct())
            {
                await StockItemAvailabilityStore.RecalculateAsync(
                    dbContext,
                    stockItemId,
                    cutoffUtc,
                    cancellationToken);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        await stockItemLock.CommitAsync(cancellationToken);

        return new ExpireReservationsDto(
            dueReservations.Length,
            dueReservations.Select(reservation => reservation.ToDto()).ToArray());
    }
}
