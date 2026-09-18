using Inventory.Api.Application.Vendors;

namespace Inventory.Api.Api.Vendors;

public sealed record CreateVendorRequest(string? Name);

public sealed record VendorResponse(VendorDto Vendor);

public sealed record VendorListResponse(IReadOnlyList<VendorDto> Items);
