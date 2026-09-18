using Inventory.Api.Application.Common;
using MediatR;

namespace Inventory.Api.Application.Vendors.GetVendor;

public sealed record GetVendorQuery(Guid VendorId) : IRequest<ApplicationResult<VendorDto>>;
