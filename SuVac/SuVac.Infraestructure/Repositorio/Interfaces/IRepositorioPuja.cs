using SuVac.Infraestructure.Modelos;

namespace SuVac.Infraestructure.Repositorio.Interfaces;

public interface IRepositorioPuja
{
    /// <summary>Retorna el historial de pujas de una subasta, ordenado cronológicamente ascendente.</summary>
    Task<ICollection<Puja>> ListarPorSubastaAsync(int subastaId);
}
