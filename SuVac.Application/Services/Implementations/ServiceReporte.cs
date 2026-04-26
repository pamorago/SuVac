using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Repository.Interfaces;

namespace SuVac.Application.Services.Implementations;

public class ServiceReporte : IServiceReporte
{
    private readonly IRepositoryReporte _repo;

    public ServiceReporte(IRepositoryReporte repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<ReporteSubastaDTO>> GetSubastasPorPeriodoAsync(
        DateTime desde, DateTime hasta, string? estado)
    {
        var subastas = await _repo.GetSubastasPorPeriodoAsync(desde, hasta, estado);
        return subastas.Select(s => new ReporteSubastaDTO
        {
            SubastaId = s.SubastaId,
            NombreGanado = s.IdGanadoNavigation?.Nombre ?? $"Ganado #{s.GanadoId}",
            NombreCreador = s.IdUsuarioCreadorNavigation?.NombreCompleto ?? "—",
            FechaInicio = s.FechaInicio,
            FechaFin = s.FechaFin,
            PrecioBase = s.PrecioBase,
            EstadoSubasta = s.IdEstadoSubastaNavigation?.Nombre ?? "—"
        });
    }

    public Task<IEnumerable<string>> GetEstadosSubastaAsync()
        => _repo.GetEstadosSubastaAsync();

    public Task<decimal> GetMontoRecaudadoAsync(DateTime desde, DateTime hasta)
        => _repo.GetMontoRecaudadoAsync(desde, hasta);

    public async Task<IEnumerable<ReporteTopCompradorDTO>> GetTopCompradoresAsync(
        DateTime desde, DateTime hasta, int top)
    {
        var pujas = await _repo.GetPujasPorPeriodoConUsuarioAsync(desde, hasta);
        var resultados = await _repo.GetResultadosPorPeriodoAsync(desde, hasta);

        var ganadores = resultados
            .GroupBy(r => r.UsuarioGanadorId)
            .ToDictionary(g => g.Key, g => g.Count());

        return pujas
            .GroupBy(p => new { p.UsuarioId, Nombre = p.IdUsuarioNavigation?.NombreCompleto ?? $"Usuario #{p.UsuarioId}" })
            .Select(g => new ReporteTopCompradorDTO
            {
                UsuarioId = g.Key.UsuarioId,
                Nombre = g.Key.Nombre,
                TotalPujas = g.Count(),
                MontoMaximo = g.Max(p => p.Monto),
                MontoPromedio = g.Average(p => p.Monto),
                SubastasGanadas = ganadores.GetValueOrDefault(g.Key.UsuarioId, 0)
            })
            .OrderByDescending(x => x.TotalPujas)
            .Take(top);
    }
}
