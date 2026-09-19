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
| 1 | Define frontend architecture | Select stack, repo shape, API-client strategy, testing approach, and design-system rules. | M4 closure | Reviewed plan and architecture decision | To create after M4 |
| 2 | Scaffold frontend app | Create the repo/workspace, build tooling, local scripts, routing, and app shell. | Task 1 | Local run, lint/type-check, smoke render | To create after M4 |
| 3 | Add design system foundation | Add tokens, layout primitives, base components, icon approach, and accessibility conventions. | Task 2 | Component review and responsive checks | To create after M4 |
| 4 | Add API client foundation | Wire typed API access to the OpenAPI contract and environment configuration. | Tasks 1-2 | Generated/validated client and basic backend connectivity check | To create after M4 |
| 5 | Add frontend quality gates | Add unit/component testing and Playwright smoke-test foundation. | Tasks 2-4 | Quality commands pass locally and in CI where available | To create after M4 |
| 6 | Add Compose verification path | Build and run the frontend foundation with relevant backend services through Docker Compose. | Tasks 2-5 | `docker compose up --build` and smoke checks | To create after M4 |
| 7 | Complete M5 validation and docs | Verify implementation against the plan and update knowledge. | Tasks 1-6 | Planned-state validation and docs update | To create after M4 |

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
- Issues: create after M4 closure and detailed M5 planning.
- Project board: keep each active task isolated to one branch and pull request unless explicitly combined.

## Previous Milestone Closure

- Report: M4 report must exist before M5 implementation starts.
- Knowledge/docs: M4 durable facts must be captured in `docs/knowledge/`.
- GitHub status: M4 issues and milestone state must match the completed implementation.
- Result: Pending M4 completion.

## Start Gate Result

- [ ] Previous milestone report exists or was not required.
- [ ] Previous milestone durable facts are captured in docs/knowledge.
- [ ] Plan is complete enough to implement.
- [ ] No implementation-blocking decisions remain undecided.
- [ ] GitHub tracking matches this plan.
- [ ] Knowledge docs capture durable planned behavior.

Status: Not ready
