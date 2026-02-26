using SuVac.Infraestructure.Models;

namespace SuVac.Infraestructure.Repository.Interfaces;

public interface IRepositoryCategoria
{
    Task<IEnumerable<Categoria>> GetAll();
    Task<Categoria> GetById(int id);
    Task<bool> Create(Categoria entity);
    Task<bool> Update(Categoria entity);
    Task<bool> Delete(int id);
}
