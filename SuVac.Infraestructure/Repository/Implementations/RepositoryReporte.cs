using SuVac.Application.DTOs;
using SuVac.Infraestructure.Data;
using SuVac.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SuVac.Infraestructure.Repository.Implementations;

public class RepositoryReporte : IRepositoryReporte
{
    private readonly SuVacContext _context;

    public RepositoryReporte(SuVacContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReporteSubastaDTO>> GetSubastasPorPeriodoAsync(
        DateTime desde, DateTime hasta, string? estado)
    {
        var query = _context.Subastas
            .Include(s => s.IdEstadoSubastaNavigation)
            .Include(s => s.IdGanadoNavigation)
            .Include(s => s.IdUsuarioCreadorNavigation)
            .Where(s => s.FechaInicio >= desde && s.FechaInicio <= hasta.AddDays(1));

        if (!string.IsNullOrEmpty(estado))
            query = query.Where(s => s.IdEstadoSubastaNavigation.Nombre == estado);

        return await query
            .AsNoTracking()
            .Select(s => new ReporteSubastaDTO
            {
                SubastaId = s.SubastaId,
                NombreGanado = s.IdGanadoNavigation.Nombre,
                NombreCreador = s.IdUsuarioCreadorNavigation.NombreCompleto,
                FechaInicio = s.FechaInicio,
                FechaFin = s.FechaFin,
                PrecioBase = s.PrecioBase,
                EstadoSubasta = s.IdEstadoSubastaNavigation.Nombre
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetEstadosSubastaAsync()
    {
        return await _context.EstadosSubasta
            .AsNoTracking()
            .Select(e => e.Nombre)
            .ToListAsync();
    }

    public async Task<decimal> GetMontoRecaudadoAsync(DateTime desde, DateTime hasta)
    {
        return await _context.ResultadosSubasta
            .Where(r => r.IdSubastaNavigation.FechaInicio >= desde
                     && r.IdSubastaNavigation.FechaInicio <= hasta.AddDays(1))
            .SumAsync(r => (decimal?)r.MontoFinal) ?? 0;
    }

    public async Task<IEnumerable<ReporteTopCompradorDTO>> GetTopCompradoresAsync(
        DateTime desde, DateTime hasta, int top)
    {
        // Agrupar pujas por UsuarioId (scalar) para compatibilidad con EF Core
        var pujasPorUsuario = await _context.Pujas
            .Where(p => p.FechaHora >= desde && p.FechaHora <= hasta.AddDays(1))
            .GroupBy(p => p.UsuarioId)
            .Select(g => new
            {
                UsuarioId = g.Key,
                TotalPujas = g.Count(),
                MontoMaximo = g.Max(p => p.Monto),
                MontoPromedio = g.Average(p => p.Monto)
            })
            .OrderByDescending(x => x.TotalPujas)
            .Take(top)
            .ToListAsync();

        var usuarioIds = pujasPorUsuario.Select(x => x.UsuarioId).ToList();

        var nombres = await _context.Usuarios
            .Where(u => usuarioIds.Contains(u.UsuarioId))
            .Select(u => new { u.UsuarioId, u.NombreCompleto })
            .ToListAsync();

        var ganadores = await _context.ResultadosSubasta
            .Where(r => r.FechaCierre >= desde && r.FechaCierre <= hasta.AddDays(1))
            .GroupBy(r => r.UsuarioGanadorId)
            .Select(g => new { UsuarioId = g.Key, SubastasGanadas = g.Count() })
            .ToListAsync();

        return pujasPorUsuario.Select(c => new ReporteTopCompradorDTO
        {
            UsuarioId = c.UsuarioId,
            Nombre = nombres.FirstOrDefault(u => u.UsuarioId == c.UsuarioId)?.NombreCompleto
                             ?? $"Usuario #{c.UsuarioId}",
            TotalPujas = c.TotalPujas,
            MontoMaximo = c.MontoMaximo,
            MontoPromedio = c.MontoPromedio,
            SubastasGanadas = ganadores.FirstOrDefault(g => g.UsuarioId == c.UsuarioId)?.SubastasGanadas ?? 0
        }).ToList();
    }
}
