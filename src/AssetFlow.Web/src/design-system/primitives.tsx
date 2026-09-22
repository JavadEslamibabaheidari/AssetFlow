import type { ReactNode } from "react";

type PageHeaderProps = {
  eyebrow: string;
  title: string;
  description: string;
  titleId: string;
  compact?: boolean;
};

export function PageHeader({
  eyebrow,
  title,
  description,
  titleId,
  compact = false
}: PageHeaderProps) {
  return (
    <div className={`section-heading${compact ? " compact" : ""}`}>
      <div>
        <p className="eyebrow">{eyebrow}</p>
        <h2 id={titleId}>{title}</h2>
      </div>
      <p>{description}</p>
    </div>
  );
}

type MetricCardProps = {
  label: string;
  value: string;
  description: string;
};

export function MetricCard({ label, value, description }: MetricCardProps) {
  return (
    <article className="metric-card">
      <span>{label}</span>
      <strong>{value}</strong>
      <p>{description}</p>
    </article>
  );
}

type InfoPanelProps = {
  title: string;
  children: ReactNode;
};

export function InfoPanel({ title, children }: InfoPanelProps) {
  return (
    <article className="info-panel">
      <h3>{title}</h3>
      <div>{children}</div>
    </article>
  );
}

type StatusBadgeProps = {
  children: ReactNode;
  tone?: "neutral" | "success" | "warning" | "danger";
};

export function StatusBadge({ children, tone = "neutral" }: StatusBadgeProps) {
  return <span className={`status-badge status-badge-${tone}`}>{children}</span>;
}

type DefinitionListProps = {
  items: Array<{
    term: string;
    description: string;
  }>;
};

export function DefinitionList({ items }: DefinitionListProps) {
  return (
    <dl className="settings-list">
      {items.map((item) => (
        <div key={item.term}>
          <dt>{item.term}</dt>
          <dd>{item.description}</dd>
        </div>
      ))}
    </dl>
  );
}
