# Milestone Plan: M5 - Frontend Application Foundation

## Goal

Create the frontend foundation for AssetFlow as a modern, maintainable product application before building full inventory workflows.

## Scope

In scope:

- Decide the frontend architecture, repository shape, framework, build tooling, package management, routing, state/data-fetching approach, design-system foundation, and API-client strategy.
- Create the frontend repository or workspace structure.
- Build the first app shell with routing, navigation, responsive layout, error boundaries, environment configuration, and placeholder product areas.
- Establish a design system foundation with tokens, component conventions, icons, accessibility rules, and responsive behavior.
- Generate or maintain typed API access from the OpenAPI contracts so frontend and backend stay aligned.
- Add frontend quality gates: linting, formatting, type checks, unit/component tests, and a small Playwright smoke-test setup.
- Keep Docker Compose aligned so the frontend app can be built and run with the relevant backend services for local end-to-end verification.
- Document frontend architecture and local development.

Out of scope:

- Completing all inventory, reservation, and availability screens. That is M6.
- Replacing backend integration tests with browser tests.
- Building deep dashboard automation, observability, or Agentic OS features.
- Choosing external workspace or memory products.

## Architecture Principles

- The frontend should be treated as a real product surface, not a temporary demo.
- Prefer a simple, strongly typed, component-driven architecture with clear boundaries between UI components, route/page composition, API clients, and domain-specific view models.
- Keep business rules authoritative in backend services. Frontend code should present workflows, validate obvious input mistakes, and map API responses/errors clearly, but it must not duplicate backend domain decisions as a second source of truth.
- Use OpenAPI-generated or OpenAPI-validated API clients where practical.
- Design for operational workflows: dense, scannable, accessible, responsive, and efficient for repeated use.
- Document decisions well enough that future frontend work can be done consistently by someone without prior frontend background.

## Architecture Decision

M5 will add a first-party frontend workspace under `src/AssetFlow.Web` using React, TypeScript, Vite, React Router, TanStack Query, Vitest, Testing Library, Playwright, ESLint, and Prettier. Package management will use npm to keep the host and CI requirements familiar and avoid introducing a monorepo package manager before the repository needs one.

The frontend will remain in this repository so backend contracts, Docker Compose, CI, and milestone documentation can evolve together. It will consume the checked-in OpenAPI contract from `contracts/openapi/inventory-api.yaml` through a typed generated client or generated TypeScript types plus a small hand-written fetch wrapper. Runtime API base URLs will come from explicit environment variables, not committed secrets.

The app shell will be operational-tooling-first: dense, scannable, responsive, accessible, and built around routes, navigation, loading/error states, and placeholder product areas. M5 may include non-functional placeholders for inventory areas, but full vendor/product/channel/stock/reservation workflows remain M6.

## Deliverables

- Final frontend architecture decision and milestone start-gate update.
- Frontend app repository or workspace with documented setup.
- App shell, routing, layout, navigation, and base error/loading states.
- Design tokens and starter component set.
- API client strategy wired to the backend OpenAPI contract.
- Frontend CI/local quality checks.
- Playwright smoke-test foundation.
- Docker Compose build/run path for the frontend foundation and relevant backend service.
- Updated project knowledge and roadmap links.

## Acceptance Criteria

- [ ] A new developer can run the frontend locally from documented commands.
- [ ] The app shell renders on desktop and mobile viewports without broken layout or overlapping UI.
- [ ] Routing, navigation, global error handling, and loading states are present.
- [ ] Shared UI primitives and design tokens exist and are documented.
- [ ] Frontend API access is typed and traceable to the backend OpenAPI contract.
- [ ] Lint, format, type-check, unit/component test, and Playwright smoke-test commands exist.
- [ ] Docker Compose can build and run the frontend foundation with the relevant backend service configuration.
- [ ] No secrets or environment-specific credentials are committed.
- [ ] Knowledge docs describe the frontend foundation and how future screens should be built.

## Task Breakdown

| Order | Task | Objective | Depends on | Verification | GitHub issue |
| --- | --- | --- | --- | --- | --- |
| 1 | Define frontend architecture | Select stack, repo shape, API-client strategy, testing approach, and design-system rules. | M4 closure | Reviewed plan and architecture decision | #60 |
| 2 | Scaffold frontend app | Create the repo/workspace, build tooling, local scripts, routing, and app shell. | Task 1 | Local run, lint/type-check, smoke render | #61 |
| 3 | Add design system foundation | Add tokens, layout primitives, base components, icon approach, and accessibility conventions. | Task 2 | Component review and responsive checks | #62 |
| 4 | Add API client foundation | Wire typed API access to the OpenAPI contract and environment configuration. | Tasks 1-2 | Generated/validated client and basic backend connectivity check | #63 |
| 5 | Add frontend quality gates | Add unit/component testing and Playwright smoke-test foundation. | Tasks 2-4 | Quality commands pass locally and in CI where available | #64 |
| 6 | Add Compose verification path | Build and run the frontend foundation with relevant backend services through Docker Compose. | Tasks 2-5 | `docker compose up --build` and smoke checks | #65 |
| 7 | Complete M5 validation and docs | Verify implementation against the plan and update knowledge. | Tasks 1-6 | Planned-state validation and docs update | #66 |

## Prioritized Execution

1. Critical prerequisite: #60 defines and records the architecture decision so all later branches share the same stack and boundaries.
2. High implementation: #61 creates the frontend workspace, scripts, routing, app shell, and basic run path.
3. High implementation: #62 adds design tokens and starter UI primitives before feature screens multiply.
4. High integration: #63 wires typed OpenAPI-aligned API access and environment configuration.
5. High verification: #64 adds frontend linting, formatting, type checks, unit/component tests, and Playwright smoke tests.
6. Medium integration: #65 adds the Docker Compose frontend/backend verification path after the app and checks exist.
7. Critical validation: #66 compares the final implementation against this plan, updates knowledge, and closes M5 tracking.

## Risks and Mitigations

- Risk: choosing a frontend stack before product workflows are understood. Mitigation: make M5 focus on app architecture, routing, design-system primitives, API-client strategy, and testing rather than overbuilding screens.
- Risk: frontend and backend contracts drift. Mitigation: use OpenAPI as the boundary and add validation or generation into the frontend workflow.
- Risk: UI code becomes hard to maintain for someone new to frontend. Mitigation: prefer clear component boundaries, concise documentation, readable naming, and examples for common patterns.
- Risk: E2E tests become expensive too early. Mitigation: start with a tiny smoke suite in M5 and expand it only around high-value M6 workflows.

## Testing Strategy

- Static checks: formatting, linting, and type checking.
- Unit/component: reusable components, formatting helpers, API error mapping, and view-model logic.
- Visual/manual: desktop and mobile review for layout stability, text fit, contrast, and focus states.
- E2E: small Playwright smoke suite proving the app loads, navigation works, and the shell can reach configured routes.
- Containerized: Docker Compose build/run verification for the frontend app and relevant backend service configuration.

## GitHub Tracking

- Milestone: `M5 - Frontend Application Foundation`
- Issues: #60 through #66.
- Project board: keep each active task isolated to one branch and pull request unless explicitly combined. Project-board field access is not yet verified from this environment and must be recorded as a sync gap if it remains unavailable.

## Previous Milestone Closure

- Report: `docs/milestone-4-report.md` exists and records planned-state validation.
- Knowledge/docs: M4 durable facts are captured in `docs/knowledge/overview.md`.
- GitHub status: M4 milestone #5 is closed and issues #49 through #53 are closed.
- Result: Passed.

## Start Gate Result

- [x] Previous milestone report exists or was not required.
- [x] Previous milestone durable facts are captured in docs/knowledge.
- [x] Plan is complete enough to implement.
- [x] No implementation-blocking decisions remain undecided.
- [x] GitHub tracking matches this plan.
- [x] Knowledge docs capture durable planned behavior.

Status: Ready to start
