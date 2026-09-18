namespace Inventory.Api.Application.Products;

public sealed record ProductDto(
    Guid Id,
    Guid VendorId,
    string Sku,
    string Name,
    DateTimeOffset CreatedAtUtc);
