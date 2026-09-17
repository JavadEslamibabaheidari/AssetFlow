# M1 Report - Spec-First Workflow

## Status

M1 is complete. The milestone established the workflow and contracts needed before building the core inventory domain.

## Implemented

- Added a GitHub status workflow skill so issues, PRs, milestones, and local status stay synchronized.
- Strengthened repo knowledge rules and added planned-state validation for milestones, big issues, and features.
- Added the initial repo knowledge overview in `docs/knowledge/overview.md`.
- Added the OpenAPI style guide in `docs/openapi-style-guide.md`.
- Expanded the AI development workflow with intake, spec, task breakdown, review, and model-selection guidance.
- Added the first real inventory OpenAPI contract in `contracts/openapi/inventory-api.yaml`.

## Added to the App and Repository

- A contract-first path for Inventory API development.
- Clear conventions for operation IDs, schemas, error responses, versioning, and examples.
- A first inventory API design for vendors, products, channels, and stock items.
- A repeatable AI-assisted feature workflow.
- A required planned-vs-actual validation checkpoint before major work is considered done.

## Verification

- Each M1 task was completed in its own PR.
- CI passed before each PR was merged.
- All planned M1 issues are closed.

## Next

M2 can start core inventory domain implementation from the reviewed contracts and workflow created in M1.
