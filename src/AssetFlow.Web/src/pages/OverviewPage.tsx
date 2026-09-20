const foundationItems = [
  ["App shell", "Routing, navigation, loading, and error boundaries are in place."],
  ["Design system", "Tokens and reusable primitives are scheduled for the next slice."],
  ["API client", "OpenAPI-aligned TypeScript access will wire into the checked-in contract."],
  ["Quality gates", "Linting, type checks, component tests, and smoke tests follow this shell."]
];

export function OverviewPage() {
  return (
    <section className="page-section" aria-labelledby="overview-title">
      <div className="section-heading">
        <div>
          <p className="eyebrow">Foundation</p>
          <h2 id="overview-title">Ready for product workflows</h2>
        </div>
        <p>
          The first frontend surface gives AssetFlow a stable place for inventory and reservation
          workflows without pulling M6 screens into the foundation milestone.
        </p>
      </div>

      <div className="metric-grid" aria-label="Current foundation areas">
        <article>
          <span>Routes</span>
          <strong>5</strong>
          <p>Overview, inventory, reservations, operations, and settings.</p>
        </article>
        <article>
          <span>Milestone</span>
          <strong>M5</strong>
          <p>Focused on shell, contracts, checks, and local run paths.</p>
        </article>
        <article>
          <span>Backend</span>
          <strong>8080</strong>
          <p>Default API base URL for local development.</p>
        </article>
      </div>

      <div className="work-list">
        {foundationItems.map(([title, body]) => (
          <article key={title}>
            <h3>{title}</h3>
            <p>{body}</p>
          </article>
        ))}
      </div>
    </section>
  );
}
