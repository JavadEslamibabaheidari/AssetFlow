using Inventory.Api.Domain.Entities;

namespace Inventory.Api.Application.Products;

public static class ProductMapping
{
    public static ProductDto ToDto(this Product product) =>
        new(product.Id, product.VendorId, product.Sku, product.Name, product.CreatedAtUtc);
}
