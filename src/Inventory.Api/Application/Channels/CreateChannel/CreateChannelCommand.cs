using Inventory.Api.Application.Common;
using MediatR;

namespace Inventory.Api.Application.Channels.CreateChannel;

public sealed record CreateChannelCommand(string? Code, string? Name)
    : IRequest<ApplicationResult<ChannelDto>>;
