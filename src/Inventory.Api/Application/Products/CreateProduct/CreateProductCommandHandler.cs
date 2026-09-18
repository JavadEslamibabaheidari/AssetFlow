using Inventory.Api.Application.Common;
using Inventory.Api.Domain.Entities;
using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Products.CreateProduct;

public sealed class CreateProductCommandHandler(InventoryDbContext dbContext)
    : IRequestHandler<CreateProductCommand, ApplicationResult<ProductDto>>
{
    public async Task<ApplicationResult<ProductDto>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var sku = request.Sku?.Trim();
        var name = request.Name?.Trim();

        if (request.VendorId == Guid.Empty)
        {
            return ApplicationResult<ProductDto>.Validation(
                "product.vendorIdRequired",
                "Vendor id is required.");
        }

        if (string.IsNullOrWhiteSpace(sku))
        {
            return ApplicationResult<ProductDto>.Validation(
                "product.skuRequired",
                "Product SKU is required.");
        }

        if (sku.Length > 80)
        {
            return ApplicationResult<ProductDto>.Validation(
                "product.skuTooLong",
                "Product SKU must be 80 characters or fewer.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return ApplicationResult<ProductDto>.Validation(
                "product.nameRequired",
                "Product name is required.");
        }

        if (name.Length > 240)
        {
            return ApplicationResult<ProductDto>.Validation(
                "product.nameTooLong",
                "Product name must be 240 characters or fewer.");
        }

        var vendorExists = await dbContext.Vendors
            .AnyAsync(vendor => vendor.Id == request.VendorId, cancellationToken);

        if (!vendorExists)
        {
            return ApplicationResult<ProductDto>.NotFound(
                "vendor.notFound",
                "Vendor was not found.");
        }

        var duplicateExists = await dbContext.Products
            .AnyAsync(
                product => product.VendorId == request.VendorId && product.Sku == sku,
                cancellationToken);

        if (duplicateExists)
        {
            return ApplicationResult<ProductDto>.Conflict(
                "product.duplicateSku",
                "A product with this SKU already exists for this vendor.");
        }

        var product = new Product(Guid.NewGuid(), request.VendorId, sku, name, DateTimeOffset.UtcNow);

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ApplicationResult<ProductDto>.Success(product.ToDto());
    }
}
