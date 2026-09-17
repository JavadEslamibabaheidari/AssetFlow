# OpenAPI Style Guide

This guide defines how AssetFlow REST APIs are designed before implementation. Every public HTTP API should have an OpenAPI contract reviewed before code is written.

## Contract Location

Store OpenAPI contracts under `contracts/openapi/`.

Use one contract per API service:

```text
contracts/openapi/inventory-api.yaml
```

Use OpenAPI `3.1.0` unless a tool forces a lower version.

## Paths and Methods

Use plural resource names and stable nouns:

```yaml
paths:
  /v1/vendors:
    get:
      operationId: listVendors
    post:
      operationId: createVendor
  /v1/vendors/{vendorId}:
    get:
      operationId: getVendor
```

Prefer shallow paths. Use nested paths only when the child resource has no useful identity outside the parent.

## Operation IDs

Use lower camel case operation IDs with a verb followed by the domain resource name.

Preferred verbs:

- `list`
- `get`
- `create`
- `update`
- `delete`
- `reserve`
- `release`
- `sync`

Examples:

```yaml
operationId: listProducts
operationId: getStockItem
operationId: createReservation
operationId: releaseReservation
```

Operation IDs must be unique within the contract and stable once implemented.

## Schemas

Use PascalCase schema names and lower camel case property names.

```yaml
components:
  schemas:
    Product:
      type: object
      required:
        - id
        - sku
        - name
      properties:
        id:
          type: string
          format: uuid
        sku:
          type: string
          minLength: 1
        name:
          type: string
          minLength: 1
```

Separate request and response schemas when fields differ.

```yaml
CreateProductRequest:
  type: object
  required:
    - sku
    - name
  properties:
    sku:
      type: string
      minLength: 1
    name:
      type: string
      minLength: 1

ProductResponse:
  type: object
  required:
    - id
    - sku
    - name
  properties:
    id:
      type: string
      format: uuid
    sku:
      type: string
    name:
      type: string
```

Use explicit validation constraints such as `minLength`, `maxLength`, `minimum`, `maximum`, `pattern`, and `format` when they are part of the product rule.

Use ISO 8601 strings for dates and times:

```yaml
createdAtUtc:
  type: string
  format: date-time
```

## Responses

Success responses should use explicit status codes and schemas.

```yaml
responses:
  "200":
    description: Product found.
    content:
      application/json:
        schema:
          $ref: "#/components/schemas/ProductResponse"
  "404":
    $ref: "#/components/responses/NotFound"
```

For collection endpoints, return an object wrapper so pagination can evolve without breaking clients.

```yaml
ProductListResponse:
  type: object
  required:
    - items
  properties:
    items:
      type: array
      items:
        $ref: "#/components/schemas/ProductResponse"
```

## Errors

All non-2xx responses that include a body should use an RFC 7807 style problem response.

```yaml
ProblemDetails:
  type: object
  required:
    - type
    - title
    - status
  properties:
    type:
      type: string
      format: uri
    title:
      type: string
    status:
      type: integer
    detail:
      type: string
    instance:
      type: string
    traceId:
      type: string
```

Common reusable responses:

```yaml
components:
  responses:
    BadRequest:
      description: The request is invalid.
      content:
        application/problem+json:
          schema:
            $ref: "#/components/schemas/ProblemDetails"
    NotFound:
      description: The requested resource was not found.
      content:
        application/problem+json:
          schema:
            $ref: "#/components/schemas/ProblemDetails"
    Conflict:
      description: The request conflicts with current resource state.
      content:
        application/problem+json:
          schema:
            $ref: "#/components/schemas/ProblemDetails"
```

Use `400` for validation or malformed requests, `404` for missing resources, `409` for state conflicts such as duplicate SKU or reservation conflicts, and `500` only for unexpected server errors.

## Versioning

Use URL path versioning for public APIs:

```text
/v1/products
```

Start with `v1`. Add a new version only for breaking contract changes. Non-breaking additions, such as optional response fields or new endpoints, stay in the current version.

## Examples

Every operation should include at least one realistic request or response example. Examples must use domain language from the current milestone.

```yaml
examples:
  product:
    summary: Marketplace product
    value:
      id: "4a1a8c74-b4e4-4b2a-93a8-44d9352fb3c1"
      sku: "TSHIRT-BLK-M"
      name: "Black T-Shirt Medium"
```

## Review Checklist

Before implementation starts, confirm:

- operation IDs follow the verb plus resource convention
- schemas separate create/update requests from responses where needed
- validation constraints are explicit
- problem responses use `application/problem+json`
- versioning uses the `/v1` path prefix
- examples are realistic and match the target domain
- acceptance criteria and tests can be traced back to the contract
