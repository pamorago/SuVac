using SuVac.Infraestructure.Data;
using SuVac.Infraestructure.Models;
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

    public async Task<IEnumerable<Subasta>> GetSubastasPorPeriodoAsync(
        DateTime desde, DateTime hasta, string? estado)
    {
        var query = _context.Subastas
            .Include(s => s.IdEstadoSubastaNavigation)
            .Include(s => s.IdGanadoNavigation)
            .Include(s => s.IdUsuarioCreadorNavigation)
            .Where(s => s.FechaInicio >= desde && s.FechaInicio <= hasta.AddDays(1));

        if (!string.IsNullOrEmpty(estado))
            query = query.Where(s => s.IdEstadoSubastaNavigation.Nombre == estado);

        return await query.AsNoTracking().ToListAsync();
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

    public async Task<IEnumerable<Puja>> GetPujasPorPeriodoConUsuarioAsync(
        DateTime desde, DateTime hasta)
    {
        return await _context.Pujas
            .Include(p => p.IdUsuarioNavigation)
            .Where(p => p.FechaHora >= desde && p.FechaHora <= hasta.AddDays(1))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ResultadoSubasta>> GetResultadosPorPeriodoAsync(
        DateTime desde, DateTime hasta)
    {
        return await _context.ResultadosSubasta
            .Where(r => r.FechaCierre >= desde && r.FechaCierre <= hasta.AddDays(1))
            .AsNoTracking()
            .ToListAsync();
    }
}
