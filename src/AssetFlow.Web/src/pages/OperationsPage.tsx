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

  return (
    <section className="page-section" aria-labelledby="operations-title">
      <PageHeader
        eyebrow="Markets"
        title="Marketplace channels"
        titleId="operations-title"
        description="Review sales-channel master data. Synchronization health stays deferred until the event-driven milestone."
        compact
      />

      {channels.isLoading ? (
        <WorkflowLoadingState
          title="Loading sales channels"
          description="Reading channel master data."
        />
      ) : null}
      {channels.error ? <ApiErrorFeedback error={channels.error} /> : null}

      <section className="workflow-panel full-span" aria-labelledby="channel-master-data-title">
        <div className="workflow-panel-heading">
          <div>
            <h3 id="channel-master-data-title">Channel master data</h3>
            <p>Channels define where stock items can be listed before synchronization is added.</p>
          </div>
          <StatusBadge>{channels.data?.length ?? 0} channels</StatusBadge>
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
                </tr>
              </thead>
              <tbody>
                {channels.data.map((channel) => (
                  <tr key={channel.id}>
                    <td>{channel.code}</td>
                    <td>{channel.name}</td>
                    <td>{formatDate(channel.createdAtUtc)}</td>
                    <td>Deferred to M7</td>
                  </tr>
                ))}
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

function formatDate(value: string): string {
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short"
  }).format(new Date(value));
}
