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
    /// <summary>Actualiza únicamente NombreCompleto y Correo sin tocar otros campos.</summary>
    Task<bool> UpdatePerfil(int id, string nombreCompleto, string correo);
    /// <summary>Alterna estado Activo(1) ↔ Bloqueado(2).</summary>
    Task<bool> ToggleEstado(int id);
    Task<bool> Delete(int id);

    /// <summary>Busca usuario por correo y contraseña ya cifrada. Solo retorna usuarios Activos.</summary>
    Task<Usuario?> GetByCorreoYPassword(string correo, string passwordEncriptado);

    /// <summary>Retorna las subastas creadas por el usuario y aquellas en las que pujó.</summary>
    Task<IEnumerable<(Subasta subasta, string rolEnSubasta, decimal? mejorPuja, bool esGanador)>> GetHistorialAsync(int usuarioId);
}
