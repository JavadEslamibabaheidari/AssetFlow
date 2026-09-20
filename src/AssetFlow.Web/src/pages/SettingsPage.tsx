import { DefinitionList, PageHeader } from "../design-system";

export function SettingsPage() {
  return (
    <section className="page-section" aria-labelledby="settings-title">
      <PageHeader
        eyebrow="Settings"
        title="Workspace settings"
        titleId="settings-title"
        description="Keep channel, warehouse, and availability behavior visible for operators."
        compact
      />

      <DefinitionList
        items={[
          { term: "Primary markets", description: "Amazon, MediaWorld, and Unieuro" },
          { term: "Stock source", description: "Warehouse on-hand quantities" },
          { term: "Availability rule", description: "Active reservations reduce sellable stock" },
          { term: "Channel sync", description: "Market availability follows inventory state" }
        ]}
      />
    </section>
  );
}
