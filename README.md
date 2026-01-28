# PowerMas - Practica Backend

Backend para el caso técnico: CRUD de **Beneficiarios** usando **ASP.NET Core Web API** y **Azure SQL (SQL Server)** mediante **Stored Procedures**.

## Stack
- ASP.NET Core Web API (.NET 9)
- SQL Server (Azure SQL)
- Stored Procedures (CRUD + listados)

---

## Requisitos
- .NET SDK 9.x
- Acceso a Azure SQL (o SQL Server)
- SSMS o Azure Data Studio (opcional)

---

## Configuración

Este repositorio **no** incluye credenciales (passwords/tokens).  
Configura la cadena de conexión en **PowerMas.Api/appsettings.Development.json** (recomendado) o como variable de entorno.

### Connection String (ejemplo)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:powermas-omar-sql-2026.database.windows.net,1433;Initial Catalog=db_beneficiarios;Persist Security Info=False;User ID=<USER>;Password=<PASSWORD>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

## Base de datos

- Servidor: `powermas-omar-sql-2026.database.windows.net`
- Base de datos: `db_beneficiarios`

### Scripts
Los scripts SQL estan en la carpeta:

- `database/`

Contenido esperado:
- Creación de tablas (por ejemplo: `DocumentoIdentidad`, `Beneficiario`)
- Inserts de ejemplo en `DocumentoIdentidad`
- Stored Procedures:
  - `sp_DocumentoIdentidad_ListarActivos`
  - `sp_Beneficiario_Listar`
  - `sp_Beneficiario_ObtenerPorId`
  - `sp_Beneficiario_Crear`
  - `sp_Beneficiario_Actualizar`
  - `sp_Beneficiario_Eliminar`

---

## Ejecutar el proyecto

Desde la raíz del repositorio:

```bash
cd PowerMas.Api
dotnet restore
dotnet run
```

- API (local): `http://localhost:5005`
- Swagger: `http://localhost:5005/swagger`

> Nota: La ruta `/` puede devolver 404 y es normal en una Web API.

---

## Endpoints (planificados)

### Documentos de Identidad
- `GET /api/documentos-identidad`  
  Devuelve únicamente documentos **Activos = 1** (para el dropdown del frontend).

### Beneficiarios
- `GET /api/beneficiarios` (lista)
- `GET /api/beneficiarios/{id}` (detalle)
- `POST /api/beneficiarios` (crear)
- `PUT /api/beneficiarios/{id}` (actualizar)
- `DELETE /api/beneficiarios/{id}` (eliminar)

---

## Validaciones

- `NumeroDocumento` se valida según el tipo de documento seleccionado:
  - Debe cumplir la **Longitud**
  - Si `SoloNumeros = 1`, debe contener solo dígitos
- `Sexo` debe ser: `M` o `F`

---

## Estructura del proyecto

- `PowerMas.Api/` → API ASP.NET Core
- `database/` → Scripts SQL (tablas, inserts, stored procedures)

---

## Notas

- Se utiliza Azure SQL + Stored Procedures como parte del caso técnico.
- No se suben secretos al repositorio (usar `appsettings.Development.json` o variables de entorno).
