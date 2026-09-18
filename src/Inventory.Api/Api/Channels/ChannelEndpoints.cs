using Inventory.Api.Api.Common;
using Inventory.Api.Application.Channels.CreateChannel;
using Inventory.Api.Application.Channels.ListChannels;
using MediatR;

namespace Inventory.Api.Api.Channels;

public static class ChannelEndpoints
{
    public static IEndpointRouteBuilder MapChannelEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/channels")
            .WithTags("Channels");

        group.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var channels = await sender.Send(new ListChannelsQuery(), cancellationToken);

            return Results.Ok(new ChannelListResponse(channels));
        })
        .WithName("ListChannels");

        group.MapPost("/", async (
            CreateChannelRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new CreateChannelCommand(request.Code, request.Name),
                cancellationToken);

            return result.ToHttpResult(channel =>
                Results.Created($"/channels/{channel.Id}", new ChannelResponse(channel)));
        })
        .WithName("CreateChannel");

        return endpoints;
    }
}
