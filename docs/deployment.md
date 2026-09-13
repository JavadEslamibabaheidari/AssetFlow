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

## Local Kubernetes

GitHub does not host Kubernetes applications directly. GitHub Actions can build, test, publish images, and deploy to a Kubernetes cluster.

For learning, this repo includes local Kubernetes manifests that can run in `kind` or `minikube`.

Build the local image:

```bash
docker build -t inventory-api:local .
```

For `kind`, load the image into the cluster:

```bash
kind load docker-image inventory-api:local
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

## GitHub Deployment Direction

The first GitHub Actions workflow will:

- restore dependencies
- build
- test
- build Docker image

Later workflows may:

- publish Docker image to GitHub Container Registry
- deploy to a real hosting target
- deploy to a Kubernetes cluster when a cluster is available
