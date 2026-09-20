# AssetFlow Web API Client

The frontend API boundary is generated from `../../contracts/openapi/inventory-api.yaml`.

## Regenerate Contract Types

```bash
cd src/AssetFlow.Web
npm run generate:api
```

Generated types are written to `src/api/generated/inventory-api.ts`. Do not edit that file directly; update the OpenAPI contract and regenerate it.

## Client Shape

- `ApiHttpClient` owns base URL handling, JSON headers, query-string building, network errors, and Problem Details mapping.
- `InventoryApiClient` exposes typed Inventory API methods using generated operation response types.
- `ApiError.kind` normalizes `400`, `404`, `409`, unexpected status codes, and network failures for UI code.

Runtime configuration uses `VITE_ASSETFLOW_API_BASE_URL`, falling back to `http://localhost:8080`.
