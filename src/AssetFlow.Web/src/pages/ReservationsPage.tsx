import { InfoPanel, PageHeader } from "../design-system";

export function ReservationsPage() {
  return (
    <section className="page-section" aria-labelledby="reservations-title">
      <PageHeader
        eyebrow="Reservations"
        title="Reserved stock"
        titleId="reservations-title"
        description="See which items are held, when holds expire, and how reservations affect market availability."
        compact
      />

      <div className="work-list">
        <InfoPanel title="Active holds">
          <p>Review stock currently protected from overselling while orders or holds complete.</p>
        </InfoPanel>
        <InfoPanel title="Expiring holds">
          <p>Prioritize reservations that are close to returning stock to available inventory.</p>
        </InfoPanel>
      </div>
      <InfoPanel title="Availability impact">
        <p>Reservation-aware availability keeps marketplace quantities honest across channels.</p>
      </InfoPanel>
    </section>
  );
}
