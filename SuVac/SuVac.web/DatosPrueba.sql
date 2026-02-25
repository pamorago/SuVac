USE SuVac;
GO

-- ==============================================================
-- DATOS DE PRUEBA - Avance 2
-- IMÃGENES: copiar los archivos a SuVac.web/wwwroot/img/
-- y referenciarlas como /img/nombre-archivo.jpg
-- ==============================================================

-- ==============================================================
-- CATEGORÃAS
-- ==============================================================
IF NOT EXISTS (SELECT 1 FROM Categoria WHERE Nombre = 'ReproducciÃ³n')
    INSERT INTO Categoria (Nombre, Descripcion) VALUES
    ('ReproducciÃ³n',   'Ganado destinado a mejoramiento genÃ©tico y reproducciÃ³n'),
    ('Engorde',        'Ganado de alta conversiÃ³n alimenticia para producciÃ³n de carne'),
    ('Lechero',        'Ganado de alta producciÃ³n de leche'),
    ('Doble PropÃ³sito','Ganado apto tanto para carne como para leche'),
    ('Trabajo',        'Ganado utilizado para labores agrÃ­colas y transporte'),
    ('ExposiciÃ³n',     'Ganado de alto registro criado para ferias y concursos');
GO


-- ==============================================================
-- GANADO (6 registros variados)
-- IMÃGENES: coloca los archivos en SuVac.web/wwwroot/img/
-- Nombres sugeridos: brahman.jpg, angus.jpg, holstein.jpg,
--   charolais.jpg, simmental.jpg, jersey.jpg
-- ==============================================================
IF NOT EXISTS (SELECT 1 FROM Ganado WHERE Nombre = 'Toro Majestuoso')
BEGIN
    INSERT INTO Ganado (Nombre, Descripcion, TipoGanadoId, RazaId, SexoId, FechaNacimiento, PesoKg, CertificadoSalud, EstadoGanadoId, UsuarioVendedorId)
    VALUES
    -- 1. Brahman Macho - Crianza
    ('Toro Majestuoso',
     'Toro Brahman de excelente conformaciÃ³n corporal, criado en clima tropical con vacunas al dÃ­a. Ideal para reproducciÃ³n y mejoramiento genÃ©tico del hato.',
     3, 1, 1, '2021-03-15', 520.50, 'CERT-2024-0012', 1, 2),
    -- 2. Angus Hembra - Carne
    ('Vaca Negra Fortuna',
     'Vaca Angus de alto rendimiento cÃ¡rnico, temperamento dÃ³cil y excelente conversiÃ³n alimenticia. Ha parido dos veces con crÃ­as vigorosas.',
     1, 2, 2, '2020-07-22', 410.00, 'CERT-2024-0045', 1, 2),
    -- 3. Holstein Hembra - Leche
    ('Holstein Estrella',
     'Vaca Holstein de alta producciÃ³n lechera, promedio de 28 litros diarios. Sistema estabulado, sin historial de enfermedades.',
     2, 3, 2, '2019-11-05', 560.75, 'CERT-2023-0078', 1, 3),
    -- 4. Charolais Macho - Carne
    ('Charolais Rey Blanco',
     'Toro Charolais de gran musculatura y talla sobresaliente. GenÃ©tica importada de Francia, certificado de pureza racial disponible.',
     1, 5, 1, '2022-01-10', 680.00, 'CERT-2024-0099', 1, 3),
    -- 5. Simmental Hembra - Doble propÃ³sito
    ('Simmental Dorada',
     'Vaca Simmental de doble propÃ³sito con buen potencial lechero y alta ganancia de peso. CarÃ¡cter tranquilo y fÃ¡cil manejo.',
     1, 6, 2, '2020-05-18', 480.25, 'CERT-2023-0055', 1, 2),
    -- 6. Jersey Hembra - Leche
    ('Jersey Dulce Luna',
     'Vaca Jersey compacta de alta eficiencia, leche con 5,5% de grasa. Rendimiento sostenido en Ã©pocas secas. Excelente para queserÃ­a artesanal.',
     2, 4, 2, '2021-09-30', 320.00, 'CERT-2024-0133', 1, 3);
END
GO

-- ==============================================================
-- IMÃGENES DE GANADO
-- Las URLs /img/... apuntan a wwwroot/img/ dentro del proyecto
-- ==============================================================
IF NOT EXISTS (SELECT 1 FROM ImagenGanado WHERE GanadoId = (SELECT TOP 1 GanadoId FROM Ganado WHERE Nombre = 'Toro Majestuoso'))
BEGIN
    DECLARE @g1 INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Toro Majestuoso');
    DECLARE @g2 INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Vaca Negra Fortuna');
    DECLARE @g3 INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Holstein Estrella');
    DECLARE @g4 INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Charolais Rey Blanco');
    DECLARE @g5 INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Simmental Dorada');
    DECLARE @g6 INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Jersey Dulce Luna');

    INSERT INTO ImagenGanado (GanadoId, UrlImagen) VALUES
    -- Toro Majestuoso
    (@g1, '/img/Brahman.jpg'),
    (@g1, '/img/Brangus.jpg'),
    -- Vaca Negra Fortuna
    (@g2, '/img/Angus.jpg'),
    (@g2, '/img/Beefmaster.jpg'),
    -- Holstein Estrella
    (@g3, '/img/Holstein.jpg'),
    (@g3, '/img/Gyr.jpg'),
    -- Charolais Rey Blanco
    (@g4, '/img/Charolais.jpeg'),
    (@g4, '/img/Chianina.jpg'),
    -- Simmental Dorada
    (@g5, '/img/Simmental.jpg'),
    (@g5, '/img/Limousin.jpeg'),
    -- Jersey Dulce Luna (no hay Jersey.jpg; se usan razas similares)
    (@g6, '/img/Hereford.jpg'),
    (@g6, '/img/Guzerat.png');
END
GO

-- ==============================================================
-- GANADO-CATEGORÃA
-- ==============================================================
IF NOT EXISTS (SELECT 1 FROM GanadoCategoria)
BEGIN
    DECLARE @g1c INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Toro Majestuoso');
    DECLARE @g2c INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Vaca Negra Fortuna');
    DECLARE @g3c INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Holstein Estrella');
    DECLARE @g4c INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Charolais Rey Blanco');
    DECLARE @g5c INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Simmental Dorada');
    DECLARE @g6c INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Jersey Dulce Luna');

    DECLARE @catRep INT = (SELECT CategoriaId FROM Categoria WHERE Nombre = 'ReproducciÃ³n');
    DECLARE @catEng INT = (SELECT CategoriaId FROM Categoria WHERE Nombre = 'Engorde');
    DECLARE @catLec INT = (SELECT CategoriaId FROM Categoria WHERE Nombre = 'Lechero');
    DECLARE @catDob INT = (SELECT CategoriaId FROM Categoria WHERE Nombre = 'Doble PropÃ³sito');
    DECLARE @catExp INT = (SELECT CategoriaId FROM Categoria WHERE Nombre = 'ExposiciÃ³n');

    INSERT INTO GanadoCategoria (GanadoId, CategoriaId) VALUES
    (@g1c, @catRep),
    (@g1c, @catExp),
    (@g2c, @catEng),
    (@g3c, @catLec),
    (@g4c, @catEng),
    (@g4c, @catExp),
    (@g5c, @catDob),
    (@g5c, @catLec),
    (@g6c, @catLec);
END
GO

-- ==============================================================
-- SUBASTAS (6 activas + 3 finalizadas + 1 cancelada)
-- ==============================================================
IF NOT EXISTS (SELECT 1 FROM Subasta)
BEGIN
    DECLARE @eActiva INT = (SELECT EstadoSubastaId FROM EstadoSubasta WHERE Nombre = 'Activa');
    DECLARE @eFin    INT = (SELECT EstadoSubastaId FROM EstadoSubasta WHERE Nombre = 'Finalizada');
    DECLARE @eCan    INT = (SELECT EstadoSubastaId FROM EstadoSubasta WHERE Nombre = 'Cancelada');

    DECLARE @gA INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Toro Majestuoso');
    DECLARE @gB INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Vaca Negra Fortuna');
    DECLARE @gC INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Holstein Estrella');
    DECLARE @gD INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Charolais Rey Blanco');
    DECLARE @gE INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Simmental Dorada');
    DECLARE @gF INT = (SELECT GanadoId FROM Ganado WHERE Nombre = 'Jersey Dulce Luna');

    -- â”€â”€ SUBASTAS ACTIVAS â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    -- Activa 1: Toro Majestuoso
    INSERT INTO Subasta VALUES (@gA,'2026-02-20 08:00','2026-02-27 18:00',750000.00,25000.00,@eActiva,2);
    -- Activa 2: Vaca Negra Fortuna
    INSERT INTO Subasta VALUES (@gB,'2026-02-21 09:00','2026-02-28 20:00',520000.00,15000.00,@eActiva,2);
    -- Activa 3: Holstein Estrella
    INSERT INTO Subasta VALUES (@gC,'2026-02-22 10:00','2026-03-01 17:00',680000.00,20000.00,@eActiva,3);
    -- Activa 4: Charolais Rey Blanco (segunda ronda)
    INSERT INTO Subasta VALUES (@gD,'2026-02-23 08:00','2026-03-02 18:00',950000.00,30000.00,@eActiva,3);
    -- Activa 5: Simmental Dorada
    INSERT INTO Subasta VALUES (@gE,'2026-02-22 07:00','2026-02-28 16:00',610000.00,20000.00,@eActiva,2);
    -- Activa 6: Jersey Dulce Luna
    INSERT INTO Subasta VALUES (@gF,'2026-02-23 09:00','2026-03-03 17:00',420000.00,12000.00,@eActiva,3);

    -- â”€â”€ SUBASTAS FINALIZADAS â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    -- Finalizada 1: Charolais Rey Blanco
    INSERT INTO Subasta VALUES (@gD,'2026-02-01 08:00','2026-02-10 18:00',900000.00,30000.00,@eFin,3);
    -- Finalizada 2: Simmental Dorada
    INSERT INTO Subasta VALUES (@gE,'2026-01-15 08:00','2026-01-25 18:00',600000.00,20000.00,@eFin,2);
    -- Finalizada 3: Holstein Estrella (primera ronda)
    INSERT INTO Subasta VALUES (@gC,'2025-12-01 08:00','2025-12-15 18:00',650000.00,18000.00,@eFin,3);

    -- â”€â”€ CANCELADA â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    -- Cancelada 1: Toro Majestuoso (primera ronda)
    INSERT INTO Subasta VALUES (@gA,'2026-01-05 08:00','2026-01-10 18:00',500000.00,10000.00,@eCan,2);
END
GO

-- ==============================================================
-- PUJAS (6+ por subasta)
-- ==============================================================
IF NOT EXISTS (SELECT 1 FROM Puja)
BEGIN
    DECLARE @c1 INT = (SELECT UsuarioId FROM Usuario WHERE Correo = 'comprador1@mail.com');
    DECLARE @c2 INT = (SELECT UsuarioId FROM Usuario WHERE Correo = 'comprador2@mail.com');

    -- IDs de subastas activas
    DECLARE @sA1 INT = (SELECT TOP 1 s.SubastaId FROM Subasta s JOIN EstadoSubasta e ON e.EstadoSubastaId=s.EstadoSubastaId JOIN Ganado g ON g.GanadoId=s.GanadoId WHERE e.Nombre='Activa' AND g.Nombre='Toro Majestuoso'        ORDER BY s.SubastaId DESC);
    DECLARE @sA2 INT = (SELECT TOP 1 s.SubastaId FROM Subasta s JOIN EstadoSubasta e ON e.EstadoSubastaId=s.EstadoSubastaId JOIN Ganado g ON g.GanadoId=s.GanadoId WHERE e.Nombre='Activa' AND g.Nombre='Vaca Negra Fortuna'    ORDER BY s.SubastaId DESC);
    DECLARE @sA3 INT = (SELECT TOP 1 s.SubastaId FROM Subasta s JOIN EstadoSubasta e ON e.EstadoSubastaId=s.EstadoSubastaId JOIN Ganado g ON g.GanadoId=s.GanadoId WHERE e.Nombre='Activa' AND g.Nombre='Holstein Estrella'     ORDER BY s.SubastaId DESC);
    DECLARE @sA4 INT = (SELECT TOP 1 s.SubastaId FROM Subasta s JOIN EstadoSubasta e ON e.EstadoSubastaId=s.EstadoSubastaId JOIN Ganado g ON g.GanadoId=s.GanadoId WHERE e.Nombre='Activa' AND g.Nombre='Charolais Rey Blanco'  ORDER BY s.SubastaId DESC);
    DECLARE @sA5 INT = (SELECT TOP 1 s.SubastaId FROM Subasta s JOIN EstadoSubasta e ON e.EstadoSubastaId=s.EstadoSubastaId JOIN Ganado g ON g.GanadoId=s.GanadoId WHERE e.Nombre='Activa' AND g.Nombre='Simmental Dorada'     ORDER BY s.SubastaId DESC);
    DECLARE @sA6 INT = (SELECT TOP 1 s.SubastaId FROM Subasta s JOIN EstadoSubasta e ON e.EstadoSubastaId=s.EstadoSubastaId JOIN Ganado g ON g.GanadoId=s.GanadoId WHERE e.Nombre='Activa' AND g.Nombre='Jersey Dulce Luna'    ORDER BY s.SubastaId DESC);

    -- IDs de subastas finalizadas
    DECLARE @sF1 INT = (SELECT TOP 1 s.SubastaId FROM Subasta s JOIN EstadoSubasta e ON e.EstadoSubastaId=s.EstadoSubastaId JOIN Ganado g ON g.GanadoId=s.GanadoId WHERE e.Nombre='Finalizada' AND g.Nombre='Charolais Rey Blanco');
    DECLARE @sF2 INT = (SELECT TOP 1 s.SubastaId FROM Subasta s JOIN EstadoSubasta e ON e.EstadoSubastaId=s.EstadoSubastaId JOIN Ganado g ON g.GanadoId=s.GanadoId WHERE e.Nombre='Finalizada' AND g.Nombre='Simmental Dorada');
    DECLARE @sF3 INT = (SELECT TOP 1 s.SubastaId FROM Subasta s JOIN EstadoSubasta e ON e.EstadoSubastaId=s.EstadoSubastaId JOIN Ganado g ON g.GanadoId=s.GanadoId WHERE e.Nombre='Finalizada' AND g.Nombre='Holstein Estrella');

    -- â”€â”€ Activa 1: Toro Majestuoso (6 pujas) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    INSERT INTO Puja (SubastaId,UsuarioId,Monto,FechaHora) VALUES
    (@sA1,@c1, 775000.00,'2026-02-20 09:15'),
    (@sA1,@c2, 800000.00,'2026-02-21 11:30'),
    (@sA1,@c1, 825000.00,'2026-02-21 15:00'),
    (@sA1,@c2, 850000.00,'2026-02-22 08:45'),
    (@sA1,@c1, 875000.00,'2026-02-22 14:20'),
    (@sA1,@c2, 900000.00,'2026-02-23 10:00');

    -- â”€â”€ Activa 2: Vaca Negra Fortuna (6 pujas) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    INSERT INTO Puja (SubastaId,UsuarioId,Monto,FechaHora) VALUES
    (@sA2,@c2, 535000.00,'2026-02-21 10:00'),
    (@sA2,@c1, 550000.00,'2026-02-21 16:20'),
    (@sA2,@c2, 565000.00,'2026-02-22 09:00'),
    (@sA2,@c1, 580000.00,'2026-02-22 12:30'),
    (@sA2,@c2, 595000.00,'2026-02-23 07:00'),
    (@sA2,@c1, 610000.00,'2026-02-23 11:45');

    -- â”€â”€ Activa 3: Holstein Estrella (6 pujas) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    INSERT INTO Puja (SubastaId,UsuarioId,Monto,FechaHora) VALUES
    (@sA3,@c1, 700000.00,'2026-02-22 11:00'),
    (@sA3,@c2, 720000.00,'2026-02-22 14:30'),
    (@sA3,@c1, 740000.00,'2026-02-22 17:00'),
    (@sA3,@c2, 760000.00,'2026-02-23 07:30'),
    (@sA3,@c1, 780000.00,'2026-02-23 10:00'),
    (@sA3,@c2, 800000.00,'2026-02-23 12:00');

    -- â”€â”€ Activa 4: Charolais Rey Blanco (4 pujas, subasta nueva) â”€
    INSERT INTO Puja (SubastaId,UsuarioId,Monto,FechaHora) VALUES
    (@sA4,@c1, 980000.00, '2026-02-23 09:00'),
    (@sA4,@c2,1010000.00, '2026-02-23 11:00'),
    (@sA4,@c1,1040000.00, '2026-02-23 13:00'),
    (@sA4,@c2,1070000.00, '2026-02-23 15:00');

    -- â”€â”€ Activa 5: Simmental Dorada (3 pujas) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    INSERT INTO Puja (SubastaId,UsuarioId,Monto,FechaHora) VALUES
    (@sA5,@c2, 630000.00,'2026-02-22 08:00'),
    (@sA5,@c1, 650000.00,'2026-02-22 10:30'),
    (@sA5,@c2, 670000.00,'2026-02-23 09:15');

    -- â”€â”€ Activa 6: Jersey Dulce Luna (0 pujas, variedad) â”€â”€â”€â”€â”€â”€
    -- Sin pujas intencionalmente para mostrar variedad de casos

    -- â”€â”€ Finalizada 1: Charolais Rey Blanco (6 pujas) â”€â”€â”€â”€â”€â”€â”€â”€â”€
    INSERT INTO Puja (SubastaId,UsuarioId,Monto,FechaHora) VALUES
    (@sF1,@c1, 930000.00, '2026-02-02 09:00'),
    (@sF1,@c2, 960000.00, '2026-02-04 14:00'),
    (@sF1,@c1, 990000.00, '2026-02-06 11:30'),
    (@sF1,@c2,1020000.00, '2026-02-08 17:00'),
    (@sF1,@c1,1050000.00, '2026-02-09 16:00'),
    (@sF1,@c2,1080000.00, '2026-02-10 09:30');

    -- â”€â”€ Finalizada 2: Simmental Dorada (6 pujas) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    INSERT INTO Puja (SubastaId,UsuarioId,Monto,FechaHora) VALUES
    (@sF2,@c2, 620000.00,'2026-01-16 10:00'),
    (@sF2,@c1, 640000.00,'2026-01-18 09:15'),
    (@sF2,@c2, 660000.00,'2026-01-20 15:30'),
    (@sF2,@c1, 680000.00,'2026-01-22 08:00'),
    (@sF2,@c2, 700000.00,'2026-01-23 11:00'),
    (@sF2,@c1, 720000.00,'2026-01-24 16:45');

    -- â”€â”€ Finalizada 3: Holstein Estrella (6 pujas) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    INSERT INTO Puja (SubastaId,UsuarioId,Monto,FechaHora) VALUES
    (@sF3,@c1, 668000.00,'2025-12-02 09:00'),
    (@sF3,@c2, 686000.00,'2025-12-05 10:30'),
    (@sF3,@c1, 704000.00,'2025-12-08 14:00'),
    (@sF3,@c2, 722000.00,'2025-12-10 08:45'),
    (@sF3,@c1, 740000.00,'2025-12-12 11:00'),
    (@sF3,@c2, 758000.00,'2025-12-14 16:00');
END
GO

-- ==============================================================
-- RESULTADO DE SUBASTAS FINALIZADAS
-- ==============================================================
IF NOT EXISTS (SELECT 1 FROM ResultadoSubasta)
BEGIN
    DECLARE @rF1 INT = (SELECT TOP 1 s.SubastaId FROM Subasta s JOIN EstadoSubasta e ON e.EstadoSubastaId=s.EstadoSubastaId JOIN Ganado g ON g.GanadoId=s.GanadoId WHERE e.Nombre='Finalizada' AND g.Nombre='Charolais Rey Blanco');
    DECLARE @rF2 INT = (SELECT TOP 1 s.SubastaId FROM Subasta s JOIN EstadoSubasta e ON e.EstadoSubastaId=s.EstadoSubastaId JOIN Ganado g ON g.GanadoId=s.GanadoId WHERE e.Nombre='Finalizada' AND g.Nombre='Simmental Dorada');
    DECLARE @rF3 INT = (SELECT TOP 1 s.SubastaId FROM Subasta s JOIN EstadoSubasta e ON e.EstadoSubastaId=s.EstadoSubastaId JOIN Ganado g ON g.GanadoId=s.GanadoId WHERE e.Nombre='Finalizada' AND g.Nombre='Holstein Estrella');

    DECLARE @rc1 INT = (SELECT UsuarioId FROM Usuario WHERE Correo='comprador1@mail.com');
    DECLARE @rc2 INT = (SELECT UsuarioId FROM Usuario WHERE Correo='comprador2@mail.com');

    INSERT INTO ResultadoSubasta (SubastaId, UsuarioGanadorId, MontoFinal, FechaCierre) VALUES
    (@rF1, @rc2, 1080000.00, '2026-02-10 18:00'),
    (@rF2, @rc1,  720000.00, '2026-01-25 18:00'),
    (@rF3, @rc2,  758000.00, '2025-12-15 18:00');
END
GO
-- FIN DatosPrueba.sql
-- Los archivos de imagen van en: SuVac.web/wwwroot/img/
-- Nombres: brahman1.jpg, brahman2.jpg, angus1.jpg, angus2.jpg,
--          holstein1.jpg, holstein2.jpg, charolais1.jpg, charolais2.jpg,
--          simmental1.jpg, simmental2.jpg, jersey1.jpg, jersey2.jpg

