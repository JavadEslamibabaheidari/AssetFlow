using MediatR;

namespace Inventory.Api.Application.Observability.GetObservabilitySnapshot;

public sealed record GetObservabilitySnapshotQuery(DateTimeOffset CheckedAtUtc)
    : IRequest<ObservabilitySnapshotDto>;
