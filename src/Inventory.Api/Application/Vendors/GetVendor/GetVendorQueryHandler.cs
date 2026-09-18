using Inventory.Api.Application.Common;
using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Vendors.GetVendor;

public sealed class GetVendorQueryHandler(InventoryDbContext dbContext)
    : IRequestHandler<GetVendorQuery, ApplicationResult<VendorDto>>
{
    public async Task<ApplicationResult<VendorDto>> Handle(
        GetVendorQuery request,
        CancellationToken cancellationToken)
    {
        var vendor = await dbContext.Vendors
            .AsNoTracking()
            .Where(vendor => vendor.Id == request.VendorId)
            .Select(vendor => vendor.ToDto())
            .SingleOrDefaultAsync(cancellationToken);

        return vendor is null
            ? ApplicationResult<VendorDto>.NotFound(
                "vendor.notFound",
                "Vendor was not found.")
            : ApplicationResult<VendorDto>.Success(vendor);
    }
}
