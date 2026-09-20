import { getAppConfig } from "../shared/config";

export function SettingsPage() {
  const config = getAppConfig();

  return (
    <section className="page-section" aria-labelledby="settings-title">
      <div className="section-heading compact">
        <div>
          <p className="eyebrow">Settings</p>
          <h2 id="settings-title">Runtime configuration</h2>
        </div>
        <p>Environment-driven settings stay visible without committing local secrets.</p>
      </div>

      <dl className="settings-list">
        <div>
          <dt>Inventory API base URL</dt>
          <dd>{config.apiBaseUrl}</dd>
        </div>
        <div>
          <dt>Environment variable</dt>
          <dd>VITE_ASSETFLOW_API_BASE_URL</dd>
        </div>
      </dl>
    </section>
  );
}
