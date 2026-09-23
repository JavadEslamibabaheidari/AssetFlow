using Inventory.Api.Application.Observability;

namespace Inventory.Api.Api.Observability;

public sealed record ObservabilityHealthResponse(
    string Status,
    string Service,
    DateTimeOffset CheckedAtUtc,
    string TraceId,
    string CorrelationId,
    OutboxObservabilityDto Outbox,
    ChannelSyncObservabilityDto ChannelSync);

public sealed record ObservabilityMetricsResponse(
    string Status,
    string Service,
    DateTimeOffset CheckedAtUtc,
    string TraceId,
    string CorrelationId,
    OutboxObservabilityDto Outbox,
    ChannelSyncObservabilityDto ChannelSync);
