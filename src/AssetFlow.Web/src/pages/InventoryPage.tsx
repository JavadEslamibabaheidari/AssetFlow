import { type CSSProperties, FormEvent, ReactNode, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Boxes, Building2, PackagePlus, Plus, Store, Warehouse } from "lucide-react";
import {
  ApiErrorFeedback,
  WorkflowEmptyState,
  WorkflowFeedback,
  WorkflowLoadingState
} from "../shared/workflowStates";
import { PageHeader, StatusBadge } from "../design-system";
import { inventoryApiClient } from "../api";
import type { components } from "../api/generated/inventory-api";

type Product = components["schemas"]["Product"];
type Channel = components["schemas"]["Channel"];
type StockItem = components["schemas"]["StockItem"];
type StockItemAvailability = components["schemas"]["StockItemAvailability"];

const inventoryKeys = {
  vendors: ["vendors"] as const,
  products: ["products"] as const,
  channels: ["channels"] as const,
  stockItems: ["stock-items"] as const,
  stockItem: (id: string) => ["stock-items", id] as const,
  availability: (id: string) => ["stock-items", id, "availability"] as const
};

export function InventoryPage() {
  const queryClient = useQueryClient();
  const [selectedStockItemId, setSelectedStockItemId] = useState<string>("");
  const [success, setSuccess] = useState<string>("");

  const vendors = useQuery({
    queryKey: inventoryKeys.vendors,
    queryFn: ({ signal }) =>
      inventoryApiClient.listVendors(signal).then((response) => response.items)
  });
  const products = useQuery({
    queryKey: inventoryKeys.products,
    queryFn: ({ signal }) =>
      inventoryApiClient.listProducts({}, signal).then((response) => response.items)
  });
  const channels = useQuery({
    queryKey: inventoryKeys.channels,
    queryFn: ({ signal }) =>
      inventoryApiClient.listChannels(signal).then((response) => response.items)
  });
  const stockItems = useQuery({
    queryKey: inventoryKeys.stockItems,
    queryFn: ({ signal }) =>
      inventoryApiClient.listStockItems({}, signal).then((response) => response.items)
  });
  const selectedStockItem = useQuery({
    enabled: Boolean(selectedStockItemId),
    queryKey: inventoryKeys.stockItem(selectedStockItemId),
    queryFn: ({ signal }) =>
      inventoryApiClient
        .getStockItem(selectedStockItemId, signal)
        .then((response) => response.stockItem)
  });
  const selectedAvailability = useQuery({
    enabled: Boolean(selectedStockItemId),
    queryKey: inventoryKeys.availability(selectedStockItemId),
    queryFn: ({ signal }) =>
      inventoryApiClient
        .getStockItemAvailability(selectedStockItemId, signal)
        .then((response) => response.availability)
  });

  const invalidateInventory = async () => {
    await Promise.all([
      queryClient.invalidateQueries({ queryKey: inventoryKeys.vendors }),
      queryClient.invalidateQueries({ queryKey: inventoryKeys.products }),
      queryClient.invalidateQueries({ queryKey: inventoryKeys.channels }),
      queryClient.invalidateQueries({ queryKey: inventoryKeys.stockItems })
    ]);
  };

  const createVendor = useMutation({
    mutationFn: (body: { name: string }) => inventoryApiClient.createVendor(body),
    onSuccess: async ({ vendor }) => {
      setSuccess(`Vendor ${vendor.name} created.`);
      await invalidateInventory();
    }
  });
  const createProduct = useMutation({
    mutationFn: (body: { vendorId: string; sku: string; name: string }) =>
      inventoryApiClient.createProduct(body),
    onSuccess: async ({ product }) => {
      setSuccess(`Product ${product.sku} created.`);
      await invalidateInventory();
    }
  });
  const createChannel = useMutation({
    mutationFn: (body: { code: string; name: string }) => inventoryApiClient.createChannel(body),
    onSuccess: async ({ channel }) => {
      setSuccess(`Channel ${channel.code} created.`);
      await invalidateInventory();
    }
  });
  const createStockItem = useMutation({
    mutationFn: (body: { productId: string; channelId: string; onHandQuantity: number }) =>
      inventoryApiClient.createStockItem(body),
    onSuccess: async ({ stockItem }) => {
      setSelectedStockItemId(stockItem.id);
      setSuccess("Stock item created.");
      await invalidateInventory();
      await queryClient.invalidateQueries({ queryKey: inventoryKeys.availability(stockItem.id) });
    }
  });

  const isLoading =
    vendors.isLoading || products.isLoading || channels.isLoading || stockItems.isLoading;
  const firstError = vendors.error ?? products.error ?? channels.error ?? stockItems.error;

  const vendorItems = vendors.data ?? [];
  const productItems = products.data ?? [];
  const channelItems = channels.data ?? [];
  const stockItemRows = stockItems.data ?? [];

  return (
    <section className="page-section inventory-workspace" aria-labelledby="inventory-title">
      <PageHeader
        eyebrow="Assets"
        title="Products and warehouse stock"
        titleId="inventory-title"
        description="Create sellable inventory records, connect them to channels, and inspect reservation-aware availability from the backend contract."
        compact
      />

      {success ? <WorkflowFeedback kind="success" title={success} /> : null}
      {firstError ? <ApiErrorFeedback error={firstError} /> : null}
      {isLoading ? (
        <WorkflowLoadingState
          title="Loading inventory"
          description="Reading vendors, products, channels, and stock items."
        />
      ) : null}

      <div className="asset-summary" aria-label="Asset registry summary">
        <RegistryStat icon={Building2} label="Vendors" value={vendorItems.length} />
        <RegistryStat icon={Boxes} label="Products" value={productItems.length} />
        <RegistryStat icon={Store} label="Channels" value={channelItems.length} />
        <RegistryStat icon={Warehouse} label="Stock records" value={stockItemRows.length} />
      </div>

      <div className="workflow-grid asset-catalog-grid">
        <WorkflowPanel
          title="Vendors"
          description="Supplier ownership for products."
          icon={Building2}
        >
          <form
            className="workflow-form"
            onSubmit={(event) => handleVendorSubmit(event, createVendor.mutate)}
          >
            <label>
              Vendor name
              <input name="name" placeholder="Northwind Supply" required />
            </label>
            <button type="submit" disabled={createVendor.isPending}>
              <Plus size={16} aria-hidden="true" />
              Create vendor
            </button>
          </form>
          <DataList
            emptyTitle="No vendors yet"
            emptyDescription="Create a vendor before adding products."
            items={vendorItems}
            renderItem={(vendor) => (
              <>
                <strong>{vendor.name}</strong>
                <span>{formatDate(vendor.createdAtUtc)}</span>
              </>
            )}
          />
          {createVendor.error ? <ApiErrorFeedback error={createVendor.error} /> : null}
        </WorkflowPanel>

        <WorkflowPanel
          title="Products"
          description="Catalog records with vendor-owned SKUs."
          icon={Boxes}
        >
          <form
            className="workflow-form"
            onSubmit={(event) => handleProductSubmit(event, createProduct.mutate)}
          >
            <label>
              Vendor
              <select name="vendorId" required disabled={vendorItems.length === 0}>
                <option value="">Select vendor</option>
                {vendorItems.map((vendor) => (
                  <option key={vendor.id} value={vendor.id}>
                    {vendor.name}
                  </option>
                ))}
              </select>
            </label>
            <label>
              SKU
              <input name="sku" placeholder="BAT-200" required />
            </label>
            <label>
              Product name
              <input name="name" placeholder="Battery pack" required />
            </label>
            <button type="submit" disabled={createProduct.isPending || vendorItems.length === 0}>
              <Plus size={16} aria-hidden="true" />
              Create product
            </button>
          </form>
          <DataList
            emptyTitle="No products yet"
            emptyDescription="Products appear here after a vendor owns a SKU."
            items={productItems}
            renderItem={(product) => (
              <>
                <strong>{product.sku}</strong>
                <span>{product.name}</span>
              </>
            )}
          />
          {createProduct.error ? <ApiErrorFeedback error={createProduct.error} /> : null}
        </WorkflowPanel>

        <WorkflowPanel
          title="Sales channels"
          description="Markets where stock can be listed."
          icon={Store}
        >
          <form
            className="workflow-form"
            onSubmit={(event) => handleChannelSubmit(event, createChannel.mutate)}
          >
            <label>
              Channel code
              <input name="code" placeholder="AMAZON" required />
            </label>
            <label>
              Channel name
              <input name="name" placeholder="Amazon" required />
            </label>
            <button type="submit" disabled={createChannel.isPending}>
              <Plus size={16} aria-hidden="true" />
              Create channel
            </button>
          </form>
          <DataList
            emptyTitle="No channels yet"
            emptyDescription="Create a sales channel before creating channel stock."
            items={channelItems}
            renderItem={(channel) => (
              <>
                <strong>{channel.code}</strong>
                <span>{channel.name}</span>
              </>
            )}
          />
          {createChannel.error ? <ApiErrorFeedback error={createChannel.error} /> : null}
        </WorkflowPanel>

        <WorkflowPanel
          title="Stock items"
          description="On-hand stock by product and sales channel."
          icon={PackagePlus}
        >
          <form
            className="workflow-form"
            onSubmit={(event) => handleStockSubmit(event, createStockItem.mutate)}
          >
            <label>
              Product
              <select name="productId" required disabled={productItems.length === 0}>
                <option value="">Select product</option>
                {productItems.map((product) => (
                  <option key={product.id} value={product.id}>
                    {product.sku} - {product.name}
                  </option>
                ))}
              </select>
            </label>
            <label>
              Channel
              <select name="channelId" required disabled={channelItems.length === 0}>
                <option value="">Select channel</option>
                {channelItems.map((channel) => (
                  <option key={channel.id} value={channel.id}>
                    {channel.code}
                  </option>
                ))}
              </select>
            </label>
            <label>
              On hand
              <input name="onHandQuantity" type="number" min="0" step="1" required />
            </label>
            <button
              type="submit"
              disabled={
                createStockItem.isPending || productItems.length === 0 || channelItems.length === 0
              }
            >
              <Plus size={16} aria-hidden="true" />
              Create stock
            </button>
          </form>
          {createStockItem.error ? <ApiErrorFeedback error={createStockItem.error} /> : null}
        </WorkflowPanel>
      </div>

      <section className="workflow-panel full-span" aria-labelledby="stock-table-title">
        <div className="workflow-panel-heading">
          <div>
            <h3 id="stock-table-title">Stock and availability</h3>
            <p>
              Inspect backend-calculated availability before creating or releasing reservations.
            </p>
          </div>
          <StatusBadge>{stockItemRows.length} stock records</StatusBadge>
        </div>

        {stockItemRows.length === 0 ? (
          <WorkflowEmptyState
            title="No stock items yet"
            description="Create product and channel stock to inspect reservation-aware availability."
          />
        ) : (
          <div className="table-wrap">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Product</th>
                  <th>Channel</th>
                  <th>On hand</th>
                  <th>Available</th>
                  <th>Updated</th>
                  <th>Action</th>
                </tr>
              </thead>
              <tbody>
                {stockItemRows.map((stockItem) => (
                  <tr key={stockItem.id}>
                    <td>{findProductLabel(productItems, stockItem.productId)}</td>
                    <td>{findChannelLabel(channelItems, stockItem.channelId)}</td>
                    <td>{stockItem.onHandQuantity}</td>
                    <td>
                      <StatusBadge tone={stockItem.availableQuantity > 0 ? "success" : "warning"}>
                        {stockItem.availableQuantity}
                      </StatusBadge>
                    </td>
                    <td>{formatDate(stockItem.updatedAtUtc)}</td>
                    <td>
                      <button type="button" onClick={() => setSelectedStockItemId(stockItem.id)}>
                        Inspect
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {selectedStockItemId ? (
          <StockItemDetail
            stockItem={selectedStockItem.data}
            availability={selectedAvailability.data}
            loading={selectedStockItem.isLoading || selectedAvailability.isLoading}
            error={selectedStockItem.error ?? selectedAvailability.error}
            products={productItems}
            channels={channelItems}
          />
        ) : null}
      </section>
    </section>
  );
}

type WorkflowPanelProps = {
  title: string;
  description: string;
  children: ReactNode;
  icon: typeof Boxes;
};

function WorkflowPanel({ title, description, children, icon: Icon }: WorkflowPanelProps) {
  return (
    <section className="workflow-panel" aria-labelledby={`${slugify(title)}-title`}>
      <div className="workflow-panel-heading">
        <span className="workflow-panel-icon" aria-hidden="true">
          <Icon size={18} />
        </span>
        <div className="workflow-panel-copy">
          <h3 id={`${slugify(title)}-title`}>{title}</h3>
          <p>{description}</p>
        </div>
      </div>
      {children}
    </section>
  );
}

function RegistryStat({
  icon: Icon,
  label,
  value
}: {
  icon: typeof Boxes;
  label: string;
  value: number;
}) {
  return (
    <div className="registry-stat">
      <Icon size={17} aria-hidden="true" />
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  );
}

type DataListProps<TItem> = {
  emptyTitle: string;
  emptyDescription: string;
  items: TItem[];
  renderItem: (item: TItem) => ReactNode;
};

function DataList<TItem extends { id: string }>({
  emptyTitle,
  emptyDescription,
  items,
  renderItem
}: DataListProps<TItem>) {
  if (items.length === 0) {
    return <WorkflowEmptyState title={emptyTitle} description={emptyDescription} />;
  }

  return (
    <ul className="compact-list">
      {items.slice(0, 6).map((item) => (
        <li key={item.id}>{renderItem(item)}</li>
      ))}
    </ul>
  );
}

function StockItemDetail({
  stockItem,
  availability,
  loading,
  error,
  products,
  channels
}: {
  stockItem?: StockItem;
  availability?: StockItemAvailability;
  loading: boolean;
  error: unknown;
  products: Product[];
  channels: Channel[];
}) {
  if (loading) {
    return <WorkflowLoadingState title="Loading stock detail" />;
  }

  if (error) {
    return <ApiErrorFeedback error={error} />;
  }

  if (!stockItem || !availability) {
    return null;
  }

  const reservedShare =
    availability.onHandQuantity === 0
      ? 0
      : Math.min(100, (availability.reservedQuantity / availability.onHandQuantity) * 100);
  const orbitStyle = { "--reserved-share": `${reservedShare}%` } as CSSProperties;

  return (
    <div className="asset-inspector" aria-label="Selected stock item detail">
      <div className="availability-orbit" style={orbitStyle} aria-hidden="true">
        <span>
          <strong>{availability.availableQuantity}</strong>
          available
        </span>
      </div>
      <div className="detail-grid">
        <div>
          <span>Product</span>
          <strong>{findProductLabel(products, stockItem.productId)}</strong>
        </div>
        <div>
          <span>Channel</span>
          <strong>{findChannelLabel(channels, stockItem.channelId)}</strong>
        </div>
        <div>
          <span>On hand</span>
          <strong>{availability.onHandQuantity}</strong>
        </div>
        <div>
          <span>Reserved</span>
          <strong>{availability.reservedQuantity}</strong>
        </div>
        <div>
          <span>Available</span>
          <strong>{availability.availableQuantity}</strong>
        </div>
        <div>
          <span>Next expiration</span>
          <strong>
            {availability.nextExpirationUtc ? formatDate(availability.nextExpirationUtc) : "None"}
          </strong>
        </div>
      </div>
    </div>
  );
}

function handleVendorSubmit(
  event: FormEvent<HTMLFormElement>,
  submit: (body: { name: string }) => void
) {
  event.preventDefault();
  const form = event.currentTarget;
  const data = new FormData(form);
  submit({ name: String(data.get("name") ?? "").trim() });
  form.reset();
}

function handleProductSubmit(
  event: FormEvent<HTMLFormElement>,
  submit: (body: { vendorId: string; sku: string; name: string }) => void
) {
  event.preventDefault();
  const form = event.currentTarget;
  const data = new FormData(form);
  submit({
    vendorId: String(data.get("vendorId") ?? ""),
    sku: String(data.get("sku") ?? "").trim(),
    name: String(data.get("name") ?? "").trim()
  });
  form.reset();
}

function handleChannelSubmit(
  event: FormEvent<HTMLFormElement>,
  submit: (body: { code: string; name: string }) => void
) {
  event.preventDefault();
  const form = event.currentTarget;
  const data = new FormData(form);
  submit({
    code: String(data.get("code") ?? "").trim(),
    name: String(data.get("name") ?? "").trim()
  });
  form.reset();
}

function handleStockSubmit(
  event: FormEvent<HTMLFormElement>,
  submit: (body: { productId: string; channelId: string; onHandQuantity: number }) => void
) {
  event.preventDefault();
  const form = event.currentTarget;
  const data = new FormData(form);
  submit({
    productId: String(data.get("productId") ?? ""),
    channelId: String(data.get("channelId") ?? ""),
    onHandQuantity: Number(data.get("onHandQuantity") ?? 0)
  });
  form.reset();
}

function findProductLabel(products: Product[], productId: string): string {
  const product = products.find((item) => item.id === productId);
  return product ? `${product.sku} - ${product.name}` : productId;
}

function findChannelLabel(channels: Channel[], channelId: string): string {
  const channel = channels.find((item) => item.id === channelId);
  return channel ? channel.code : channelId;
}

function formatDate(value: string): string {
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short"
  }).format(new Date(value));
}

function slugify(value: string): string {
  return value
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/(^-|-$)/g, "");
}
