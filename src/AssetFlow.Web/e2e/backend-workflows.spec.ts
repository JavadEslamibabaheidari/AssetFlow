import { expect, test, type APIRequestContext, type Page } from "@playwright/test";

const apiBaseUrl = "http://localhost:8080";

test("assets buttons create vendor, product, channel, and stock records", async ({ page }) => {
  const suffix = Date.now().toString();
  const vendorName = `Browser Vendor ${suffix}`;
  const sku = `BROWSER-${suffix}`;
  const productName = "Browser scanner";
  const channelCode = `browser-${suffix}`;
  const channelName = "Browser Marketplace";

  await page.goto("/inventory");
  await expect(page.getByRole("heading", { name: "Products and warehouse stock" })).toBeVisible();
  await expectApiReachable(page);

  const vendors = page.getByRole("region", { name: "Vendors" });
  await vendors.getByRole("textbox", { name: "Vendor name" }).fill(vendorName);
  await vendors.getByRole("button", { name: "Create vendor" }).click();
  await expect(page.getByText(`Vendor ${vendorName} created.`)).toBeVisible();
  await expect(vendors.getByText(vendorName)).toBeVisible();

  const products = page.getByRole("region", { name: "Products" });
  await products.getByRole("combobox", { name: "Vendor" }).selectOption({ label: vendorName });
  await products.getByRole("textbox", { name: "SKU" }).fill(sku);
  await products.getByRole("textbox", { name: "Product name" }).fill(productName);
  await products.getByRole("button", { name: "Create product" }).click();
  await expect(page.getByText(`Product ${sku} created.`)).toBeVisible();
  await expect(products.getByRole("listitem").filter({ hasText: sku })).toBeVisible();

  const channels = page.getByRole("region", { name: "Sales channels" });
  await channels.getByRole("textbox", { name: "Channel code" }).fill(channelCode);
  await channels.getByRole("textbox", { name: "Channel name" }).fill(channelName);
  await channels.getByRole("button", { name: "Create channel" }).click();
  await expect(page.getByText(`Channel ${channelCode} created.`)).toBeVisible();
  await expect(channels.getByRole("listitem").filter({ hasText: channelCode })).toBeVisible();

  const stockItems = page.getByRole("region", { name: "Stock items" });
  await stockItems
    .getByRole("combobox", { name: "Product" })
    .selectOption({ label: `${sku} - ${productName}` });
  await stockItems.getByRole("combobox", { name: "Channel" }).selectOption({ label: channelCode });
  await stockItems.getByRole("spinbutton", { name: "On hand" }).fill("9");
  await stockItems.getByRole("button", { name: "Create stock" }).click();
  await expect(page.getByText("Stock item created.")).toBeVisible();

  const stockTable = page.getByRole("region", { name: "Stock and availability" });
  const stockRow = stockTable.getByRole("row").filter({ hasText: `${sku} - ${productName}` });
  await expect(stockRow.getByRole("cell", { name: `${sku} - ${productName}` })).toBeVisible();
  await expect(stockRow.getByRole("cell", { name: channelCode, exact: true })).toBeVisible();
});

test("reservation buttons create, inspect, release, and expire holds", async ({
  page,
  request
}) => {
  const seeded = await seedStockItem(request, "reservation");

  await page.goto("/reservations");
  await expect(page.getByRole("heading", { name: "Reserved stock" })).toBeVisible();
  await expectApiReachable(page);

  const createReservation = page.getByRole("region", { name: "Create reservation" });
  await createReservation
    .getByRole("combobox", { name: "Stock item" })
    .selectOption({ label: seeded.stockLabel });
  await createReservation.getByRole("spinbutton", { name: "Quantity" }).fill("3");
  await createReservation.getByRole("button", { name: "Create reservation" }).click();
  await expect(page.getByText("Reservation created.")).toBeVisible();

  const ledger = page.getByRole("region", { name: "Reservation ledger" });
  const reservationRow = ledger.getByRole("row").filter({ hasText: seeded.stockLabel });
  await expect(reservationRow.getByRole("cell", { name: "3", exact: true })).toBeVisible();
  await reservationRow.getByRole("button", { name: "Inspect" }).click();
  await expect(page.getByLabel("Selected reservation detail")).toBeVisible();
  await reservationRow.getByRole("button", { name: "Release" }).click();
  await expect(page.getByText("Reservation released.")).toBeVisible();

  await page
    .getByRole("region", { name: "Expire due holds" })
    .getByRole("button", { name: "Expire reservations" })
    .click();
  await expect(page.getByText(/reservations expired|reservation expired/)).toBeVisible();
});

test("marketplace page reads channel and observability data from the backend", async ({
  page,
  request
}) => {
  const seeded = await seedStockItem(request, "marketplace");

  await page.goto("/operations");
  await expect(page.getByRole("heading", { name: "Marketplace channels" })).toBeVisible();
  await expectApiReachable(page);
  await expect(page.getByRole("cell", { name: seeded.channelCode, exact: true })).toBeVisible();
  await expect(page.getByRole("heading", { name: "Service observability" })).toBeVisible();
});

async function expectApiReachable(page: Page) {
  await expect(page.getByText(/failed to fetch/i)).toHaveCount(0);
  await expect(page.getByText(/could not reach the api/i)).toHaveCount(0);
}

async function seedStockItem(request: APIRequestContext, prefix: string) {
  const suffix = Date.now().toString();
  const vendor = await postJson(request, "/vendors", {
    name: `${prefix} Vendor ${suffix}`
  });
  const product = await postJson(request, "/products", {
    vendorId: vendor.vendor.id,
    sku: `${prefix.toUpperCase()}-${suffix}`,
    name: `${prefix} scanner`
  });
  const channelCode = `${prefix}-${suffix}`;
  const channel = await postJson(request, "/channels", {
    code: channelCode,
    name: `${prefix} Marketplace`
  });
  await postJson(request, "/stock-items", {
    productId: product.product.id,
    channelId: channel.channel.id,
    onHandQuantity: 10
  });

  return {
    channelCode,
    stockLabel: `${product.product.sku} - ${product.product.name} on ${channelCode}`
  };
}

async function postJson(request: APIRequestContext, path: string, data: unknown) {
  const response = await request.post(`${apiBaseUrl}${path}`, { data });
  expect(response.ok()).toBeTruthy();
  return response.json();
}
