using Inventory.Api.Application.Observability;
using Inventory.Api.Application.Observability.GetObservabilitySnapshot;
using Inventory.Api.Infrastructure.Observability;
using MediatR;

namespace Inventory.Api.Api.Observability;

public static class ObservabilityEndpoints
{
    public static IEndpointRouteBuilder MapObservabilityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/observability")
            .WithTags("Observability");

        group.MapGet("/health", async (
            ISender sender,
            HttpContext httpContext,
            TimeProvider timeProvider,
            CancellationToken cancellationToken) =>
        {
            var snapshot = await sender.Send(
                new GetObservabilitySnapshotQuery(timeProvider.GetUtcNow()),
                cancellationToken);

            return Results.Ok(ToResponse(snapshot, httpContext));
        })
        .WithName("GetObservabilityHealth");

        group.MapGet("/metrics", async (
            ISender sender,
            HttpContext httpContext,
            TimeProvider timeProvider,
            CancellationToken cancellationToken) =>
        {
            var snapshot = await sender.Send(
                new GetObservabilitySnapshotQuery(timeProvider.GetUtcNow()),
                cancellationToken);

            return Results.Ok(ToMetricsResponse(snapshot, httpContext));
        })
        .WithName("GetObservabilityMetrics");

        group.MapGet("/prometheus", async (
            ISender sender,
            TimeProvider timeProvider,
            CancellationToken cancellationToken) =>
        {
            var snapshot = await sender.Send(
                new GetObservabilitySnapshotQuery(timeProvider.GetUtcNow()),
                cancellationToken);

            return Results.Text(
                PrometheusMetricsFormatter.Format(snapshot),
                "text/plain; version=0.0.4; charset=utf-8");
        })
        .WithName("GetPrometheusMetrics");

        return endpoints;
    }

    private static ObservabilityHealthResponse ToResponse(
        ObservabilitySnapshotDto snapshot,
        HttpContext httpContext)
    {
        return new ObservabilityHealthResponse(
            snapshot.Status,
            "Inventory API",
            snapshot.CheckedAtUtc,
            ObservabilityHttpContext.GetTraceId(httpContext),
            ObservabilityHttpContext.GetCorrelationId(httpContext),
            snapshot.Outbox,
            snapshot.ChannelSync);
    }

    private static ObservabilityMetricsResponse ToMetricsResponse(
        ObservabilitySnapshotDto snapshot,
        HttpContext httpContext)
    {
        return new ObservabilityMetricsResponse(
            snapshot.Status,
            "Inventory API",
            snapshot.CheckedAtUtc,
            ObservabilityHttpContext.GetTraceId(httpContext),
            ObservabilityHttpContext.GetCorrelationId(httpContext),
            snapshot.Outbox,
            snapshot.ChannelSync);
    }
}
