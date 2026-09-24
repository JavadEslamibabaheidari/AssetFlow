import { Boxes, Database, Globe2, RadioTower, ShieldCheck } from "lucide-react";
import { DefinitionList, PageHeader, StatusBadge } from "../design-system";
import { getAppConfig } from "../shared/config";

export function SettingsPage() {
  const config = getAppConfig();

  return (
    <section className="page-section" aria-labelledby="settings-title">
      <PageHeader
        eyebrow="Settings"
        title="Workspace settings"
        titleId="settings-title"
        description="Keep channel, warehouse, and availability behavior visible for operators."
        compact
      />

      <div className="settings-grid">
        <section className="settings-runtime" aria-labelledby="runtime-title">
          <div className="workflow-panel-heading">
            <span className="workflow-panel-icon" aria-hidden="true">
              <Database size={18} />
            </span>
            <div className="workflow-panel-copy">
              <h3 id="runtime-title">Runtime connection</h3>
              <p>The endpoint used by this browser session for every inventory workflow.</p>
            </div>
            <StatusBadge tone="success">Configured</StatusBadge>
          </div>
          <div className="runtime-endpoint">
            <Globe2 size={18} aria-hidden="true" />
            <div>
              <span>Inventory API</span>
              <strong>{config.apiBaseUrl}</strong>
            </div>
          </div>
          <dl className="runtime-facts">
            <div>
              <dt>Frontend mode</dt>
              <dd>{import.meta.env.MODE}</dd>
            </div>
            <div>
              <dt>Contract</dt>
              <dd>Typed OpenAPI client</dd>
            </div>
          </dl>
        </section>

        <section className="settings-rules" aria-labelledby="rules-title">
          <div className="workflow-panel-heading">
            <span className="workflow-panel-icon" aria-hidden="true">
              <ShieldCheck size={18} />
            </span>
            <div className="workflow-panel-copy">
              <h3 id="rules-title">Operational rules</h3>
              <p>Shared behavior that protects sellable inventory across markets.</p>
            </div>
          </div>
          <DefinitionList
            items={[
              { term: "Primary markets", description: "Amazon, MediaWorld, and Unieuro" },
              { term: "Stock source", description: "Warehouse on-hand quantities" },
              {
                term: "Availability rule",
                description: "Active reservations reduce sellable stock"
              },
              { term: "Channel sync", description: "Market availability follows inventory state" }
            ]}
          />
        </section>
      </div>

      <div className="settings-capabilities" aria-label="Configured capabilities">
        <span>
          <Boxes size={16} aria-hidden="true" /> Inventory source of truth
        </span>
        <span>
          <RadioTower size={16} aria-hidden="true" /> Observable channel delivery
        </span>
      </div>
    </section>
  );
}
