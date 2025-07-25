# Draw Steel
An aspiring character builder application for the Draw Steel RPG.

Built using dotnet with the following:
- Function app in the `API` project
- Interaction Driven Design core application:
    - `API`
    - `Application`
    - `Domain`
    - `Infrastructure`

Uses a `psakefile` for common commands.

## Developing locally

### Run the Application locally through docker-compose
```
Invoke-Psake DockerRun
```

### Run Unit and Integration Tests
```
Invoke-Psake Test
```

## Infra
Built for Azure Container Apps and managed via Terraform through the `infra` directory.

### Apply infrastructure
```
Invoke-Psake ApplyInfra
```
### Plan infrastructure
```
Invoke-Psake PlanInfra
```
### Destroy infrastructure
```
Invoke-Psake DestroyInfra
```

## Deployment
Deployment through Github Actions currently triggers on push to `main` and will run tests, build and push to DockerHub and deploy to Azure.
