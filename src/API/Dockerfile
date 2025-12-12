ARG APP_CONFIGURATION_MODE=Release
ARG STARTING_PROJECT=API/API.csproj
ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS base
EXPOSE 8080
EXPOSE 8081
WORKDIR /app

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
RUN dotnet build ${STARTING_PROJECT} --configuration ${APP_CONFIGURATION_MODE} --no-restore

FROM build AS publish
ARG APP_CONFIGURATION_MODE
ARG STARTING_PROJECT
RUN dotnet publish ${STARTING_PROJECT} --configuration ${APP_CONFIGURATION_MODE} --no-build --output /app/publish

FROM base AS final
COPY --from=publish /app/publish /app
ENTRYPOINT ["dotnet", "API.dll"]
