USE master;
GO

IF DB_ID('SuVac') IS NULL
BEGIN
    CREATE DATABASE SuVac;
END;
GO

USE SuVac;
GO

-- Eliminar tablas (incluye nombres antiguos y nuevos)
DROP TABLE IF EXISTS Pago;
DROP TABLE IF EXISTS ResultadoSubasta;
DROP TABLE IF EXISTS Puja;
DROP TABLE IF EXISTS Subasta;

DROP TABLE IF EXISTS GanadoCategoria;
DROP TABLE IF EXISTS ImagenGanado;
DROP TABLE IF EXISTS Ganado;

DROP TABLE IF EXISTS VacaCategoria;
DROP TABLE IF EXISTS ImagenVaca;
DROP TABLE IF EXISTS Vaca;

DROP TABLE IF EXISTS Usuario;

DROP TABLE IF EXISTS EstadoPago;
DROP TABLE IF EXISTS EstadoSubasta;

DROP TABLE IF EXISTS TipoGanado;
DROP TABLE IF EXISTS EstadoGanado;

DROP TABLE IF EXISTS TipoVaca;
DROP TABLE IF EXISTS EstadoVaca;

DROP TABLE IF EXISTS EstadoUsuario;
DROP TABLE IF EXISTS Rol;
DROP TABLE IF EXISTS Categoria;
DROP TABLE IF EXISTS Raza;
DROP TABLE IF EXISTS Sexo;
GO

-- =========================
-- CATALOGOS DE NORMALIZACION
-- =========================
CREATE TABLE Rol (
    RolId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE EstadoUsuario (
    EstadoUsuarioId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE EstadoGanado (
    EstadoGanadoId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE TipoGanado (
    TipoGanadoId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200)
);

CREATE TABLE EstadoSubasta (
    EstadoSubastaId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE EstadoPago (
    EstadoPagoId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE Raza (
    RazaId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL UNIQUE,
    Descripcion NVARCHAR(250)
);

CREATE TABLE Sexo (
    SexoId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(20) NOT NULL UNIQUE
);

-- =========================
-- USUARIO
-- =========================
CREATE TABLE Usuario (
    UsuarioId INT IDENTITY PRIMARY KEY,
    Correo NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    NombreCompleto NVARCHAR(150) NOT NULL,
    RolId INT NOT NULL,
    EstadoUsuarioId INT NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Usuario_Rol
        FOREIGN KEY (RolId)
        REFERENCES Rol(RolId),

    CONSTRAINT FK_Usuario_Estado
        FOREIGN KEY (EstadoUsuarioId)
        REFERENCES EstadoUsuario(EstadoUsuarioId)
);

-- =========================
-- GANADO
-- =========================
CREATE TABLE Ganado (
    GanadoId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(MAX) NOT NULL,
    TipoGanadoId INT NOT NULL,
    RazaId INT NOT NULL,
    SexoId INT NOT NULL,
    FechaNacimiento DATE NOT NULL,
    PesoKg DECIMAL(8,2) NOT NULL,
    CertificadoSalud NVARCHAR(500),
    EstadoGanadoId INT NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    UsuarioVendedorId INT NOT NULL,

    CONSTRAINT FK_Ganado_Usuario
        FOREIGN KEY (UsuarioVendedorId)
        REFERENCES Usuario(UsuarioId),

    CONSTRAINT FK_Ganado_Tipo
        FOREIGN KEY (TipoGanadoId)
        REFERENCES TipoGanado(TipoGanadoId),

    CONSTRAINT FK_Ganado_Raza
        FOREIGN KEY (RazaId)
        REFERENCES Raza(RazaId),

    CONSTRAINT FK_Ganado_Sexo
        FOREIGN KEY (SexoId)
        REFERENCES Sexo(SexoId),

    CONSTRAINT FK_Ganado_Estado
        FOREIGN KEY (EstadoGanadoId)
        REFERENCES EstadoGanado(EstadoGanadoId)
);

-- =========================
-- IMAGEN_GANADO
-- =========================
CREATE TABLE ImagenGanado (
    ImagenId INT IDENTITY PRIMARY KEY,
    GanadoId INT NOT NULL,
    UrlImagen NVARCHAR(300) NOT NULL,

    CONSTRAINT FK_Imagen_Ganado
        FOREIGN KEY (GanadoId)
        REFERENCES Ganado(GanadoId)
        ON DELETE CASCADE
);

-- =========================
-- CATEGORIA
-- =========================
CREATE TABLE Categoria (
    CategoriaId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(250)
);

-- =========================
-- GANADO_CATEGORIA (M:N)
-- =========================
CREATE TABLE GanadoCategoria (
    GanadoId INT NOT NULL,
    CategoriaId INT NOT NULL,

    CONSTRAINT PK_GanadoCategoria
        PRIMARY KEY (GanadoId, CategoriaId),

    CONSTRAINT FK_OC_Ganado
        FOREIGN KEY (GanadoId)
        REFERENCES Ganado(GanadoId)
        ON DELETE CASCADE,

    CONSTRAINT FK_OC_Categoria
        FOREIGN KEY (CategoriaId)
        REFERENCES Categoria(CategoriaId)
        ON DELETE CASCADE
);

-- =========================
-- SUBASTA
-- =========================
CREATE TABLE Subasta (
    SubastaId INT IDENTITY PRIMARY KEY,
    GanadoId INT NOT NULL,
    FechaInicio DATETIME NOT NULL,
    FechaFin DATETIME NOT NULL,
    PrecioBase DECIMAL(12,2) NOT NULL,
    IncrementoMinimo DECIMAL(12,2) NOT NULL,
    EstadoSubastaId INT NOT NULL,
    UsuarioCreadorId INT NOT NULL,

    CONSTRAINT FK_Subasta_Ganado
        FOREIGN KEY (GanadoId)
        REFERENCES Ganado(GanadoId),

    CONSTRAINT FK_Subasta_Usuario
        FOREIGN KEY (UsuarioCreadorId)
        REFERENCES Usuario(UsuarioId),

    CONSTRAINT FK_Subasta_Estado
        FOREIGN KEY (EstadoSubastaId)
        REFERENCES EstadoSubasta(EstadoSubastaId)
);

-- =========================
-- PUJA
-- =========================
CREATE TABLE Puja (
    PujaId INT IDENTITY PRIMARY KEY,
    SubastaId INT NOT NULL,
    UsuarioId INT NOT NULL,
    Monto DECIMAL(12,2) NOT NULL,
    FechaHora DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Puja_Subasta
        FOREIGN KEY (SubastaId)
        REFERENCES Subasta(SubastaId)
        ON DELETE CASCADE,

    CONSTRAINT FK_Puja_Usuario
        FOREIGN KEY (UsuarioId)
        REFERENCES Usuario(UsuarioId)
);

-- =========================
-- RESULTADO_SUBASTA
-- =========================
CREATE TABLE ResultadoSubasta (
    ResultadoId INT IDENTITY PRIMARY KEY,
    SubastaId INT NOT NULL UNIQUE,
    UsuarioGanadorId INT NOT NULL,
    MontoFinal DECIMAL(12,2) NOT NULL,
    FechaCierre DATETIME NOT NULL,

    CONSTRAINT FK_Resultado_Subasta
        FOREIGN KEY (SubastaId)
        REFERENCES Subasta(SubastaId),

    CONSTRAINT FK_Resultado_Usuario
        FOREIGN KEY (UsuarioGanadorId)
        REFERENCES Usuario(UsuarioId)
);

-- =========================
-- PAGO
-- =========================
CREATE TABLE Pago (
    PagoId INT IDENTITY PRIMARY KEY,
    SubastaId INT NOT NULL UNIQUE,
    UsuarioId INT NOT NULL,
    Monto DECIMAL(12,2) NOT NULL,
    EstadoPagoId INT NOT NULL,
    FechaPago DATETIME NULL,

    CONSTRAINT FK_Pago_Subasta
        FOREIGN KEY (SubastaId)
        REFERENCES Subasta(SubastaId),

    CONSTRAINT FK_Pago_Usuario
        FOREIGN KEY (UsuarioId)
        REFERENCES Usuario(UsuarioId),

    CONSTRAINT FK_Pago_Estado
        FOREIGN KEY (EstadoPagoId)
        REFERENCES EstadoPago(EstadoPagoId)
);
GO

-- Inserts seguros (evita duplicados si corres el script varias veces)
IF NOT EXISTS (SELECT 1 FROM Rol WHERE Nombre = 'Admin')
    INSERT INTO Rol (Nombre) VALUES ('Admin'), ('Vendedor'), ('Comprador');

IF NOT EXISTS (SELECT 1 FROM EstadoUsuario WHERE Nombre = 'Activo')
    INSERT INTO EstadoUsuario (Nombre) VALUES ('Activo'), ('Bloqueado');

IF NOT EXISTS (SELECT 1 FROM EstadoGanado WHERE Nombre = 'Activo')
    INSERT INTO EstadoGanado (Nombre) VALUES ('Activo'), ('Inactivo');

IF NOT EXISTS (SELECT 1 FROM TipoGanado WHERE Nombre = 'Carne')
    INSERT INTO TipoGanado (Nombre, Descripcion) VALUES
    ('Carne','Ganado destinado a produccion de carne'),
    ('Leche','Ganado destinado a produccion de leche'),
    ('Crianza','Ganado para reproduccion y crianza');

IF NOT EXISTS (SELECT 1 FROM EstadoSubasta WHERE Nombre = 'Programada')
    INSERT INTO EstadoSubasta (Nombre) VALUES ('Programada'), ('Activa'), ('Finalizada'), ('Cancelada');

IF NOT EXISTS (SELECT 1 FROM EstadoPago WHERE Nombre = 'Pendiente')
    INSERT INTO EstadoPago (Nombre) VALUES ('Pendiente'), ('Confirmado');

IF NOT EXISTS (SELECT 1 FROM Raza WHERE Nombre = 'Brahman')
    INSERT INTO Raza (Nombre, Descripcion) VALUES
    ('Brahman','Raza bovina originaria de India, adaptada a climas calidos'),
    ('Angus','Raza de carne de origen escoces, conocida por su calidad'),
    ('Holstein','Raza lechera de alto rendimiento, originaria de Holanda'),
    ('Jersey','Raza lechera pequena con leche de alto contenido graso'),
    ('Charolais','Raza francesa de carne, de gran tamano y musculatura'),
    ('Simmental','Raza dual proposito (carne y leche) de origen suizo');

IF NOT EXISTS (SELECT 1 FROM Sexo WHERE Nombre = 'Macho')
    INSERT INTO Sexo (Nombre) VALUES ('Macho'), ('Hembra');

IF NOT EXISTS (SELECT 1 FROM Usuario WHERE Correo = 'admin@subasta.com')
    INSERT INTO Usuario (Correo, PasswordHash, NombreCompleto, RolId, EstadoUsuarioId)
    VALUES
    ('admin@subasta.com','hash1','Administrador General',1,1),
    ('vendedor1@mail.com','hash2','Carlos Vendedor',2,1),
    ('vendedor2@mail.com','hash3','Laura Tech',2,1),
    ('comprador1@mail.com','hash4','Ana Compradora',3,1),
    ('comprador2@mail.com','hash5','Luis Ofertas',3,1);

-- =========================
-- GANADO - DATOS DE PRUEBA
-- =========================
IF NOT EXISTS (SELECT 1 FROM Ganado WHERE Nombre = 'Toro Brahman 001')
    INSERT INTO Ganado (Nombre, Descripcion, TipoGanadoId, RazaId, SexoId, FechaNacimiento, PesoKg, CertificadoSalud, EstadoGanadoId, FechaRegistro, UsuarioVendedorId)
    VALUES
    ('Toro Brahman 001','Toro Brahman de excelente genetica, ideal para produccion de carne. Certificado de salud vigente.',1,1,1,'2020-03-15',750.50,'CERT-2024-001',1,GETDATE(),2),
    ('Vaca Holstien 001','Vaca Holstein de alta produccion lechera. Registrada y con historial de produccion comprobado.',2,3,2,'2019-05-20',680.75,'CERT-2024-002',1,GETDATE(),2),
    ('Toro Angus 001','Toro Angus Negro, raza pura de carne premium. Genealogia completa documentada.',1,2,1,'2021-08-10',620.00,'CERT-2024-003',1,GETDATE(),3),
    ('Vaca Jersey 001','Vaca Jersey de produccion lechera, pequena pero eficiente. Leche de alta calidad con 4.8% grasa.',2,4,2,'2020-11-25',450.25,'CERT-2024-004',1,GETDATE(),2),
    ('Toro Charolais 001','Toro Charolais para reproduccion o engorde. Musculatura excepcional y crecimiento rapido.',1,5,1,'2021-02-14',720.80,'CERT-2024-005',1,GETDATE(),3),
    ('Vaca Simmental 001','Vaca Simmental de proposito dual (carne y leche). Excelentes caracteristicas reproductivas.',3,6,2,'2019-09-30',650.00,'CERT-2024-006',1,GETDATE(),2),
    ('Ternero Brahman 002','Ternero Brahman macho, joven, ideal para crianza y futura reproduccion. Genealogia de campeones.',3,1,1,'2023-06-12',280.50,'CERT-2024-007',1,GETDATE(),3),
    ('Vaca Brahman 002','Hembra Brahman de 6 anos, con registro de sangre pura. Productiva para crianza.',3,1,2,'2018-04-22',700.00,'CERT-2024-008',1,GETDATE(),2);
USE master;
GO

IF DB_ID('SuVac') IS NULL
BEGIN
    CREATE DATABASE SuVac;
END;
GO

USE SuVac;
GO

-- Eliminar tablas (incluye nombres antiguos y nuevos)
DROP TABLE IF EXISTS Pago;
DROP TABLE IF EXISTS ResultadoSubasta;
DROP TABLE IF EXISTS Puja;
DROP TABLE IF EXISTS Subasta;

DROP TABLE IF EXISTS GanadoCategoria;
DROP TABLE IF EXISTS ImagenGanado;
DROP TABLE IF EXISTS Ganado;

DROP TABLE IF EXISTS VacaCategoria;
DROP TABLE IF EXISTS ImagenVaca;
DROP TABLE IF EXISTS Vaca;

DROP TABLE IF EXISTS Usuario;

DROP TABLE IF EXISTS EstadoPago;
DROP TABLE IF EXISTS EstadoSubasta;

DROP TABLE IF EXISTS TipoGanado;
DROP TABLE IF EXISTS EstadoGanado;

DROP TABLE IF EXISTS TipoVaca;
DROP TABLE IF EXISTS EstadoVaca;

DROP TABLE IF EXISTS EstadoUsuario;
DROP TABLE IF EXISTS Rol;
DROP TABLE IF EXISTS Categoria;
DROP TABLE IF EXISTS Raza;
DROP TABLE IF EXISTS Sexo;
GO

-- =========================
-- CATALOGOS DE NORMALIZACION
-- =========================
CREATE TABLE Rol (
    RolId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE EstadoUsuario (
    EstadoUsuarioId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE EstadoGanado (
    EstadoGanadoId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE TipoGanado (
    TipoGanadoId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200)
);

CREATE TABLE EstadoSubasta (
    EstadoSubastaId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE EstadoPago (
    EstadoPagoId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE Raza (
    RazaId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL UNIQUE,
    Descripcion NVARCHAR(250)
);

CREATE TABLE Sexo (
    SexoId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(20) NOT NULL UNIQUE
);

-- =========================
-- USUARIO
-- =========================
CREATE TABLE Usuario (
    UsuarioId INT IDENTITY PRIMARY KEY,
    Correo NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    NombreCompleto NVARCHAR(150) NOT NULL,
    RolId INT NOT NULL,
    EstadoUsuarioId INT NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Usuario_Rol
        FOREIGN KEY (RolId)
        REFERENCES Rol(RolId),

    CONSTRAINT FK_Usuario_Estado
        FOREIGN KEY (EstadoUsuarioId)
        REFERENCES EstadoUsuario(EstadoUsuarioId)
);

-- =========================
-- GANADO
-- =========================
CREATE TABLE Ganado (
    GanadoId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(MAX) NOT NULL,
    TipoGanadoId INT NOT NULL,
    RazaId INT NOT NULL,
    SexoId INT NOT NULL,
    FechaNacimiento DATE NOT NULL,
    PesoKg DECIMAL(8,2) NOT NULL,
    CertificadoSalud NVARCHAR(500),
    EstadoGanadoId INT NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    UsuarioVendedorId INT NOT NULL,

    CONSTRAINT FK_Ganado_Usuario
        FOREIGN KEY (UsuarioVendedorId)
        REFERENCES Usuario(UsuarioId),

    CONSTRAINT FK_Ganado_Tipo
        FOREIGN KEY (TipoGanadoId)
        REFERENCES TipoGanado(TipoGanadoId),

    CONSTRAINT FK_Ganado_Raza
        FOREIGN KEY (RazaId)
        REFERENCES Raza(RazaId),

    CONSTRAINT FK_Ganado_Sexo
        FOREIGN KEY (SexoId)
        REFERENCES Sexo(SexoId),

    CONSTRAINT FK_Ganado_Estado
        FOREIGN KEY (EstadoGanadoId)
        REFERENCES EstadoGanado(EstadoGanadoId)
);

-- =========================
-- IMAGEN_GANADO
-- =========================
CREATE TABLE ImagenGanado (
    ImagenId INT IDENTITY PRIMARY KEY,
    GanadoId INT NOT NULL,
    UrlImagen NVARCHAR(300) NOT NULL,

    CONSTRAINT FK_Imagen_Ganado
        FOREIGN KEY (GanadoId)
        REFERENCES Ganado(GanadoId)
        ON DELETE CASCADE
);

-- =========================
-- CATEGORIA
-- =========================
CREATE TABLE Categoria (
    CategoriaId INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(250)
);

-- =========================
-- GANADO_CATEGORIA (M:N)
-- =========================
CREATE TABLE GanadoCategoria (
    GanadoId INT NOT NULL,
    CategoriaId INT NOT NULL,

    CONSTRAINT PK_GanadoCategoria
        PRIMARY KEY (GanadoId, CategoriaId),

    CONSTRAINT FK_OC_Ganado
        FOREIGN KEY (GanadoId)
        REFERENCES Ganado(GanadoId)
        ON DELETE CASCADE,

    CONSTRAINT FK_OC_Categoria
        FOREIGN KEY (CategoriaId)
        REFERENCES Categoria(CategoriaId)
        ON DELETE CASCADE
);

-- =========================
-- SUBASTA
-- =========================
CREATE TABLE Subasta (
    SubastaId INT IDENTITY PRIMARY KEY,
    GanadoId INT NOT NULL,
    FechaInicio DATETIME NOT NULL,
    FechaFin DATETIME NOT NULL,
    PrecioBase DECIMAL(12,2) NOT NULL,
    IncrementoMinimo DECIMAL(12,2) NOT NULL,
    EstadoSubastaId INT NOT NULL,
    UsuarioCreadorId INT NOT NULL,

    CONSTRAINT FK_Subasta_Ganado
        FOREIGN KEY (GanadoId)
        REFERENCES Ganado(GanadoId),

    CONSTRAINT FK_Subasta_Usuario
        FOREIGN KEY (UsuarioCreadorId)
        REFERENCES Usuario(UsuarioId),

    CONSTRAINT FK_Subasta_Estado
        FOREIGN KEY (EstadoSubastaId)
        REFERENCES EstadoSubasta(EstadoSubastaId)
);

-- =========================
-- PUJA
-- =========================
CREATE TABLE Puja (
    PujaId INT IDENTITY PRIMARY KEY,
    SubastaId INT NOT NULL,
    UsuarioId INT NOT NULL,
    Monto DECIMAL(12,2) NOT NULL,
    FechaHora DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Puja_Subasta
        FOREIGN KEY (SubastaId)
        REFERENCES Subasta(SubastaId)
        ON DELETE CASCADE,

    CONSTRAINT FK_Puja_Usuario
        FOREIGN KEY (UsuarioId)
        REFERENCES Usuario(UsuarioId)
);

-- =========================
-- RESULTADO_SUBASTA
-- =========================
CREATE TABLE ResultadoSubasta (
    ResultadoId INT IDENTITY PRIMARY KEY,
    SubastaId INT NOT NULL UNIQUE,
    UsuarioGanadorId INT NOT NULL,
    MontoFinal DECIMAL(12,2) NOT NULL,
    FechaCierre DATETIME NOT NULL,

    CONSTRAINT FK_Resultado_Subasta
        FOREIGN KEY (SubastaId)
        REFERENCES Subasta(SubastaId),

    CONSTRAINT FK_Resultado_Usuario
        FOREIGN KEY (UsuarioGanadorId)
        REFERENCES Usuario(UsuarioId)
);

-- =========================
-- PAGO
-- =========================
CREATE TABLE Pago (
    PagoId INT IDENTITY PRIMARY KEY,
    SubastaId INT NOT NULL UNIQUE,
    UsuarioId INT NOT NULL,
    Monto DECIMAL(12,2) NOT NULL,
    EstadoPagoId INT NOT NULL,
    FechaPago DATETIME NULL,

    CONSTRAINT FK_Pago_Subasta
        FOREIGN KEY (SubastaId)
        REFERENCES Subasta(SubastaId),

    CONSTRAINT FK_Pago_Usuario
        FOREIGN KEY (UsuarioId)
        REFERENCES Usuario(UsuarioId),

    CONSTRAINT FK_Pago_Estado
        FOREIGN KEY (EstadoPagoId)
        REFERENCES EstadoPago(EstadoPagoId)
);
GO

-- Inserts seguros (evita duplicados si corres el script varias veces)
IF NOT EXISTS (SELECT 1 FROM Rol WHERE Nombre = 'Admin')
    INSERT INTO Rol (Nombre) VALUES ('Admin'), ('Vendedor'), ('Comprador');

IF NOT EXISTS (SELECT 1 FROM EstadoUsuario WHERE Nombre = 'Activo')
    INSERT INTO EstadoUsuario (Nombre) VALUES ('Activo'), ('Bloqueado');

IF NOT EXISTS (SELECT 1 FROM EstadoGanado WHERE Nombre = 'Activo')
    INSERT INTO EstadoGanado (Nombre) VALUES ('Activo'), ('Inactivo');

IF NOT EXISTS (SELECT 1 FROM TipoGanado WHERE Nombre = 'Carne')
    INSERT INTO TipoGanado (Nombre, Descripcion) VALUES
    ('Carne','Ganado destinado a produccion de carne'),
    ('Leche','Ganado destinado a produccion de leche'),
    ('Crianza','Ganado para reproduccion y crianza');

IF NOT EXISTS (SELECT 1 FROM EstadoSubasta WHERE Nombre = 'Programada')
    INSERT INTO EstadoSubasta (Nombre) VALUES ('Programada'), ('Activa'), ('Finalizada'), ('Cancelada');

IF NOT EXISTS (SELECT 1 FROM EstadoPago WHERE Nombre = 'Pendiente')
    INSERT INTO EstadoPago (Nombre) VALUES ('Pendiente'), ('Confirmado');

IF NOT EXISTS (SELECT 1 FROM Raza WHERE Nombre = 'Brahman')
    INSERT INTO Raza (Nombre, Descripcion) VALUES
    ('Brahman','Raza bovina originaria de India, adaptada a climas calidos'),
    ('Angus','Raza de carne de origen escoces, conocida por su calidad'),
    ('Holstein','Raza lechera de alto rendimiento, originaria de Holanda'),
    ('Jersey','Raza lechera pequena con leche de alto contenido graso'),
    ('Charolais','Raza francesa de carne, de gran tamano y musculatura'),
    ('Simmental','Raza dual proposito (carne y leche) de origen suizo');

IF NOT EXISTS (SELECT 1 FROM Sexo WHERE Nombre = 'Macho')
    INSERT INTO Sexo (Nombre) VALUES ('Macho'), ('Hembra');

IF NOT EXISTS (SELECT 1 FROM Usuario WHERE Correo = 'admin@subasta.com')
    INSERT INTO Usuario (Correo, PasswordHash, NombreCompleto, RolId, EstadoUsuarioId)
    VALUES
    ('admin@subasta.com','hash1','Administrador General',1,1),
    ('vendedor1@mail.com','hash2','Carlos Vendedor',2,1),
    ('vendedor2@mail.com','hash3','Laura Tech',2,1),
    ('comprador1@mail.com','hash4','Ana Compradora',3,1),
    ('comprador2@mail.com','hash5','Luis Ofertas',3,1);