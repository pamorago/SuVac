using SuVac.Infraestructure.Modelos;

namespace SuVac.Infraestructure.Repositorio.Interfaces;

public interface IRepositorioSubasta
{
    /// <summary>Retorna todas las subastas activas con sus relaciones.</summary>
    Task<ICollection<Subasta>> ListarActivasAsync();

    /// <summary>Retorna todas las subastas finalizadas o canceladas.</summary>
    Task<ICollection<Subasta>> ListarFinalizadasAsync();

    /// <summary>Retorna el detalle completo de una subasta por su Id.</summary>
    Task<Subasta?> BuscarPorIdAsync(int id);
}
