ARG APP_CONFIGURATION_MODE=Release
ARG STARTING_PROJECT=API/API.csproj
ARG DOTNET_RUNTIME_TAG=4-dotnet-isolated8.0

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:${DOTNET_RUNTIME_TAG} AS base
EXPOSE 80
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG APP_CONFIGURATION_MODE
ARG STARTING_PROJECT
WORKDIR /src
# COPY API/API.csproj API/
# COPY Application/Application.csproj Application/
# COPY Domain/Domain.csproj Domain/
# COPY Infrastructure/Infrastructure.csproj Infrastructure/
# RUN dotnet restore ${STARTING_PROJECT}
COPY . .
RUN dotnet build ${STARTING_PROJECT} \
--configuration ${APP_CONFIGURATION_MODE}
# --no-restore

FROM build AS publish
ARG APP_CONFIGURATION_MODE
ARG STARTING_PROJECT
RUN mkdir -p /home/site/wwwroot && \
dotnet publish ${STARTING_PROJECT} \
  --configuration ${APP_CONFIGURATION_MODE} \
  --no-build \
  --output /home/site/wwwroot

FROM base AS final
COPY --from=publish ["/home/site/wwwroot", "/home/site/wwwroot"]