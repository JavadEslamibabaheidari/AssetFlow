export function InventoryPage() {
  return (
    <section className="page-section" aria-labelledby="inventory-title">
      <div className="section-heading compact">
        <div>
          <p className="eyebrow">Inventory</p>
          <h2 id="inventory-title">Catalog and stock workspace</h2>
        </div>
        <p>
          M6 will add API-backed vendor, product, channel, and stock item workflows here.
        </p>
      </div>

      <div className="placeholder-panel">
        <h3>Reserved for M6 inventory parity</h3>
        <p>
          This route establishes navigation and layout now, while keeping real inventory behavior
          tied to the dedicated M6 acceptance criteria.
        </p>
      </div>
    </section>
  );
}
