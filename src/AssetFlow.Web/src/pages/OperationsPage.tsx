import { useQuery } from "@tanstack/react-query";
import { inventoryApiClient } from "../api";
import { PageHeader, StatusBadge } from "../design-system";
import {
  ApiErrorFeedback,
  WorkflowEmptyState,
  WorkflowLoadingState
} from "../shared/workflowStates";

export function OperationsPage() {
  const channels = useQuery({
    queryKey: ["markets", "channels"],
    queryFn: ({ signal }) =>
      inventoryApiClient.listChannels(signal).then((response) => response.items)
  });
  const syncStatuses = useQuery({
    queryKey: ["markets", "sync-statuses"],
    queryFn: ({ signal }) =>
      inventoryApiClient.listChannelSyncStatuses(signal).then((response) => response.items)
  });
  const syncStatusByChannel = new Map(
    (syncStatuses.data ?? []).map((status) => [status.channelId, status])
  );

  return (
    <section className="page-section" aria-labelledby="operations-title">
      <PageHeader
        eyebrow="Markets"
        title="Marketplace channels"
        titleId="operations-title"
        description="Review sales-channel master data and the latest backend channel synchronization state."
        compact
      />

      {channels.isLoading || syncStatuses.isLoading ? (
        <WorkflowLoadingState
          title="Loading channel operations"
          description="Reading channel master data and synchronization status."
        />
      ) : null}
      {channels.error ? <ApiErrorFeedback error={channels.error} /> : null}
      {syncStatuses.error ? <ApiErrorFeedback error={syncStatuses.error} /> : null}

      <section className="workflow-panel full-span" aria-labelledby="channel-master-data-title">
        <div className="workflow-panel-heading">
          <div>
            <h3 id="channel-master-data-title">Channel synchronization</h3>
            <p>Channels define where stock items can be listed and synchronized.</p>
          </div>
          <StatusBadge>
            {syncStatuses.data?.filter((status) => status.status === "Failed").length ?? 0} failed
          </StatusBadge>
        </div>

        {channels.data?.length ? (
          <div className="table-wrap">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Code</th>
                  <th>Name</th>
                  <th>Created</th>
                  <th>Sync status</th>
                  <th>Available</th>
                  <th>Attempts</th>
                  <th>Next retry</th>
                </tr>
              </thead>
              <tbody>
                {channels.data.map((channel) => {
                  const syncStatus = syncStatusByChannel.get(channel.id);

                  return (
                    <tr key={channel.id}>
                      <td>{channel.code}</td>
                      <td>{channel.name}</td>
                      <td>{formatDate(channel.createdAtUtc)}</td>
                      <td>
                        <StatusBadge tone={statusTone(syncStatus?.status)}>
                          {statusLabel(syncStatus?.status)}
                        </StatusBadge>
                        {syncStatus?.lastError ? (
                          <div className="table-note">{syncStatus.lastError}</div>
                        ) : null}
                      </td>
                      <td>{syncStatus ? syncStatus.availableQuantity : "Not synced"}</td>
                      <td>{syncStatus?.attemptCount ?? 0}</td>
                      <td>
                        {syncStatus?.nextAttemptAtUtc
                          ? formatDate(syncStatus.nextAttemptAtUtc)
                          : "None scheduled"}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        ) : channels.isLoading ? null : (
          <WorkflowEmptyState
            title="No channels yet"
            description="Create sales channels from Assets before listing stock by market."
          />
        )}
      </section>
    </section>
  );
}

type SyncStatus = "Pending" | "InProgress" | "Succeeded" | "Failed";

function statusLabel(status?: SyncStatus): string {
  if (!status) {
    return "No sync yet";
  }

  return status;
}

function statusTone(status?: SyncStatus): "neutral" | "success" | "warning" | "danger" {
  switch (status) {
    case "Succeeded":
      return "success";
    case "Pending":
    case "InProgress":
      return "warning";
    case "Failed":
      return "danger";
    default:
      return "neutral";
  }
}

function formatDate(value: string): string {
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short"
  }).format(new Date(value));
}
