import type { operations } from "./generated/inventory-api";
import { ApiHttpClient } from "./http";

type JsonResponse<TOperation extends keyof operations, TStatus extends keyof operations[TOperation]["responses"]> =
  operations[TOperation]["responses"][TStatus] extends {
    content: { "application/json": infer TResponse };
  }
    ? TResponse
    : never;

type ListProductsQuery = NonNullable<operations["listProducts"]["parameters"]["query"]>;
type ListStockItemsQuery = NonNullable<operations["listStockItems"]["parameters"]["query"]>;
type ListReservationsQuery = NonNullable<operations["listReservations"]["parameters"]["query"]>;

export class InventoryApiClient {
  private readonly http: ApiHttpClient;

  public constructor(http = new ApiHttpClient()) {
    this.http = http;
  }

  public listVendors(signal?: AbortSignal): Promise<JsonResponse<"listVendors", 200>> {
    return this.http.request("/vendors", { signal });
  }

  public listProducts(
    query: ListProductsQuery = {},
    signal?: AbortSignal
  ): Promise<JsonResponse<"listProducts", 200>> {
    return this.http.request("/products", { query, signal });
  }

  public listChannels(signal?: AbortSignal): Promise<JsonResponse<"listChannels", 200>> {
    return this.http.request("/channels", { signal });
  }

  public listStockItems(
    query: ListStockItemsQuery = {},
    signal?: AbortSignal
  ): Promise<JsonResponse<"listStockItems", 200>> {
    return this.http.request("/stock-items", { query, signal });
  }

  public getStockItemAvailability(
    stockItemId: string,
    signal?: AbortSignal
  ): Promise<JsonResponse<"getStockItemAvailability", 200>> {
    return this.http.request(`/stock-items/${encodeURIComponent(stockItemId)}/availability`, {
      signal
    });
  }

  public listReservations(
    query: ListReservationsQuery = {},
    signal?: AbortSignal
  ): Promise<JsonResponse<"listReservations", 200>> {
    return this.http.request("/reservations", { query, signal });
  }
}

export const inventoryApiClient = new InventoryApiClient();
