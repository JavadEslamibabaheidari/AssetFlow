using Inventory.Api.Domain.Entities;

namespace Inventory.Api.Application.Channels;

public static class ChannelMapping
{
    public static ChannelDto ToDto(this SalesChannel channel) =>
        new(channel.Id, channel.Code, channel.Name, channel.CreatedAtUtc);
}
