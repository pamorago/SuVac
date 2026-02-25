using SuVac.Application.DTOs;

namespace SuVac.Application.Servicios.Interfaces;

public interface IServicioPuja
{
    /// <summary>Retorna el historial de pujas de una subasta en orden cronológico ascendente.</summary>
    Task<ICollection<PujaDTO>> ListarPorSubastaAsync(int subastaId);
}
