using Inventory.Api.Domain.Entities;

namespace Inventory.Api.Tests;

public sealed class ReservationTests
{
    [Fact]
    public void Constructor_CreatesActiveReservation()
    {
        var createdAtUtc = new DateTimeOffset(2026, 9, 19, 10, 0, 0, TimeSpan.Zero);
        var expiresAtUtc = createdAtUtc.AddMinutes(30);

        var reservation = new Reservation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            2,
            expiresAtUtc,
            createdAtUtc);

        Assert.Equal(ReservationStatus.Active, reservation.Status);
        Assert.Equal(2, reservation.Quantity);
        Assert.Equal(expiresAtUtc, reservation.ExpiresAtUtc);
        Assert.Equal(createdAtUtc, reservation.CreatedAtUtc);
        Assert.Equal(createdAtUtc, reservation.UpdatedAtUtc);
        Assert.Null(reservation.ReleasedAtUtc);
        Assert.Null(reservation.ExpiredAtUtc);
    }

    [Fact]
    public void Release_WhenActive_MarksReservationReleased()
    {
        var reservation = CreateReservation();
        var releasedAtUtc = reservation.CreatedAtUtc.AddMinutes(5);

        var changed = reservation.Release(releasedAtUtc);

        Assert.True(changed);
        Assert.Equal(ReservationStatus.Released, reservation.Status);
        Assert.Equal(releasedAtUtc, reservation.ReleasedAtUtc);
        Assert.Equal(releasedAtUtc, reservation.UpdatedAtUtc);
        Assert.Null(reservation.ExpiredAtUtc);
    }

    [Fact]
    public void Release_WhenAlreadyReleased_IsIdempotent()
    {
        var reservation = CreateReservation();
        var releasedAtUtc = reservation.CreatedAtUtc.AddMinutes(5);
        reservation.Release(releasedAtUtc);

        var changed = reservation.Release(releasedAtUtc.AddMinutes(1));

        Assert.False(changed);
        Assert.Equal(ReservationStatus.Released, reservation.Status);
        Assert.Equal(releasedAtUtc, reservation.ReleasedAtUtc);
        Assert.Equal(releasedAtUtc, reservation.UpdatedAtUtc);
    }

    [Fact]
    public void Release_WhenExpired_Throws()
    {
        var reservation = CreateReservation();
        reservation.Expire(reservation.ExpiresAtUtc);

        var exception = Assert.Throws<InvalidOperationException>(
            () => reservation.Release(reservation.ExpiresAtUtc.AddMinutes(1)));

        Assert.Equal("Expired reservations cannot be released.", exception.Message);
    }

    [Fact]
    public void Expire_WhenActive_MarksReservationExpired()
    {
        var reservation = CreateReservation();
        var expiredAtUtc = reservation.ExpiresAtUtc;

        var changed = reservation.Expire(expiredAtUtc);

        Assert.True(changed);
        Assert.Equal(ReservationStatus.Expired, reservation.Status);
        Assert.Equal(expiredAtUtc, reservation.ExpiredAtUtc);
        Assert.Equal(expiredAtUtc, reservation.UpdatedAtUtc);
        Assert.Null(reservation.ReleasedAtUtc);
    }

    [Fact]
    public void Expire_WhenReleased_DoesNotChangeReservation()
    {
        var reservation = CreateReservation();
        var releasedAtUtc = reservation.CreatedAtUtc.AddMinutes(5);
        reservation.Release(releasedAtUtc);

        var changed = reservation.Expire(releasedAtUtc.AddMinutes(1));

        Assert.False(changed);
        Assert.Equal(ReservationStatus.Released, reservation.Status);
        Assert.Equal(releasedAtUtc, reservation.ReleasedAtUtc);
        Assert.Null(reservation.ExpiredAtUtc);
    }

    private static Reservation CreateReservation()
    {
        var createdAtUtc = new DateTimeOffset(2026, 9, 19, 10, 0, 0, TimeSpan.Zero);

        return new Reservation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            2,
            createdAtUtc.AddMinutes(30),
            createdAtUtc);
    }
}
