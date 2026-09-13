using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Inventory.Api.Tests;

public class InventoryApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public InventoryApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Root_ReturnsServiceInfo()
    {
        var response = await _client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ServiceInfoResponse>();

        Assert.NotNull(body);
        Assert.Equal("Inventory API", body.Service);
        Assert.Equal("Marketplace inventory platform is running.", body.Message);
    }

    [Fact]
    public async Task Health_ReturnsHealthyStatus()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();

        Assert.NotNull(body);
        Assert.Equal("Healthy", body.Status);
        Assert.Equal("Inventory API", body.Service);
        Assert.NotEqual(default, body.CheckedAtUtc);
    }

    private sealed record ServiceInfoResponse(string Service, string Message);

    private sealed record HealthResponse(string Status, string Service, DateTimeOffset CheckedAtUtc);
}
