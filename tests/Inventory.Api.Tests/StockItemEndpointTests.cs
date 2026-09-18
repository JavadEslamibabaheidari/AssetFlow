using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Inventory.Api.Tests;

public sealed class StockItemEndpointTests
{
    [Fact]
    public async Task CreateStockItem_ReturnsCreatedStockItem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var product = await CreateProductWithVendorAsync(client, "SKU-100", "Winter Coat");
        var channel = await CreateChannelAsync(client, "SHOPIFY", "Shopify");

        var response = await client.PostAsJsonAsync(
            "/stock-items",
            new CreateStockItemRequest(product.Id, channel.Id, 15));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<StockItemResponse>();

        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.StockItem.Id);
        Assert.Equal(product.Id, body.StockItem.ProductId);
        Assert.Equal(channel.Id, body.StockItem.ChannelId);
        Assert.Equal(15, body.StockItem.OnHandQuantity);
        Assert.Equal(15, body.StockItem.AvailableQuantity);
        Assert.NotEqual(default, body.StockItem.UpdatedAtUtc);
        Assert.Equal($"/stock-items/{body.StockItem.Id}", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task GetStockItem_ReturnsCreatedStockItem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var product = await CreateProductWithVendorAsync(client, "SKU-101", "Hat");
        var channel = await CreateChannelAsync(client, "AMAZON", "Amazon");
        var created = await CreateStockItemAsync(client, product.Id, channel.Id, 7);

        var response = await client.GetAsync($"/stock-items/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<StockItemResponse>();

        Assert.NotNull(body);
        Assert.Equal(created.Id, body.StockItem.Id);
        Assert.Equal(product.Id, body.StockItem.ProductId);
        Assert.Equal(channel.Id, body.StockItem.ChannelId);
        Assert.Equal(7, body.StockItem.OnHandQuantity);
        Assert.Equal(7, body.StockItem.AvailableQuantity);
    }

    [Fact]
    public async Task ListStockItems_WithProductAndChannelFilters_ReturnsMatchingItems()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var firstProduct = await CreateProductWithVendorAsync(client, "SKU-102", "Shoes");
        var secondProduct = await CreateProductWithVendorAsync(client, "SKU-103", "Bag");
        var firstChannel = await CreateChannelAsync(client, "MEDIAWORLD", "MediaWorld");
        var secondChannel = await CreateChannelAsync(client, "UNIEURO", "Unieuro");
        var expected = await CreateStockItemAsync(client, firstProduct.Id, firstChannel.Id, 4);
        await CreateStockItemAsync(client, firstProduct.Id, secondChannel.Id, 5);
        await CreateStockItemAsync(client, secondProduct.Id, firstChannel.Id, 6);

        var response = await client.GetAsync(
            $"/stock-items?productId={firstProduct.Id}&channelId={firstChannel.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<StockItemListResponse>();

        Assert.NotNull(body);
        Assert.Equal([expected.Id], body.Items.Select(stockItem => stockItem.Id));
    }

    [Fact]
    public async Task CreateStockItem_WithUnknownProduct_ReturnsNotFoundProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var channel = await CreateChannelAsync(client, "CUSTOM", "Custom");

        var response = await client.PostAsJsonAsync(
            "/stock-items",
            new CreateStockItemRequest(Guid.NewGuid(), channel.Id, 1));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await AssertProblemCodeAsync(response, "product.notFound");
    }

    [Fact]
    public async Task CreateStockItem_WithUnknownChannel_ReturnsNotFoundProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var product = await CreateProductWithVendorAsync(client, "SKU-104", "Gloves");

        var response = await client.PostAsJsonAsync(
            "/stock-items",
            new CreateStockItemRequest(product.Id, Guid.NewGuid(), 1));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await AssertProblemCodeAsync(response, "channel.notFound");
    }

    [Fact]
    public async Task CreateStockItem_WithDuplicateProductChannel_ReturnsConflictProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var product = await CreateProductWithVendorAsync(client, "SKU-105", "Belt");
        var channel = await CreateChannelAsync(client, "MARKET", "Market");

        await CreateStockItemAsync(client, product.Id, channel.Id, 3);

        var response = await client.PostAsJsonAsync(
            "/stock-items",
            new CreateStockItemRequest(product.Id, channel.Id, 9));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertProblemCodeAsync(response, "stockItem.duplicateProductChannel");
    }

    [Fact]
    public async Task CreateStockItem_WithNegativeQuantity_ReturnsBadRequestProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var product = await CreateProductWithVendorAsync(client, "SKU-106", "Scarf");
        var channel = await CreateChannelAsync(client, "STORE", "Store");

        var response = await client.PostAsJsonAsync(
            "/stock-items",
            new CreateStockItemRequest(product.Id, channel.Id, -1));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "stockItem.onHandQuantityNegative");
    }

    [Fact]
    public async Task GetStockItem_WhenMissing_ReturnsNotFoundProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/stock-items/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await AssertProblemCodeAsync(response, "stockItem.notFound");
    }

    private static async Task<ProductDto> CreateProductWithVendorAsync(
        HttpClient client,
        string sku,
        string name)
    {
        var vendor = await CreateVendorAsync(client, $"Vendor {sku}");
        return await CreateProductAsync(client, vendor.Id, sku, name);
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

    private static async Task<ChannelDto> CreateChannelAsync(HttpClient client, string code, string name)
    {
        var response = await client.PostAsJsonAsync("/channels", new CreateChannelRequest(code, name));
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<ChannelResponse>();

        Assert.NotNull(body);
        return body.Channel;
    }

    private static async Task<StockItemDto> CreateStockItemAsync(
        HttpClient client,
        Guid productId,
        Guid channelId,
        int onHandQuantity)
    {
        var response = await client.PostAsJsonAsync(
            "/stock-items",
            new CreateStockItemRequest(productId, channelId, onHandQuantity));
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<StockItemResponse>();

        Assert.NotNull(body);
        return body.StockItem;
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

    private sealed record ProductDto(
        Guid Id,
        Guid VendorId,
        string Sku,
        string Name,
        DateTimeOffset CreatedAtUtc);

    private sealed record CreateChannelRequest(string Code, string Name);

    private sealed record ChannelResponse(ChannelDto Channel);

    private sealed record ChannelDto(Guid Id, string Code, string Name, DateTimeOffset CreatedAtUtc);

    private sealed record CreateStockItemRequest(Guid ProductId, Guid ChannelId, int OnHandQuantity);

    private sealed record StockItemResponse(StockItemDto StockItem);

    private sealed record StockItemListResponse(IReadOnlyList<StockItemDto> Items);

    private sealed record StockItemDto(
        Guid Id,
        Guid ProductId,
        Guid ChannelId,
        int OnHandQuantity,
        int AvailableQuantity,
        DateTimeOffset UpdatedAtUtc);
}
