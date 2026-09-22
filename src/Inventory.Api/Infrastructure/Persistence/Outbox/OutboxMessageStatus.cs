namespace Inventory.Api.Infrastructure.Persistence.Outbox;

public enum OutboxMessageStatus
{
    Pending,
    Processing,
    Published,
    Failed
}
