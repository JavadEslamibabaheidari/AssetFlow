import { describe, expect, it, vi } from "vitest";
import { ApiHttpClient } from "./http";

describe("ApiHttpClient", () => {
  it("builds query strings and returns JSON responses", async () => {
    const fetcher = vi.fn<typeof fetch>().mockResolvedValue(
      new Response(JSON.stringify({ items: [] }), {
        status: 200,
        headers: { "content-type": "application/json" }
      })
    );
    const client = new ApiHttpClient("http://localhost:8080", fetcher);

    const result = await client.request<{ items: unknown[] }>("/products", {
      query: { vendorId: "vendor-1" }
    });

    expect(result.items).toEqual([]);
    expect(fetcher).toHaveBeenCalledWith(
      "http://localhost:8080/products?vendorId=vendor-1",
      expect.objectContaining({ method: "GET" })
    );
  });

  it("maps Problem Details responses to ApiError", async () => {
    const fetcher = vi.fn<typeof fetch>().mockResolvedValue(
      new Response(JSON.stringify({ title: "Not found", status: 404 }), {
        status: 404,
        statusText: "Not Found",
        headers: { "content-type": "application/json" }
      })
    );
    const client = new ApiHttpClient("http://localhost:8080", fetcher);

    await expect(client.request("/missing")).rejects.toMatchObject({
      kind: "notFound",
      status: 404,
      problem: { title: "Not found", status: 404 }
    });
  });
});
