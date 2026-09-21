# AssetFlow Web

AssetFlow Web is the frontend surface for AssetFlow inventory operations. It is a React, TypeScript, and Vite app with API-backed inventory and reservation workflows.

## Requirements

- Node.js 20 or newer
- npm 10 or newer

## Local Development

```bash
cd src/AssetFlow.Web
npm install
npm run dev
```

The app runs on `http://localhost:5173` by default.

Set the backend API base URL when it differs from the local default:

```bash
VITE_ASSETFLOW_API_BASE_URL=http://localhost:8080 npm run dev
```

For local files, copy `.env.example` to `.env.local` and adjust the value without committing machine-specific settings.

## Docker

Build and run the frontend with the backend from the repository root:

```bash
docker compose up --build
```

The frontend container listens on `http://localhost:5173` and serves `/health`. If `5173` is already busy locally, run `ASSETFLOW_WEB_PORT=6173 docker compose up --build` to choose another host port. Its browser-facing API base URL is set at image build time through `VITE_ASSETFLOW_API_BASE_URL`, defaulting to `http://localhost:8080` in `docker-compose.yml`.

## Scripts

- `npm run dev`: start the Vite dev server
- `npm run build`: type-check and build the frontend
- `npm run format:check`: verify Prettier formatting
- `npm run lint`: run ESLint
- `npm run preview`: preview the production build locally
- `npm run test`: run Vitest component/unit tests
- `npm run test:e2e`: run Playwright smoke tests
- `npm run typecheck`: run TypeScript without emitting files

## Design System

Design tokens, starter primitives, icon guidance, and accessibility conventions live in `src/design-system/README.md`. Use those primitives for route headings, metric cards, panels, status labels, and definition-list settings before creating new page-specific patterns.

Shared M6 workflow states live in `src/shared/workflowStates.tsx`. Use them for loading, empty, success, validation, conflict, not-found, and generic error states so feature slices present backend feedback consistently.

## API Client

Typed Inventory API access lives in `src/api`. Regenerate OpenAPI types after contract changes:

```bash
npm run generate:api
```

The generated file is `src/api/generated/inventory-api.ts`; do not edit it directly.

## M6 Workflow Model

- Assets is the primary inventory workspace for vendors, products, stock items, and reservation-aware availability.
- Reservations owns create, list, detail, release, expiration, and oversell-conflict workflows.
- Markets owns sales-channel master data until event-driven channel synchronization is added later.
- Keep planning and milestone status in docs and GitHub issues, not in the visible app UI.
