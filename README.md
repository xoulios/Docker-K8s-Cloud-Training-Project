# Docker & Kubernetes Cloud Training Project

[![CI](https://github.com/xoulios/Docker-K8s-Cloud-Training-Project/actions/workflows/ci.yml/badge.svg)](https://github.com/xoulios/Docker-K8s-Cloud-Training-Project/actions/workflows/ci.yml)

A RESTful movie/series catalog API built as a hands-on exploration of containerization and
orchestration: a Clean Architecture .NET Web API, containerized with **Docker**, orchestrated
locally with **Docker Compose**, and deployed to a **Kubernetes** cluster (minikube).

University cloud technologies course project — built to genuinely learn the tooling, not just to
tick boxes: multi-stage Docker builds, container security practices, health-checked service
dependencies, and Kubernetes primitives (Deployment, StatefulSet, headless Services,
PersistentVolumes).

## Tech stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core Web API (.NET 10), controllers, OpenAPI/Swagger |
| Domain | Rich domain model, invariant validation, Result pattern |
| Persistence | EF Core, SQL Server |
| Tests | xUnit (domain unit tests + integration tests) |
| Logging | Serilog (structured) |
| Containers | Docker (multi-stage builds), Docker Compose |
| Orchestration | Kubernetes manifests (Deployment, StatefulSet, Services, PVC) — minikube |
| CI/CD | GitHub Actions |

## Architecture

Clean Architecture, dependency rule pointing inward:

```
MovieStreaming.Api             → composition root, controllers, DI wiring
MovieStreaming.Infrastructure  → EF Core, persistence, external concerns
MovieStreaming.Domain          → entities, invariants, no external dependencies
MovieStreaming.Tests           → unit + integration tests
```

## Getting started

```bash
dotnet build
dotnet test
dotnet run --project src/MovieStreaming.Api
```

Docker and Kubernetes instructions land in their respective sections as those phases are built
out — see `docker/`, `docker-compose.yml`, and `yaml/` once available.

## Status

This project is being built incrementally, phase by phase:

- [ ] REST API + database
- [ ] Docker containerization
- [ ] Docker Compose orchestration
- [ ] Kubernetes deployment (minikube)

## License

MIT — see [LICENSE](LICENSE).
