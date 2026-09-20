import { expect, test } from "@playwright/test";

test("loads the product-facing operations overview", async ({ page }) => {
  await page.goto("/");

  await expect(page.getByRole("heading", { name: "Asset operations" })).toBeVisible();
  await expect(page.getByRole("heading", { name: "Marketplace channels" })).toBeVisible();
  await expect(page.getByRole("heading", { name: "Warehouse stock" })).toBeVisible();
});

test("navigates between app shell routes", async ({ page }) => {
  await page.goto("/");

  await page.getByRole("link", { name: "Assets" }).click();
  await expect(page.getByRole("heading", { name: "Products and warehouse stock" })).toBeVisible();

  await page.getByRole("link", { name: "Markets" }).click();
  await expect(page.getByRole("heading", { name: "Marketplace channels" })).toBeVisible();

  await page.getByRole("link", { name: "Settings" }).click();
  await expect(page.getByRole("heading", { name: "Workspace settings" })).toBeVisible();
});
