using System.Net.Http.Json;
using Inventory.Api.Infrastructure.ChannelSync;
using Inventory.Api.Infrastructure.Persistence;
using Inventory.Api.Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Api.Tests;

public sealed class ChannelSynchronizationProcessorTests
{
    [Fact]
    public async Task ProcessDueAsync_WhenAdapterSucceeds_PublishesAvailabilityMessageAndRecordsSyncState()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var stockItem = await CreateStockItemWithProductAndChannelAsync(client, "SYNC-100", "SYNC-SUCCESS", 9);
        var adapter = new RecordingChannelAvailabilitySyncAdapter(ChannelAvailabilitySyncResult.Success());
        var nowUtc = DateTimeOffset.UtcNow;

        var processed = await ProcessDueAsync(factory, adapter, nowUtc);

        Assert.Equal(1, processed);
        var request = Assert.Single(adapter.Requests);
        Assert.Equal(stockItem.Id, request.StockItemId);
        Assert.Equal(stockItem.ChannelId, request.ChannelId);
        Assert.Equal(9, request.AvailableQuantity);

        var messages = await ReadOutboxMessagesAsync(factory);
        Assert.Contains(messages, message =>
            message.EventType == "StockItemCreated" &&
            message.Status == OutboxMessageStatus.Pending);
        var availabilityMessage = Assert.Single(
            messages,
            message => message.EventType == "StockAvailabilityChanged");
        Assert.Equal(OutboxMessageStatus.Published, availabilityMessage.Status);
        Assert.Equal(1, availabilityMessage.AttemptCount);
        Assert.Null(availabilityMessage.LastError);
        Assert.Null(availabilityMessage.NextAttemptAtUtc);
        Assert.Equal(nowUtc, availabilityMessage.ProcessedAtUtc);

        var syncState = await ReadSyncStateAsync(factory);
        Assert.Equal(ChannelSyncStatus.Succeeded, syncState.Status);
        Assert.Equal(stockItem.ChannelId, syncState.ChannelId);
        Assert.Equal(stockItem.Id, syncState.StockItemId);
        Assert.Equal(availabilityMessage.Id, syncState.SourceEventId);
        Assert.Equal(9, syncState.AvailableQuantity);
        Assert.Equal(1, syncState.AttemptCount);
        Assert.Equal(nowUtc, syncState.LastAttemptedAtUtc);
        Assert.Equal(nowUtc, syncState.LastSucceededAtUtc);
        Assert.Null(syncState.LastError);

        var secondProcessed = await ProcessDueAsync(factory, adapter, nowUtc.AddMinutes(1));

        Assert.Equal(0, secondProcessed);
        Assert.Single(adapter.Requests);
    }

    [Fact]
    public async Task ProcessDueAsync_WhenAdapterFails_RecordsRetryAndPreservesSourceEvent()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var stockItem = await CreateStockItemWithProductAndChannelAsync(client, "SYNC-101", "SYNC-RETRY", 4);
        var adapter = new RecordingChannelAvailabilitySyncAdapter(
            ChannelAvailabilitySyncResult.Failure(retryable: true, "Temporary channel outage"));
        var firstAttemptUtc = DateTimeOffset.UtcNow;

        var firstProcessed = await ProcessDueAsync(factory, adapter, firstAttemptUtc);

        Assert.Equal(1, firstProcessed);
        var messageAfterFailure = Assert.Single(
            await ReadOutboxMessagesAsync(factory),
            message => message.EventType == "StockAvailabilityChanged");
        Assert.Equal(OutboxMessageStatus.Failed, messageAfterFailure.Status);
        Assert.Equal(1, messageAfterFailure.AttemptCount);
        Assert.Equal("Temporary channel outage", messageAfterFailure.LastError);
        Assert.Equal(firstAttemptUtc.AddMinutes(5), messageAfterFailure.NextAttemptAtUtc);
        Assert.Null(messageAfterFailure.ProcessedAtUtc);

        var stateAfterFailure = await ReadSyncStateAsync(factory);
        Assert.Equal(ChannelSyncStatus.Failed, stateAfterFailure.Status);
        Assert.Equal(stockItem.Id, stateAfterFailure.StockItemId);
        Assert.Equal(1, stateAfterFailure.AttemptCount);
        Assert.Equal("Temporary channel outage", stateAfterFailure.LastError);
        Assert.Equal(firstAttemptUtc.AddMinutes(5), stateAfterFailure.NextAttemptAtUtc);

        var beforeRetryProcessed = await ProcessDueAsync(
            factory,
            adapter,
            firstAttemptUtc.AddMinutes(4));

        Assert.Equal(0, beforeRetryProcessed);
        Assert.Single(adapter.Requests);

        adapter.NextResult = ChannelAvailabilitySyncResult.Success();
        var retryUtc = firstAttemptUtc.AddMinutes(5);

        var retryProcessed = await ProcessDueAsync(factory, adapter, retryUtc);

        Assert.Equal(1, retryProcessed);
        Assert.Equal(2, adapter.Requests.Count);
        var messageAfterRetry = Assert.Single(
            await ReadOutboxMessagesAsync(factory),
            message => message.EventType == "StockAvailabilityChanged");
        Assert.Equal(messageAfterFailure.Id, messageAfterRetry.Id);
        Assert.Equal(OutboxMessageStatus.Published, messageAfterRetry.Status);
        Assert.Equal(2, messageAfterRetry.AttemptCount);
        Assert.Null(messageAfterRetry.LastError);
        Assert.Null(messageAfterRetry.NextAttemptAtUtc);
        Assert.Equal(retryUtc, messageAfterRetry.ProcessedAtUtc);

        var stateAfterRetry = await ReadSyncStateAsync(factory);
        Assert.Equal(ChannelSyncStatus.Succeeded, stateAfterRetry.Status);
        Assert.Equal(2, stateAfterRetry.AttemptCount);
        Assert.Equal(retryUtc, stateAfterRetry.LastSucceededAtUtc);
        Assert.Null(stateAfterRetry.LastError);
    }

    [Fact]
    public async Task ProcessDueAsync_WhenAdapterThrows_RecordsRetryableFailure()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var stockItem = await CreateStockItemWithProductAndChannelAsync(client, "SYNC-102", "SYNC-THROW", 6);
        var adapter = new ThrowingChannelAvailabilitySyncAdapter();
        var nowUtc = DateTimeOffset.UtcNow;

        var processed = await ProcessDueAsync(factory, adapter, nowUtc);

        Assert.Equal(1, processed);
        var message = Assert.Single(
            await ReadOutboxMessagesAsync(factory),
            message => message.EventType == "StockAvailabilityChanged");
        Assert.Equal(OutboxMessageStatus.Failed, message.Status);
        Assert.Equal(1, message.AttemptCount);
        Assert.Equal("Provider timeout", message.LastError);
        Assert.Equal(nowUtc.AddMinutes(5), message.NextAttemptAtUtc);

        var syncState = await ReadSyncStateAsync(factory);
        Assert.Equal(ChannelSyncStatus.Failed, syncState.Status);
        Assert.Equal(stockItem.Id, syncState.StockItemId);
        Assert.Equal("Provider timeout", syncState.LastError);
        Assert.Equal(nowUtc.AddMinutes(5), syncState.NextAttemptAtUtc);
    }

    private static async Task<int> ProcessDueAsync(
        TestInventoryApiFactory factory,
        IChannelAvailabilitySyncAdapter adapter,
        DateTimeOffset nowUtc)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
        var processor = new ChannelSynchronizationProcessor(dbContext, adapter);

        return await processor.ProcessDueAsync(10, nowUtc, CancellationToken.None);
    }

    private static async Task<IReadOnlyList<OutboxMessage>> ReadOutboxMessagesAsync(
        TestInventoryApiFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

        return await dbContext.OutboxMessages
            .OrderBy(message => message.CreatedAtUtc)
            .ToArrayAsync();
    }

    private static async Task<ChannelSyncState> ReadSyncStateAsync(TestInventoryApiFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

        return await dbContext.ChannelSyncStates.SingleAsync();
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

    private sealed class RecordingChannelAvailabilitySyncAdapter(
        ChannelAvailabilitySyncResult initialResult) : IChannelAvailabilitySyncAdapter
    {
        public List<ChannelAvailabilitySyncRequest> Requests { get; } = [];

        public ChannelAvailabilitySyncResult NextResult { get; set; } = initialResult;

        public Task<ChannelAvailabilitySyncResult> SyncAvailabilityAsync(
            ChannelAvailabilitySyncRequest request,
            CancellationToken cancellationToken)
        {
            Requests.Add(request);

            return Task.FromResult(NextResult);
        }
    }

    private sealed class ThrowingChannelAvailabilitySyncAdapter : IChannelAvailabilitySyncAdapter
    {
        public Task<ChannelAvailabilitySyncResult> SyncAvailabilityAsync(
            ChannelAvailabilitySyncRequest request,
            CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Provider timeout");
        }
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

    private sealed record StockItemDto(
        Guid Id,
        Guid ProductId,
        Guid ChannelId,
        int OnHandQuantity,
        int AvailableQuantity,
        DateTimeOffset UpdatedAtUtc);
}
