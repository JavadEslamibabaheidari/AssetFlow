using MediatR;

namespace Inventory.Api.Application.ChannelSync.ListChannelSyncStatuses;

public sealed record ListChannelSyncStatusesQuery : IRequest<IReadOnlyList<ChannelSyncStatusDto>>;
