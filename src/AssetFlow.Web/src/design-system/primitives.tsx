import { motion } from "motion/react";
import type { ReactNode } from "react";

const surfaceTransition = { duration: 0.2, ease: [0.22, 1, 0.36, 1] as const };

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
    <motion.div
      className={`section-heading${compact ? " compact" : ""}`}
      initial={{ y: 10 }}
      animate={{ y: 0 }}
      transition={{ duration: 0.28, ease: [0.22, 1, 0.36, 1] }}
    >
      <div>
        <p className="eyebrow">{eyebrow}</p>
        <h2 id={titleId}>{title}</h2>
      </div>
      <p>{description}</p>
    </motion.div>
  );
}

type MetricCardProps = {
  label: string;
  value: string;
  description: string;
};

export function MetricCard({ label, value, description }: MetricCardProps) {
  return (
    <motion.article
      className="metric-card"
      initial={{ y: 12 }}
      animate={{ y: 0 }}
      whileHover={{ y: -4, scale: 1.01 }}
      whileTap={{ scale: 0.99 }}
      transition={surfaceTransition}
    >
      <span>{label}</span>
      <strong>{value}</strong>
      <p>{description}</p>
    </motion.article>
  );
}

type InfoPanelProps = {
  title: string;
  children: ReactNode;
};

export function InfoPanel({ title, children }: InfoPanelProps) {
  return (
    <motion.article
      className="info-panel"
      initial={{ y: 10 }}
      animate={{ y: 0 }}
      whileHover={{ y: -3 }}
      transition={surfaceTransition}
    >
      <h3>{title}</h3>
      <div>{children}</div>
    </motion.article>
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
      {items.map((item, index) => (
        <motion.div
          key={item.term}
          initial={{ x: -10 }}
          animate={{ x: 0 }}
          transition={{ ...surfaceTransition, delay: index * 0.04 }}
          whileHover={{ x: 3 }}
        >
          <dt>{item.term}</dt>
          <dd>{item.description}</dd>
        </motion.div>
      ))}
    </dl>
  );
}
