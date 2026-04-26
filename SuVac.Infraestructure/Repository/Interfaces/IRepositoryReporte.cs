using SuVac.Infraestructure.Models;

namespace SuVac.Infraestructure.Repository.Interfaces;

public interface IRepositoryReporte
{
    Task<IEnumerable<Subasta>> GetSubastasPorPeriodoAsync(DateTime desde, DateTime hasta, string? estado);
    Task<IEnumerable<string>> GetEstadosSubastaAsync();
    Task<decimal> GetMontoRecaudadoAsync(DateTime desde, DateTime hasta);
    Task<IEnumerable<Puja>> GetPujasPorPeriodoConUsuarioAsync(DateTime desde, DateTime hasta);
    Task<IEnumerable<ResultadoSubasta>> GetResultadosPorPeriodoAsync(DateTime desde, DateTime hasta);
}
