using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Inventory.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Api.Tests;

public sealed class ReservationEndpointTests
{
    [Fact]
    public async Task CreateReservation_ReturnsCreatedActiveReservation()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var stockItem = await CreateStockItemWithProductAndChannelAsync(client, "RES-100", "SHOPIFY", 10);
        var expiresAtUtc = DateTimeOffset.UtcNow.AddHours(1);

        var response = await client.PostAsJsonAsync(
            "/reservations",
            new CreateReservationRequest(stockItem.Id, 3, expiresAtUtc));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ReservationResponse>();

        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.Reservation.Id);
        Assert.Equal(stockItem.Id, body.Reservation.StockItemId);
        Assert.Equal(3, body.Reservation.Quantity);
        Assert.Equal("Active", body.Reservation.Status);
        Assert.Equal(expiresAtUtc, body.Reservation.ExpiresAtUtc);
        Assert.NotEqual(default, body.Reservation.CreatedAtUtc);
        Assert.Equal(body.Reservation.CreatedAtUtc, body.Reservation.UpdatedAtUtc);
        Assert.Null(body.Reservation.ReleasedAtUtc);
        Assert.Null(body.Reservation.ExpiredAtUtc);
        Assert.Equal($"/reservations/{body.Reservation.Id}", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task GetReservation_ReturnsCreatedReservation()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var stockItem = await CreateStockItemWithProductAndChannelAsync(client, "RES-101", "AMAZON", 7);
        var created = await CreateReservationAsync(client, stockItem.Id, 2);

        var response = await client.GetAsync($"/reservations/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ReservationResponse>();

        Assert.NotNull(body);
        Assert.Equal(created.Id, body.Reservation.Id);
        Assert.Equal(stockItem.Id, body.Reservation.StockItemId);
        Assert.Equal(2, body.Reservation.Quantity);
        Assert.Equal("Active", body.Reservation.Status);
    }

    [Fact]
    public async Task ListReservations_WithStockItemAndStatusFilters_ReturnsMatchingReservations()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var firstStockItem = await CreateStockItemWithProductAndChannelAsync(client, "RES-102", "MEDIAWORLD", 10);
        var secondStockItem = await CreateStockItemWithProductAndChannelAsync(client, "RES-103", "UNIEURO", 10);
        var expected = await CreateReservationAsync(client, firstStockItem.Id, 2);
        await CreateReservationAsync(client, firstStockItem.Id, 1);
        await CreateReservationAsync(client, secondStockItem.Id, 3);

        var response = await client.GetAsync($"/reservations?stockItemId={firstStockItem.Id}&status=Active");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ReservationListResponse>();

        Assert.NotNull(body);
        Assert.Contains(body.Items, reservation => reservation.Id == expected.Id);
        Assert.All(body.Items, reservation =>
        {
            Assert.Equal(firstStockItem.Id, reservation.StockItemId);
            Assert.Equal("Active", reservation.Status);
        });
    }

    [Fact]
    public async Task ListReservations_WithInvalidStatus_ReturnsBadRequestProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/reservations?status=Held");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "reservation.statusInvalid");
    }

    [Fact]
    public async Task CreateReservation_WithUnknownStockItem_ReturnsNotFoundProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/reservations",
            new CreateReservationRequest(Guid.NewGuid(), 1, DateTimeOffset.UtcNow.AddHours(1)));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await AssertProblemCodeAsync(response, "stockItem.notFound");
    }

    [Fact]
    public async Task CreateReservation_WithNonPositiveQuantity_ReturnsBadRequestProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var stockItem = await CreateStockItemWithProductAndChannelAsync(client, "RES-104", "CUSTOM", 5);

        var response = await client.PostAsJsonAsync(
            "/reservations",
            new CreateReservationRequest(stockItem.Id, 0, DateTimeOffset.UtcNow.AddHours(1)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "reservation.quantityNotPositive");
    }

    [Fact]
    public async Task CreateReservation_WithPastExpiration_ReturnsBadRequestProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var stockItem = await CreateStockItemWithProductAndChannelAsync(client, "RES-105", "STORE", 5);

        var response = await client.PostAsJsonAsync(
            "/reservations",
            new CreateReservationRequest(stockItem.Id, 1, DateTimeOffset.UtcNow.AddMinutes(-1)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "reservation.expirationNotFuture");
    }

    [Fact]
    public async Task CreateReservation_WhenRequestWouldOversell_ReturnsConflictProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var stockItem = await CreateStockItemWithProductAndChannelAsync(client, "RES-106", "MARKET", 3);

        await CreateReservationAsync(client, stockItem.Id, 2);

        var response = await client.PostAsJsonAsync(
            "/reservations",
            new CreateReservationRequest(stockItem.Id, 2, DateTimeOffset.UtcNow.AddHours(1)));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertProblemCodeAsync(response, "reservation.insufficientAvailability");
    }

    [Fact]
    public async Task GetReservation_WhenMissing_ReturnsNotFoundProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/reservations/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await AssertProblemCodeAsync(response, "reservation.notFound");
    }

    [Fact]
    public async Task ReleaseReservation_WhenActive_ReturnsReleasedReservation()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var stockItem = await CreateStockItemWithProductAndChannelAsync(client, "RES-107", "DIRECT", 5);
        var created = await CreateReservationAsync(client, stockItem.Id, 2);

        var response = await client.PostAsync($"/reservations/{created.Id}/release", content: null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ReservationResponse>();

        Assert.NotNull(body);
        Assert.Equal(created.Id, body.Reservation.Id);
        Assert.Equal("Released", body.Reservation.Status);
        Assert.NotNull(body.Reservation.ReleasedAtUtc);
        Assert.Null(body.Reservation.ExpiredAtUtc);
    }

    [Fact]
    public async Task ReleaseReservation_WhenAlreadyReleased_IsIdempotent()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var stockItem = await CreateStockItemWithProductAndChannelAsync(client, "RES-108", "WHOLESALE", 5);
        var created = await CreateReservationAsync(client, stockItem.Id, 2);

        var firstResponse = await client.PostAsync($"/reservations/{created.Id}/release", content: null);
        firstResponse.EnsureSuccessStatusCode();
        var firstBody = await firstResponse.Content.ReadFromJsonAsync<ReservationResponse>();
        Assert.NotNull(firstBody);

        var secondResponse = await client.PostAsync($"/reservations/{created.Id}/release", content: null);

        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);

        var secondBody = await secondResponse.Content.ReadFromJsonAsync<ReservationResponse>();

        Assert.NotNull(secondBody);
        Assert.Equal("Released", secondBody.Reservation.Status);
        Assert.Equal(firstBody.Reservation.ReleasedAtUtc, secondBody.Reservation.ReleasedAtUtc);
        Assert.Equal(firstBody.Reservation.UpdatedAtUtc, secondBody.Reservation.UpdatedAtUtc);
    }

    [Fact]
    public async Task ReleaseReservation_WhenExpired_ReturnsConflictProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var stockItem = await CreateStockItemWithProductAndChannelAsync(client, "RES-109", "PARTNER", 5);
        var created = await CreateReservationAsync(client, stockItem.Id, 2);
        await ExpireReservationAsync(factory, created.Id);

        var response = await client.PostAsync($"/reservations/{created.Id}/release", content: null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertProblemCodeAsync(response, "reservation.alreadyExpired");
    }

    [Fact]
    public async Task ReleaseReservation_WhenMissing_ReturnsNotFoundProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsync($"/reservations/{Guid.NewGuid()}/release", content: null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await AssertProblemCodeAsync(response, "reservation.notFound");
    }

    [Fact]
    public async Task CreateReservation_AfterRelease_CanReuseAvailability()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();
        var stockItem = await CreateStockItemWithProductAndChannelAsync(client, "RES-110", "OUTLET", 3);
        var created = await CreateReservationAsync(client, stockItem.Id, 3);

        var releaseResponse = await client.PostAsync($"/reservations/{created.Id}/release", content: null);
        releaseResponse.EnsureSuccessStatusCode();

        var secondReservation = await CreateReservationAsync(client, stockItem.Id, 3);

        Assert.NotEqual(created.Id, secondReservation.Id);
        Assert.Equal(stockItem.Id, secondReservation.StockItemId);
        Assert.Equal(3, secondReservation.Quantity);
        Assert.Equal("Active", secondReservation.Status);
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

    private static async Task<ReservationDto> CreateReservationAsync(
        HttpClient client,
        Guid stockItemId,
        int quantity)
    {
        var response = await client.PostAsJsonAsync(
            "/reservations",
            new CreateReservationRequest(stockItemId, quantity, DateTimeOffset.UtcNow.AddHours(1)));
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<ReservationResponse>();

        Assert.NotNull(body);
        return body.Reservation;
    }

    private static async Task AssertProblemCodeAsync(HttpResponseMessage response, string expectedCode)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(expectedCode, document.RootElement.GetProperty("code").GetString());
    }

    private static async Task ExpireReservationAsync(TestInventoryApiFactory factory, Guid reservationId)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
        var reservation = await dbContext.Reservations.SingleAsync(reservation => reservation.Id == reservationId);

        reservation.Expire(DateTimeOffset.UtcNow);
        await dbContext.SaveChangesAsync();
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

    private sealed record CreateReservationRequest(
        Guid StockItemId,
        int Quantity,
        DateTimeOffset ExpiresAtUtc);

    private sealed record ReservationResponse(ReservationDto Reservation);

    private sealed record ReservationListResponse(IReadOnlyList<ReservationDto> Items);

    private sealed record ReservationDto(
        Guid Id,
        Guid StockItemId,
        int Quantity,
        string Status,
        DateTimeOffset ExpiresAtUtc,
        DateTimeOffset CreatedAtUtc,
        DateTimeOffset UpdatedAtUtc,
        DateTimeOffset? ReleasedAtUtc,
        DateTimeOffset? ExpiredAtUtc);
}
