# M0 Status - Repository and Hello Service

## Goal

Prove that AssetFlow has a working repository skeleton, a minimal API service, Docker support, local Kubernetes manifests, CI configuration, and project planning docs.

## Completed

- Created ASP.NET Core `Inventory.Api`
- Added `GET /`
- Added `GET /health`
- Added Dockerfile
- Added Docker Compose file
- Added local Kubernetes deployment and service manifests
- Added GitHub Actions CI workflow
- Added GitHub issue templates
- Added planning docs for vision, roadmap, architecture, AI workflow, deployment, and project management

## Verified

Commands run locally:

```bash
dotnet build src/Inventory.Api/Inventory.Api.csproj --no-restore
dotnet test --configuration Release
docker build -t inventory-api:local .
docker run --rm -d -p 18080:8080 --name assetflow-inventory-api-smoke inventory-api:local
curl http://localhost:18080/health
docker stop assetflow-inventory-api-smoke
```

Health response:

```json
{
  "status": "Healthy",
  "service": "Inventory API"
}
```

## Open

- Initialize/push Git repository after GitHub access is ready.
- Create GitHub milestones, labels, issues, and project board.
- Verify local Kubernetes with `kind` or `minikube`.

