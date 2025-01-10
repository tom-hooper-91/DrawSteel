# Draw Steel
An aspiring character builder application for the Draw Steel RPG.

Built using dotnet with the following:
- Function app in the `API` project
- Web app in the `Web` project
- Interaction Driven Design core application:
    - `Application`
    - `Domain`
    - `Infrastructure`

Uses a `psakefile` for common commands.

## Developing locally

### Run the Application locally through docker-compose
```
Invoke-Psake DockerRun
```

## Infra
Built for Azure Container Apps and managed via Terraform through the `infra` directory.

### Apply infrastrucure
```
Invoke-Psake ApplyInfra
```
### Plan infrastructure
```
Invoke-Psake PlanInfra
```
### Destory infrastructure
```
Invoke-Psake DestroyInfra
```

## Deployment
Deployment through Github Actions currently triggers on push to `main` and will run tests, build and push to DockerHub and deploy to Azure.
