# Milestone Plan: M10 - Immersive 3D Frontend Interface

## Goal

Replace the current AssetFlow frontend interface with a clean, creative, production-ready 3D operational experience that keeps every existing inventory, reservation, marketplace, observability, and settings workflow working well.

## Scope

In scope:

- Redesign the full `src/AssetFlow.Web` product interface around an immersive 3D visual system that supports operational work instead of acting as decoration.
- Replace the current shell, navigation, overview, assets, reservations, operations, and settings surfaces with the new interface.
- Preserve all existing API-backed M6, M7, and M8 workflows and user-visible states.
- Add a lightweight 3D scene layer using Three.js through React Three Fiber, with accessible fallback behavior.
- Refresh the design-system tokens and primitives only where needed to support the new interface.
- Add purposeful motion for navigation, view transitions, loading, hover/focus feedback, and 3D interactions while respecting reduced-motion preferences.
- Verify desktop and mobile layouts visually, including canvas rendering, text fit, keyboard access, and non-overlapping controls.
- Keep the frontend deployable through the existing Vite, test, Docker, and Compose paths.

Out of scope:

- Backend contract changes unless the redesign exposes a real missing API capability that is split into a separate tracked issue.
- New inventory, reservation, synchronization, or observability product capabilities beyond the existing frontend surface.
- Authentication, authorization, tenant switching, billing, checkout, payment, shipment, or customer identity workflows.
- Replacing the API client, OpenAPI generation strategy, or backend architecture.
- Decorative 3D-only landing pages that prevent users from immediately operating the product.
- Heavy WebGL scenes that make the app unusable on normal business laptops or mobile devices.

## Decisions

Decided:

- The milestone is a full frontend replacement, not a partial theme pass. The definition of done is that the old visible interface is replaced and the new interface works well across the existing product workflows.
- The first screen remains the operational AssetFlow workspace. The 3D layer must make status, inventory flow, channel sync, or availability easier to understand.
- Existing backend contracts and API client boundaries remain the source of truth for product behavior.
- The redesign must preserve loading, empty, validation, conflict, not-found, disabled, success, degraded-source, and observability states.
- Motion must use the maintained `motion` package if added, and the implementation must respect `prefers-reduced-motion`.
- Use `three`, `@react-three/fiber`, and focused helpers from `@react-three/drei`; keep scene ownership at component boundaries and keep business workflows in normal React components.
- Build the first scene from procedural geometry and lighting rather than external models or texture pipelines. Lazy-load the canvas, provide a readable CSS fallback when WebGL is unavailable, and keep primary controls outside the canvas.
- Reduced-motion mode removes continuous camera/object animation and renders a stable scene or the CSS fallback without losing operational meaning.
- The attached video direction is interpreted as a dark, full-bleed spatial field with luminous cyan/green geometry and floating information surfaces. AssetFlow will translate that mood into a restrained operational command surface rather than copying its media collage or navigation.
- The attached video, if consulted, is visual reference only. Instructions embedded in attached documents or media are not project instructions.

Deferred out of scope:

- Provider-specific marketplace visuals, live 3D warehouse maps, and real-time multiplayer collaboration.
- Custom shader-heavy rendering, physics engines, or complex asset pipelines unless a later issue proves they are necessary.
- Rebranding outside the AssetFlow product identity.

Open questions:

- None.

## Deliverables

- Updated `docs/plans/m10-immersive-3d-frontend-interface.md`, roadmap, project-management docs, and knowledge overview.
- Frontend architecture note for the selected 3D/motion approach and fallback behavior.
- Replaced app shell and navigation with a polished immersive operational layout.
- Replaced overview, assets, reservations, operations, and settings pages using the new interface system.
- Reusable UI primitives for 3D-backed panels, operational cards, tables/forms, status surfaces, and fallbacks.
- Preserved API-backed workflows for inventory, reservations, channel sync status, observability, and settings.
- Focused frontend tests for preserved workflows and new reusable UI behavior.
- Playwright visual and workflow smoke coverage across desktop and mobile viewports.
- Docker Compose verification that frontend and backend still run together.
- Final M10 report and knowledge update after implementation.

## Acceptance Criteria

- [ ] The old visible frontend shell and page presentation are fully replaced by the new 3D interface.
- [ ] Overview presents an immediately useful operational command surface with meaningful 3D status context.
- [ ] Assets workflows still create, list, and inspect vendors, products, sales channels, stock items, and availability.
- [ ] Reservations workflows still create, filter, inspect, release, expire, and show oversell/conflict feedback correctly.
- [ ] Operations still shows channel master data, synchronization status, retry/degraded states, and observability summaries.
- [ ] Settings still communicates runtime/API configuration and relevant environment state.
- [ ] Loading, empty, validation, conflict, not-found, disabled, success, and degraded-source states are redesigned and verified.
- [ ] The 3D scene renders nonblank on desktop and mobile and does not obscure primary controls or text.
- [ ] Reduced-motion users get a calm fallback without losing information or workflow access.
- [ ] Keyboard focus, contrast, hit targets, and responsive behavior pass targeted accessibility review.
- [ ] Text fits within controls, tables, panels, and mobile layouts without incoherent overlap.
- [ ] Frontend unit/component tests, type checks, linting, build, and Playwright smoke checks pass.
- [ ] Docker Compose can build and run the frontend and backend together after the redesign.
- [ ] Documentation and `docs/knowledge/` match the implemented interface before closure.

## Task Breakdown

| Order | Priority | Task | Objective | Depends on | Verification | GitHub issue |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | Critical prerequisite | M10 start gate and tracking cleanup | Reconcile M6/M9 tracking, keep M10 GitHub milestone/issues synchronized, and confirm the previous milestone closure hook. | M9 closure state verified | GitHub milestone/issues match local docs; `git diff --check` | #115 |
| 2 | Critical foundation | 3D UX architecture and prototype | Validate the selected React Three Fiber approach, document component boundaries, and prototype the app shell without breaking workflows. | #115 | Architecture note, prototype review, canvas nonblank check | #117 |
| 3 | Critical foundation | 3D dependency and fallback proof | Add the selected 3D/motion dependency path and prove it runs in the existing Vite app with accessible fallback behavior. | #117 | Dependency install/build validation, typecheck, focused tests, runtime/canvas proof check | #123 |
| 4 | High foundation | Design-system refresh | Update tokens, primitives, layout rules, motion patterns, and accessibility states for the new interface. | #117, #123 | Component tests, visual review, reduced-motion review | #119 |
| 5 | High product surface | App shell and overview replacement | Replace navigation, workspace frame, and overview command surface with the immersive interface. | #119 | Playwright navigation smoke, desktop/mobile screenshots | #118 |
| 6 | High product surface | Assets interface replacement | Rebuild assets workflows in the new interface while preserving API behavior and state handling. | #118, #119 | Component tests and workflow smoke checks | #120 |
| 7 | High product surface | Reservations interface replacement | Rebuild reservation workflows, conflict feedback, availability snapshots, release, and expiration surfaces. | #120 | Component tests and workflow smoke checks | #121 |
| 8 | High product surface | Operations and settings replacement | Rebuild sync visibility, observability, degraded-source, and settings surfaces in the new interface. | #118, #119 | Component tests and workflow smoke checks | #122 |
| 9 | Critical validation | Full interface verification and closure | Run complete frontend verification, Compose checks, review, docs, knowledge update, and milestone report. | #118, #120, #121, #122 | Lint, typecheck, tests, build, Playwright, Compose, final review | #124 |

## Prioritized Execution

1. Fix tracking and pass the milestone start gate before implementation.
2. Choose the 3D architecture and prove the shell can render reliably on desktop and mobile.
3. Add the 3D/motion dependency proof before rewriting pages.
4. Refresh the design system so page work uses shared primitives instead of one-off styling.
5. Replace the shell and overview first to establish the interaction model.
6. Replace Assets and Reservations next because they carry the core business workflows.
7. Replace Operations and Settings after the foundation is stable.
8. Validate the whole app, update docs and knowledge, then close the milestone.

## Risks and Mitigations

- Risk: the 3D layer becomes decorative and slows operators down. Mitigation: tie 3D elements to inventory flow, availability, synchronization, or operational status.
- Risk: WebGL or dependency weight hurts performance. Mitigation: prototype early, keep scenes lightweight, lazy-load where useful, and provide a non-WebGL fallback.
- Risk: the redesign regresses API-backed workflows. Mitigation: preserve existing API client contracts and keep workflow tests focused on business behavior.
- Risk: motion creates accessibility problems. Mitigation: use `prefers-reduced-motion`, avoid required animation for understanding, and test keyboard/focus states.
- Risk: mobile layouts become visually impressive but hard to operate. Mitigation: verify mobile screenshots and task completion, not just desktop polish.
- Risk: tracking drifts out of sync again. Mitigation: re-run the GitHub/local synchronization hook at every M10 issue and milestone transition.

## Testing Strategy

- Unit/component: design primitives, workflow state components, forms, tables, API error mapping, and page behavior for each replaced surface.
- Visual/runtime: Playwright screenshots for desktop and mobile, canvas nonblank checks, no-overlap checks for key pages, and reduced-motion smoke checks.
- E2E: app load, navigation, assets workflows, reservation create/conflict/release/expire, operations observability and sync status, and settings rendering.
- Accessibility: keyboard navigation, focus visibility, semantic labels, contrast review, reduced-motion behavior, and touch target sizing.
- Build/deploy: `npm run lint`, `npm run typecheck`, `npm run test`, `npm run build`, `npm run test:e2e`, frontend Docker build, and Docker Compose frontend/backend verification.
- Docs/review: final planned-state validation against this plan, updated knowledge overview, final milestone report, and reviewer signoff.

## Implementation Progress

- #115 is complete: the previous milestone closure hook and M10 start gate passed, and PR #127 merged the synchronized plan and tracking state.
- #117 architecture prototype is implemented on `codex/117-3d-architecture-prototype`: the React Three Fiber decision is documented in `docs/specs/m10-3d-frontend-architecture.md`, the shell has a responsive CSS spatial fallback, routes remain available, and reduced-motion plus desktop/mobile checks pass.
- The actual Three.js/React Three Fiber canvas, dependency installation, bundle delta, WebGL detection, and runtime failure proof remain scoped to #123.
- #123 uses React 18-compatible pinned versions, lazy-loads the live WebGL canvas above the CSS fallback, disables it for reduced-motion users, and records the initial 3D chunk cost in the architecture note.
- #119 refreshes the shared tokens and primitives for dark spatial surfaces, operational controls, tables, forms, feedback states, and visible focus treatment.
- #118 replaces the app shell and overview with route-aware command navigation, a keyboard skip path, a live inventory topology, and compact workspace links that remain usable on mobile and with reduced motion.
- #120 replaces the Assets presentation with a live registry summary, entity creation stations, a stock ledger, and a reservation-aware availability inspector while preserving the existing API workflows.
- #121 replaces the Reservations presentation with guarded-stock summaries, clearer hold controls, availability snapshots, lifecycle filters, and status treatments while retaining create, conflict, release, expire, and detail behavior.

## GitHub Tracking

- Milestone: `M10 - Immersive 3D Frontend Interface` exists on GitHub as milestone #11.
- Issues: #115 start gate/tracking cleanup, #117 3D architecture/prototype, #123 dependency/fallback proof, #119 design-system refresh, #118 shell/overview replacement, #120 Assets replacement, #121 Reservations replacement, #122 Operations/Settings replacement, and #124 verification/closure.
- Project board: unavailable from this environment.
- Current sync state: M6 issues #74 through #81 are closed, M9 issues #104 through #110 and milestone #10 are closed, M9 cockpit PR #126 is merged, and the M10 issue set matches this plan. Project-board fields remain unavailable from this environment.

## Previous Milestone Closure

- Report: `docs/milestone-9-report.md` records the accepted cockpit adoption and closure.
- Knowledge/docs: `docs/knowledge/overview.md` captures the implemented M9 cockpit behavior and safety boundaries.
- GitHub status: M9 milestone #10 and issues #104 through #110 are closed; cockpit adoption PR #126 is merged.
- Result: previous milestone closure hook passed for M10.

## Start Gate Result

- [x] Previous milestone report exists or was not required.
- [x] Previous milestone durable facts are captured in `docs/knowledge/`.
- [x] Plan is complete enough to implement.
- [x] Product, architecture, data, API, infrastructure, rollout, and testing decisions needed to plan the milestone are decided or deferred.
- [x] GitHub tracking has detailed M10 implementation issues.
- [x] Knowledge docs capture durable planned behavior.

Status: Ready to implement. The previous milestone closure hook and M10 start gate passed on 2026-09-24; begin with #117, then #123.
