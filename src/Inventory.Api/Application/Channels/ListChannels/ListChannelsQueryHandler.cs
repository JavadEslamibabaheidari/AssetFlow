using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Channels.ListChannels;

public sealed class ListChannelsQueryHandler(InventoryDbContext dbContext)
    : IRequestHandler<ListChannelsQuery, IReadOnlyList<ChannelDto>>
{
    public async Task<IReadOnlyList<ChannelDto>> Handle(
        ListChannelsQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Channels
            .AsNoTracking()
            .OrderByDescending(channel => channel.CreatedAtUtc)
            .Select(channel => channel.ToDto())
            .ToListAsync(cancellationToken);
    }
}
