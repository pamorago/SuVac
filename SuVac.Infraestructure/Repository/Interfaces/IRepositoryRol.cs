using SuVac.Infraestructure.Models;

namespace SuVac.Infraestructure.Repository.Interfaces;

public interface IRepositoryRol
{
    Task<IEnumerable<Rol>> GetAll();
    Task<Rol> GetById(int id);
    Task<bool> Create(Rol entity);
    Task<bool> Update(Rol entity);
    Task<bool> Delete(int id);
}
