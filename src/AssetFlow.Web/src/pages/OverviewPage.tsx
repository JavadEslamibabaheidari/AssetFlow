import { ArrowUpRight, Boxes, RadioTower, Store, TimerReset } from "lucide-react";
import { Link } from "react-router-dom";
import { PageHeader, StatusBadge } from "../design-system";

const operationsPanels = [
  {
    to: "/inventory",
    label: "Asset registry",
    detail: "Products, vendors, and warehouse stock",
    signal: "Live",
    icon: Boxes
  },
  {
    to: "/reservations",
    label: "Reservation control",
    detail: "Held stock and expiration workflow",
    signal: "Guarded",
    icon: TimerReset
  },
  {
    to: "/operations",
    label: "Market channels",
    detail: "Delivery health and retry visibility",
    signal: "3 linked",
    icon: Store
  }
];

export function OverviewPage() {
  return (
    <section className="page-section" aria-labelledby="overview-title">
      <PageHeader
        eyebrow="Live topology"
        title="Inventory command"
        titleId="overview-title"
        description="Follow the operational path from warehouse stock through reservation-aware availability to every connected market."
      />

      <div className="overview-command-grid">
        <section className="topology-panel" aria-labelledby="topology-title">
          <div className="panel-kicker">
            <RadioTower size={17} aria-hidden="true" />
            <span>Operational route</span>
          </div>
          <div className="topology-heading">
            <div>
              <h3 id="topology-title">Warehouse to market</h3>
              <p>
                Availability is calculated after active reservations, then delivered to channels.
              </p>
            </div>
            <StatusBadge tone="success">Connected</StatusBadge>
          </div>

          <div className="topology-route" aria-label="Inventory delivery path">
            <div className="topology-node topology-node-source">
              <span>Stock</span>
              <strong>On hand</strong>
            </div>
            <span className="topology-line" aria-hidden="true" />
            <div className="topology-node topology-node-core">
              <span>Availability</span>
              <strong>Reservation aware</strong>
            </div>
            <span className="topology-line" aria-hidden="true" />
            <div className="topology-node topology-node-target">
              <span>Channels</span>
              <strong>3 markets</strong>
            </div>
          </div>

          <div className="market-band" aria-label="Connected marketplaces">
            <span>Amazon</span>
            <span>MediaWorld</span>
            <span>Unieuro</span>
          </div>
        </section>

        <aside className="overview-signal-panel" aria-labelledby="signal-title">
          <div>
            <p className="eyebrow">Current posture</p>
            <h3 id="signal-title">Core flows online</h3>
          </div>
          <dl className="signal-list">
            <div>
              <dt>Inventory</dt>
              <dd>Source of truth</dd>
            </div>
            <div>
              <dt>Reservations</dt>
              <dd>Oversell protected</dd>
            </div>
            <div>
              <dt>Delivery</dt>
              <dd>Retry observable</dd>
            </div>
          </dl>
        </aside>
      </div>

      <nav className="command-links" aria-label="Operational workspaces">
        {operationsPanels.map(({ to, label, detail, signal, icon: Icon }) => (
          <Link key={to} to={to} className="command-link">
            <span className="command-link-icon" aria-hidden="true">
              <Icon size={19} />
            </span>
            <span className="command-link-copy">
              <strong>{label}</strong>
              <small>{detail}</small>
            </span>
            <span className="command-link-signal">{signal}</span>
            <ArrowUpRight size={17} aria-hidden="true" />
          </Link>
        ))}
      </nav>
    </section>
  );
}
