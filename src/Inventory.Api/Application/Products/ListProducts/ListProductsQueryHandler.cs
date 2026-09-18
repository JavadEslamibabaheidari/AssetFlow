using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Products.ListProducts;

public sealed class ListProductsQueryHandler(InventoryDbContext dbContext)
    : IRequestHandler<ListProductsQuery, IReadOnlyList<ProductDto>>
{
    public async Task<IReadOnlyList<ProductDto>> Handle(
        ListProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = dbContext.Products.AsNoTracking();

        if (request.VendorId is { } vendorId)
        {
            products = products.Where(product => product.VendorId == vendorId);
        }

        return await products
            .OrderByDescending(product => product.CreatedAtUtc)
            .Select(product => product.ToDto())
            .ToListAsync(cancellationToken);
    }
}
