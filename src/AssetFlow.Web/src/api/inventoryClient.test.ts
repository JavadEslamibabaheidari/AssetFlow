import { describe, expect, it, vi } from "vitest";
import { ApiHttpClient } from "./http";
import { InventoryApiClient } from "./inventoryClient";

function createJsonResponse(body: unknown, status = 200): Response {
  return new Response(JSON.stringify(body), {
    status,
    headers: { "content-type": "application/json" }
  });
}

describe("InventoryApiClient", () => {
  it("posts typed create requests with JSON bodies", async () => {
    const fetcher = vi.fn<typeof fetch>().mockResolvedValue(
      createJsonResponse(
        {
          vendor: {
            id: "vendor-1",
            name: "Northwind Supply",
            createdAtUtc: "2026-09-21T09:00:00Z"
          }
        },
        201
      )
    );
    const client = new InventoryApiClient(new ApiHttpClient("http://localhost:8080", fetcher));

    const response = await client.createVendor({ name: "Northwind Supply" });

    expect(response.vendor.name).toBe("Northwind Supply");
    expect(fetcher).toHaveBeenCalledWith(
      "http://localhost:8080/vendors",
      expect.objectContaining({
        method: "POST",
        body: JSON.stringify({ name: "Northwind Supply" })
      })
    );
  });

  it("encodes detail route identifiers", async () => {
    const fetcher = vi.fn<typeof fetch>().mockResolvedValue(
      createJsonResponse({
        stockItem: {
          id: "stock item/1",
          productId: "product-1",
          channelId: "channel-1",
          onHandQuantity: 10,
          availableQuantity: 8,
          createdAtUtc: "2026-09-21T09:00:00Z",
          updatedAtUtc: "2026-09-21T09:00:00Z"
        }
      })
    );
    const client = new InventoryApiClient(new ApiHttpClient("http://localhost:8080", fetcher));

    await client.getStockItem("stock item/1");

    expect(fetcher).toHaveBeenCalledWith(
      "http://localhost:8080/stock-items/stock%20item%2F1",
      expect.objectContaining({ method: "GET" })
    );
  });

  it("passes reservation filters and release commands to backend routes", async () => {
    const fetcher = vi
      .fn<typeof fetch>()
      .mockResolvedValueOnce(createJsonResponse({ items: [] }))
      .mockResolvedValueOnce(
        createJsonResponse({
          reservation: {
            id: "reservation-1",
            stockItemId: "stock-1",
            quantity: 2,
            status: "Released",
            expiresAtUtc: "2026-09-21T10:00:00Z",
            createdAtUtc: "2026-09-21T09:00:00Z",
            updatedAtUtc: "2026-09-21T09:05:00Z",
            releasedAtUtc: "2026-09-21T09:05:00Z",
            expiredAtUtc: null
          }
        })
      );
    const client = new InventoryApiClient(new ApiHttpClient("http://localhost:8080", fetcher));

    await client.listReservations({ stockItemId: "stock-1", status: "Active" });
    await client.releaseReservation("reservation-1");

    expect(fetcher).toHaveBeenNthCalledWith(
      1,
      "http://localhost:8080/reservations?stockItemId=stock-1&status=Active",
      expect.objectContaining({ method: "GET" })
    );
    expect(fetcher).toHaveBeenNthCalledWith(
      2,
      "http://localhost:8080/reservations/reservation-1/release",
      expect.objectContaining({ method: "POST" })
    );
  });

  it("posts expiration requests even when the cutoff is omitted", async () => {
    const fetcher = vi.fn<typeof fetch>().mockResolvedValue(
      createJsonResponse({
        expiredCount: 0,
        items: []
      })
    );
    const client = new InventoryApiClient(new ApiHttpClient("http://localhost:8080", fetcher));

    await client.expireReservations();

    expect(fetcher).toHaveBeenCalledWith(
      "http://localhost:8080/reservations/expire",
      expect.objectContaining({
        method: "POST",
        body: JSON.stringify({})
      })
    );
  });
});
