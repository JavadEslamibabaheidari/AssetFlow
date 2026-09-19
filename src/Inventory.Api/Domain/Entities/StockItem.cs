namespace Inventory.Api.Domain.Entities;

public sealed class StockItem
{
    private StockItem()
    {
    }

    public StockItem(
        Guid id,
        Guid productId,
        Guid channelId,
        int onHandQuantity,
        DateTimeOffset updatedAtUtc)
    {
        Id = id;
        ProductId = productId;
        ChannelId = channelId;
        OnHandQuantity = onHandQuantity;
        AvailableQuantity = onHandQuantity;
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid ProductId { get; private set; }

    public Guid ChannelId { get; private set; }

    public int OnHandQuantity { get; private set; }

    public int AvailableQuantity { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public Product Product { get; private set; } = null!;

    public SalesChannel Channel { get; private set; } = null!;

    public void UpdateAvailableQuantity(int availableQuantity, DateTimeOffset updatedAtUtc)
    {
        AvailableQuantity = availableQuantity;
        UpdatedAtUtc = updatedAtUtc;
    }
}
