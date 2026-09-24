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

for (const viewport of [
  { name: "desktop", width: 1440, height: 900 },
  { name: "mobile", width: 390, height: 844 }
]) {
  test(`renders the immersive fallback without obscuring routes on ${viewport.name}`, async ({
    page
  }) => {
    await page.setViewportSize(viewport);
    await page.goto("/");

    await expect(page.getByTestId("immersive-backdrop")).toBeVisible();
    await expect(page.getByRole("link", { name: "Assets" })).toBeVisible();
    await expect(page.getByRole("heading", { name: "Asset operations" })).toBeVisible();

    const contentBox = await page.locator("#main-content").boundingBox();
    expect(contentBox).not.toBeNull();
    expect(contentBox?.x).toBeGreaterThanOrEqual(0);
    expect(contentBox?.width).toBeLessThanOrEqual(viewport.width);
  });
}

test("keeps the spatial fallback available with reduced motion", async ({ page }) => {
  await page.emulateMedia({ reducedMotion: "reduce" });
  await page.goto("/");

  await expect(page.getByTestId("immersive-backdrop")).toBeVisible();
  await expect(page.getByTestId("immersive-scene-canvas")).toHaveCount(0);
  await expect(page.getByRole("link", { name: "Assets" })).toBeVisible();
});

test("renders a live WebGL canvas when motion is allowed", async ({ page }) => {
  await page.goto("/");

  const canvas = page.getByTestId("immersive-scene-canvas");
  await expect(canvas).toBeVisible();
  const canvasSurface = canvas.locator("canvas");
  await expect(canvasSurface).toBeVisible();
  await expect
    .poll(async () => canvas.evaluate((element) => element.getBoundingClientRect().width))
    .toBeGreaterThan(300);

  const hasWebGLContext = await canvasSurface.evaluate((element) => {
    const target = element as HTMLCanvasElement;
    return Boolean(target.getContext("webgl2") || target.getContext("webgl"));
  });
  expect(hasWebGLContext).toBe(true);
});
