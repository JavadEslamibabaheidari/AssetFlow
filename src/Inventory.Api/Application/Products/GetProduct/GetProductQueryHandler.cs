using Inventory.Api.Application.Common;
using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Products.GetProduct;

public sealed class GetProductQueryHandler(InventoryDbContext dbContext)
    : IRequestHandler<GetProductQuery, ApplicationResult<ProductDto>>
{
    public async Task<ApplicationResult<ProductDto>> Handle(
        GetProductQuery request,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Where(product => product.Id == request.ProductId)
            .Select(product => product.ToDto())
            .SingleOrDefaultAsync(cancellationToken);

        return product is null
            ? ApplicationResult<ProductDto>.NotFound(
                "product.notFound",
                "Product was not found.")
            : ApplicationResult<ProductDto>.Success(product);
    }
}
