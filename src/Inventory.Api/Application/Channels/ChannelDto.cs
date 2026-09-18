namespace Inventory.Api.Application.Channels;

public sealed record ChannelDto(
    Guid Id,
    string Code,
    string Name,
    DateTimeOffset CreatedAtUtc);
