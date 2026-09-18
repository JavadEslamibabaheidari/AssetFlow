using Inventory.Api.Application.Common;
using MediatR;

namespace Inventory.Api.Application.Products.CreateProduct;

public sealed record CreateProductCommand(Guid VendorId, string? Sku, string? Name)
    : IRequest<ApplicationResult<ProductDto>>;
