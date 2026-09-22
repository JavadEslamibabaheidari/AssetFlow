using System.Text.Json;
using Inventory.Api.Application.Events;

namespace Inventory.Api.Infrastructure.Persistence.Outbox;

public sealed class EfIntegrationEventOutbox(InventoryDbContext dbContext) : IIntegrationEventOutbox
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public void Enqueue(IntegrationEvent integrationEvent)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);

        dbContext.OutboxMessages.Add(new OutboxMessage(
            Guid.NewGuid(),
            integrationEvent.EventType,
            integrationEvent.SchemaVersion,
            integrationEvent.AggregateType,
            integrationEvent.AggregateId,
            integrationEvent.OccurredAtUtc,
            JsonSerializer.Serialize(integrationEvent.Payload, SerializerOptions),
            OutboxMessageStatus.Pending,
            0,
            null,
            null,
            null,
            DateTimeOffset.UtcNow));
    }
}
