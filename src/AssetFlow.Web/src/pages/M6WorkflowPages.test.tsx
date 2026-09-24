import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { cleanup, fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import type { ReactNode } from "react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { InventoryPage } from "./InventoryPage";
import { OperationsPage } from "./OperationsPage";
import { ReservationsPage } from "./ReservationsPage";
import { ApiError } from "../api/errors";

const inventoryApiClientMock = vi.hoisted(() => ({
  listVendors: vi.fn(),
  createVendor: vi.fn(),
  listProducts: vi.fn(),
  createProduct: vi.fn(),
  listChannels: vi.fn(),
  listChannelSyncStatuses: vi.fn(),
  getObservabilityHealth: vi.fn(),
  getObservabilityMetrics: vi.fn(),
  createChannel: vi.fn(),
  listStockItems: vi.fn(),
  createStockItem: vi.fn(),
  getStockItem: vi.fn(),
  getStockItemAvailability: vi.fn(),
  listReservations: vi.fn(),
  createReservation: vi.fn(),
  getReservation: vi.fn(),
  releaseReservation: vi.fn(),
  expireReservations: vi.fn()
}));

vi.mock("../api", () => ({
  inventoryApiClient: inventoryApiClientMock
}));

afterEach(() => {
  cleanup();
});

function renderWithQueryClient(children: ReactNode) {
  const queryClient = new QueryClient({
    defaultOptions: {
      mutations: { retry: false },
      queries: { retry: false }
    }
  });

  return render(<QueryClientProvider client={queryClient}>{children}</QueryClientProvider>);
}

const vendor = {
  id: "vendor-1",
  name: "Northwind Supply",
  createdAtUtc: "2026-09-21T09:00:00Z"
};
const product = {
  id: "product-1",
  vendorId: "vendor-1",
  sku: "BAT-200",
  name: "Battery pack",
  createdAtUtc: "2026-09-21T09:00:00Z"
};
const channel = {
  id: "channel-1",
  code: "AMAZON",
  name: "Amazon",
  createdAtUtc: "2026-09-21T09:00:00Z"
};
const stockItem = {
  id: "stock-1",
  productId: "product-1",
  channelId: "channel-1",
  onHandQuantity: 10,
  availableQuantity: 8,
  updatedAtUtc: "2026-09-21T09:00:00Z"
};
const reservation = {
  id: "reservation-1",
  stockItemId: "stock-1",
  quantity: 2,
  status: "Active" as const,
  expiresAtUtc: "2026-09-21T12:00:00Z",
  createdAtUtc: "2026-09-21T09:00:00Z",
  updatedAtUtc: "2026-09-21T09:00:00Z",
  releasedAtUtc: null,
  expiredAtUtc: null
};

describe("M6 workflow pages", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    inventoryApiClientMock.listVendors.mockResolvedValue({ items: [vendor] });
    inventoryApiClientMock.listProducts.mockResolvedValue({ items: [product] });
    inventoryApiClientMock.listChannels.mockResolvedValue({ items: [channel] });
    inventoryApiClientMock.listChannelSyncStatuses.mockResolvedValue({
      items: [
        {
          id: "sync-1",
          channelId: "channel-1",
          stockItemId: "stock-1",
          sourceEventId: "event-1",
          availableQuantity: 8,
          status: "Failed",
          attemptCount: 2,
          lastAttemptedAtUtc: "2026-09-21T09:05:00Z",
          nextAttemptAtUtc: "2026-09-21T09:10:00Z",
          lastSucceededAtUtc: null,
          lastError: "Marketplace unavailable",
          updatedAtUtc: "2026-09-21T09:05:00Z"
        }
      ]
    });
    inventoryApiClientMock.getObservabilityHealth.mockResolvedValue({
      status: "Degraded",
      service: "Inventory API",
      checkedAtUtc: "2026-09-21T09:06:00Z",
      traceId: "trace-123",
      correlationId: "corr-123",
      outbox: {
        totalMessages: 4,
        pendingMessages: 1,
        processingMessages: 1,
        publishedMessages: 1,
        failedMessages: 1,
        oldestPendingAtUtc: "2026-09-21T09:00:00Z",
        nextAttemptAtUtc: "2026-09-21T09:10:00Z",
        byStatus: [
          { status: "Pending", count: 1 },
          { status: "Processing", count: 1 },
          { status: "Published", count: 1 },
          { status: "Failed", count: 1 }
        ],
        byEventType: [{ eventType: "StockAvailabilityChanged", status: "Failed", count: 1 }]
      },
      channelSync: {
        totalStates: 1,
        pendingStates: 0,
        inProgressStates: 0,
        succeededStates: 0,
        failedStates: 1,
        retryableFailures: 1,
        nextRetryAtUtc: "2026-09-21T09:10:00Z",
        lastFailureAtUtc: "2026-09-21T09:05:00Z"
      }
    });
    inventoryApiClientMock.listStockItems.mockResolvedValue({ items: [stockItem] });
    inventoryApiClientMock.getStockItem.mockResolvedValue({ stockItem });
    inventoryApiClientMock.getStockItemAvailability.mockResolvedValue({
      availability: {
        stockItemId: "stock-1",
        onHandQuantity: 10,
        reservedQuantity: 2,
        availableQuantity: 8,
        nextExpirationUtc: "2026-09-21T12:00:00Z"
      }
    });
    inventoryApiClientMock.listReservations.mockResolvedValue({ items: [reservation] });
    inventoryApiClientMock.createVendor.mockResolvedValue({ vendor });
    inventoryApiClientMock.createStockItem.mockResolvedValue({ stockItem });
    inventoryApiClientMock.createReservation.mockResolvedValue({ reservation });
    inventoryApiClientMock.releaseReservation.mockResolvedValue({
      reservation: { ...reservation, status: "Released", releasedAtUtc: "2026-09-21T10:00:00Z" }
    });
    inventoryApiClientMock.expireReservations.mockResolvedValue({
      expiredCount: 1,
      items: [{ ...reservation, status: "Expired", expiredAtUtc: "2026-09-21T12:00:00Z" }]
    });
  });

  it("lists inventory master data and creates vendors through the API client", async () => {
    renderWithQueryClient(<InventoryPage />);

    expect((await screen.findAllByText("Northwind Supply"))[0]).toBeVisible();
    expect(screen.getByText("BAT-200")).toBeVisible();
    expect(screen.getAllByText("AMAZON")[0]).toBeVisible();

    fireEvent.change(screen.getByLabelText("Vendor name"), {
      target: { value: "Contoso Supply" }
    });
    fireEvent.click(screen.getByRole("button", { name: "Create vendor" }));

    await waitFor(() => {
      expect(inventoryApiClientMock.createVendor).toHaveBeenCalledWith({
        name: "Contoso Supply"
      });
    });
  });

  it("creates stock items and inspects reservation-aware availability", async () => {
    renderWithQueryClient(<InventoryPage />);

    expect(await screen.findByText("BAT-200")).toBeVisible();
    const stockItems = screen.getByRole("region", { name: "Stock items" });

    fireEvent.change(within(stockItems).getByLabelText("Product"), {
      target: { value: "product-1" }
    });
    fireEvent.change(within(stockItems).getByLabelText("Channel"), {
      target: { value: "channel-1" }
    });
    fireEvent.change(within(stockItems).getByLabelText("On hand"), {
      target: { value: "12" }
    });
    fireEvent.click(within(stockItems).getByRole("button", { name: "Create stock" }));

    await waitFor(() => {
      expect(inventoryApiClientMock.createStockItem).toHaveBeenCalledWith({
        productId: "product-1",
        channelId: "channel-1",
        onHandQuantity: 12
      });
    });

    expect(await screen.findByText("Stock item created.")).toBeVisible();
    expect(await screen.findByLabelText("Selected stock item detail")).toBeVisible();
    expect(screen.getByText("Reserved")).toBeVisible();
    expect(screen.getAllByText("8").length).toBeGreaterThanOrEqual(1);
  });

  it("lists reservations and releases active holds through the API client", async () => {
    renderWithQueryClient(<ReservationsPage />);

    expect(await screen.findByText("Reservation ledger")).toBeVisible();
    expect(await screen.findByText("Active")).toBeVisible();

    fireEvent.click(await screen.findByRole("button", { name: "Release" }));

    await waitFor(() => {
      expect(inventoryApiClientMock.releaseReservation).toHaveBeenCalledWith("reservation-1");
    });
  });

  it("creates reservations and shows oversell conflict feedback", async () => {
    renderWithQueryClient(<ReservationsPage />);

    await screen.findByRole("heading", { name: "Create reservation" });
    const createReservation = screen.getByRole("region", { name: "Create reservation" });
    const stockItemSelect = within(createReservation).getByLabelText("Stock item");
    await waitFor(() => {
      expect(stockItemSelect).not.toBeDisabled();
    });
    fireEvent.change(stockItemSelect, {
      target: { value: "stock-1" }
    });
    await waitFor(() => {
      expect(stockItemSelect).toHaveValue("stock-1");
    });
    fireEvent.change(within(createReservation).getByLabelText("Quantity"), {
      target: { value: "3" }
    });
    const createReservationButton = within(createReservation).getByRole("button", {
      name: "Create reservation"
    });
    const reservationForm = createReservationButton.closest("form");
    expect(reservationForm).not.toBeNull();
    fireEvent.submit(reservationForm!);

    await waitFor(() => {
      expect(inventoryApiClientMock.createReservation).toHaveBeenCalledWith(
        expect.objectContaining({
          stockItemId: "stock-1",
          quantity: 3
        })
      );
    });
    expect(await screen.findByText("Reservation created.")).toBeVisible();

    inventoryApiClientMock.createReservation.mockRejectedValueOnce(
      new ApiError("conflict", "Not enough availability", 409, {
        type: "https://assetflow.local/problems/insufficient-availability",
        title: "Insufficient availability",
        status: 409,
        detail: "Only 8 units are currently available."
      })
    );

    fireEvent.change(within(createReservation).getByLabelText("Quantity"), {
      target: { value: "99" }
    });
    fireEvent.submit(reservationForm!);

    expect(await screen.findByText("This change conflicts with current inventory")).toBeVisible();
    expect(await screen.findByText("Only 8 units are currently available.")).toBeVisible();
  });

  it("disables unavailable reservation actions and reports expiration results", async () => {
    inventoryApiClientMock.listReservations.mockResolvedValue({
      items: [{ ...reservation, status: "Released" }]
    });

    renderWithQueryClient(<ReservationsPage />);

    const releaseButton = await screen.findByRole("button", { name: "Release" });
    expect(releaseButton).toBeDisabled();

    fireEvent.click(screen.getByRole("button", { name: "Expire reservations" }));

    await waitFor(() => {
      expect(inventoryApiClientMock.expireReservations).toHaveBeenCalledWith({});
    });
    expect(await screen.findByText("Expiration complete")).toBeVisible();
  });

  it("disables reservation creation when no stock item is available", async () => {
    inventoryApiClientMock.listStockItems.mockResolvedValue({ items: [] });
    inventoryApiClientMock.listReservations.mockResolvedValue({ items: [] });

    renderWithQueryClient(<ReservationsPage />);

    expect(await screen.findByRole("button", { name: "Create reservation" })).toBeDisabled();
    expect(
      screen.getByText("Choose a stock item to inspect reservation-aware availability.")
    ).toBeVisible();
  });

  it("shows operations sync health from backend status", async () => {
    renderWithQueryClient(<OperationsPage />);

    expect(await screen.findByText("Channel synchronization")).toBeVisible();
    expect(await screen.findByText("Service observability")).toBeVisible();
    expect(await screen.findByText("Outbox backlog")).toBeVisible();
    expect(await screen.findByText("Sync failures")).toBeVisible();
    expect(await screen.findByText("trace-123")).toBeVisible();
    expect(await screen.findByText("corr-123")).toBeVisible();
    expect(await screen.findByText("AMAZON")).toBeVisible();
    expect(await screen.findByText("Failed")).toBeVisible();
    expect(await screen.findByText("Marketplace unavailable")).toBeVisible();
    expect(screen.getByText("8")).toBeVisible();
    expect(screen.getAllByText("2").length).toBeGreaterThanOrEqual(1);

    await waitFor(() => {
      expect(inventoryApiClientMock.listChannelSyncStatuses).toHaveBeenCalled();
      expect(inventoryApiClientMock.getObservabilityHealth).toHaveBeenCalled();
    });
  });

  it("shows channels with no sync state as not synced", async () => {
    inventoryApiClientMock.listChannelSyncStatuses.mockResolvedValue({ items: [] });

    renderWithQueryClient(<OperationsPage />);

    expect(await screen.findByText("No sync yet")).toBeVisible();
    expect(await screen.findByText("Not synced")).toBeVisible();
    expect(await screen.findByText("None scheduled")).toBeVisible();
  });
});
