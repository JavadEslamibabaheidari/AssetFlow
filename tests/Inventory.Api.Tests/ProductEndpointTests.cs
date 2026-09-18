using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Inventory.Api.Tests;

public sealed class ProductEndpointTests
{
    [Fact]
    public async Task CreateProduct_ReturnsCreatedProduct()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var vendor = await CreateVendorAsync(client, "Northwind Supply");

        var response = await client.PostAsJsonAsync(
            "/products",
            new CreateProductRequest(vendor.Id, "SKU-001", "Blue Jacket"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.Product.Id);
        Assert.Equal(vendor.Id, body.Product.VendorId);
        Assert.Equal("SKU-001", body.Product.Sku);
        Assert.Equal("Blue Jacket", body.Product.Name);
        Assert.NotEqual(default, body.Product.CreatedAtUtc);
        Assert.Equal($"/products/{body.Product.Id}", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task GetProduct_ReturnsCreatedProduct()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var vendor = await CreateVendorAsync(client, "Contoso Goods");
        var created = await CreateProductAsync(client, vendor.Id, "SKU-002", "Green Shirt");

        var response = await client.GetAsync($"/products/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(body);
        Assert.Equal(created.Id, body.Product.Id);
        Assert.Equal(vendor.Id, body.Product.VendorId);
        Assert.Equal("SKU-002", body.Product.Sku);
        Assert.Equal("Green Shirt", body.Product.Name);
    }

    [Fact]
    public async Task ListProducts_WithVendorFilter_ReturnsMatchingProductsNewestFirst()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var firstVendor = await CreateVendorAsync(client, "First Vendor");
        var secondVendor = await CreateVendorAsync(client, "Second Vendor");
        var firstProduct = await CreateProductAsync(client, firstVendor.Id, "SKU-003", "First Product");
        var secondProduct = await CreateProductAsync(client, firstVendor.Id, "SKU-004", "Second Product");
        await CreateProductAsync(client, secondVendor.Id, "SKU-005", "Other Vendor Product");

        var response = await client.GetAsync($"/products?vendorId={firstVendor.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProductListResponse>();

        Assert.NotNull(body);
        Assert.Equal([secondProduct.Id, firstProduct.Id], body.Items.Select(product => product.Id));
    }

    [Fact]
    public async Task CreateProduct_WithUnknownVendor_ReturnsNotFoundProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/products",
            new CreateProductRequest(Guid.NewGuid(), "SKU-006", "Unknown Vendor Product"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await AssertProblemCodeAsync(response, "vendor.notFound");
    }

    [Fact]
    public async Task CreateProduct_WithDuplicateSkuForSameVendor_ReturnsConflictProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var vendor = await CreateVendorAsync(client, "Duplicate SKU Vendor");

        await CreateProductAsync(client, vendor.Id, "SKU-007", "First Product");

        var response = await client.PostAsJsonAsync(
            "/products",
            new CreateProductRequest(vendor.Id, "SKU-007", "Second Product"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertProblemCodeAsync(response, "product.duplicateSku");
    }

    [Fact]
    public async Task CreateProduct_AllowsSameSkuForDifferentVendors()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var firstVendor = await CreateVendorAsync(client, "First SKU Vendor");
        var secondVendor = await CreateVendorAsync(client, "Second SKU Vendor");

        await CreateProductAsync(client, firstVendor.Id, "SKU-008", "First Product");

        var response = await client.PostAsJsonAsync(
            "/products",
            new CreateProductRequest(secondVendor.Id, "SKU-008", "Second Product"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithBlankSku_ReturnsBadRequestProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var vendor = await CreateVendorAsync(client, "Validation Vendor");

        var response = await client.PostAsJsonAsync(
            "/products",
            new CreateProductRequest(vendor.Id, " ", "Valid Name"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "product.skuRequired");
    }

    [Fact]
    public async Task GetProduct_WhenMissing_ReturnsNotFoundProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await AssertProblemCodeAsync(response, "product.notFound");
    }

    private static async Task<VendorDto> CreateVendorAsync(HttpClient client, string name)
    {
        var response = await client.PostAsJsonAsync("/vendors", new CreateVendorRequest(name));
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<VendorResponse>();

        Assert.NotNull(body);
        return body.Vendor;
    }

    private static async Task<ProductDto> CreateProductAsync(
        HttpClient client,
        Guid vendorId,
        string sku,
        string name)
    {
        var response = await client.PostAsJsonAsync("/products", new CreateProductRequest(vendorId, sku, name));
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(body);
        return body.Product;
    }

    private static async Task AssertProblemCodeAsync(HttpResponseMessage response, string expectedCode)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(expectedCode, document.RootElement.GetProperty("code").GetString());
    }

    private sealed record CreateVendorRequest(string Name);

    private sealed record VendorResponse(VendorDto Vendor);

    private sealed record VendorDto(Guid Id, string Name, DateTimeOffset CreatedAtUtc);

    private sealed record CreateProductRequest(Guid VendorId, string Sku, string Name);

    private sealed record ProductResponse(ProductDto Product);

    private sealed record ProductListResponse(IReadOnlyList<ProductDto> Items);

    private sealed record ProductDto(
        Guid Id,
        Guid VendorId,
        string Sku,
        string Name,
        DateTimeOffset CreatedAtUtc);
}
