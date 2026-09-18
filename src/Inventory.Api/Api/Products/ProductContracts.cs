using Inventory.Api.Application.Products;

namespace Inventory.Api.Api.Products;

public sealed record CreateProductRequest(Guid VendorId, string? Sku, string? Name);

public sealed record ProductResponse(ProductDto Product);

public sealed record ProductListResponse(IReadOnlyList<ProductDto> Items);
