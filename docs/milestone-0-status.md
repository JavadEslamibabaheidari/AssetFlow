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
- Verified GitHub milestones, labels, issues, and project tracking
- Verified local Kubernetes run path with `kind`

## Verified

Commands run locally:

```bash
dotnet build src/Inventory.Api/Inventory.Api.csproj --no-restore
dotnet test --configuration Release
docker build -t inventory-api:local .
docker run --rm -d -p 18080:8080 --name assetflow-inventory-api-smoke inventory-api:local
curl http://localhost:18080/health
docker stop assetflow-inventory-api-smoke
kind create cluster --name assetflow-m0
kind load docker-image inventory-api:local --name assetflow-m0
kubectl apply -f k8s/
kubectl rollout status deployment/inventory-api --timeout=120s
kubectl port-forward service/inventory-api 8080:80
curl http://localhost:8080/health
```

Health response:

```json
{
  "status": "Healthy",
  "service": "Inventory API"
}
```

Kubernetes health response:

```json
{
  "status": "Healthy",
  "service": "Inventory API",
  "checkedAtUtc": "2026-09-17T20:20:46.6106586+00:00"
}
```

## Open

- No open M0 work remains.
