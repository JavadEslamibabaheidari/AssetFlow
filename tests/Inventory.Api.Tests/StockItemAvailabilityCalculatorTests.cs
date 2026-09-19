using Inventory.Api.Application.Availability;
using Inventory.Api.Domain.Entities;

namespace Inventory.Api.Tests;

public sealed class StockItemAvailabilityCalculatorTests
{
    [Fact]
    public void Calculate_UsesOnlyActiveUnexpiredReservations()
    {
        var stockItemId = Guid.NewGuid();
        var now = new DateTimeOffset(2026, 9, 19, 10, 0, 0, TimeSpan.Zero);
        ReservationAvailabilityInput[] reservations =
        [
            new(2, ReservationStatus.Active, now.AddMinutes(30)),
            new(3, ReservationStatus.Active, now.AddMinutes(10)),
            new(4, ReservationStatus.Active, now),
            new(5, ReservationStatus.Released, now.AddMinutes(5)),
            new(6, ReservationStatus.Expired, now.AddMinutes(5))
        ];

        var availability = StockItemAvailabilityCalculator.Calculate(
            stockItemId,
            10,
            reservations,
            now);

        Assert.Equal(stockItemId, availability.StockItemId);
        Assert.Equal(10, availability.OnHandQuantity);
        Assert.Equal(5, availability.ReservedQuantity);
        Assert.Equal(5, availability.AvailableQuantity);
        Assert.Equal(now.AddMinutes(10), availability.NextExpirationUtc);
    }

    [Fact]
    public void Calculate_WhenNoActiveUnexpiredReservations_ReturnsFullAvailability()
    {
        var stockItemId = Guid.NewGuid();
        var now = new DateTimeOffset(2026, 9, 19, 10, 0, 0, TimeSpan.Zero);
        ReservationAvailabilityInput[] reservations =
        [
            new(2, ReservationStatus.Active, now.AddTicks(-1)),
            new(3, ReservationStatus.Released, now.AddMinutes(20)),
            new(4, ReservationStatus.Expired, now.AddMinutes(20))
        ];

        var availability = StockItemAvailabilityCalculator.Calculate(
            stockItemId,
            8,
            reservations,
            now);

        Assert.Equal(0, availability.ReservedQuantity);
        Assert.Equal(8, availability.AvailableQuantity);
        Assert.Null(availability.NextExpirationUtc);
    }
}
