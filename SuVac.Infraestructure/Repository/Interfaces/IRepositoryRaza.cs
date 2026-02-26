using SuVac.Infraestructure.Models;

namespace SuVac.Infraestructure.Repository.Interfaces;

public interface IRepositoryRaza
{
    Task<IEnumerable<Raza>> GetAll();
    Task<Raza> GetById(int id);
    Task<bool> Create(Raza entity);
    Task<bool> Update(Raza entity);
    Task<bool> Delete(int id);
}
