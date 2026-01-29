# PowerMas - Practica Backend

Backend para el caso técnico: CRUD de **Beneficiarios** usando **ASP.NET Core Web API** y **Azure SQL (SQL Server)** mediante **Stored Procedures**.

## Stack
- ASP.NET Core Web API (.NET 9)
- SQL Server (Azure SQL)
- Stored Procedures (CRUD + listados)
- Docker (para deploy)

---

## Variables de Entorno

| Variable | Requerida | Descripción | Ejemplo |
|----------|-----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | ✅ Sí | Connection string de Azure SQL | `Server=tcp:xxx.database.windows.net,1433;...` |
| `CORS_ALLOWED_ORIGINS` | ⚠️ Producción | Orígenes permitidos (separados por coma) | `https://miapp.netlify.app,https://otro.com` |
| `ENABLE_SWAGGER` | ❌ No | Habilitar Swagger en Production | `true` |
| `ASPNETCORE_ENVIRONMENT` | ⚠️ Producción | Entorno de ejecución | `Production` |
| `ASPNETCORE_URLS` | ❌ No | URL/puerto de escucha (Docker lo configura) | `http://+:8080` |

> **Nota**: En .NET la convención para variables anidadas usa doble guion bajo: `ConnectionStrings__DefaultConnection`

---

## Requisitos
- .NET SDK 9.x
- Acceso a Azure SQL (o SQL Server)
- Docker (opcional, para deploy)
- SSMS o Azure Data Studio (opcional)

---

## Configuración Local

Este repositorio **no** incluye credenciales (passwords/tokens).

### Opción 1: appsettings.Development.json (recomendado)

Crea el archivo `PowerMas.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:tu-servidor.database.windows.net,1433;Initial Catalog=db_beneficiarios;User ID=<USER>;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=False;"
  }
}
```

### Opción 2: Variable de entorno

```bash
# PowerShell
$env:ConnectionStrings__DefaultConnection = "Server=tcp:..."

# Bash
export ConnectionStrings__DefaultConnection="Server=tcp:..."
```

---

## Ejecutar el Proyecto

### Desarrollo local (dotnet)

```bash
cd PowerMas.Api
dotnet restore
dotnet run
```

- API: `http://localhost:5005` (o puerto configurado)
- Swagger: `http://localhost:5005/swagger`
- Health: `http://localhost:5005/health`

### Con Docker

```bash
# Construir imagen
docker build -t powermas-api .

# Ejecutar contenedor
docker run -p 8080:8080 \
  -e "ConnectionStrings__DefaultConnection=Server=tcp:xxx.database.windows.net,1433;Initial Catalog=db_beneficiarios;User ID=xxx;Password=xxx;Encrypt=True;TrustServerCertificate=False;" \
  -e "CORS_ALLOWED_ORIGINS=http://localhost:5173" \
  -e "ENABLE_SWAGGER=true" \
  powermas-api
```

Probar:
- Health: `http://localhost:8080/health`
- Swagger: `http://localhost:8080/swagger`
- API: `http://localhost:8080/api/beneficiarios`

---

## Deploy en Render

### Configuración del servicio

1. **Tipo**: Web Service
2. **Runtime**: Docker
3. **Puerto**: 8080

### Variables de entorno en Render

```
ConnectionStrings__DefaultConnection = <TU_AZURE_SQL_CONNECTION_STRING>
CORS_ALLOWED_ORIGINS = https://tu-app.netlify.app
ASPNETCORE_ENVIRONMENT = Production
ENABLE_SWAGGER = true  (opcional, para debug)
```

### Firewall de Azure SQL

Configura el firewall de Azure SQL para permitir conexiones desde Render:
1. En Azure Portal → tu SQL Server → Networking
2. Agrega la IP de Render (o habilita "Allow Azure services" temporalmente)
3. Para IPs dinámicas de Render, considera usar Azure SQL con "Allow access to Azure services"

---

## Endpoints

### Health Check
- `GET /health` → `200 OK` con body `"ok"`

### Documentos de Identidad
- `GET /api/documentos-identidad/activos` → Lista documentos activos (para dropdown)

### Beneficiarios
- `GET /api/beneficiarios` → Lista todos
- `GET /api/beneficiarios/{id}` → Obtener por ID
- `POST /api/beneficiarios` → Crear
- `PUT /api/beneficiarios/{id}` → Actualizar
- `DELETE /api/beneficiarios/{id}` → Eliminar

---

## Base de datos

### Scripts SQL
Los scripts están en la carpeta `database/`:
- Creación de tablas (`DocumentoIdentidad`, `Beneficiario`)
- Inserts de ejemplo
- Stored Procedures:
  - `sp_DocumentoIdentidad_ListarActivos`
  - `sp_Beneficiario_Listar`
  - `sp_Beneficiario_ObtenerPorId`
  - `sp_Beneficiario_Crear`
  - `sp_Beneficiario_Actualizar`
  - `sp_Beneficiario_Eliminar`

---

## Validaciones

- `NumeroDocumento` se valida según el tipo de documento:
  - Debe cumplir la **Longitud** especificada
  - Si `SoloNumeros = 1`, debe contener solo dígitos
- `Sexo` debe ser: `M` o `F`

---

## Estructura del Proyecto

```
├── PowerMas.Api/          # API ASP.NET Core
│   ├── Controllers/       # Controladores REST
│   ├── Contracts/         # DTOs
│   ├── Data/              # Repositorios
│   ├── Domain/            # Modelos
│   ├── Middlewares/       # Manejo de errores
│   ├── Services/          # Lógica de negocio
│   └── Program.cs         # Configuración
├── database/              # Scripts SQL
├── Dockerfile             # Build multi-stage
└── .dockerignore
```

---

## Notas

- Se utiliza Azure SQL + Stored Procedures como parte del caso técnico
- No se suben secretos al repositorio
- CORS configurado dinámicamente por variable de entorno
- Swagger disponible en `/swagger` (habilitado en Dev, configurable en Prod)
