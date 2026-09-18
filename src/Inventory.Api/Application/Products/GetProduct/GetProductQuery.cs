using Inventory.Api.Application.Common;
using MediatR;

namespace Inventory.Api.Application.Products.GetProduct;

public sealed record GetProductQuery(Guid ProductId) : IRequest<ApplicationResult<ProductDto>>;
