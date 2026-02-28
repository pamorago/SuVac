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

-- =========================
-- CATEGORIAS
-- =========================
IF NOT EXISTS (SELECT 1 FROM Categoria WHERE Nombre = 'Premium')
    INSERT INTO Categoria (Nombre, Descripcion) VALUES
    ('Premium',     'Ganado de alta genetica y calidad superior'),
    ('Lechero',     'Ganado especializado en produccion de leche'),
    ('Reproductor', 'Ganado destinado a la reproduccion y mejora genetica'),
    ('Engorde',     'Ganado orientado al engorde y produccion de carne');

-- =========================
-- IMAGENES DE GANADO
-- =========================
IF NOT EXISTS (SELECT 1 FROM ImagenGanado WHERE GanadoId = 1)
    INSERT INTO ImagenGanado (GanadoId, UrlImagen) VALUES
    (1, '/img/Brahman.jpg'),
    (2, '/img/Holstein.jpg'),
    (3, '/img/Angus.jpg'),
    (4, '/img/Gyr.jpg'),
    (5, '/img/Charolais.jpeg'),
    (6, '/img/Simmental.jpg'),
    (7, '/img/Brahman.jpg'),
    (8, '/img/Brangus.jpg');

-- =========================
-- GANADO - CATEGORIAS
-- =========================
IF NOT EXISTS (SELECT 1 FROM GanadoCategoria WHERE GanadoId = 1)
    INSERT INTO GanadoCategoria (GanadoId, CategoriaId) VALUES
    (1, 1), (1, 4),   -- Brahman: Premium, Engorde
    (2, 2),           -- Holstein: Lechero
    (3, 1), (3, 4),   -- Angus: Premium, Engorde
    (4, 2),           -- Jersey: Lechero
    (5, 1), (5, 4),   -- Charolais: Premium, Engorde
    (6, 2), (6, 3),   -- Simmental: Lechero, Reproductor
    (7, 3),           -- Ternero Brahman: Reproductor
    (8, 3);           -- Vaca Brahman: Reproductor

-- =========================
-- SUBASTAS
-- Plan:
--   Activas  (EstadoSubastaId=2): ganados 1, 2, 3  +  7 (reactivado)
--   Finalizadas (=3): ganados 4, 5, 6  +  8 (reactivado y cerrado)
--   Canceladas  (=4): ganados 7 y 8 (primera vez, antes de reactivar)
-- =========================
IF NOT EXISTS (SELECT 1 FROM Subasta WHERE GanadoId = 1)
BEGIN
    -- ── SUBASTAS ACTIVAS NORMALES ───────────────────────────────────────────
    INSERT INTO Subasta (GanadoId, FechaInicio, FechaFin, PrecioBase, IncrementoMinimo, EstadoSubastaId, UsuarioCreadorId)
    VALUES
    -- Activa 1: Toro Brahman 001
    (1, '2026-02-10 08:00', '2026-03-10 18:00', 450000.00, 15000.00, 2, 2),
    -- Activa 2: Vaca Holstein 001
    (2, '2026-02-20 09:00', '2026-03-07 18:00', 380000.00, 10000.00, 2, 2),
    -- Activa 3: Toro Angus 001
    (3, '2026-02-25 08:00', '2026-03-15 18:00', 520000.00, 20000.00, 2, 3),

    -- ── CANCELADA (primera vez) ─────────────────────────────────────────────
    -- Cancelada 1: Ternero Brahman 002 - no llego al minimo, se cancelo
    (7, '2026-01-05 08:00', '2026-01-25 18:00', 250000.00,  8000.00, 4, 2),

    -- ── REACTIVADA como ACTIVA tras cancelacion ─────────────────────────────
    -- Activa 4: Ternero Brahman 002 - segunda oportunidad (precio ajustado)
    (7, '2026-02-26 08:00', '2026-03-12 18:00', 265000.00,  8000.00, 2, 2),

    -- ── CANCELADA (primera vez) ─────────────────────────────────────────────
    -- Cancelada 2: Vaca Brahman 002 - problema tecnico, se cancelo
    (8, '2026-01-10 09:00', '2026-01-28 18:00', 400000.00, 12000.00, 4, 3),

    -- ── REACTIVADA y ya FINALIZADA ───────────────────────────────────────────
    -- Finalizada 1: Vaca Brahman 002 - se relisto y cerro con vendedor
    (8, '2026-02-05 09:00', '2026-02-22 18:00', 415000.00, 12000.00, 3, 3),

    -- ── SUBASTAS FINALIZADAS NORMALES ───────────────────────────────────────
    -- Finalizada 2: Vaca Jersey 001
    (4, '2026-01-08 08:00', '2026-02-05 18:00', 300000.00, 10000.00, 3, 2),
    -- Finalizada 3: Toro Charolais 001
    (5, '2026-01-12 08:00', '2026-02-15 18:00', 480000.00, 15000.00, 3, 3),
    -- Finalizada 4: Vaca Simmental 001
    (6, '2026-02-01 09:00', '2026-02-20 18:00', 350000.00, 12000.00, 3, 2);
END

-- =========================
-- PUJAS
-- SubastaId asignado por orden de insercion arriba:
--   1=Brahman Activa, 2=Holstein Activa, 3=Angus Activa,
--   4=Ternero Cancelada, 5=Ternero Reactivada,
--   6=VacaBrahman Cancelada, 7=VacaBrahman Finalizada,
--   8=Jersey Finalizada, 9=Charolais Finalizada, 10=Simmental Finalizada
-- =========================
IF NOT EXISTS (SELECT 1 FROM Puja WHERE SubastaId = 1)
BEGIN
    -- Subasta 1 - Toro Brahman 001 (Activa) - 4 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (1, 4, 450000.00, '2026-02-11 10:15'),
    (1, 5, 465000.00, '2026-02-13 14:30'),
    (1, 4, 480000.00, '2026-02-18 09:05'),
    (1, 5, 495000.00, '2026-02-22 16:45');

    -- Subasta 2 - Vaca Holstein 001 (Activa) - 3 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (2, 5, 380000.00, '2026-02-21 11:00'),
    (2, 4, 390000.00, '2026-02-23 08:30'),
    (2, 5, 400000.00, '2026-02-26 17:10');

    -- Subasta 3 - Toro Angus 001 (Activa) - 5 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (3, 4, 520000.00, '2026-02-25 09:00'),
    (3, 5, 540000.00, '2026-02-25 11:30'),
    (3, 4, 560000.00, '2026-02-26 08:15'),
    (3, 5, 580000.00, '2026-02-26 13:00'),
    (3, 4, 600000.00, '2026-02-27 09:40');

    -- Subasta 4 - Ternero Brahman 002 (CANCELADA) - 2 pujas (no llego al minimo)
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (4, 4, 250000.00, '2026-01-07 10:00'),
    (4, 5, 258000.00, '2026-01-10 15:20');

    -- Subasta 5 - Ternero Brahman 002 (Reactivada - Activa) - 3 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (5, 5, 265000.00, '2026-02-26 10:00'),
    (5, 4, 273000.00, '2026-02-26 14:30'),
    (5, 5, 281000.00, '2026-02-27 08:00');

    -- Subasta 6 - Vaca Brahman 002 (CANCELADA) - 1 puja
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (6, 4, 400000.00, '2026-01-12 12:00');

    -- Subasta 7 - Vaca Brahman 002 (Reactivada - Finalizada) - 6 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (7, 4, 415000.00, '2026-02-06 09:00'),
    (7, 5, 427000.00, '2026-02-08 11:15'),
    (7, 4, 439000.00, '2026-02-11 14:00'),
    (7, 5, 451000.00, '2026-02-14 09:30'),
    (7, 4, 463000.00, '2026-02-18 16:00'),
    (7, 5, 475000.00, '2026-02-21 10:45');

    -- Subasta 8 - Vaca Jersey 001 (Finalizada) - 4 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (8, 5, 300000.00, '2026-01-10 08:30'),
    (8, 4, 310000.00, '2026-01-15 12:00'),
    (8, 5, 320000.00, '2026-01-22 15:45'),
    (8, 4, 330000.00, '2026-02-03 10:20');

    -- Subasta 9 - Toro Charolais 001 (Finalizada) - 5 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (9, 4, 480000.00, '2026-01-14 09:00'),
    (9, 5, 495000.00, '2026-01-18 14:00'),
    (9, 4, 510000.00, '2026-01-24 11:30'),
    (9, 5, 525000.00, '2026-01-30 08:45'),
    (9, 4, 540000.00, '2026-02-12 17:00');

    -- Subasta 10 - Vaca Simmental 001 (Finalizada) - 3 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (10, 5, 350000.00, '2026-02-03 10:00'),
    (10, 4, 362000.00, '2026-02-10 13:30'),
    (10, 5, 374000.00, '2026-02-17 09:15');
END
