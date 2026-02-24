using SuVac.Infraestructure.Models;

namespace SuVac.Infraestructure.Repository.Interfaces;

public interface IRepositoryUsuario
{
    Task<IEnumerable<Usuario>> GetAll();
    Task<Usuario> GetById(int id);
    Task<bool> Create(Usuario entity);
    Task<bool> Update(Usuario entity);
    Task<bool> Delete(int id);
}
