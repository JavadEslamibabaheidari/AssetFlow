# GitHub Backlog

These issues are the first items to create in GitHub after the repository is published.

## Milestone: M0 - Repository and Hello Service

### [Task] Create repository skeleton

Goal: create the initial .NET solution, docs folder, GitHub templates, Docker files, Kubernetes files, and CI workflow.

Checklist:

- [ ] Create solution file
- [ ] Create `Inventory.Api`
- [ ] Create initial docs
- [ ] Create GitHub issue templates
- [ ] Create Dockerfile and Compose file
- [ ] Create Kubernetes manifests
- [ ] Create GitHub Actions CI workflow

Verification:

- Repository structure is committed
- README explains current milestone

### [Task] Add minimal Inventory API endpoints

Goal: expose a minimal API that proves the service is running.

Checklist:

- [ ] Add `GET /`
- [ ] Add `GET /health`
- [ ] Return clear JSON responses

Verification:

- `GET /health` returns HTTP 200
- Docker container can serve the endpoint

### [Task] Add Docker local run path

Goal: run the service locally through Docker.

Checklist:

- [ ] Add `Dockerfile`
- [ ] Add `docker-compose.yml`
- [ ] Document commands in README or deployment docs

Verification:

- `docker compose up --build` starts the API
- `curl http://localhost:8080/health` returns HTTP 200

### [Task] Add local Kubernetes run path

Goal: run the service in local Kubernetes using `kind` or `minikube`.

Checklist:

- [ ] Add deployment manifest
- [ ] Add service manifest
- [ ] Document image loading and port-forwarding steps

Verification:

- Pod becomes ready
- `kubectl port-forward service/inventory-api 8080:80` works
- `curl http://localhost:8080/health` returns HTTP 200

### [Task] Add GitHub Actions CI

Goal: run build verification on push and pull request.

Checklist:

- [ ] Restore dependencies
- [ ] Build solution
- [ ] Run tests
- [ ] Build Docker image

Verification:

- CI workflow passes on GitHub

## Milestone: M1 - Spec-First Workflow

### [Feature] Define OpenAPI style guide

Goal: document how APIs will be designed before implementation.

Acceptance criteria:

- operationId naming convention exists
- request/response schema rules exist
- error response style exists
- versioning decision exists
- examples are included

### [Feature] Define AI feature workflow

Goal: document the repeatable workflow from requirement to implementation.

Acceptance criteria:

- requirement intake template exists
- spec template exists
- task breakdown template exists
- review checklist exists
- model selection guidance exists

### [Feature] Design first real inventory contract

Goal: create the OpenAPI contract for vendors, products, channels, and stock items.

Acceptance criteria:

- OpenAPI file exists
- schemas are defined
- endpoint descriptions are clear
- edge cases are listed
- tests are planned before implementation

