using System.Net;
using System.Net.Http.Json;
using Inventory.Api.Infrastructure.ChannelSync;
using Inventory.Api.Infrastructure.Observability;
using Inventory.Api.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Api.Tests;

public sealed class ObservabilityEndpointTests
{
    [Fact]
    public async Task ObservabilityHealth_ReturnsOperationalSummaryAndCorrelationHeader()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        await SeedFailedSyncStateAsync(factory, client);
        using var request = new HttpRequestMessage(HttpMethod.Get, "/observability/health");
        request.Headers.Add(ObservabilityHttpContext.CorrelationIdHeaderName, "corr-test");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(
            response.Headers.TryGetValues(ObservabilityHttpContext.CorrelationIdHeaderName, out var headers));
        Assert.Equal("corr-test", Assert.Single(headers));
        var body = await response.Content.ReadFromJsonAsync<ObservabilityResponse>();

        Assert.NotNull(body);
        Assert.Equal("Degraded", body.Status);
        Assert.Equal("Inventory API", body.Service);
        Assert.Equal("corr-test", body.CorrelationId);
        Assert.False(string.IsNullOrWhiteSpace(body.TraceId));
        Assert.Equal(2, body.Outbox.TotalMessages);
        Assert.Equal(1, body.Outbox.PendingMessages);
        Assert.Equal(1, body.Outbox.FailedMessages);
        Assert.Equal(1, body.ChannelSync.TotalStates);
        Assert.Equal(1, body.ChannelSync.FailedStates);
        Assert.Equal(1, body.ChannelSync.RetryableFailures);
        Assert.NotNull(body.ChannelSync.NextRetryAtUtc);
    }

    [Fact]
    public async Task ObservabilityPrometheus_ReturnsPrometheusTextMetrics()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        await SeedFailedSyncStateAsync(factory, client);

        var response = await client.GetAsync("/observability/prometheus");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("# TYPE assetflow_health_status gauge", body);
        Assert.Contains("assetflow_health_status 0", body);
        Assert.Contains("assetflow_outbox_messages_total{status=\"Pending\"} 1", body);
        Assert.Contains("assetflow_outbox_messages_total{status=\"Failed\"} 1", body);
        Assert.Contains("assetflow_channel_sync_states_total{status=\"Failed\"} 1", body);
        Assert.Contains("assetflow_channel_sync_retryable_failures 1", body);
    }

    private static async Task SeedFailedSyncStateAsync(
        TestInventoryApiFactory factory,
        HttpClient client)
    {
        var vendor = await CreateVendorAsync(client, "Observability Vendor");
        var product = await CreateProductAsync(client, vendor.Id, "OBS-100", "Observed product");
        var channel = await CreateChannelAsync(client, "OBS", "Observed channel");
        await CreateStockItemAsync(client, product.Id, channel.Id, 7);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
        var processor = new ChannelSynchronizationProcessor(
            dbContext,
            new StaticChannelAvailabilitySyncAdapter(
                ChannelAvailabilitySyncResult.Failure(retryable: true, "Temporary outage")));

        var processed = await processor.ProcessDueAsync(10, DateTimeOffset.UtcNow, CancellationToken.None);

        Assert.Equal(1, processed);
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

    private static async Task CreateStockItemAsync(
        HttpClient client,
        Guid productId,
        Guid channelId,
        int onHandQuantity)
    {
        var response = await client.PostAsJsonAsync(
            "/stock-items",
            new CreateStockItemRequest(productId, channelId, onHandQuantity));
        response.EnsureSuccessStatusCode();
    }

    private sealed class StaticChannelAvailabilitySyncAdapter(
        ChannelAvailabilitySyncResult result) : IChannelAvailabilitySyncAdapter
    {
        public Task<ChannelAvailabilitySyncResult> SyncAvailabilityAsync(
            ChannelAvailabilitySyncRequest request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(result);
        }
    }

    private sealed record ObservabilityResponse(
        string Status,
        string Service,
        DateTimeOffset CheckedAtUtc,
        string TraceId,
        string CorrelationId,
        OutboxObservability Outbox,
        ChannelSyncObservability ChannelSync);

    private sealed record OutboxObservability(
        int TotalMessages,
        int PendingMessages,
        int ProcessingMessages,
        int PublishedMessages,
        int FailedMessages,
        DateTimeOffset? OldestPendingAtUtc,
        DateTimeOffset? NextAttemptAtUtc);

    private sealed record ChannelSyncObservability(
        int TotalStates,
        int PendingStates,
        int InProgressStates,
        int SucceededStates,
        int FailedStates,
        int RetryableFailures,
        DateTimeOffset? NextRetryAtUtc,
        DateTimeOffset? LastFailureAtUtc);

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
}
