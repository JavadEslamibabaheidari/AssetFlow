namespace Inventory.Api.Domain.Entities;

public sealed class Product
{
    private Product()
    {
    }

    public Product(Guid id, Guid vendorId, string sku, string name, DateTimeOffset createdAtUtc)
    {
        Id = id;
        VendorId = vendorId;
        Sku = sku;
        Name = name;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid VendorId { get; private set; }

    public string Sku { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public Vendor Vendor { get; private set; } = null!;
}
