using Inventory.Api.Api.Common;
using Inventory.Api.Application.Vendors.CreateVendor;
using Inventory.Api.Application.Vendors.GetVendor;
using Inventory.Api.Application.Vendors.ListVendors;
using MediatR;

namespace Inventory.Api.Api.Vendors;

public static class VendorEndpoints
{
    public static IEndpointRouteBuilder MapVendorEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/vendors")
            .WithTags("Vendors");

        group.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var vendors = await sender.Send(new ListVendorsQuery(), cancellationToken);

            return Results.Ok(new VendorListResponse(vendors));
        })
        .WithName("ListVendors");

        group.MapPost("/", async (
            CreateVendorRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new CreateVendorCommand(request.Name), cancellationToken);

            return result.ToHttpResult(vendor =>
                Results.Created($"/vendors/{vendor.Id}", new VendorResponse(vendor)));
        })
        .WithName("CreateVendor");

        group.MapGet("/{vendorId:guid}", async (
            Guid vendorId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetVendorQuery(vendorId), cancellationToken);

            return result.ToHttpResult(vendor => Results.Ok(new VendorResponse(vendor)));
        })
        .WithName("GetVendor");

        return endpoints;
    }
}
