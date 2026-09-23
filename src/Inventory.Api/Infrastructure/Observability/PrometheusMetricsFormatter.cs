using System.Globalization;
using System.Text;
using Inventory.Api.Application.Observability;

namespace Inventory.Api.Infrastructure.Observability;

public static class PrometheusMetricsFormatter
{
    public static string Format(ObservabilitySnapshotDto snapshot)
    {
        var builder = new StringBuilder();

        AppendGauge(builder, "assetflow_health_status", "AssetFlow service health where 1 is healthy, 0.5 is busy, and 0 is degraded.");
        AppendSample(builder, "assetflow_health_status", HealthValue(snapshot.Status));

        AppendGauge(builder, "assetflow_outbox_messages_total", "Outbox message count by status.");
        foreach (var statusMetric in snapshot.Outbox.ByStatus)
        {
            AppendSample(
                builder,
                "assetflow_outbox_messages_total",
                statusMetric.Count,
                ("status", statusMetric.Status));
        }

        AppendGauge(builder, "assetflow_outbox_event_messages_total", "Outbox message count by event type and status.");
        foreach (var eventMetric in snapshot.Outbox.ByEventType)
        {
            AppendSample(
                builder,
                "assetflow_outbox_event_messages_total",
                eventMetric.Count,
                ("event_type", eventMetric.EventType),
                ("status", eventMetric.Status));
        }

        AppendGauge(builder, "assetflow_channel_sync_states_total", "Channel synchronization state count by status.");
        AppendSample(builder, "assetflow_channel_sync_states_total", snapshot.ChannelSync.PendingStates, ("status", "Pending"));
        AppendSample(builder, "assetflow_channel_sync_states_total", snapshot.ChannelSync.InProgressStates, ("status", "InProgress"));
        AppendSample(builder, "assetflow_channel_sync_states_total", snapshot.ChannelSync.SucceededStates, ("status", "Succeeded"));
        AppendSample(builder, "assetflow_channel_sync_states_total", snapshot.ChannelSync.FailedStates, ("status", "Failed"));

        AppendGauge(builder, "assetflow_channel_sync_retryable_failures", "Channel synchronization failures with a scheduled retry.");
        AppendSample(builder, "assetflow_channel_sync_retryable_failures", snapshot.ChannelSync.RetryableFailures);

        if (snapshot.ChannelSync.NextRetryAtUtc is not null)
        {
            AppendGauge(builder, "assetflow_channel_sync_next_retry_timestamp_seconds", "Next channel synchronization retry Unix timestamp.");
            AppendSample(
                builder,
                "assetflow_channel_sync_next_retry_timestamp_seconds",
                snapshot.ChannelSync.NextRetryAtUtc.Value.ToUnixTimeSeconds());
        }

        return builder.ToString();
    }

    private static void AppendGauge(StringBuilder builder, string name, string help)
    {
        builder.Append("# HELP ").Append(name).Append(' ').Append(help).AppendLine();
        builder.Append("# TYPE ").Append(name).AppendLine(" gauge");
    }

    private static void AppendSample(
        StringBuilder builder,
        string name,
        double value,
        params (string Name, string Value)[] labels)
    {
        builder.Append(name);
        if (labels.Length > 0)
        {
            builder.Append('{');
            for (var i = 0; i < labels.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(',');
                }

                builder
                    .Append(labels[i].Name)
                    .Append("=\"")
                    .Append(EscapeLabelValue(labels[i].Value))
                    .Append('"');
            }

            builder.Append('}');
        }

        builder
            .Append(' ')
            .Append(value.ToString(CultureInfo.InvariantCulture))
            .AppendLine();
    }

    private static double HealthValue(string status)
    {
        return status switch
        {
            "Healthy" => 1,
            "Busy" => 0.5,
            _ => 0
        };
    }

    private static string EscapeLabelValue(string value)
    {
        return value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);
    }
}
