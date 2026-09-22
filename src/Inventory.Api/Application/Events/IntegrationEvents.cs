namespace Inventory.Api.Application.Events;

public static class IntegrationEvents
{
    public static IntegrationEvent StockItemCreated(
        Guid stockItemId,
        Guid productId,
        Guid channelId,
        int onHandQuantity,
        int availableQuantity,
        DateTimeOffset updatedAtUtc)
    {
        return new IntegrationEvent(
            IntegrationEventNames.StockItemCreated,
            1,
            "stockItem",
            stockItemId,
            updatedAtUtc,
            new StockItemCreatedPayload(
                stockItemId,
                productId,
                channelId,
                onHandQuantity,
                availableQuantity,
                updatedAtUtc));
    }

    public static IntegrationEvent StockAvailabilityChanged(
        Guid stockItemId,
        Guid productId,
        Guid channelId,
        int onHandQuantity,
        int reservedQuantity,
        int availableQuantity,
        DateTimeOffset? nextExpirationAtUtc,
        string reason,
        Guid sourceMutationId,
        DateTimeOffset occurredAtUtc)
    {
        return new IntegrationEvent(
            IntegrationEventNames.StockAvailabilityChanged,
            1,
            "stockItem",
            stockItemId,
            occurredAtUtc,
            new StockAvailabilityChangedPayload(
                stockItemId,
                productId,
                channelId,
                onHandQuantity,
                reservedQuantity,
                availableQuantity,
                nextExpirationAtUtc,
                reason,
                sourceMutationId));
    }

    public static IntegrationEvent ReservationCreated(
        Guid reservationId,
        Guid stockItemId,
        int quantity,
        DateTimeOffset expiresAtUtc,
        string status,
        DateTimeOffset createdAtUtc)
    {
        return new IntegrationEvent(
            IntegrationEventNames.ReservationCreated,
            1,
            "reservation",
            reservationId,
            createdAtUtc,
            new ReservationCreatedPayload(
                reservationId,
                stockItemId,
                quantity,
                expiresAtUtc,
                status,
                createdAtUtc));
    }

    public static IntegrationEvent ReservationReleased(
        Guid reservationId,
        Guid stockItemId,
        int quantity,
        DateTimeOffset releasedAtUtc,
        string status)
    {
        return new IntegrationEvent(
            IntegrationEventNames.ReservationReleased,
            1,
            "reservation",
            reservationId,
            releasedAtUtc,
            new ReservationReleasedPayload(
                reservationId,
                stockItemId,
                quantity,
                releasedAtUtc,
                status));
    }

    public static IntegrationEvent ReservationExpired(
        Guid reservationId,
        Guid stockItemId,
        int quantity,
        DateTimeOffset expiredAtUtc,
        string status)
    {
        return new IntegrationEvent(
            IntegrationEventNames.ReservationExpired,
            1,
            "reservation",
            reservationId,
            expiredAtUtc,
            new ReservationExpiredPayload(
                reservationId,
                stockItemId,
                quantity,
                expiredAtUtc,
                status));
    }
}

public sealed record StockItemCreatedPayload(
    Guid StockItemId,
    Guid ProductId,
    Guid ChannelId,
    int OnHandQuantity,
    int AvailableQuantity,
    DateTimeOffset UpdatedAtUtc);

public sealed record StockAvailabilityChangedPayload(
    Guid StockItemId,
    Guid ProductId,
    Guid ChannelId,
    int OnHandQuantity,
    int ReservedQuantity,
    int AvailableQuantity,
    DateTimeOffset? NextExpirationAtUtc,
    string Reason,
    Guid SourceMutationId);

public sealed record ReservationCreatedPayload(
    Guid ReservationId,
    Guid StockItemId,
    int Quantity,
    DateTimeOffset ExpiresAtUtc,
    string Status,
    DateTimeOffset CreatedAtUtc);

public sealed record ReservationReleasedPayload(
    Guid ReservationId,
    Guid StockItemId,
    int Quantity,
    DateTimeOffset ReleasedAtUtc,
    string Status);

public sealed record ReservationExpiredPayload(
    Guid ReservationId,
    Guid StockItemId,
    int Quantity,
    DateTimeOffset ExpiredAtUtc,
    string Status);

public sealed record ChannelSyncRequestedPayload(
    Guid ChannelId,
    Guid StockItemId,
    Guid SourceEventId,
    int AvailableQuantity);

public sealed record ChannelSyncSucceededPayload(
    Guid ChannelId,
    Guid StockItemId,
    Guid SourceEventId,
    int AttemptNumber,
    DateTimeOffset SucceededAtUtc);

public sealed record ChannelSyncFailedPayload(
    Guid ChannelId,
    Guid StockItemId,
    Guid SourceEventId,
    int AttemptNumber,
    bool Retryable,
    string ErrorSummary);
