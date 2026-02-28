using SuVac.Infraestructure.Models;

namespace SuVac.Infraestructure.Repository.Interfaces;

public interface IRepositoryUsuario
{
    Task<IEnumerable<Usuario>> GetAll();
    Task<Usuario?> GetById(int id);
    Task<IEnumerable<Usuario>> GetAllFull();
    Task<Usuario?> GetByIdFull(int id);

    /// <summary>Conteo de Subastas donde el Usuario es creador (LINQ CountAsync).</summary>
    Task<int> CountSubastasAsync(int usuarioId);

    /// <summary>Conteo de Pujas donde el Usuario es pujador (LINQ CountAsync).</summary>
    Task<int> CountPujasAsync(int usuarioId);

    Task<bool> Create(Usuario entity);
    Task<bool> Update(Usuario entity);
    Task<bool> Delete(int id);
}
