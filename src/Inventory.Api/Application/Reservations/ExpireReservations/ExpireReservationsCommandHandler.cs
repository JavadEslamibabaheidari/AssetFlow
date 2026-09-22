using Inventory.Api.Application.Availability;
using Inventory.Api.Application.Events;
using Inventory.Api.Domain.Entities;
using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Reservations.ExpireReservations;

public sealed class ExpireReservationsCommandHandler(
    InventoryDbContext dbContext,
    IIntegrationEventOutbox outbox)
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

            foreach (var reservation in dueReservations)
            {
                outbox.Enqueue(IntegrationEvents.ReservationExpired(
                    reservation.Id,
                    reservation.StockItemId,
                    reservation.Quantity,
                    cutoffUtc,
                    reservation.Status.ToString()));
            }

            foreach (var stockItemId in dueReservations.Select(reservation => reservation.StockItemId).Distinct())
            {
                var recalculatedAvailability = await StockItemAvailabilityStore.RecalculateAsync(
                    dbContext,
                    stockItemId,
                    cutoffUtc,
                    cancellationToken);
                if (recalculatedAvailability is not null)
                {
                    var sourceReservationId = dueReservations
                        .Where(reservation => reservation.StockItemId == stockItemId)
                        .OrderBy(reservation => reservation.ExpiresAtUtc)
                        .Select(reservation => reservation.Id)
                        .First();
                    outbox.Enqueue(IntegrationEvents.StockAvailabilityChanged(
                        recalculatedAvailability.StockItemId,
                        recalculatedAvailability.ProductId,
                        recalculatedAvailability.ChannelId,
                        recalculatedAvailability.OnHandQuantity,
                        recalculatedAvailability.ReservedQuantity,
                        recalculatedAvailability.AvailableQuantity,
                        recalculatedAvailability.NextExpirationUtc,
                        IntegrationEventNames.ReservationExpired,
                        sourceReservationId,
                        cutoffUtc));
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        await stockItemLock.CommitAsync(cancellationToken);

        return new ExpireReservationsDto(
            dueReservations.Length,
            dueReservations.Select(reservation => reservation.ToDto()).ToArray());
    }
}
