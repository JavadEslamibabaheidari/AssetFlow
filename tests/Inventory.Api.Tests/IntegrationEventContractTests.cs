using Inventory.Api.Application.Events;

namespace Inventory.Api.Tests;

public sealed class IntegrationEventContractTests
{
    [Fact]
    public void StockItemCreated_UsesStableEnvelopeAndPayload()
    {
        var stockItemId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        var occurredAtUtc = DateTimeOffset.UtcNow;

        var integrationEvent = IntegrationEvents.StockItemCreated(
            stockItemId,
            productId,
            channelId,
            12,
            12,
            occurredAtUtc);

        Assert.Equal(IntegrationEventNames.StockItemCreated, integrationEvent.EventType);
        Assert.Equal(1, integrationEvent.SchemaVersion);
        Assert.Equal("stockItem", integrationEvent.AggregateType);
        Assert.Equal(stockItemId, integrationEvent.AggregateId);
        Assert.Equal(occurredAtUtc, integrationEvent.OccurredAtUtc);

        var payload = Assert.IsType<StockItemCreatedPayload>(integrationEvent.Payload);
        Assert.Equal(stockItemId, payload.StockItemId);
        Assert.Equal(productId, payload.ProductId);
        Assert.Equal(channelId, payload.ChannelId);
        Assert.Equal(12, payload.OnHandQuantity);
        Assert.Equal(12, payload.AvailableQuantity);
        Assert.Equal(occurredAtUtc, payload.UpdatedAtUtc);
    }

    [Fact]
    public void StockAvailabilityChanged_UsesStableEnvelopeAndPayload()
    {
        var stockItemId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var channelId = Guid.NewGuid();
        var sourceMutationId = Guid.NewGuid();
        var nextExpirationAtUtc = DateTimeOffset.UtcNow.AddMinutes(15);
        var occurredAtUtc = DateTimeOffset.UtcNow;

        var integrationEvent = IntegrationEvents.StockAvailabilityChanged(
            stockItemId,
            productId,
            channelId,
            20,
            5,
            15,
            nextExpirationAtUtc,
            "ReservationCreated",
            sourceMutationId,
            occurredAtUtc);

        Assert.Equal(IntegrationEventNames.StockAvailabilityChanged, integrationEvent.EventType);
        Assert.Equal(1, integrationEvent.SchemaVersion);
        Assert.Equal("stockItem", integrationEvent.AggregateType);
        Assert.Equal(stockItemId, integrationEvent.AggregateId);
        Assert.Equal(occurredAtUtc, integrationEvent.OccurredAtUtc);

        var payload = Assert.IsType<StockAvailabilityChangedPayload>(integrationEvent.Payload);
        Assert.Equal(stockItemId, payload.StockItemId);
        Assert.Equal(productId, payload.ProductId);
        Assert.Equal(channelId, payload.ChannelId);
        Assert.Equal(20, payload.OnHandQuantity);
        Assert.Equal(5, payload.ReservedQuantity);
        Assert.Equal(15, payload.AvailableQuantity);
        Assert.Equal(nextExpirationAtUtc, payload.NextExpirationAtUtc);
        Assert.Equal("ReservationCreated", payload.Reason);
        Assert.Equal(sourceMutationId, payload.SourceMutationId);
    }

    [Fact]
    public void PlannedM7EventNames_AreStableConstants()
    {
        Assert.Equal("ReservationCreated", IntegrationEventNames.ReservationCreated);
        Assert.Equal("ReservationReleased", IntegrationEventNames.ReservationReleased);
        Assert.Equal("ReservationExpired", IntegrationEventNames.ReservationExpired);
        Assert.Equal("ChannelSyncRequested", IntegrationEventNames.ChannelSyncRequested);
        Assert.Equal("ChannelSyncSucceeded", IntegrationEventNames.ChannelSyncSucceeded);
        Assert.Equal("ChannelSyncFailed", IntegrationEventNames.ChannelSyncFailed);
    }
}
