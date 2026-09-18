using Inventory.Api.Application.Channels;

namespace Inventory.Api.Api.Channels;

public sealed record CreateChannelRequest(string? Code, string? Name);

public sealed record ChannelResponse(ChannelDto Channel);

public sealed record ChannelListResponse(IReadOnlyList<ChannelDto> Items);
