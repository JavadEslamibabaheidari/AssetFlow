namespace Inventory.Api.Domain.Entities;

public sealed class Reservation
{
    private Reservation()
    {
    }

    public Reservation(
        Guid id,
        Guid stockItemId,
        int quantity,
        DateTimeOffset expiresAtUtc,
        DateTimeOffset createdAtUtc)
    {
        Id = id;
        StockItemId = stockItemId;
        Quantity = quantity;
        Status = ReservationStatus.Active;
        ExpiresAtUtc = expiresAtUtc;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid StockItemId { get; private set; }

    public int Quantity { get; private set; }

    public ReservationStatus Status { get; private set; }

    public DateTimeOffset ExpiresAtUtc { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public DateTimeOffset? ReleasedAtUtc { get; private set; }

    public DateTimeOffset? ExpiredAtUtc { get; private set; }

    public StockItem StockItem { get; private set; } = null!;

    public bool Release(DateTimeOffset releasedAtUtc)
    {
        if (Status == ReservationStatus.Released)
        {
            return false;
        }

        if (Status == ReservationStatus.Expired)
        {
            throw new InvalidOperationException("Expired reservations cannot be released.");
        }

        Status = ReservationStatus.Released;
        ReleasedAtUtc = releasedAtUtc;
        UpdatedAtUtc = releasedAtUtc;

        return true;
    }

    public bool Expire(DateTimeOffset expiredAtUtc)
    {
        if (Status != ReservationStatus.Active)
        {
            return false;
        }

        Status = ReservationStatus.Expired;
        ExpiredAtUtc = expiredAtUtc;
        UpdatedAtUtc = expiredAtUtc;

        return true;
    }
}
