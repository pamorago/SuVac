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

    public Task<IEnumerable<ReporteSubastaDTO>> GetSubastasPorPeriodoAsync(DateTime desde, DateTime hasta, string? estado)
        => _repo.GetSubastasPorPeriodoAsync(desde, hasta, estado);

    public Task<IEnumerable<string>> GetEstadosSubastaAsync()
        => _repo.GetEstadosSubastaAsync();

    public Task<decimal> GetMontoRecaudadoAsync(DateTime desde, DateTime hasta)
        => _repo.GetMontoRecaudadoAsync(desde, hasta);

    public Task<IEnumerable<ReporteTopCompradorDTO>> GetTopCompradoresAsync(DateTime desde, DateTime hasta, int top)
        => _repo.GetTopCompradoresAsync(desde, hasta, top);
}
