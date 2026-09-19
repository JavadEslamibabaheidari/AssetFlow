# Deployment

## Local Development

Run the API directly:

```bash
dotnet run --project src/Inventory.Api
```

Run with Docker:

```bash
docker compose up --build
```

For meaningful feature work, Docker Compose is the local end-to-end verification path. After a feature is developed and lower-level tests pass, build and run the relevant images and containers with `docker-compose.yml`/`docker compose` so the integrated service behavior is checked in a containerized environment before the work is considered complete.

As the frontend and additional services are introduced, keep Compose aligned with the app's deployable shape so backend services, frontend surfaces, databases, and supporting infrastructure can be validated together locally.

## Local Kubernetes

GitHub does not host Kubernetes applications directly. GitHub Actions can build, test, publish images, and deploy to a Kubernetes cluster.

For learning, this repo includes local Kubernetes manifests that can run in `kind` or `minikube`.

Host requirement: a running Docker-compatible container runtime is required before using `kind`. On this workstation, Docker Desktop was started and the Docker context was set to `desktop-linux`.

Build the local image:

```bash
docker build -t inventory-api:local .
```

For `kind`, load the image into the cluster:

```bash
kind create cluster --name assetflow-m0
kind load docker-image inventory-api:local --name assetflow-m0
```

For `minikube`, build against the Minikube Docker daemon or load the image:

```bash
minikube image load inventory-api:local
```

Apply manifests:

```bash
kubectl apply -f k8s/
```

Port forward:

```bash
kubectl port-forward service/inventory-api 8080:80
```

Call:

```bash
curl http://localhost:8080/health
```

## GitHub Actions Container Verification

The GitHub Actions CI workflow:

- restore dependencies
- build
- test
- build Docker image
- publish the Docker image to GitHub Container Registry on pushes to `main`

After CI succeeds on `main`, `.github/workflows/deploy.yml` verifies the
published image by running it as a Docker container inside the GitHub Actions
runner. It pulls the immutable `sha-<short-commit-sha>` image tag, starts the
API, checks `http://localhost:8080/health`, and removes the container.

GitHub Actions runners are temporary. This is a deployment verification step,
not persistent hosting. It proves the published service image can run in a clean
container environment without connecting to Kubernetes on a local machine.

Later workflows may deploy that same published image to:

- a real hosting target such as Azure Container Apps, Azure App Service, Fly.io,
  Render, Railway, AWS ECS, or Google Cloud Run
- a remote Kubernetes cluster when a cluster is available
