# Milestone Plan: M6 - Frontend Inventory Parity

## Goal

Bring the frontend product experience to the same functional surface the backend reaches at the end of M3: core inventory management, reservations, and reservation-aware availability.

## Scope

In scope:

- Build frontend screens and workflows for vendors, products, sales channels, stock items, reservations, and availability.
- Connect workflows to the M2 and M3 backend OpenAPI contracts through the M5 API-client foundation.
- Validate completed workflows through Docker Compose with frontend and backend containers running together.
- Provide list, detail, and create flows where the backend supports them.
- Represent reservation release, expiration, conflict, and availability behavior clearly in the UI.
- Add clear loading, empty, validation, conflict, not-found, disabled, and success states.
- Expand frontend tests around the most important inventory and reservation workflows.

Out of scope:

- Event-driven channel synchronization UI. That follows backend synchronization work in M7 or later.
- Advanced analytics, observability dashboards, and Agentic OS expansion.
- Authentication, authorization, tenant isolation, checkout, payment, shipment, or customer identity workflows unless backend scope has already introduced them.
- Reimplementing backend business rules in the frontend.

## UX Principles

- Design for repeated operational use: scannable tables, predictable filters, compact details, and clear actions.
- Make stock and reservation state easy to compare across product, channel, on-hand quantity, reserved quantity, available quantity, and expiration.
- Treat API conflicts as normal business feedback, especially oversell-prevention cases.
- Keep destructive or state-changing actions explicit and distinguish them from read-only views.
- Avoid decorative screens that delay the primary workflow.

## Deliverables

- Vendor, product, channel, stock item, reservation, and availability screens.
- API-backed create/list/detail flows aligned with backend contracts.
- Reservation workflow UI for create, release, expiration trigger where available, and availability review.
- Error and state handling for validation, not-found, conflict, empty data, loading, disabled actions, and success feedback.
- Expanded unit/component/integration tests and Playwright coverage for critical workflows.
- Docker Compose verification of frontend/backend inventory and reservation workflows.
- Updated docs that map frontend screens to backend contracts and milestone behavior.

## Acceptance Criteria

- [ ] The frontend can create and list vendors, products, sales channels, and stock items through backend APIs.
- [ ] The frontend can create reservations and show conflict feedback when a reservation would oversell stock.
- [ ] The frontend can show reservation-aware availability for a stock item.
- [ ] The frontend can release reservations and show updated availability after release.
- [ ] The frontend can trigger or represent reservation expiration behavior exposed by the backend.
- [ ] All major workflows have loading, empty, validation, not-found, conflict, and success states.
- [ ] Critical workflows are covered by focused frontend tests and a small Playwright suite.
- [ ] Docker Compose can build and run the frontend and backend together for end-to-end workflow checks.
- [ ] Documentation explains how each screen maps to backend endpoints and contracts.

## Task Breakdown

| Order | Task | Objective | Depends on | Verification | GitHub issue |
| --- | --- | --- | --- | --- | --- |
| 1 | Define inventory UX spec | Finalize navigation, screens, workflow order, state model, and test coverage. | M5 closure and M3 backend closure | Reviewed UX spec and start-gate update | To create after M5 |
| 2 | Build master-data workflows | Add vendor, product, and sales-channel screens. | Task 1 | UI tests and backend-backed manual checks | To create after M5 |
| 3 | Build stock item workflows | Add stock item list/detail/create flows and inventory state display. | Tasks 1-2 | UI tests and API contract checks | To create after M5 |
| 4 | Build reservation workflows | Add reservation create/list/detail/release/expiration surfaces. | Tasks 1 and 3 | Conflict, release, expiration, and state tests | To create after M5 |
| 5 | Build availability views | Add reservation-aware availability views and cross-links from stock/reservation screens. | Tasks 3-4 | Availability before/after reservation changes | To create after M5 |
| 6 | Verify Compose end-to-end workflows | Run frontend and backend together and validate critical inventory/reservation paths. | Tasks 2-5 | Docker Compose build/run plus Playwright or manual smoke checks | To create after M5 |
| 7 | Complete M6 validation and docs | Verify frontend parity with M2/M3 backend behavior and update knowledge. | Tasks 1-6 | Planned-state validation, Playwright, docs update | To create after M5 |

## Risks and Mitigations

- Risk: frontend workflows expose gaps in backend contracts. Mitigation: record contract gaps and handle them through explicit backend follow-up issues rather than frontend workarounds.
- Risk: UI state handling becomes inconsistent across screens. Mitigation: reuse shared state, error, and form patterns from M5.
- Risk: Playwright coverage becomes broad and slow. Mitigation: keep E2E coverage focused on critical happy paths and core conflict behavior; use component tests for detailed UI states.
- Risk: reservation availability is misunderstood by users. Mitigation: make on-hand, reserved, available, and expiration state visible together wherever decisions are made.

## Testing Strategy

- Unit/component: forms, tables, state components, error mapping, and reservation/availability presentation logic.
- Integration: API-client behavior against test doubles or configured local backend where practical.
- E2E: Playwright coverage for app load, navigation, create master data, create stock item, create reservation, oversell conflict, release reservation, and availability update.
- Containerized: Docker Compose build/run verification for frontend and backend services before calling each end-to-end feature complete.
- Accessibility/responsive: keyboard flow, focus visibility, contrast, text fit, and desktop/mobile layout checks for core screens.

## GitHub Tracking

- Milestone: `M6 - Frontend Inventory Parity`
- Issues: create after M5 closure and detailed M6 planning.
- Project board: keep each active task isolated to one branch and pull request unless explicitly combined.

## Previous Milestone Closure

- Report: M5 report must exist before M6 implementation starts.
- Knowledge/docs: M5 durable facts must be captured in `docs/knowledge/`.
- GitHub status: M5 issues and milestone state must match the completed implementation.
- Result: Pending M5 completion.

## Start Gate Result

- [ ] Previous milestone report exists or was not required.
- [ ] Previous milestone durable facts are captured in docs/knowledge.
- [ ] Plan is complete enough to implement.
- [ ] No implementation-blocking decisions remain undecided.
- [ ] GitHub tracking matches this plan.
- [ ] Knowledge docs capture durable planned behavior.

Status: Not ready
