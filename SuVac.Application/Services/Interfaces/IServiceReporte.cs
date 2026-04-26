using SuVac.Application.DTOs;

namespace SuVac.Application.Services.Interfaces;

public interface IServiceReporte
{
    Task<IEnumerable<ReporteSubastaDTO>> GetSubastasPorPeriodoAsync(DateTime desde, DateTime hasta, string? estado);
    Task<IEnumerable<string>> GetEstadosSubastaAsync();
    Task<decimal> GetMontoRecaudadoAsync(DateTime desde, DateTime hasta);
    Task<IEnumerable<ReporteTopCompradorDTO>> GetTopCompradoresAsync(DateTime desde, DateTime hasta, int top);
}
