import { FormEvent, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { inventoryApiClient } from "../api";
import type { components } from "../api/generated/inventory-api";
import { PageHeader, StatusBadge } from "../design-system";
import {
  ApiErrorFeedback,
  WorkflowEmptyState,
  WorkflowFeedback,
  WorkflowLoadingState
} from "../shared/workflowStates";

type Product = components["schemas"]["Product"];
type Channel = components["schemas"]["Channel"];
type StockItem = components["schemas"]["StockItem"];
type Reservation = components["schemas"]["Reservation"];
type ReservationStatus = components["schemas"]["ReservationStatus"];

const reservationKeys = {
  products: ["reservation-products"] as const,
  channels: ["reservation-channels"] as const,
  stockItems: ["reservation-stock-items"] as const,
  reservations: (stockItemId: string, status: string) =>
    ["reservations", stockItemId, status] as const,
  reservation: (id: string) => ["reservations", id] as const,
  availability: (id: string) => ["reservations", id, "availability"] as const
};

export function ReservationsPage() {
  const queryClient = useQueryClient();
  const [reservationStockItemId, setReservationStockItemId] = useState("");
  const [selectedStockItemId, setSelectedStockItemId] = useState("");
  const [statusFilter, setStatusFilter] = useState<"" | ReservationStatus>("");
  const [selectedReservationId, setSelectedReservationId] = useState("");
  const [success, setSuccess] = useState("");

  const products = useQuery({
    queryKey: reservationKeys.products,
    queryFn: ({ signal }) =>
      inventoryApiClient.listProducts({}, signal).then((response) => response.items)
  });
  const channels = useQuery({
    queryKey: reservationKeys.channels,
    queryFn: ({ signal }) =>
      inventoryApiClient.listChannels(signal).then((response) => response.items)
  });
  const stockItems = useQuery({
    queryKey: reservationKeys.stockItems,
    queryFn: ({ signal }) =>
      inventoryApiClient.listStockItems({}, signal).then((response) => response.items)
  });
  const reservations = useQuery({
    queryKey: reservationKeys.reservations(selectedStockItemId, statusFilter),
    queryFn: ({ signal }) =>
      inventoryApiClient
        .listReservations(
          {
            ...(selectedStockItemId ? { stockItemId: selectedStockItemId } : {}),
            ...(statusFilter ? { status: statusFilter } : {})
          },
          signal
        )
        .then((response) => response.items)
  });
  const selectedReservation = useQuery({
    enabled: Boolean(selectedReservationId),
    queryKey: reservationKeys.reservation(selectedReservationId),
    queryFn: ({ signal }) =>
      inventoryApiClient
        .getReservation(selectedReservationId, signal)
        .then((response) => response.reservation)
  });
  const selectedAvailability = useQuery({
    enabled: Boolean(selectedStockItemId),
    queryKey: reservationKeys.availability(selectedStockItemId),
    queryFn: ({ signal }) =>
      inventoryApiClient
        .getStockItemAvailability(selectedStockItemId, signal)
        .then((response) => response.availability)
  });

  const refreshReservations = async (stockItemId?: string) => {
    await Promise.all([
      queryClient.invalidateQueries({ queryKey: ["reservations"] }),
      queryClient.invalidateQueries({ queryKey: reservationKeys.stockItems }),
      stockItemId
        ? queryClient.invalidateQueries({ queryKey: reservationKeys.availability(stockItemId) })
        : Promise.resolve()
    ]);
  };

  const createReservation = useMutation({
    mutationFn: (body: { stockItemId: string; quantity: number; expiresAtUtc: string }) =>
      inventoryApiClient.createReservation(body),
    onSuccess: async ({ reservation }) => {
      setReservationStockItemId(reservation.stockItemId);
      setSelectedReservationId(reservation.id);
      setSelectedStockItemId(reservation.stockItemId);
      setSuccess("Reservation created.");
      await refreshReservations(reservation.stockItemId);
    }
  });
  const releaseReservation = useMutation({
    mutationFn: (reservation: Reservation) => inventoryApiClient.releaseReservation(reservation.id),
    onSuccess: async ({ reservation }) => {
      setSelectedReservationId(reservation.id);
      setSuccess("Reservation released.");
      await refreshReservations(reservation.stockItemId);
    }
  });
  const expireReservations = useMutation({
    mutationFn: (expiresBeforeUtc?: string) =>
      inventoryApiClient.expireReservations(expiresBeforeUtc ? { expiresBeforeUtc } : {}),
    onSuccess: async ({ expiredCount, items }) => {
      setSuccess(`${expiredCount} reservation${expiredCount === 1 ? "" : "s"} expired.`);
      await refreshReservations(items[0]?.stockItemId ?? selectedStockItemId);
    }
  });

  const productItems = products.data ?? [];
  const channelItems = channels.data ?? [];
  const stockItemRows = stockItems.data ?? [];
  const reservationRows = reservations.data ?? [];
  const stockItemOptions = stockItemRows.map((stockItem) => ({
    stockItem,
    label: `${findProductLabel(productItems, stockItem.productId)} on ${findChannelLabel(
      channelItems,
      stockItem.channelId
    )}`
  }));

  const firstError =
    products.error ??
    channels.error ??
    stockItems.error ??
    reservations.error ??
    selectedAvailability.error;
  const isLoading =
    products.isLoading || channels.isLoading || stockItems.isLoading || reservations.isLoading;

  return (
    <section className="page-section reservation-workspace" aria-labelledby="reservations-title">
      <PageHeader
        eyebrow="Reservations"
        title="Reserved stock"
        titleId="reservations-title"
        description="Create holds, inspect oversell prevention, release reservations, and expire due holds against backend availability."
        compact
      />

      {success ? <WorkflowFeedback kind="success" title={success} /> : null}
      {firstError ? <ApiErrorFeedback error={firstError} /> : null}
      {isLoading ? (
        <WorkflowLoadingState
          title="Loading reservations"
          description="Reading stock items and reservation state."
        />
      ) : null}

      <div className="workflow-grid">
        <section className="workflow-panel" aria-labelledby="create-reservation-title">
          <div className="workflow-panel-heading">
            <div>
              <h3 id="create-reservation-title">Create reservation</h3>
              <p>Hold available stock until an order completes or the hold expires.</p>
            </div>
          </div>
          <form
            className="workflow-form"
            onSubmit={(event) => handleReservationSubmit(event, createReservation.mutate)}
          >
            <label>
              Stock item
              <select
                name="stockItemId"
                required
                disabled={stockItemOptions.length === 0}
                value={reservationStockItemId}
                onChange={(event) => {
                  setReservationStockItemId(event.target.value);
                  setSelectedStockItemId(event.target.value);
                }}
              >
                <option value="">Select stock</option>
                {stockItemOptions.map(({ stockItem, label }) => (
                  <option key={stockItem.id} value={stockItem.id}>
                    {label}
                  </option>
                ))}
              </select>
            </label>
            <label>
              Quantity
              <input name="quantity" type="number" min="1" step="1" required />
            </label>
            <label>
              Expires at
              <input
                name="expiresAtUtc"
                type="datetime-local"
                defaultValue={defaultExpiry()}
                required
              />
            </label>
            <button
              type="submit"
              disabled={createReservation.isPending || stockItemOptions.length === 0}
            >
              Create reservation
            </button>
          </form>
          {createReservation.error ? <ApiErrorFeedback error={createReservation.error} /> : null}
        </section>

        <section className="workflow-panel" aria-labelledby="availability-snapshot-title">
          <div className="workflow-panel-heading">
            <div>
              <h3 id="availability-snapshot-title">Availability snapshot</h3>
              <p>Backend-calculated on-hand, reserved, and sellable quantity.</p>
            </div>
          </div>
          {selectedAvailability.isLoading ? (
            <WorkflowLoadingState title="Loading availability" />
          ) : null}
          {selectedAvailability.data ? (
            <div className="detail-grid compact">
              <div>
                <span>On hand</span>
                <strong>{selectedAvailability.data.onHandQuantity}</strong>
              </div>
              <div>
                <span>Reserved</span>
                <strong>{selectedAvailability.data.reservedQuantity}</strong>
              </div>
              <div>
                <span>Available</span>
                <strong>{selectedAvailability.data.availableQuantity}</strong>
              </div>
              <div>
                <span>Next expiration</span>
                <strong>
                  {selectedAvailability.data.nextExpirationUtc
                    ? formatDate(selectedAvailability.data.nextExpirationUtc)
                    : "None"}
                </strong>
              </div>
            </div>
          ) : (
            <WorkflowEmptyState
              title="Select stock"
              description="Choose a stock item to inspect reservation-aware availability."
            />
          )}
        </section>

        <section className="workflow-panel" aria-labelledby="expire-reservations-title">
          <div className="workflow-panel-heading">
            <div>
              <h3 id="expire-reservations-title">Expire due holds</h3>
              <p>Ask the backend to expire active reservations due before a cutoff.</p>
            </div>
          </div>
          <form
            className="workflow-form"
            onSubmit={(event) => handleExpireSubmit(event, expireReservations.mutate)}
          >
            <label>
              Cutoff
              <input name="expiresBeforeUtc" type="datetime-local" />
            </label>
            <button type="submit" disabled={expireReservations.isPending}>
              Expire reservations
            </button>
          </form>
          {expireReservations.data ? (
            <WorkflowFeedback kind="success" title="Expiration complete">
              <p>{expireReservations.data.expiredCount} reservations moved to expired.</p>
            </WorkflowFeedback>
          ) : null}
          {expireReservations.error ? <ApiErrorFeedback error={expireReservations.error} /> : null}
        </section>
      </div>

      <section className="workflow-panel full-span" aria-labelledby="reservation-list-title">
        <div className="workflow-panel-heading">
          <div>
            <h3 id="reservation-list-title">Reservation ledger</h3>
            <p>Filter by stock item or status, then inspect and release active reservations.</p>
          </div>
          <StatusBadge>{reservationRows.length} reservations</StatusBadge>
        </div>

        <div className="filter-row" aria-label="Reservation filters">
          <label>
            Stock
            <select
              value={selectedStockItemId}
              onChange={(event) => setSelectedStockItemId(event.target.value)}
            >
              <option value="">All stock</option>
              {stockItemOptions.map(({ stockItem, label }) => (
                <option key={stockItem.id} value={stockItem.id}>
                  {label}
                </option>
              ))}
            </select>
          </label>
          <label>
            Status
            <select
              value={statusFilter}
              onChange={(event) => setStatusFilter(event.target.value as "" | ReservationStatus)}
            >
              <option value="">All statuses</option>
              <option value="Active">Active</option>
              <option value="Released">Released</option>
              <option value="Expired">Expired</option>
            </select>
          </label>
        </div>

        {reservationRows.length === 0 ? (
          <WorkflowEmptyState
            title="No reservations found"
            description="Create a reservation or adjust the filters to review held stock."
          />
        ) : (
          <div className="table-wrap">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Stock</th>
                  <th>Quantity</th>
                  <th>Status</th>
                  <th>Expires</th>
                  <th>Updated</th>
                  <th>Action</th>
                </tr>
              </thead>
              <tbody>
                {reservationRows.map((reservation) => (
                  <tr key={reservation.id}>
                    <td>{findStockLabel(stockItemOptions, reservation.stockItemId)}</td>
                    <td>{reservation.quantity}</td>
                    <td>
                      <StatusBadge>{reservation.status}</StatusBadge>
                    </td>
                    <td>{formatDate(reservation.expiresAtUtc)}</td>
                    <td>{formatDate(reservation.updatedAtUtc)}</td>
                    <td>
                      <div className="table-actions">
                        <button
                          type="button"
                          onClick={() => setSelectedReservationId(reservation.id)}
                        >
                          Inspect
                        </button>
                        <button
                          type="button"
                          disabled={reservation.status !== "Active" || releaseReservation.isPending}
                          onClick={() => releaseReservation.mutate(reservation)}
                        >
                          Release
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
        {releaseReservation.error ? <ApiErrorFeedback error={releaseReservation.error} /> : null}
        {selectedReservation.data ? (
          <ReservationDetail
            reservation={selectedReservation.data}
            stockLabel={findStockLabel(stockItemOptions, selectedReservation.data.stockItemId)}
          />
        ) : selectedReservation.isLoading ? (
          <WorkflowLoadingState title="Loading reservation detail" />
        ) : selectedReservation.error ? (
          <ApiErrorFeedback error={selectedReservation.error} />
        ) : null}
      </section>
    </section>
  );
}

function ReservationDetail({
  reservation,
  stockLabel
}: {
  reservation: Reservation;
  stockLabel: string;
}) {
  return (
    <div className="detail-grid" aria-label="Selected reservation detail">
      <div>
        <span>Stock</span>
        <strong>{stockLabel}</strong>
      </div>
      <div>
        <span>Quantity</span>
        <strong>{reservation.quantity}</strong>
      </div>
      <div>
        <span>Status</span>
        <strong>{reservation.status}</strong>
      </div>
      <div>
        <span>Expires</span>
        <strong>{formatDate(reservation.expiresAtUtc)}</strong>
      </div>
      <div>
        <span>Released</span>
        <strong>
          {reservation.releasedAtUtc ? formatDate(reservation.releasedAtUtc) : "Not released"}
        </strong>
      </div>
      <div>
        <span>Expired</span>
        <strong>
          {reservation.expiredAtUtc ? formatDate(reservation.expiredAtUtc) : "Not expired"}
        </strong>
      </div>
    </div>
  );
}

function handleReservationSubmit(
  event: FormEvent<HTMLFormElement>,
  submit: (body: { stockItemId: string; quantity: number; expiresAtUtc: string }) => void
) {
  event.preventDefault();
  const form = event.currentTarget;
  const data = new FormData(form);
  submit({
    stockItemId: String(data.get("stockItemId") ?? ""),
    quantity: Number(data.get("quantity") ?? 0),
    expiresAtUtc: toIso(String(data.get("expiresAtUtc") ?? ""))
  });
}

function handleExpireSubmit(
  event: FormEvent<HTMLFormElement>,
  submit: (expiresBeforeUtc?: string) => void
) {
  event.preventDefault();
  const form = event.currentTarget;
  const data = new FormData(form);
  const cutoff = String(data.get("expiresBeforeUtc") ?? "");
  submit(cutoff ? toIso(cutoff) : undefined);
}

function findProductLabel(products: Product[], productId: string): string {
  const product = products.find((item) => item.id === productId);
  return product ? `${product.sku} - ${product.name}` : productId;
}

function findChannelLabel(channels: Channel[], channelId: string): string {
  const channel = channels.find((item) => item.id === channelId);
  return channel ? channel.code : channelId;
}

function findStockLabel(
  options: Array<{ stockItem: StockItem; label: string }>,
  stockItemId: string
): string {
  return options.find((option) => option.stockItem.id === stockItemId)?.label ?? stockItemId;
}

function defaultExpiry(): string {
  const date = new Date(Date.now() + 60 * 60 * 1000);
  return toDateTimeLocal(date);
}

function toIso(value: string): string {
  return new Date(value).toISOString();
}

function toDateTimeLocal(date: Date): string {
  const offset = date.getTimezoneOffset();
  const local = new Date(date.getTime() - offset * 60_000);
  return local.toISOString().slice(0, 16);
}

function formatDate(value: string): string {
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short"
  }).format(new Date(value));
}
