using Inventory.Api.Application.Availability;
using Inventory.Api.Application.Common;
using Inventory.Api.Application.Events;
using Inventory.Api.Domain.Entities;
using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Reservations.ReleaseReservation;

public sealed class ReleaseReservationCommandHandler(
    InventoryDbContext dbContext,
    IIntegrationEventOutbox outbox)
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

        await using var stockItemLock = await StockItemLockTransaction.BeginAsync(
            dbContext,
            [reservation.StockItemId],
            cancellationToken);

        var releasedAtUtc = DateTimeOffset.UtcNow;
        var changed = reservation.Release(releasedAtUtc);

        if (changed)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            var recalculatedAvailability = await StockItemAvailabilityStore.RecalculateAsync(
                dbContext,
                reservation.StockItemId,
                releasedAtUtc,
                cancellationToken);
            outbox.Enqueue(IntegrationEvents.ReservationReleased(
                reservation.Id,
                reservation.StockItemId,
                reservation.Quantity,
                releasedAtUtc,
                reservation.Status.ToString()));
            if (recalculatedAvailability is not null)
            {
                outbox.Enqueue(IntegrationEvents.StockAvailabilityChanged(
                    recalculatedAvailability.StockItemId,
                    recalculatedAvailability.ProductId,
                    recalculatedAvailability.ChannelId,
                    recalculatedAvailability.OnHandQuantity,
                    recalculatedAvailability.ReservedQuantity,
                    recalculatedAvailability.AvailableQuantity,
                    recalculatedAvailability.NextExpirationUtc,
                    IntegrationEventNames.ReservationReleased,
                    reservation.Id,
                    releasedAtUtc));
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        await stockItemLock.CommitAsync(cancellationToken);

        return ApplicationResult<ReservationDto>.Success(reservation.ToDto());
    }
}
