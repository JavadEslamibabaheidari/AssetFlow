using System.Diagnostics;

namespace Inventory.Api.Infrastructure.Observability;

public static class ObservabilityHttpContext
{
    public const string CorrelationIdHeaderName = "X-Correlation-ID";
    public const string CorrelationIdItemName = "AssetFlow.CorrelationId";

    public static string GetTraceId(HttpContext httpContext)
    {
        return Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;
    }

    public static string GetCorrelationId(HttpContext httpContext)
    {
        if (httpContext.Items.TryGetValue(CorrelationIdItemName, out var item) &&
            item is string correlationId &&
            !string.IsNullOrWhiteSpace(correlationId))
        {
            return correlationId;
        }

        if (httpContext.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var header) &&
            !string.IsNullOrWhiteSpace(header))
        {
            return header.ToString();
        }

        return httpContext.TraceIdentifier;
    }
}
