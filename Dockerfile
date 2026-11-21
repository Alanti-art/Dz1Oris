# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files for restore
COPY server/MyHttpServer.sln server/MyHttpServer.sln
COPY server/MyHttpServer/MyHttpServer.csproj server/MyHttpServer/MyHttpServer.csproj
COPY server/MiniHttpServer.ORM/MiniHttpServer.ORM.csproj server/MiniHttpServer.ORM/MiniHttpServer.ORM.csproj
COPY server/MiniTemplateEngine/MiniTemplateEngine.csproj server/MiniTemplateEngine/MiniTemplateEngine.csproj
COPY server/MyHttpServer.UnitTests/MyHttpServer.UnitTests.csproj server/MyHttpServer.UnitTests/MyHttpServer.UnitTests.csproj
COPY server/HttpTemplateRenderTestUnit/HttpTemplateRenderTestUnit.csproj server/HttpTemplateRenderTestUnit/HttpTemplateRenderTestUnit.csproj

RUN dotnet restore server/MyHttpServer.sln

# Copy the rest of the source and publish the web app
COPY server/ server/
RUN dotnet publish server/MyHttpServer/MyHttpServer.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Docker-specific settings (binds to all interfaces, connects to db service)
COPY docker/settings.docker.json ./settings.json

EXPOSE 8888
ENTRYPOINT ["dotnet", "MyHttpServer.dll"]
