import { InfoPanel, PageHeader } from "../design-system";

export function InventoryPage() {
  return (
    <section className="page-section" aria-labelledby="inventory-title">
      <PageHeader
        eyebrow="Assets"
        title="Products and warehouse stock"
        titleId="inventory-title"
        description="Keep product records, stock items, and warehouse quantities aligned before they reach each marketplace."
        compact
      />

      <div className="work-list">
        <InfoPanel title="Product assets">
          <p>Maintain product identity, SKU coverage, and market-ready catalog details.</p>
        </InfoPanel>
        <InfoPanel title="Warehouse stock">
          <p>Track on-hand units by warehouse location and availability state.</p>
        </InfoPanel>
        <InfoPanel title="Channel inventory">
          <p>Connect stock items to the markets where they are listed and sold.</p>
        </InfoPanel>
        <InfoPanel title="Vendor ownership">
          <p>Keep supplier and vendor relationships visible alongside sellable assets.</p>
        </InfoPanel>
      </div>
      <InfoPanel title="Availability view">
        <p>
          AssetFlow separates on-hand stock from reserved stock so teams can see what is truly
          available to sell.
        </p>
      </InfoPanel>
    </section>
  );
}
