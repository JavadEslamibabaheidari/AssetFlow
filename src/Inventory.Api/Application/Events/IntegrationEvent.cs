namespace Inventory.Api.Application.Events;

public sealed record IntegrationEvent(
    string EventType,
    int SchemaVersion,
    string AggregateType,
    Guid AggregateId,
    DateTimeOffset OccurredAtUtc,
    object Payload);
