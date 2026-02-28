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
    ('Simmental','Raza dual proposito (carne y leche) de origen suizo'),
    ('Hereford','Raza britanica de carne, rusticu y adaptable'),
    ('Brangus','Cruce de Brahman y Angus, resistente al calor con carne de calidad'),
    ('Gyr','Raza cebuina de origen indio, buena produccion lechera en tropico'),
    ('Nelore','Raza cebuina de Brasil, excelente para carne en tropico');

IF NOT EXISTS (SELECT 1 FROM Sexo WHERE Nombre = 'Macho')
    INSERT INTO Sexo (Nombre) VALUES ('Macho'), ('Hembra');

IF NOT EXISTS (SELECT 1 FROM Usuario WHERE Correo = 'admin@subasta.com')
    INSERT INTO Usuario (Correo, PasswordHash, NombreCompleto, RolId, EstadoUsuarioId)
    VALUES
    ('admin@subasta.com',       'hash1', 'Administrador General',    1, 1),
    ('vendedor1@mail.com',      'hash2', 'Carlos Vendedor',          2, 1),
    ('vendedor2@mail.com',      'hash3', 'Laura Tech',               2, 1),
    ('vendedor3@mail.com',      'hash6', 'Roberto Ganadero',         2, 1),
    ('comprador1@mail.com',     'hash4', 'Ana Compradora',           3, 1),
    ('comprador2@mail.com',     'hash5', 'Luis Ofertas',             3, 1),
    ('comprador3@mail.com',     'hash7', 'Maria Subastas',           3, 1),
    ('comprador4@mail.com',     'hash8', 'Pedro Inversor',           3, 2),  -- Bloqueado
    ('vendedor4@mail.com',      'hash9', 'Sofia Rancho',             2, 2);  -- Bloqueado

-- =========================
-- GANADO - DATOS DE PRUEBA (15 registros)
-- Vendedores: 2=Carlos, 3=Laura, 4=Roberto, 9=Sofia(bloq.)
-- =========================
IF NOT EXISTS (SELECT 1 FROM Ganado WHERE Nombre = 'Toro Brahman 001')
    INSERT INTO Ganado (Nombre, Descripcion, TipoGanadoId, RazaId, SexoId, FechaNacimiento, PesoKg, CertificadoSalud, EstadoGanadoId, FechaRegistro, UsuarioVendedorId)
    VALUES
    ('Toro Brahman 001',    'Toro Brahman de excelente genetica, ideal para produccion de carne. Certificado de salud vigente.',                 1,1,1,'2020-03-15',750.50,'CERT-2024-001',1,GETDATE(),2),
    ('Vaca Holstein 001',   'Vaca Holstein de alta produccion lechera. Registrada y con historial de produccion comprobado.',                    2,3,2,'2019-05-20',680.75,'CERT-2024-002',1,GETDATE(),2),
    ('Toro Angus 001',      'Toro Angus Negro, raza pura de carne premium. Genealogia completa documentada.',                                   1,2,1,'2021-08-10',620.00,'CERT-2024-003',1,GETDATE(),3),
    ('Vaca Jersey 001',     'Vaca Jersey de produccion lechera, pequena pero eficiente. Leche de alta calidad con 4.8% grasa.',                  2,4,2,'2020-11-25',450.25,'CERT-2024-004',1,GETDATE(),2),
    ('Toro Charolais 001',  'Toro Charolais para reproduccion o engorde. Musculatura excepcional y crecimiento rapido.',                         1,5,1,'2021-02-14',720.80,'CERT-2024-005',1,GETDATE(),3),
    ('Vaca Simmental 001',  'Vaca Simmental de proposito dual (carne y leche). Excelentes caracteristicas reproductivas.',                       3,6,2,'2019-09-30',650.00,'CERT-2024-006',1,GETDATE(),2),
    ('Ternero Brahman 002', 'Ternero Brahman macho, joven, ideal para crianza y futura reproduccion. Genealogia de campeones.',                  3,1,1,'2023-06-12',280.50,'CERT-2024-007',1,GETDATE(),3),
    ('Vaca Brahman 002',    'Hembra Brahman de 6 anos, con registro de sangre pura. Productiva para crianza.',                                   3,1,2,'2018-04-22',700.00,'CERT-2024-008',1,GETDATE(),2),
    ('Toro Hereford 001',   'Toro Hereford de cara blanca, rustico y de facil manejo. Ideal para cruces comerciales en zonas templadas.',         1,7,1,'2022-01-18',690.30,'CERT-2024-009',1,GETDATE(),4),
    ('Vaca Brangus 001',    'Vaca Brangus F1 de primera generacion. Combina resistencia cebuina con calidad carnica angus.',                     1,8,2,'2021-07-05',580.60, NULL,           1,GETDATE(),4),
    ('Toro Gyr 001',        'Toro Gyr lechero de alta genetica brasilena. Excelentes indices de fertilidad en condiciones tropicales.',           2,9,1,'2020-10-30',550.00,'CERT-2024-010',1,GETDATE(),3),
    ('Vaca Nelore 001',     'Vaca Nelore de registro, temperamento docil. Gran eficiencia alimenticia para engorde a pastoreo.',                  1,10,2,'2022-03-20',520.40, NULL,          1,GETDATE(),4),
    ('Ternero Angus 002',   'Ternero Angus Rojo, 10 meses de edad. Destetado y con plan sanitario completo. Potencial reproductor.',             3,2,1,'2025-04-11',310.00,'CERT-2025-001',1,GETDATE(),2),
    ('Vaca Holstein 002',   'Vaca Holstein multimpara con 3 partos. Produccion de 28 litros diarios promedio. Libre de mastitis.',                2,3,2,'2018-12-08',710.90,'CERT-2025-002',1,GETDATE(),3),
    ('Toro Brahman 003',    'Toro Brahman joven, listo para servicio. Pruebas de progenie sobresalientes y aplomos correctos.',                   3,1,1,'2023-09-02',420.75,'CERT-2025-003',1,GETDATE(),2);

-- =========================
-- CATEGORIAS
-- =========================
IF NOT EXISTS (SELECT 1 FROM Categoria WHERE Nombre = 'Premium')
    INSERT INTO Categoria (Nombre, Descripcion) VALUES
    ('Premium',     'Ganado de alta genetica y calidad superior'),
    ('Lechero',     'Ganado especializado en produccion de leche'),
    ('Reproductor', 'Ganado destinado a la reproduccion y mejora genetica'),
    ('Engorde',     'Ganado orientado al engorde y produccion de carne'),
    ('Criollo',     'Ganado de razas adaptadas localmente, alta rusticidad'),
    ('Joven',       'Ganado menor de 2 anos, en etapa de desarrollo');

-- =========================
-- IMAGENES DE GANADO (varias imagenes por ganado para probar carrusel)
-- =========================
DELETE FROM ImagenGanado;
INSERT INTO ImagenGanado (GanadoId, UrlImagen) VALUES
    (1,  '/img/Brahman.jpg'),
    (1,  '/img/Nelore.jpeg'),
    (2,  '/img/Holstein.jpg'),
    (2,  '/img/Gelbvieh.jpg'),
    (3,  '/img/Angus.jpg'),
    (3,  '/img/Limousin.jpeg'),
    (4,  '/img/Gyr.jpg'),
    (5,  '/img/Charolais.jpeg'),
    (5,  '/img/Chianina.jpg'),
    (6,  '/img/Simmental.jpg'),
    (7,  '/img/Brahman.jpg'),
    (8,  '/img/Brangus.jpg'),
    (8,  '/img/Brahman.jpg'),
    (9,  '/img/Hereford.jpg'),
    (9,  '/img/Beefmaster.jpg'),
    (10, '/img/Brangus.jpg'),
    (11, '/img/Gyr.jpg'),
    (11, '/img/Guzerat.png'),
    (12, '/img/Nelore.jpeg'),
    (13, '/img/Angus.jpg'),
    (14, '/img/Holstein.jpg'),
    (14, '/img/Simmental.jpg'),
    (15, '/img/Brahman.jpg'),
    (15, '/img/Droughmaster.jpg');

-- =========================
-- GANADO - CATEGORIAS
-- =========================
IF NOT EXISTS (SELECT 1 FROM GanadoCategoria WHERE GanadoId = 1)
    INSERT INTO GanadoCategoria (GanadoId, CategoriaId) VALUES
    (1, 1), (1, 4),          -- Brahman 001: Premium, Engorde
    (2, 2),                  -- Holstein 001: Lechero
    (3, 1), (3, 4),          -- Angus 001: Premium, Engorde
    (4, 2),                  -- Jersey 001: Lechero
    (5, 1), (5, 4),          -- Charolais 001: Premium, Engorde
    (6, 2), (6, 3),          -- Simmental 001: Lechero, Reproductor
    (7, 3), (7, 6),          -- Ternero Brahman 002: Reproductor, Joven
    (8, 3),                  -- Vaca Brahman 002: Reproductor
    (9, 1), (9, 4), (9, 5),  -- Hereford 001: Premium, Engorde, Criollo
    (10, 4), (10, 5),        -- Brangus 001: Engorde, Criollo
    (11, 2), (11, 3),        -- Gyr 001: Lechero, Reproductor
    (12, 4), (12, 5),        -- Nelore 001: Engorde, Criollo
    (13, 3), (13, 6),        -- Ternero Angus 002: Reproductor, Joven
    (14, 1), (14, 2),        -- Holstein 002: Premium, Lechero
    (15, 3), (15, 6);        -- Brahman 003: Reproductor, Joven

-- =========================
-- SUBASTAS (14 subastas)
-- Vendedores: 2=Carlos, 3=Laura, 4=Roberto
-- Compradores: 5=Ana, 6=Luis, 7=Maria, 8=Pedro(bloq.)
--
--   ACTIVAS (EstadoSubastaId=2):  ganados 1,2,3,9,11,13  + 7 reactivado
--   FINALIZADAS (=3):             ganados 4,5,6,10,14     + 8 reactivado
--   CANCELADAS (=4):              ganados 7,8 (primera vez)
-- =========================
IF NOT EXISTS (SELECT 1 FROM Subasta WHERE GanadoId = 1)
BEGIN
    INSERT INTO Subasta (GanadoId, FechaInicio, FechaFin, PrecioBase, IncrementoMinimo, EstadoSubastaId, UsuarioCreadorId)
    VALUES
    -- ── ACTIVAS ─────────────────────────────────────────────────────────────
    -- Sub 1: Toro Brahman 001
    (1,  '2026-02-10 08:00', '2026-03-10 18:00', 450000.00, 15000.00, 2, 2),
    -- Sub 2: Vaca Holstein 001
    (2,  '2026-02-20 09:00', '2026-03-07 18:00', 380000.00, 10000.00, 2, 2),
    -- Sub 3: Toro Angus 001
    (3,  '2026-02-25 08:00', '2026-03-15 18:00', 520000.00, 20000.00, 2, 3),
    -- Sub 4: CANCELADA - Ternero Brahman 002
    (7,  '2026-01-05 08:00', '2026-01-25 18:00', 250000.00,  8000.00, 4, 2),
    -- Sub 5: REACTIVADA Activa - Ternero Brahman 002
    (7,  '2026-02-26 08:00', '2026-03-12 18:00', 265000.00,  8000.00, 2, 2),
    -- Sub 6: CANCELADA - Vaca Brahman 002
    (8,  '2026-01-10 09:00', '2026-01-28 18:00', 400000.00, 12000.00, 4, 3),
    -- Sub 7: REACTIVADA Finalizada - Vaca Brahman 002
    (8,  '2026-02-05 09:00', '2026-02-22 18:00', 415000.00, 12000.00, 3, 3),
    -- Sub 8: FINALIZADA - Vaca Jersey 001
    (4,  '2026-01-08 08:00', '2026-02-05 18:00', 300000.00, 10000.00, 3, 2),
    -- Sub 9: FINALIZADA - Toro Charolais 001
    (5,  '2026-01-12 08:00', '2026-02-15 18:00', 480000.00, 15000.00, 3, 3),
    -- Sub 10: FINALIZADA - Vaca Simmental 001
    (6,  '2026-02-01 09:00', '2026-02-20 18:00', 350000.00, 12000.00, 3, 2),
    -- Sub 11: ACTIVA - Toro Hereford 001
    (9,  '2026-02-15 10:00', '2026-03-20 18:00', 500000.00, 18000.00, 2, 4),
    -- Sub 12: ACTIVA - Toro Gyr 001
    (11, '2026-02-22 08:00', '2026-03-18 18:00', 320000.00, 10000.00, 2, 3),
    -- Sub 13: FINALIZADA - Vaca Brangus 001
    (10, '2026-01-20 09:00', '2026-02-18 18:00', 280000.00,  8000.00, 3, 4),
    -- Sub 14: ACTIVA - Ternero Angus 002
    (13, '2026-02-27 08:00', '2026-03-25 18:00', 200000.00,  5000.00, 2, 2);
END

-- =========================
-- PUJAS
-- SubastaId por orden de insercion:
--   1=Brahman Act, 2=Holstein Act, 3=Angus Act,
--   4=TerneroBrahman Canc, 5=TerneroBrahman React-Act,
--   6=VacaBrahman Canc, 7=VacaBrahman React-Fin,
--   8=Jersey Fin, 9=Charolais Fin, 10=Simmental Fin,
--   11=Hereford Act, 12=Gyr Act, 13=Brangus Fin, 14=TerneroAngus Act
--
-- Compradores: 5=Ana, 6=Luis, 7=Maria
-- =========================
IF NOT EXISTS (SELECT 1 FROM Puja WHERE SubastaId = 1)
BEGIN
    -- Subasta 1 - Toro Brahman 001 (Activa) — 6 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (1, 5, 450000.00, '2026-02-11 10:15'),
    (1, 6, 465000.00, '2026-02-13 14:30'),
    (1, 5, 480000.00, '2026-02-18 09:05'),
    (1, 7, 495000.00, '2026-02-20 11:00'),
    (1, 6, 510000.00, '2026-02-24 16:45'),
    (1, 5, 525000.00, '2026-02-27 08:20');

    -- Subasta 2 - Vaca Holstein 001 (Activa) — 4 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (2, 6, 380000.00, '2026-02-21 11:00'),
    (2, 5, 390000.00, '2026-02-23 08:30'),
    (2, 7, 400000.00, '2026-02-25 14:10'),
    (2, 6, 410000.00, '2026-02-27 17:10');

    -- Subasta 3 - Toro Angus 001 (Activa) — 5 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (3, 5, 520000.00, '2026-02-25 09:00'),
    (3, 6, 540000.00, '2026-02-25 11:30'),
    (3, 7, 560000.00, '2026-02-26 08:15'),
    (3, 5, 580000.00, '2026-02-26 13:00'),
    (3, 6, 600000.00, '2026-02-27 09:40');

    -- Subasta 4 - Ternero Brahman 002 (CANCELADA) — 2 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (4, 5, 250000.00, '2026-01-07 10:00'),
    (4, 6, 258000.00, '2026-01-10 15:20');

    -- Subasta 5 - Ternero Brahman 002 (Reactivada-Activa) — 3 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (5, 6, 265000.00, '2026-02-26 10:00'),
    (5, 5, 273000.00, '2026-02-26 14:30'),
    (5, 7, 281000.00, '2026-02-27 08:00');

    -- Subasta 6 - Vaca Brahman 002 (CANCELADA) — 1 puja
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (6, 5, 400000.00, '2026-01-12 12:00');

    -- Subasta 7 - Vaca Brahman 002 (Reactivada-Finalizada) — 6 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (7, 5, 415000.00, '2026-02-06 09:00'),
    (7, 6, 427000.00, '2026-02-08 11:15'),
    (7, 5, 439000.00, '2026-02-11 14:00'),
    (7, 7, 451000.00, '2026-02-14 09:30'),
    (7, 5, 463000.00, '2026-02-18 16:00'),
    (7, 6, 475000.00, '2026-02-21 10:45');

    -- Subasta 8 - Vaca Jersey 001 (Finalizada) — 4 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (8, 6, 300000.00, '2026-01-10 08:30'),
    (8, 5, 310000.00, '2026-01-15 12:00'),
    (8, 7, 320000.00, '2026-01-22 15:45'),
    (8, 5, 330000.00, '2026-02-03 10:20');

    -- Subasta 9 - Toro Charolais 001 (Finalizada) — 7 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (9, 5, 480000.00, '2026-01-14 09:00'),
    (9, 6, 495000.00, '2026-01-18 14:00'),
    (9, 7, 510000.00, '2026-01-22 10:00'),
    (9, 5, 525000.00, '2026-01-24 11:30'),
    (9, 6, 540000.00, '2026-01-28 08:45'),
    (9, 7, 555000.00, '2026-02-05 14:20'),
    (9, 5, 570000.00, '2026-02-12 17:00');

    -- Subasta 10 - Vaca Simmental 001 (Finalizada) — 5 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (10, 6, 350000.00, '2026-02-03 10:00'),
    (10, 5, 362000.00, '2026-02-06 13:30'),
    (10, 7, 374000.00, '2026-02-10 09:15'),
    (10, 6, 386000.00, '2026-02-14 11:00'),
    (10, 5, 398000.00, '2026-02-17 16:30');

    -- Subasta 11 - Toro Hereford 001 (Activa) — 4 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (11, 7, 500000.00, '2026-02-16 09:30'),
    (11, 5, 518000.00, '2026-02-19 14:15'),
    (11, 6, 536000.00, '2026-02-23 10:00'),
    (11, 7, 554000.00, '2026-02-27 11:45');

    -- Subasta 12 - Toro Gyr 001 (Activa) — 3 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (12, 5, 320000.00, '2026-02-23 09:00'),
    (12, 7, 330000.00, '2026-02-25 15:00'),
    (12, 6, 340000.00, '2026-02-27 10:30');

    -- Subasta 13 - Vaca Brangus 001 (Finalizada) — 5 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (13, 6, 280000.00, '2026-01-22 10:00'),
    (13, 7, 288000.00, '2026-01-26 14:30'),
    (13, 5, 296000.00, '2026-02-01 08:45'),
    (13, 6, 304000.00, '2026-02-08 11:20'),
    (13, 7, 312000.00, '2026-02-15 16:00');

    -- Subasta 14 - Ternero Angus 002 (Activa) — 3 pujas
    INSERT INTO Puja (SubastaId, UsuarioId, Monto, FechaHora) VALUES
    (14, 7, 200000.00, '2026-02-27 09:00'),
    (14, 5, 205000.00, '2026-02-27 12:30'),
    (14, 6, 210000.00, '2026-02-28 08:00');
END
