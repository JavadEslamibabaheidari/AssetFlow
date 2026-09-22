namespace Inventory.Api.Application.Events;

public interface IIntegrationEventOutbox
{
    void Enqueue(IntegrationEvent integrationEvent);
}
