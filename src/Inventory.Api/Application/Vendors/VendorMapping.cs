using Inventory.Api.Domain.Entities;

namespace Inventory.Api.Application.Vendors;

public static class VendorMapping
{
    public static VendorDto ToDto(this Vendor vendor) =>
        new(vendor.Id, vendor.Name, vendor.CreatedAtUtc);
}
