import { InfoPanel, MetricCard, PageHeader } from "../design-system";

const operationsPanels = [
  ["Marketplace channels", "Track sales channels, listing readiness, and availability sync."],
  ["Warehouse stock", "Review on-hand quantities and where items are physically available."],
  ["Product assets", "Organize sellable products, identifiers, and channel-ready catalog data."],
  ["Reservations", "Protect availability when items are temporarily held or released."]
];

export function OverviewPage() {
  return (
    <section className="page-section" aria-labelledby="overview-title">
      <PageHeader
        eyebrow="Overview"
        title="Asset operations"
        titleId="overview-title"
        description="Manage market listings, warehouse stock, product assets, and reservation-aware availability from one workspace."
      />

      <div className="metric-grid" aria-label="Current operation areas">
        <MetricCard
          label="Markets"
          value="3"
          description="Amazon, MediaWorld, and Unieuro channel coverage."
        />
        <MetricCard
          label="Inventory"
          value="Live"
          description="Warehouse stock and channel availability stay connected."
        />
        <MetricCard
          label="Reservations"
          value="Guarded"
          description="Held stock is separated from sellable availability."
        />
      </div>

      <div className="work-list">
        {operationsPanels.map(([title, body]) => (
          <InfoPanel key={title} title={title}>
            <p>{body}</p>
          </InfoPanel>
        ))}
      </div>
    </section>
  );
}
