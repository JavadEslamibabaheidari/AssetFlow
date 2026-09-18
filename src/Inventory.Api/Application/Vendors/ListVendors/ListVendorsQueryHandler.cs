using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Vendors.ListVendors;

public sealed class ListVendorsQueryHandler(InventoryDbContext dbContext)
    : IRequestHandler<ListVendorsQuery, IReadOnlyList<VendorDto>>
{
    public async Task<IReadOnlyList<VendorDto>> Handle(
        ListVendorsQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Vendors
            .AsNoTracking()
            .OrderByDescending(vendor => vendor.CreatedAtUtc)
            .Select(vendor => vendor.ToDto())
            .ToListAsync(cancellationToken);
    }
}
