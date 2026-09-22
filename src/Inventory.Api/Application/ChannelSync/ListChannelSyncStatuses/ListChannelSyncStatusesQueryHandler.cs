using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.ChannelSync.ListChannelSyncStatuses;

public sealed class ListChannelSyncStatusesQueryHandler(InventoryDbContext dbContext)
    : IRequestHandler<ListChannelSyncStatusesQuery, IReadOnlyList<ChannelSyncStatusDto>>
{
    public async Task<IReadOnlyList<ChannelSyncStatusDto>> Handle(
        ListChannelSyncStatusesQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.ChannelSyncStates
            .AsNoTracking()
            .OrderByDescending(state => state.UpdatedAtUtc)
            .Select(state => new ChannelSyncStatusDto(
                state.Id,
                state.ChannelId,
                state.StockItemId,
                state.SourceEventId,
                state.AvailableQuantity,
                state.Status.ToString(),
                state.AttemptCount,
                state.LastAttemptedAtUtc,
                state.NextAttemptAtUtc,
                state.LastSucceededAtUtc,
                state.LastError,
                state.UpdatedAtUtc))
            .ToArrayAsync(cancellationToken);
    }
}
