using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Inventory.Api.Tests;

public sealed class ChannelEndpointTests
{
    [Fact]
    public async Task CreateChannel_ReturnsCreatedChannel()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/channels",
            new CreateChannelRequest("SHOPIFY", "Shopify"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ChannelResponse>();

        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.Channel.Id);
        Assert.Equal("SHOPIFY", body.Channel.Code);
        Assert.Equal("Shopify", body.Channel.Name);
        Assert.NotEqual(default, body.Channel.CreatedAtUtc);
        Assert.Equal($"/channels/{body.Channel.Id}", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task ListChannels_ReturnsNewestFirst()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var first = await CreateChannelAsync(client, "AMAZON", "Amazon");
        var second = await CreateChannelAsync(client, "MEDIAWORLD", "MediaWorld");

        var response = await client.GetAsync("/channels");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ChannelListResponse>();

        Assert.NotNull(body);
        Assert.Equal([second.Id, first.Id], body.Items.Select(channel => channel.Id));
    }

    [Fact]
    public async Task CreateChannel_WithDuplicateCode_ReturnsConflictProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        await CreateChannelAsync(client, "UNIEURO", "Unieuro");

        var response = await client.PostAsJsonAsync(
            "/channels",
            new CreateChannelRequest("UNIEURO", "Unieuro Duplicate"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        await AssertProblemCodeAsync(response, "channel.duplicateCode");
    }

    [Fact]
    public async Task CreateChannel_WithBlankCode_ReturnsBadRequestProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/channels",
            new CreateChannelRequest(" ", "Valid Channel"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "channel.codeRequired");
    }

    [Fact]
    public async Task CreateChannel_WithBlankName_ReturnsBadRequestProblem()
    {
        await using var factory = new TestInventoryApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/channels",
            new CreateChannelRequest("CUSTOM", " "));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AssertProblemCodeAsync(response, "channel.nameRequired");
    }

    private static async Task<ChannelDto> CreateChannelAsync(HttpClient client, string code, string name)
    {
        var response = await client.PostAsJsonAsync("/channels", new CreateChannelRequest(code, name));
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<ChannelResponse>();

        Assert.NotNull(body);
        return body.Channel;
    }

    private static async Task AssertProblemCodeAsync(HttpResponseMessage response, string expectedCode)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.Equal(expectedCode, document.RootElement.GetProperty("code").GetString());
    }

    private sealed record CreateChannelRequest(string Code, string Name);

    private sealed record ChannelResponse(ChannelDto Channel);

    private sealed record ChannelListResponse(IReadOnlyList<ChannelDto> Items);

    private sealed record ChannelDto(Guid Id, string Code, string Name, DateTimeOffset CreatedAtUtc);
}
