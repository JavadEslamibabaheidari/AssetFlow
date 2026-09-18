using Inventory.Api.Application.Common;
using MediatR;

namespace Inventory.Api.Application.Vendors.CreateVendor;

public sealed record CreateVendorCommand(string? Name) : IRequest<ApplicationResult<VendorDto>>;
