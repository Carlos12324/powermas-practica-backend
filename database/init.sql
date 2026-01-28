/* =========================================================
   PowerMas - Caso Evaluación
   DB: db_beneficiarios
   Incluye: Tablas + Data ejemplo DocumentoIdentidad + SPs
   ========================================================= */

SET NOCOUNT ON;
GO

/* =========================
   00) Limpieza (opcional)
   ========================= */
IF OBJECT_ID('dbo.sp_Beneficiario_Eliminar', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Beneficiario_Eliminar;
IF OBJECT_ID('dbo.sp_Beneficiario_Actualizar', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Beneficiario_Actualizar;
IF OBJECT_ID('dbo.sp_Beneficiario_Crear', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Beneficiario_Crear;
IF OBJECT_ID('dbo.sp_Beneficiario_ObtenerPorId', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Beneficiario_ObtenerPorId;
IF OBJECT_ID('dbo.sp_Beneficiario_Listar', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Beneficiario_Listar;

IF OBJECT_ID('dbo.sp_DocumentoIdentidad_ListarActivos', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_DocumentoIdentidad_ListarActivos;

-- helper de validación (si existe)
IF OBJECT_ID('dbo.sp_Beneficiario_ValidarDocumento', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Beneficiario_ValidarDocumento;

IF OBJECT_ID('dbo.Beneficiario', 'U') IS NOT NULL DROP TABLE dbo.Beneficiario;
IF OBJECT_ID('dbo.DocumentoIdentidad', 'U') IS NOT NULL DROP TABLE dbo.DocumentoIdentidad;
GO

/* =========================
   01) Tablas (según PDF)
   ========================= */
CREATE TABLE dbo.DocumentoIdentidad (
    Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DocumentoIdentidad PRIMARY KEY,
    Nombre        VARCHAR(50)  NOT NULL,
    Abreviatura   VARCHAR(10)  NOT NULL,
    Pais          VARCHAR(50)  NOT NULL,
    Longitud      INT          NOT NULL,
    SoloNumeros   BIT          NOT NULL,
    Activo        BIT          NOT NULL
);
GO

CREATE TABLE dbo.Beneficiario (
    Id                   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Beneficiario PRIMARY KEY,
    Nombres              VARCHAR(100) NOT NULL,
    Apellidos            VARCHAR(100) NOT NULL,
    DocumentoIdentidadId INT NOT NULL,
    NumeroDocumento      VARCHAR(20)  NOT NULL,
    FechaNacimiento      DATE         NOT NULL,
    Sexo                 CHAR(1)      NOT NULL,
    CONSTRAINT FK_Beneficiario_DocumentoIdentidad
        FOREIGN KEY (DocumentoIdentidadId) REFERENCES dbo.DocumentoIdentidad(Id),
    CONSTRAINT CK_Beneficiario_Sexo CHECK (Sexo IN ('M','F'))
);
GO

/* =========================
   02) Data ejemplo (requerido)
   ========================= */
INSERT INTO dbo.DocumentoIdentidad (Nombre, Abreviatura, Pais, Longitud, SoloNumeros, Activo)
VALUES
('DNI', 'DNI', 'Perú',   8, 1, 1),
('Carnet Extranjería', 'CE', 'Perú', 9, 1, 1),
('Pasaporte', 'PAS', 'España', 9, 0, 1),
('DNI', 'DNI', 'España', 9, 0, 1),
('Documento Obsoleto', 'OBS', 'N/A', 6, 1, 0); -- inactivo, para probar filtro
GO

/* =========================
   03) Stored Procedures
   ========================= */

-- Documentos activos (para dropdown)
CREATE PROCEDURE dbo.sp_DocumentoIdentidad_ListarActivos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Nombre, Abreviatura, Pais, Longitud, SoloNumeros, Activo
    FROM dbo.DocumentoIdentidad
    WHERE Activo = 1
    ORDER BY Pais, Nombre;
END
GO

-- Validación server-side de NumeroDocumento según DocumentoIdentidad
CREATE PROCEDURE dbo.sp_Beneficiario_ValidarDocumento
    @DocumentoIdentidadId INT,
    @NumeroDocumento VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Longitud INT, @SoloNumeros BIT;

    SELECT @Longitud = Longitud, @SoloNumeros = SoloNumeros
    FROM dbo.DocumentoIdentidad
    WHERE Id = @DocumentoIdentidadId;

    IF @Longitud IS NULL
        THROW 50001, 'DocumentoIdentidadId no existe.', 1;

    IF LEN(@NumeroDocumento) <> @Longitud
        THROW 50002, 'NumeroDocumento no cumple la longitud requerida.', 1;

    IF @SoloNumeros = 1 AND @NumeroDocumento LIKE '%[^0-9]%'
        THROW 50003, 'NumeroDocumento debe contener solo números.', 1;
END
GO

-- CRUD Beneficiario
CREATE PROCEDURE dbo.sp_Beneficiario_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  b.Id,
            b.Nombres,
            b.Apellidos,
            b.DocumentoIdentidadId,
            d.Nombre AS DocumentoNombre,
            d.Abreviatura AS DocumentoAbreviatura,
            d.Pais AS DocumentoPais,
            b.NumeroDocumento,
            b.FechaNacimiento,
            b.Sexo
    FROM dbo.Beneficiario b
    INNER JOIN dbo.DocumentoIdentidad d ON d.Id = b.DocumentoIdentidadId
    ORDER BY b.Id DESC;
END
GO

CREATE PROCEDURE dbo.sp_Beneficiario_ObtenerPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Beneficiario WHERE Id = @Id)
        THROW 50006, 'Beneficiario no existe.', 1;

    SELECT  Id, Nombres, Apellidos, DocumentoIdentidadId, NumeroDocumento, FechaNacimiento, Sexo
    FROM dbo.Beneficiario
    WHERE Id = @Id;
END
GO

CREATE PROCEDURE dbo.sp_Beneficiario_Crear
    @Nombres VARCHAR(100),
    @Apellidos VARCHAR(100),
    @DocumentoIdentidadId INT,
    @NumeroDocumento VARCHAR(20),
    @FechaNacimiento DATE,
    @Sexo CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;

    EXEC dbo.sp_Beneficiario_ValidarDocumento
        @DocumentoIdentidadId = @DocumentoIdentidadId,
        @NumeroDocumento = @NumeroDocumento;

    INSERT INTO dbo.Beneficiario (Nombres, Apellidos, DocumentoIdentidadId, NumeroDocumento, FechaNacimiento, Sexo)
    VALUES (@Nombres, @Apellidos, @DocumentoIdentidadId, @NumeroDocumento, @FechaNacimiento, @Sexo);

    -- Mini mejora: devolver Id como INT (no numeric)
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

CREATE PROCEDURE dbo.sp_Beneficiario_Actualizar
    @Id INT,
    @Nombres VARCHAR(100),
    @Apellidos VARCHAR(100),
    @DocumentoIdentidadId INT,
    @NumeroDocumento VARCHAR(20),
    @FechaNacimiento DATE,
    @Sexo CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Beneficiario WHERE Id = @Id)
        THROW 50004, 'Beneficiario no existe.', 1;

    EXEC dbo.sp_Beneficiario_ValidarDocumento
        @DocumentoIdentidadId = @DocumentoIdentidadId,
        @NumeroDocumento = @NumeroDocumento;

    UPDATE dbo.Beneficiario
    SET Nombres = @Nombres,
        Apellidos = @Apellidos,
        DocumentoIdentidadId = @DocumentoIdentidadId,
        NumeroDocumento = @NumeroDocumento,
        FechaNacimiento = @FechaNacimiento,
        Sexo = @Sexo
    WHERE Id = @Id;

    SELECT @Id AS Id;
END
GO

CREATE PROCEDURE dbo.sp_Beneficiario_Eliminar
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Beneficiario WHERE Id = @Id)
        THROW 50005, 'Beneficiario no existe.', 1;

    DELETE FROM dbo.Beneficiario
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO


/* =========================
   99) Pruebas rápidas (opcional)
   =========================
-- Debe mostrar los activos (sin el inactivo)
EXEC dbo.sp_DocumentoIdentidad_ListarActivos;

-- Debe salir vacío al inicio (no hay beneficiarios)
EXEC dbo.sp_Beneficiario_Listar;
*/
