using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Inventory.Api.Tests;

public sealed class VendorEndpointTests
{
    [Fact]
    public async Task CreateVendor_ReturnsCreatedVendor()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/vendors", new CreateVendorRequest("Northwind Supply"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<VendorResponse>();

        Assert.NotNull(body);
        Assert.Equal("Northwind Supply", body.Vendor.Name);
        Assert.NotEqual(Guid.Empty, body.Vendor.Id);
        Assert.NotEqual(default, body.Vendor.CreatedAtUtc);
        Assert.Equal($"/vendors/{body.Vendor.Id}", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task GetVendor_ReturnsCreatedVendor()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var created = await CreateVendorAsync(client, "Contoso Goods");

        var response = await client.GetAsync($"/vendors/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<VendorResponse>();

        Assert.NotNull(body);
        Assert.Equal(created.Id, body.Vendor.Id);
        Assert.Equal("Contoso Goods", body.Vendor.Name);
    }

    [Fact]
    public async Task ListVendors_ReturnsNewestFirst()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var first = await CreateVendorAsync(client, "First Vendor");
        var second = await CreateVendorAsync(client, "Second Vendor");

        var response = await client.GetAsync("/vendors");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<VendorListResponse>();

        Assert.NotNull(body);
        Assert.Equal([second.Id, first.Id], body.Items.Select(vendor => vendor.Id));
    }

    [Fact]
    public async Task CreateVendor_WithBlankName_ReturnsBadRequestProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/vendors", new CreateVendorRequest("   "));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "vendor.nameRequired");
    }

    [Fact]
    public async Task CreateVendor_WithDuplicateName_ReturnsConflictProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        await CreateVendorAsync(client, "Duplicate Vendor");

        var response = await client.PostAsJsonAsync("/vendors", new CreateVendorRequest("Duplicate Vendor"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertProblemCodeAsync(response, "vendor.duplicateName");
    }

    [Fact]
    public async Task GetVendor_WhenMissing_ReturnsNotFoundProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/vendors/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await AssertProblemCodeAsync(response, "vendor.notFound");
    }

    private static async Task<VendorDto> CreateVendorAsync(HttpClient client, string name)
    {
        var response = await client.PostAsJsonAsync("/vendors", new CreateVendorRequest(name));
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<VendorResponse>();

        Assert.NotNull(body);
        return body.Vendor;
    }

    private static async Task AssertProblemCodeAsync(HttpResponseMessage response, string expectedCode)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(expectedCode, document.RootElement.GetProperty("code").GetString());
    }

    private sealed record CreateVendorRequest(string Name);

    private sealed record VendorResponse(VendorDto Vendor);

    private sealed record VendorListResponse(IReadOnlyList<VendorDto> Items);

    private sealed record VendorDto(Guid Id, string Name, DateTimeOffset CreatedAtUtc);
}
