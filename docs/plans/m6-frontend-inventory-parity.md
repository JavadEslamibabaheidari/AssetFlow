# Milestone Plan: M6 - Frontend Inventory Parity

## Goal

Bring the frontend product experience to the same functional surface the backend reached at the end of M3: core inventory management, reservations, and reservation-aware availability.

## Reopen Note

M6 was reopened on 2026-09-23 after live frontend inspection showed that the browser app still needed real API-backed inventory workflows before the milestone could honestly close. The corrective implementation keeps backend and frontend abilities aligned to the existing M2/M3 contracts and tracks remaining work through reopened GitHub issues.

## Start-Gate Summary

M6 starts from the completed M5 frontend foundation and the completed M2/M3 backend contracts. The milestone should build real operational workflows in `src/AssetFlow.Web`; planning text, milestone status, and task guidance belong in docs, plans, and GitHub issues, not in the visible app UI.

Previous milestone closure passed:

- M5 report exists at `docs/milestone-5-report.md`.
- `docs/knowledge/overview.md` captures M5 frontend foundation facts and the M6 handoff.
- GitHub milestone `M5 - Frontend Application Foundation` is closed with issues #60 through #66 closed.
- PR #73 merged the final M5 validation/docs slice.
- Tag `v0.5.0` and release `v0.5.0 - M5 Frontend Application Foundation` exist.

## Scope

In scope:

- Build frontend screens and workflows for vendors, products, sales channels, stock items, reservations, and stock item availability.
- Connect workflows to the M2 and M3 backend OpenAPI contracts through the M5 API-client foundation.
- Add create, list, and detail flows where backend endpoints support them.
- Add reservation create, list, detail, release, expiration trigger, oversell-conflict, and availability-review workflows.
- Make reservation-aware stock state scannable through on-hand, reserved, available, status, and expiration fields.
- Add shared UI state handling for loading, empty, validation, conflict, not-found, disabled, stale/refetch, and success states.
- Expand focused frontend tests and Playwright coverage around the most important inventory and reservation workflows.
- Validate completed workflows through Docker Compose with frontend and backend containers running together.
- Split CI into backend, frontend, and shared/integration checks once frontend workflows are real enough to benefit from faster targeted feedback.

Out of scope:

- Event-driven channel synchronization UI. That follows backend synchronization work in M7 or later.
- Advanced analytics, observability dashboards, and Agentic OS expansion.
- Authentication, authorization, tenant isolation, checkout, payment, shipment, or customer identity workflows.
- Reimplementing backend business rules in the frontend.
- Backend contract changes except for explicit follow-up issues when frontend implementation exposes a real contract gap.
- Many tiny CI workflows. M6 should keep CI understandable: one backend pipeline, one frontend pipeline, and one shared/integration pipeline.

## UX Model

The M6 UI should feel like an operational inventory tool, not a landing page or project dashboard.

- Navigation keeps the M5 shell but turns the existing Assets, Reservations, and Markets areas into API-backed work surfaces.
- Assets is the primary inventory workspace. It should support vendor, product, channel, and stock-item scanning without forcing the user through decorative summary cards.
- Stock item detail is the main decision view for inventory parity. It should show product/channel identity, on-hand quantity, reserved quantity, available quantity, reservation status, and the next relevant expiration together.
- Reservations is the workflow surface for creating, inspecting, releasing, and expiring reservations. Oversell prevention should be presented as business feedback, not a generic failure.
- Markets/channels should remain focused on sales-channel master data until M7 adds synchronization behavior.
- Forms should use predictable labels, validation summaries near the affected fields, disabled submit states during mutation, and success feedback that leaves the user in the relevant workflow.
- Tables should favor scanability: stable columns, compact row actions, empty states with a direct next action, and filters only where backend contracts already support them.
- Responsive behavior should preserve task completion on mobile, but desktop/tablet scanning is the primary operational target.

## Deliverables

- API-client mutation and detail methods for all M2/M3 frontend-covered operations.
- Shared frontend workflow primitives for data tables, forms, async states, Problem Details feedback, and mutation success/conflict handling when useful.
- Vendor, product, sales-channel, stock-item, reservation, and availability screens.
- API-backed create/list/detail flows aligned with `contracts/openapi/inventory-api.yaml`.
- Reservation workflow UI for create, release, expiration trigger, availability review, and oversell-conflict feedback.
- Focused unit/component/integration tests and Playwright coverage for critical workflows.
- Docker Compose verification of frontend/backend inventory and reservation workflows.
- Updated docs mapping frontend screens to backend endpoints, state behavior, and milestone acceptance.
- CI workflows organized as backend, frontend, and shared/integration checks with path filters that do not hide OpenAPI or Compose integration problems.

## Acceptance Criteria

- [x] The frontend can create, list, and inspect vendors through backend APIs.
- [x] The frontend can create, list, and inspect products through backend APIs, including vendor selection.
- [x] The frontend can create and list sales channels through backend APIs.
- [x] The frontend can create, list, and inspect stock items through backend APIs, including product/channel selection.
- [x] The frontend can create reservations and show conflict feedback when a reservation would oversell stock.
- [x] The frontend can list and inspect reservations with stock item and status filtering where supported.
- [x] The frontend can show reservation-aware availability for a stock item.
- [x] The frontend can release reservations and show updated availability after release.
- [x] The frontend can trigger reservation expiration and show expiration results.
- [x] Major workflows have loading, empty, validation, not-found, conflict, disabled, and success states.
- [x] Critical workflows are covered by focused frontend tests and a small Playwright suite.
- [x] Docker Compose can build and run the frontend and backend together for end-to-end workflow checks.
- [x] CI has one backend pipeline for .NET/API tests/backend Docker, one frontend pipeline for `src/AssetFlow.Web/**`, and one shared/integration pipeline for Compose, workflow, OpenAPI, and cross-cutting changes.
- [x] OpenAPI contract changes run frontend checks as well as backend or integration checks.
- [x] Documentation explains how each screen maps to backend endpoints and contracts.

## Task Breakdown

| Priority | Task | Objective | Depends on | Verification | GitHub issue |
| --- | --- | --- | --- | --- | --- |
| Critical | UX and API foundation | Finalize workflow layout, route/state model, shared UI states, and API-client mutation/detail methods. | M5 closure, M2/M3 contracts | Unit/component tests for shared state and API client behavior | #74 |
| High | Master-data workflows | Build vendor, product, and sales-channel create/list/detail workflows. | UX and API foundation | Component tests plus backend-backed manual or Playwright checks | #81 |
| High | Stock item and availability workflows | Build stock item create/list/detail flows and reservation-aware availability display. | Master-data workflows | Component/API tests and availability before/after backend-backed checks | #80 |
| High | Reservation workflows | Build reservation create/list/detail/release/expire flows with conflict/status handling. | Stock item and availability workflows | Conflict, release, expiration, disabled-state, and success-state tests | #79 |
| Medium | Cross-workflow integration and UX hardening | Add cross-links, filter handoffs, responsive polish, accessibility checks, and coherent empty/not-found states across workflows. | Master-data, stock, and reservation workflows | Accessibility/responsive review plus focused tests | #78 |
| Medium | Split CI pipelines for frontend/backend/integration | Keep backend, frontend, and shared integration checks focused while preserving OpenAPI and Compose coverage. | Initial M6 workflows and Playwright coverage | PR path-filter checks, workflow validation, backend/frontend Docker builds | #76 |
| Medium | Compose end-to-end verification | Run frontend and backend together and validate critical inventory/reservation paths. | Implemented workflows and CI split | Docker Compose build/run plus Playwright or documented manual smoke checks | #77 |
| Medium | M6 validation and docs | Verify frontend parity with M2/M3 backend behavior, update knowledge, and prepare milestone closure evidence. | All implementation tasks | Planned-state validation, docs/knowledge update, GitHub sync | #75 |

## Execution Order

1. Start with the UX and API foundation because every later screen depends on consistent API mutations, error mapping, async state, and route conventions.
2. Build master-data workflows next so products, channels, and stock items have selectable dependencies.
3. Build stock item and availability workflows before reservations so the user has an inventory decision surface to return to after reservation changes.
4. Build reservation workflows after stock item detail exists, then wire release/expiration changes back into availability views.
5. Harden cross-workflow UX and responsive/accessibility behavior after the main flows exist.
6. Split CI into backend, frontend, and shared/integration workflows once M6 frontend tests and Playwright coverage are meaningful.
7. Run Compose E2E verification against the integrated app.
8. Complete planned-state validation, knowledge updates, and GitHub synchronization.

## Endpoint Mapping

| Frontend workflow | Backend operations |
| --- | --- |
| Vendor list/create/detail | `listVendors`, `createVendor`, `getVendor` |
| Product list/create/detail | `listProducts`, `createProduct`, `getProduct` |
| Sales channel list/create | `listChannels`, `createChannel` |
| Stock item list/create/detail | `listStockItems`, `createStockItem`, `getStockItem` |
| Stock item availability | `getStockItemAvailability` |
| Reservation list/create/detail | `listReservations`, `createReservation`, `getReservation` |
| Reservation release | `releaseReservation` |
| Reservation expiration | `expireReservations` |

## CI Pipeline Plan

M6 should end with focused CI that is faster for day-to-day frontend work without hiding integration problems.

- Backend pipeline: runs for backend service and test changes. It owns .NET restore/build/test, API tests, and backend Docker image build.
- Frontend pipeline: runs for `src/AssetFlow.Web/**` changes. It owns frontend lint, typecheck, unit/component tests, build, Playwright, and frontend Docker build.
- Shared/integration pipeline: runs for changes affecting both sides, including `docker-compose.yml`, `.github/workflows/**`, `contracts/openapi/**`, root docs/config that impact both apps, and manual dispatch. It owns Compose and integration checks.

Path-filter rules:

- Backend code changed: backend CI.
- Frontend code changed: frontend CI.
- OpenAPI contract changed: frontend CI plus backend or integration CI, because generated frontend types and API behavior both depend on the contract.
- Compose, workflow, shared root config, or cross-cutting docs changed: shared/integration CI.
- PR to `main`: integration check should remain available even if focused path filters skip unrelated pipelines.

## Risks and Mitigations

- Risk: frontend workflows expose gaps in backend contracts. Mitigation: record contract gaps and handle them through explicit backend follow-up issues rather than frontend workarounds.
- Risk: UI state handling becomes inconsistent across screens. Mitigation: build shared async/error/form patterns first and reuse them.
- Risk: Playwright coverage becomes broad and slow. Mitigation: keep E2E coverage focused on critical happy paths and core conflict behavior; use component tests for detailed UI states.
- Risk: reservation availability is misunderstood by users. Mitigation: show on-hand, reserved, available, status, and expiration together wherever users make reservation decisions.
- Risk: CI path filters skip necessary checks, especially for OpenAPI changes. Mitigation: require OpenAPI changes to run frontend checks and backend or integration checks; keep a manual integration dispatch.
- Risk: concurrent frontend work creates conflicts in `src/AssetFlow.Web`. Mitigation: keep branches issue-scoped, inspect dirty files before edits, and avoid rewriting unrelated UI or motion work.
- Risk: local shells may lack `npm`. Mitigation: use the bundled/runtime Node/npm path when available or record the exact verification gap and rely on CI where appropriate.

## Testing Strategy

- Unit/component: forms, tables, state components, error mapping, API-client methods, and reservation/availability presentation logic.
- Integration: API-client behavior against test doubles or configured local backend where practical.
- E2E: Playwright coverage for app load, navigation, create master data, create stock item, create reservation, oversell conflict, release reservation, expiration trigger, and availability update.
- Containerized: Docker Compose build/run verification for frontend and backend services before calling each end-to-end feature complete.
- Accessibility/responsive: keyboard flow, focus visibility, contrast, text fit, and desktop/mobile layout checks for core screens.
- CI: validate workflow path filters with representative backend-only, frontend-only, OpenAPI, Compose, and workflow-change scenarios before considering the split complete.

## GitHub Tracking

- Milestone: `M6 - Frontend Inventory Parity`
- Milestone status: corrective implementation and validation complete after the 2026-09-23 reopen; ready for GitHub milestone closure once the final validation branch is merged and the remaining issues are closed.
- Issues: #74, #77, #78, and #81 are closed after corrective implementation slices. #79, #80, and #75 are satisfied by the final validation/testing/docs pass.
- Project board: project-board item and column mapping remains unverified from this environment; record the gap until board access is confirmed.
- Branch/PR rule: keep each active task isolated to one branch and one pull request unless explicitly combined.

## Planned M6 Issues

Created under the `M6 - Frontend Inventory Parity` milestone:

- #74 Define M6 frontend UX and API foundation.
- #81 Build master-data frontend workflows.
- #80 Build stock item and availability frontend workflows.
- #79 Build reservation frontend workflows.
- #78 Harden M6 cross-workflow UX, accessibility, and responsive states.
- #76 Split CI into backend, frontend, and shared integration pipelines.
- #77 Verify M6 frontend/backend workflows through Docker Compose.
- #75 Complete M6 validation and docs.

## Knowledge Updates

`docs/knowledge/overview.md` must capture durable M6 planned behavior before implementation starts, then be updated again during final planned-state validation to reflect the actual shipped frontend behavior and any deliberate deviations.

## Start Gate Result

- [x] Previous milestone report exists or was not required.
- [x] Previous milestone durable facts are captured in docs/knowledge.
- [x] Plan is complete enough to implement.
- [x] No implementation-blocking decisions remain undecided.
- [x] GitHub issue tracking matches this plan.
- [x] Knowledge docs capture durable planned behavior.

Status: Corrective implementation and validation complete pending final PR merge and milestone closure.
