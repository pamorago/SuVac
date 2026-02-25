using SuVac.Infraestructure.Models;

namespace SuVac.Infraestructure.Repository.Interfaces;

public interface IRepositoryPuja
{
    Task<IEnumerable<Puja>> GetAll();
    Task<Puja> GetById(int id);
    Task<bool> Create(Puja entity);
    Task<bool> Update(Puja entity);
    Task<bool> Delete(int id);

    Task<IEnumerable<Puja>> GetBySubasta(int subastaId);
}
