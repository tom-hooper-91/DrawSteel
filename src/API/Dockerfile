ARG APP_CONFIGURATION_MODE=Release
ARG STARTING_PROJECT=API/API.csproj
ARG DOTNET_VERSION=8.0

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS base
EXPOSE 80
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS restore
ENV PATH="${PATH}:/root/.dotnet/tools"
ARG STARTING_PROJECT
RUN dotnet tool install --global --no-cache dotnet-subset
WORKDIR /restore
COPY . .
RUN dotnet subset restore ${STARTING_PROJECT} --root-directory /restore --output restore_subset/

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
ARG APP_CONFIGURATION_MODE
ARG STARTING_PROJECT
WORKDIR /src
COPY --from=restore /restore/restore_subset .
RUN dotnet restore ${STARTING_PROJECT}
COPY API/ API/
COPY Application/ Application/
COPY Domain/ Domain/
COPY Infrastructure/ Infrastructure/
RUN mkdir -p /home/site/wwwroot && \
dotnet publish ${STARTING_PROJECT} \
  --configuration ${APP_CONFIGURATION_MODE} \
  --no-restore \
  --output /home/site/wwwroot

FROM base AS final
COPY --from=build ["/home/site/wwwroot", "/home/site/wwwroot"]