# AssetFlow Web

AssetFlow Web is the M5 frontend foundation. It is a React, TypeScript, and Vite app shell for future inventory and reservation workflows.

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

## Scripts

- `npm run dev`: start the Vite dev server
- `npm run build`: type-check and build the frontend
- `npm run preview`: preview the production build locally
- `npm run typecheck`: run TypeScript without emitting files

## Design System

Design tokens, starter primitives, icon guidance, and accessibility conventions live in `src/design-system/README.md`. Use those primitives for route headings, metric cards, panels, status labels, and definition-list settings before creating new page-specific patterns.

## API Client

Typed Inventory API access lives in `src/api`. Regenerate OpenAPI types after contract changes:

```bash
npm run generate:api
```

The generated file is `src/api/generated/inventory-api.ts`; do not edit it directly.

## Scope

This shell intentionally stops short of full product workflows. API-backed vendor, product, channel, stock item, reservation, and availability screens belong to M6 after the M5 foundation adds design-system, API-client, quality-gate, and Docker Compose slices.
