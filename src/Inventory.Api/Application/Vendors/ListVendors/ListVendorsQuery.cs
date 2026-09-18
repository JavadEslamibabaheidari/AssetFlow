using MediatR;

namespace Inventory.Api.Application.Vendors.ListVendors;

public sealed record ListVendorsQuery : IRequest<IReadOnlyList<VendorDto>>;
