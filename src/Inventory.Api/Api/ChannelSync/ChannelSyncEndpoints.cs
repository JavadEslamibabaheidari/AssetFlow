using Inventory.Api.Application.ChannelSync.ListChannelSyncStatuses;
using MediatR;

namespace Inventory.Api.Api.ChannelSync;

public static class ChannelSyncEndpoints
{
    public static IEndpointRouteBuilder MapChannelSyncEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/channel-sync")
            .WithTags("ChannelSync");

        group.MapGet("/status", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var statuses = await sender.Send(new ListChannelSyncStatusesQuery(), cancellationToken);

            return Results.Ok(new ChannelSyncStatusListResponse(statuses));
        })
        .WithName("ListChannelSyncStatuses");

        return endpoints;
    }
}
