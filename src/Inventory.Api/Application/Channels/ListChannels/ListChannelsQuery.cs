using MediatR;

namespace Inventory.Api.Application.Channels.ListChannels;

public sealed record ListChannelsQuery : IRequest<IReadOnlyList<ChannelDto>>;
