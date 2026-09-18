using Inventory.Api.Api.Common;
using Inventory.Api.Application.Products.CreateProduct;
using Inventory.Api.Application.Products.GetProduct;
using Inventory.Api.Application.Products.ListProducts;
using MediatR;

namespace Inventory.Api.Api.Products;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/products")
            .WithTags("Products");

        group.MapGet("/", async (
            Guid? vendorId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var products = await sender.Send(new ListProductsQuery(vendorId), cancellationToken);

            return Results.Ok(new ProductListResponse(products));
        })
        .WithName("ListProducts");

        group.MapPost("/", async (
            CreateProductRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new CreateProductCommand(request.VendorId, request.Sku, request.Name),
                cancellationToken);

            return result.ToHttpResult(product =>
                Results.Created($"/products/{product.Id}", new ProductResponse(product)));
        })
        .WithName("CreateProduct");

        group.MapGet("/{productId:guid}", async (
            Guid productId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetProductQuery(productId), cancellationToken);

            return result.ToHttpResult(product => Results.Ok(new ProductResponse(product)));
        })
        .WithName("GetProduct");

        return endpoints;
    }
}
