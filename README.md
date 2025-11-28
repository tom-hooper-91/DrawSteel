# Draw Steel
An aspiring character builder application for the Draw Steel RPG.

Built using dotnet with the following:
- Web API in the `API` project
- Interaction Driven Design core application:
    - `API`
    - `Application`
    - `Domain`
    - `Infrastructure`

Uses a `psakefile` for common commands.

## Feature: Consistent Character JSON Contract

- **Goal**: Flatten every Character API request/response identifier to a single `id` field and lock the contract with HTTP-level tests.
- **Docs**: See `specs/001-json-serialization/plan.md`, `specs/001-json-serialization/tasks.md`, and supporting research/data-model files for implementation details.
- **API Contract**: Contract samples live in `specs/001-json-serialization/contracts/character-json-contract.openapi.yaml`; update this file alongside any serializer change.
- **Quickstart**: Follow `specs/001-json-serialization/quickstart.md` for the exact commands (unit/integration tests, API run instructions) before modifying the serialization boundary.

## Developing locally

### Run the Application locally through docker-compose

```powershell
Invoke-Psake DockerRun
```

### Run Unit and Integration Tests

```powershell
Invoke-Psake Test
```

## Infra

Built for Azure Container Apps and managed via Terraform through the `infra` directory.

### Apply infrastructure

```powershell
Invoke-Psake ApplyInfra
```

### Plan infrastructure

```powershell
Invoke-Psake PlanInfra
```

### Destroy infrastructure

```powershell
Invoke-Psake DestroyInfra
```

## Deployment

Deployment through Github Actions currently triggers on push to `main` and will run tests, build and push to DockerHub and deploy to Azure.
