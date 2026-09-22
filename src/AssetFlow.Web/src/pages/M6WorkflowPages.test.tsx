import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { cleanup, fireEvent, render, screen, waitFor } from "@testing-library/react";
import type { ReactNode } from "react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { InventoryPage } from "./InventoryPage";
import { OperationsPage } from "./OperationsPage";
import { ReservationsPage } from "./ReservationsPage";

const inventoryApiClientMock = vi.hoisted(() => ({
  listVendors: vi.fn(),
  createVendor: vi.fn(),
  listProducts: vi.fn(),
  createProduct: vi.fn(),
  listChannels: vi.fn(),
  listChannelSyncStatuses: vi.fn(),
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
    inventoryApiClientMock.releaseReservation.mockResolvedValue({
      reservation: { ...reservation, status: "Released", releasedAtUtc: "2026-09-21T10:00:00Z" }
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

  it("lists reservations and releases active holds through the API client", async () => {
    renderWithQueryClient(<ReservationsPage />);

    expect(await screen.findByText("Reservation ledger")).toBeVisible();
    expect(await screen.findByText("Active")).toBeVisible();

    fireEvent.click(await screen.findByRole("button", { name: "Release" }));

    await waitFor(() => {
      expect(inventoryApiClientMock.releaseReservation).toHaveBeenCalledWith("reservation-1");
    });
  });

  it("shows operations sync health from backend status", async () => {
    renderWithQueryClient(<OperationsPage />);

    expect(await screen.findByText("Channel synchronization")).toBeVisible();
    expect(await screen.findByText("AMAZON")).toBeVisible();
    expect(await screen.findByText("Failed")).toBeVisible();
    expect(await screen.findByText("Marketplace unavailable")).toBeVisible();
    expect(screen.getByText("8")).toBeVisible();
    expect(screen.getByText("2")).toBeVisible();

    await waitFor(() => {
      expect(inventoryApiClientMock.listChannelSyncStatuses).toHaveBeenCalled();
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
