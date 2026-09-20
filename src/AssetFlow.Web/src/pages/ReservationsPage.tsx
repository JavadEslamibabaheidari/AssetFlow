export function ReservationsPage() {
  return (
    <section className="page-section" aria-labelledby="reservations-title">
      <div className="section-heading compact">
        <div>
          <p className="eyebrow">Reservations</p>
          <h2 id="reservations-title">Availability protection</h2>
        </div>
        <p>
          Reservation create, release, expiration, and availability views arrive after the API
          client foundation is in place.
        </p>
      </div>

      <div className="placeholder-panel">
        <h3>Reservation-aware screens come next milestone</h3>
        <p>
          The shell is ready to host oversell-prevention workflows without duplicating backend
          domain rules in the browser.
        </p>
      </div>
    </section>
  );
}
