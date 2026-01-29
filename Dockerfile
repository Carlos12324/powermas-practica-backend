# ============================================
# Dockerfile multi-stage para PowerMas API
# .NET 9.0
# ============================================

# ----- Stage 1: Build -----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar csproj y restaurar dependencias
COPY PowerMas.Api/PowerMas.Api.csproj PowerMas.Api/
RUN dotnet restore PowerMas.Api/PowerMas.Api.csproj

# Copiar el resto del código y compilar
COPY PowerMas.Api/ PowerMas.Api/
WORKDIR /src/PowerMas.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# ----- Stage 2: Runtime -----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Configuración del contenedor
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Copiar artefactos publicados
COPY --from=build /app/publish .

# Usuario no-root para seguridad
USER $APP_UID

ENTRYPOINT ["dotnet", "PowerMas.Api.dll"]
