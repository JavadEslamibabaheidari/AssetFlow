import type { operations } from "./generated/inventory-api";
import { ApiHttpClient } from "./http";

export type InventoryApiResponse<
  TOperation extends keyof operations,
  TStatus extends keyof operations[TOperation]["responses"]
> = operations[TOperation]["responses"][TStatus] extends {
  content: { "application/json": infer TResponse };
}
  ? TResponse
  : never;

export type InventoryApiRequest<TOperation extends keyof operations> =
  NonNullable<operations[TOperation]["requestBody"]> extends {
    content: { "application/json": infer TRequest };
  }
    ? TRequest
    : never;

type ListProductsQuery = NonNullable<operations["listProducts"]["parameters"]["query"]>;
type ListStockItemsQuery = NonNullable<operations["listStockItems"]["parameters"]["query"]>;
type ListReservationsQuery = NonNullable<operations["listReservations"]["parameters"]["query"]>;

export class InventoryApiClient {
  private readonly http: ApiHttpClient;

  public constructor(http = new ApiHttpClient()) {
    this.http = http;
  }

  public listVendors(signal?: AbortSignal): Promise<InventoryApiResponse<"listVendors", 200>> {
    return this.http.request("/vendors", { signal });
  }

  public createVendor(
    body: InventoryApiRequest<"createVendor">,
    signal?: AbortSignal
  ): Promise<InventoryApiResponse<"createVendor", 201>> {
    return this.http.request("/vendors", { method: "POST", body, signal });
  }

  public getVendor(
    vendorId: string,
    signal?: AbortSignal
  ): Promise<InventoryApiResponse<"getVendor", 200>> {
    return this.http.request(`/vendors/${encodeURIComponent(vendorId)}`, { signal });
  }

  public listProducts(
    query: ListProductsQuery = {},
    signal?: AbortSignal
  ): Promise<InventoryApiResponse<"listProducts", 200>> {
    return this.http.request("/products", { query, signal });
  }

  public createProduct(
    body: InventoryApiRequest<"createProduct">,
    signal?: AbortSignal
  ): Promise<InventoryApiResponse<"createProduct", 201>> {
    return this.http.request("/products", { method: "POST", body, signal });
  }

  public getProduct(
    productId: string,
    signal?: AbortSignal
  ): Promise<InventoryApiResponse<"getProduct", 200>> {
    return this.http.request(`/products/${encodeURIComponent(productId)}`, { signal });
  }

  public listChannels(signal?: AbortSignal): Promise<InventoryApiResponse<"listChannels", 200>> {
    return this.http.request("/channels", { signal });
  }

  public createChannel(
    body: InventoryApiRequest<"createChannel">,
    signal?: AbortSignal
  ): Promise<InventoryApiResponse<"createChannel", 201>> {
    return this.http.request("/channels", { method: "POST", body, signal });
  }

  public listStockItems(
    query: ListStockItemsQuery = {},
    signal?: AbortSignal
  ): Promise<InventoryApiResponse<"listStockItems", 200>> {
    return this.http.request("/stock-items", { query, signal });
  }

  public createStockItem(
    body: InventoryApiRequest<"createStockItem">,
    signal?: AbortSignal
  ): Promise<InventoryApiResponse<"createStockItem", 201>> {
    return this.http.request("/stock-items", { method: "POST", body, signal });
  }

  public getStockItem(
    stockItemId: string,
    signal?: AbortSignal
  ): Promise<InventoryApiResponse<"getStockItem", 200>> {
    return this.http.request(`/stock-items/${encodeURIComponent(stockItemId)}`, { signal });
  }

  public getStockItemAvailability(
    stockItemId: string,
    signal?: AbortSignal
  ): Promise<InventoryApiResponse<"getStockItemAvailability", 200>> {
    return this.http.request(`/stock-items/${encodeURIComponent(stockItemId)}/availability`, {
      signal
    });
  }

  public listReservations(
    query: ListReservationsQuery = {},
    signal?: AbortSignal
  ): Promise<InventoryApiResponse<"listReservations", 200>> {
    return this.http.request("/reservations", { query, signal });
  }

  public createReservation(
    body: InventoryApiRequest<"createReservation">,
    signal?: AbortSignal
  ): Promise<InventoryApiResponse<"createReservation", 201>> {
    return this.http.request("/reservations", { method: "POST", body, signal });
  }

  public expireReservations(
    body: InventoryApiRequest<"expireReservations"> = {},
    signal?: AbortSignal
  ): Promise<InventoryApiResponse<"expireReservations", 200>> {
    return this.http.request("/reservations/expire", { method: "POST", body, signal });
  }

  public getReservation(
    reservationId: string,
    signal?: AbortSignal
  ): Promise<InventoryApiResponse<"getReservation", 200>> {
    return this.http.request(`/reservations/${encodeURIComponent(reservationId)}`, { signal });
  }

  public releaseReservation(
    reservationId: string,
    signal?: AbortSignal
  ): Promise<InventoryApiResponse<"releaseReservation", 200>> {
    return this.http.request(`/reservations/${encodeURIComponent(reservationId)}/release`, {
      method: "POST",
      signal
    });
  }
}

export const inventoryApiClient = new InventoryApiClient();
