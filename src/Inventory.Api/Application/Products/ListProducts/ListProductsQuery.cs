using MediatR;

namespace Inventory.Api.Application.Products.ListProducts;

public sealed record ListProductsQuery(Guid? VendorId) : IRequest<IReadOnlyList<ProductDto>>;
