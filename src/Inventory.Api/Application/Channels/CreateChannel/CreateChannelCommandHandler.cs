using Inventory.Api.Application.Common;
using Inventory.Api.Domain.Entities;
using Inventory.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Channels.CreateChannel;

public sealed class CreateChannelCommandHandler(InventoryDbContext dbContext)
    : IRequestHandler<CreateChannelCommand, ApplicationResult<ChannelDto>>
{
    public async Task<ApplicationResult<ChannelDto>> Handle(
        CreateChannelCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.Code?.Trim();
        var name = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(code))
        {
            return ApplicationResult<ChannelDto>.Validation(
                "channel.codeRequired",
                "Channel code is required.");
        }

        if (code.Length > 80)
        {
            return ApplicationResult<ChannelDto>.Validation(
                "channel.codeTooLong",
                "Channel code must be 80 characters or fewer.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return ApplicationResult<ChannelDto>.Validation(
                "channel.nameRequired",
                "Channel name is required.");
        }

        if (name.Length > 160)
        {
            return ApplicationResult<ChannelDto>.Validation(
                "channel.nameTooLong",
                "Channel name must be 160 characters or fewer.");
        }

        var duplicateExists = await dbContext.Channels
            .AnyAsync(channel => channel.Code == code, cancellationToken);

        if (duplicateExists)
        {
            return ApplicationResult<ChannelDto>.Conflict(
                "channel.duplicateCode",
                "A channel with this code already exists.");
        }

        var channel = new SalesChannel(Guid.NewGuid(), code, name, DateTimeOffset.UtcNow);

        dbContext.Channels.Add(channel);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ApplicationResult<ChannelDto>.Success(channel.ToDto());
    }
}
