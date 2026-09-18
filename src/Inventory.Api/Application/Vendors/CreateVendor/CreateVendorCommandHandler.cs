using Inventory.Api.Application.Common;
using Inventory.Api.Domain.Entities;
using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Vendors.CreateVendor;

public sealed class CreateVendorCommandHandler(InventoryDbContext dbContext)
    : IRequestHandler<CreateVendorCommand, ApplicationResult<VendorDto>>
{
    public async Task<ApplicationResult<VendorDto>> Handle(
        CreateVendorCommand request,
        CancellationToken cancellationToken)
    {
        var name = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            return ApplicationResult<VendorDto>.Validation(
                "vendor.nameRequired",
                "Vendor name is required.");
        }

        if (name.Length > 160)
        {
            return ApplicationResult<VendorDto>.Validation(
                "vendor.nameTooLong",
                "Vendor name must be 160 characters or fewer.");
        }

        var duplicateExists = await dbContext.Vendors
            .AnyAsync(vendor => vendor.Name == name, cancellationToken);

        if (duplicateExists)
        {
            return ApplicationResult<VendorDto>.Conflict(
                "vendor.duplicateName",
                "A vendor with this name already exists.");
        }

        var vendor = new Vendor(Guid.NewGuid(), name, DateTimeOffset.UtcNow);

        dbContext.Vendors.Add(vendor);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ApplicationResult<VendorDto>.Success(vendor.ToDto());
    }
}
