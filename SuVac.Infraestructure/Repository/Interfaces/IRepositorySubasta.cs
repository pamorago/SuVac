using SuVac.Infraestructure.Models;

namespace SuVac.Infraestructure.Repository.Interfaces;

public interface IRepositorySubasta
{
    Task<IEnumerable<Subasta>> GetAll();
    Task<Subasta?> GetById(int id);
    Task<bool> Create(Subasta entity);
    Task<bool> Update(Subasta entity);
    Task<bool> Delete(int id);

    Task<IEnumerable<Subasta>> GetActivas();
    Task<IEnumerable<Subasta>> GetFinalizadas();
    Task<Subasta?> GetByIdFull(int id);

    /// <summary>Conteo de pujas asociadas a una subasta (LINQ CountAsync).</summary>
    Task<int> CountPujasAsync(int subastaId);

    // ── Métodos administrativos ───────────────────────────────────────────

    /// <summary>Todos los registros con navegaciones para listado admin.</summary>
    Task<IEnumerable<Subasta>> GetAllAdmin();

    /// <summary>Ganados en estado Activo sin subasta activa/programada/borrador (para dropdown).</summary>
    Task<IEnumerable<Ganado>> GetGanadosActivos();

    /// <summary>Cambia el EstadoSubastaId de una subasta.</summary>
    Task<bool> CambiarEstado(int subastaId, int nuevoEstadoId);

    /// <summary>Verifica si la subasta tiene al menos 1 puja registrada.</summary>
    Task<bool> TienePujas(int subastaId);

    /// <summary>Verifica si el ganado ya tiene una subasta en estado activa/programada/borrador.</summary>
    Task<bool> GanadoTieneSubastaActiva(int ganadoId, int? excluirSubastaId = null);

    /// <summary>Verifica si el ganado está en estado Activo.</summary>
    Task<bool> GanadoEstaActivo(int ganadoId);

    /// <summary>Obtiene el EstadoSubastaId por nombre.</summary>
    Task<int?> GetEstadoIdByNombre(string nombre);
}
