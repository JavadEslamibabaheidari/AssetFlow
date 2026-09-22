using Inventory.Api.Application.Availability;
using Inventory.Api.Application.Common;
using Inventory.Api.Application.Events;
using Inventory.Api.Domain.Entities;
using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Reservations.CreateReservation;

public sealed class CreateReservationCommandHandler(
    InventoryDbContext dbContext,
    IIntegrationEventOutbox outbox)
    : IRequestHandler<CreateReservationCommand, ApplicationResult<ReservationDto>>
{
    public async Task<ApplicationResult<ReservationDto>> Handle(
        CreateReservationCommand request,
        CancellationToken cancellationToken)
    {
        var nowUtc = DateTimeOffset.UtcNow;

        if (request.StockItemId == Guid.Empty)
        {
            return ApplicationResult<ReservationDto>.Validation(
                "reservation.stockItemIdRequired",
                "Stock item id is required.");
        }

        if (request.Quantity <= 0)
        {
            return ApplicationResult<ReservationDto>.Validation(
                "reservation.quantityNotPositive",
                "Reservation quantity must be greater than zero.");
        }

        if (request.ExpiresAtUtc <= nowUtc)
        {
            return ApplicationResult<ReservationDto>.Validation(
                "reservation.expirationNotFuture",
                "Reservation expiration must be in the future.");
        }

        await using var stockItemLock = await StockItemLockTransaction.BeginAsync(
            dbContext,
            [request.StockItemId],
            cancellationToken);

        var stockItem = await dbContext.StockItems
            .AsNoTracking()
            .SingleOrDefaultAsync(stockItem => stockItem.Id == request.StockItemId, cancellationToken);

        if (stockItem is null)
        {
            return ApplicationResult<ReservationDto>.NotFound(
                "stockItem.notFound",
                "Stock item was not found.");
        }

        var reservations = await dbContext.Reservations
            .AsNoTracking()
            .Where(reservation => reservation.StockItemId == request.StockItemId)
            .Select(reservation => new ReservationAvailabilityInput(
                reservation.Quantity,
                reservation.Status,
                reservation.ExpiresAtUtc))
            .ToArrayAsync(cancellationToken);

        var availability = StockItemAvailabilityCalculator.Calculate(
            stockItem.Id,
            stockItem.OnHandQuantity,
            reservations,
            nowUtc);

        if (request.Quantity > availability.AvailableQuantity)
        {
            return ApplicationResult<ReservationDto>.Conflict(
                "reservation.insufficientAvailability",
                "Reservation quantity exceeds current availability.");
        }

        var reservation = new Reservation(
            Guid.NewGuid(),
            request.StockItemId,
            request.Quantity,
            request.ExpiresAtUtc,
            nowUtc);

        dbContext.Reservations.Add(reservation);
        await dbContext.SaveChangesAsync(cancellationToken);

        var recalculatedAvailability = await StockItemAvailabilityStore.RecalculateAsync(
            dbContext,
            request.StockItemId,
            nowUtc,
            cancellationToken);
        outbox.Enqueue(IntegrationEvents.ReservationCreated(
            reservation.Id,
            reservation.StockItemId,
            reservation.Quantity,
            reservation.ExpiresAtUtc,
            reservation.Status.ToString(),
            reservation.CreatedAtUtc));
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
                IntegrationEventNames.ReservationCreated,
                reservation.Id,
                nowUtc));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await stockItemLock.CommitAsync(cancellationToken);

        return ApplicationResult<ReservationDto>.Success(reservation.ToDto());
    }
}
