# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json ./
COPY api/Directory.Build.props api/Directory.Packages.props api/Renmember.slnx api/
COPY api/src/Renmember.Domain/Renmember.Domain.csproj api/src/Renmember.Domain/
COPY api/src/Renmember.Application/Renmember.Application.csproj api/src/Renmember.Application/
COPY api/src/Renmember.Infrastructure/Renmember.Infrastructure.csproj api/src/Renmember.Infrastructure/
COPY api/src/Renmember.Api/Renmember.Api.csproj api/src/Renmember.Api/
RUN dotnet restore api/src/Renmember.Api/Renmember.Api.csproj

COPY .editorconfig ./
COPY api/src api/src
RUN dotnet publish api/src/Renmember.Api/Renmember.Api.csproj \
    --configuration Release --no-restore --output /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Renmember.Api.dll"]
