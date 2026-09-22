using Inventory.Api.Application.ChannelSync;

namespace Inventory.Api.Api.ChannelSync;

public sealed record ChannelSyncStatusListResponse(IReadOnlyList<ChannelSyncStatusDto> Items);
