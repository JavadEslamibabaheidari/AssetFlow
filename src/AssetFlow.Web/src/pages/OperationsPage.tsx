import { InfoPanel, PageHeader } from "../design-system";

export function OperationsPage() {
  return (
    <section className="page-section" aria-labelledby="operations-title">
      <PageHeader
        eyebrow="Markets"
        title="Marketplace channels"
        titleId="operations-title"
        description="Coordinate channel listings and availability signals across connected markets."
        compact
      />

      <div className="work-list">
        <InfoPanel title="Amazon">
          <p>Monitor listing availability and stock commitments for Amazon sales.</p>
        </InfoPanel>
        <InfoPanel title="MediaWorld">
          <p>Keep channel quantities aligned with warehouse and reservation state.</p>
        </InfoPanel>
        <InfoPanel title="Unieuro">
          <p>Prepare channel sync visibility for marketplace-specific operations.</p>
        </InfoPanel>
        <InfoPanel title="Sync health">
          <p>Track whether channel availability is ready, delayed, or needs attention.</p>
        </InfoPanel>
      </div>
    </section>
  );
}
