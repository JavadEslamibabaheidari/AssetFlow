using System.Diagnostics;

namespace Inventory.Api.Infrastructure.Observability;

public sealed class RequestObservabilityMiddleware(
    RequestDelegate next,
    ILogger<RequestObservabilityMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ResolveCorrelationId(context);
        context.Items[ObservabilityHttpContext.CorrelationIdItemName] = correlationId;
        context.Response.Headers[ObservabilityHttpContext.CorrelationIdHeaderName] = correlationId;

        var stopwatch = Stopwatch.StartNew();

        using var activity = InventoryDiagnostics.ActivitySource.StartActivity("http.server.request");
        activity?.SetTag("http.request.method", context.Request.Method);
        activity?.SetTag("url.path", context.Request.Path.Value);
        activity?.SetTag("assetflow.correlation_id", correlationId);

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["TraceId"] = ObservabilityHttpContext.GetTraceId(context),
            ["CorrelationId"] = correlationId
        }))
        {
            try
            {
                await next(context);
            }
            finally
            {
                stopwatch.Stop();
                activity?.SetTag("http.response.status_code", context.Response.StatusCode);
                activity?.SetTag("assetflow.elapsed_ms", stopwatch.ElapsedMilliseconds);

                logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms trace {TraceId} correlation {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path.Value,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    ObservabilityHttpContext.GetTraceId(context),
                    correlationId);
            }
        }
    }

    private static string ResolveCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(
            ObservabilityHttpContext.CorrelationIdHeaderName,
            out var header) &&
            !string.IsNullOrWhiteSpace(header))
        {
            return header.ToString();
        }

        return context.TraceIdentifier;
    }
}
