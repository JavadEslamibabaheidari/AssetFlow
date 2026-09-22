namespace Inventory.Api.Infrastructure.Persistence.Outbox;

public sealed class OutboxMessage
{
    public OutboxMessage(
        Guid id,
        string eventType,
        int schemaVersion,
        string aggregateType,
        Guid aggregateId,
        DateTimeOffset occurredAtUtc,
        string payloadJson,
        OutboxMessageStatus status,
        int attemptCount,
        DateTimeOffset? nextAttemptAtUtc,
        string? lastError,
        DateTimeOffset? processedAtUtc,
        DateTimeOffset createdAtUtc)
    {
        Id = id;
        EventType = eventType;
        SchemaVersion = schemaVersion;
        AggregateType = aggregateType;
        AggregateId = aggregateId;
        OccurredAtUtc = occurredAtUtc;
        PayloadJson = payloadJson;
        Status = status;
        AttemptCount = attemptCount;
        NextAttemptAtUtc = nextAttemptAtUtc;
        LastError = lastError;
        ProcessedAtUtc = processedAtUtc;
        CreatedAtUtc = createdAtUtc;
    }

    private OutboxMessage()
    {
        EventType = string.Empty;
        AggregateType = string.Empty;
        PayloadJson = "{}";
    }

    public Guid Id { get; private set; }

    public string EventType { get; private set; }

    public int SchemaVersion { get; private set; }

    public string AggregateType { get; private set; }

    public Guid AggregateId { get; private set; }

    public DateTimeOffset OccurredAtUtc { get; private set; }

    public string PayloadJson { get; private set; }

    public OutboxMessageStatus Status { get; private set; }

    public int AttemptCount { get; private set; }

    public DateTimeOffset? NextAttemptAtUtc { get; private set; }

    public string? LastError { get; private set; }

    public DateTimeOffset? ProcessedAtUtc { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public void MarkProcessing(DateTimeOffset attemptedAtUtc)
    {
        Status = OutboxMessageStatus.Processing;
        AttemptCount++;
        NextAttemptAtUtc = null;
        LastError = null;
        ProcessedAtUtc = null;
    }

    public void MarkPublished(DateTimeOffset processedAtUtc)
    {
        Status = OutboxMessageStatus.Published;
        NextAttemptAtUtc = null;
        LastError = null;
        ProcessedAtUtc = processedAtUtc;
    }

    public void MarkFailed(
        DateTimeOffset attemptedAtUtc,
        string errorSummary,
        DateTimeOffset? nextAttemptAtUtc)
    {
        Status = OutboxMessageStatus.Failed;
        LastError = errorSummary;
        NextAttemptAtUtc = nextAttemptAtUtc;
        ProcessedAtUtc = null;
    }
}
