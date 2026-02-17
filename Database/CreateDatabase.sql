-- Crear base de datos
CREATE DATABASE SistemaGestionDespacho;
GO

USE SistemaGestionDespacho;
GO

/* ============================================================
   TABLA: Clientes
   ============================================================ */
CREATE TABLE Clientes (
    ClienteId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(150) NULL,
    DNI_CIF NVARCHAR(20) NOT NULL UNIQUE,
    Telefono NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    Direccion NVARCHAR(200) NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT(GETDATE()),
    Activo BIT NOT NULL DEFAULT(1),

    -- Al menos uno de los dos medios de contacto debe existir
    CONSTRAINT CK_Clientes_EmailOTelefono
        CHECK (Email IS NOT NULL OR Telefono IS NOT NULL)
);
GO


/* ============================================================
   TABLA: EstadosExpediente
   ============================================================ */
CREATE TABLE EstadosExpediente (
    EstadoId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200) NULL
);
GO


/* ============================================================
   TABLA: Expedientes
   ============================================================ */
CREATE TABLE Expedientes (
    ExpedienteId INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(50) NOT NULL UNIQUE,
    ClienteId INT NOT NULL,
    EstadoId INT NOT NULL,
    Tipo NVARCHAR(50) NOT NULL,
    FechaApertura DATETIME NOT NULL DEFAULT(GETDATE()),
    FechaCierre DATETIME NULL,
    Titulo NVARCHAR(200) NOT NULL,
    Descripcion NVARCHAR(MAX) NOT NULL,

    CONSTRAINT FK_Expedientes_Clientes
        FOREIGN KEY (ClienteId) REFERENCES Clientes(ClienteId),

    CONSTRAINT FK_Expedientes_Estados
        FOREIGN KEY (EstadoId) REFERENCES EstadosExpediente(EstadoId)
);
GO


/* ============================================================
   TABLA: Actuaciones
   ============================================================ */
CREATE TABLE Actuaciones (
    ActuacionId INT IDENTITY(1,1) PRIMARY KEY,
    ExpedienteId INT NOT NULL,
    Fecha DATETIME NOT NULL DEFAULT(GETDATE()),
    Tipo NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(MAX) NOT NULL,

    CONSTRAINT FK_Actuaciones_Expedientes
        FOREIGN KEY (ExpedienteId) REFERENCES Expedientes(ExpedienteId)
);
GO


/* ============================================================
   TABLA: Citas
   ============================================================ */
CREATE TABLE Citas (
    CitaId INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId INT NULL,
    ExpedienteId INT NULL,
    FechaHora DATETIME NOT NULL,
    Lugar NVARCHAR(100) NOT NULL,
    Motivo NVARCHAR(200) NOT NULL,
    Estado NVARCHAR(30) NOT NULL,

    CONSTRAINT FK_Citas_Clientes
        FOREIGN KEY (ClienteId) REFERENCES Clientes(ClienteId),

    CONSTRAINT FK_Citas_Expedientes
        FOREIGN KEY (ExpedienteId) REFERENCES Expedientes(ExpedienteId)
);
GO

-- Restricción: al menos ClienteId o ExpedienteId debe existir
ALTER TABLE Citas
ADD CONSTRAINT CK_Citas_ClienteOExpediente
CHECK (ClienteId IS NOT NULL OR ExpedienteId IS NOT NULL);
GO


/* ============================================================
   DATOS INICIALES (Opcional)
   ============================================================ */
INSERT INTO EstadosExpediente (Nombre, Descripcion)
VALUES ('Abierto', 'Expediente recién creado'),
       ('En curso', 'Expediente en tramitación'),
       ('Archivado', 'Expediente archivado temporalmente'),
       ('Cerrado', 'Expediente finalizado');
GO