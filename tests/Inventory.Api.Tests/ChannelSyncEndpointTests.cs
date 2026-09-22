using System.Net;
using System.Net.Http.Json;
using Inventory.Api.Infrastructure.ChannelSync;
using Inventory.Api.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Api.Tests;

public sealed class ChannelSyncEndpointTests
{
    [Fact]
    public async Task ListChannelSyncStatuses_WhenNoSyncState_ReturnsEmptyList()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/channel-sync/status");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ChannelSyncStatusListResponse>();

        Assert.NotNull(body);
        Assert.Empty(body.Items);
    }

    [Fact]
    public async Task ListChannelSyncStatuses_ReturnsLatestSyncState()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var stockItem = await CreateStockItemWithProductAndChannelAsync(client, "SYNC-API-100", "SYNC-API", 6);
        var nowUtc = DateTimeOffset.UtcNow;
        await ProcessDueAsync(
            factory,
            new FailingChannelAvailabilitySyncAdapter(),
            nowUtc);

        var response = await client.GetAsync("/channel-sync/status");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ChannelSyncStatusListResponse>();

        Assert.NotNull(body);
        var item = Assert.Single(body.Items);
        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal(stockItem.ChannelId, item.ChannelId);
        Assert.Equal(stockItem.Id, item.StockItemId);
        Assert.Equal(6, item.AvailableQuantity);
        Assert.Equal("Failed", item.Status);
        Assert.Equal(1, item.AttemptCount);
        Assert.Equal(nowUtc, item.LastAttemptedAtUtc);
        Assert.Equal(nowUtc.AddMinutes(5), item.NextAttemptAtUtc);
        Assert.Null(item.LastSucceededAtUtc);
        Assert.Equal("Marketplace unavailable", item.LastError);
        Assert.Equal(nowUtc, item.UpdatedAtUtc);
    }

    private static async Task ProcessDueAsync(
        TestInventoryApiFactory factory,
        IChannelAvailabilitySyncAdapter adapter,
        DateTimeOffset nowUtc)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
        var processor = new ChannelSynchronizationProcessor(dbContext, adapter);

        await processor.ProcessDueAsync(10, nowUtc, CancellationToken.None);
    }

    private static async Task<StockItemDto> CreateStockItemWithProductAndChannelAsync(
        HttpClient client,
        string sku,
        string channelCode,
        int onHandQuantity)
    {
        var vendor = await CreateVendorAsync(client, $"Vendor {sku}");
        var product = await CreateProductAsync(client, vendor.Id, sku, $"Product {sku}");
        var channel = await CreateChannelAsync(client, channelCode, channelCode);

        return await CreateStockItemAsync(client, product.Id, channel.Id, onHandQuantity);
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

    private sealed class FailingChannelAvailabilitySyncAdapter : IChannelAvailabilitySyncAdapter
    {
        public Task<ChannelAvailabilitySyncResult> SyncAvailabilityAsync(
            ChannelAvailabilitySyncRequest request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                ChannelAvailabilitySyncResult.Failure(retryable: true, "Marketplace unavailable"));
        }
    }

    private sealed record ChannelSyncStatusListResponse(IReadOnlyList<ChannelSyncStatusDto> Items);

    private sealed record ChannelSyncStatusDto(
        Guid Id,
        Guid ChannelId,
        Guid StockItemId,
        Guid SourceEventId,
        int AvailableQuantity,
        string Status,
        int AttemptCount,
        DateTimeOffset? LastAttemptedAtUtc,
        DateTimeOffset? NextAttemptAtUtc,
        DateTimeOffset? LastSucceededAtUtc,
        string? LastError,
        DateTimeOffset UpdatedAtUtc);

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

    private sealed record StockItemDto(
        Guid Id,
        Guid ProductId,
        Guid ChannelId,
        int OnHandQuantity,
        int AvailableQuantity,
        DateTimeOffset UpdatedAtUtc);
}
