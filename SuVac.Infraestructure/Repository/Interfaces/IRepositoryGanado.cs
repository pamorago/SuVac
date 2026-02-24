using SuVac.Infraestructure.Models;

namespace SuVac.Infraestructure.Repository.Interfaces;

public interface IRepositoryGanado
{
    Task<IEnumerable<Ganado>> GetAll();
    Task<Ganado> GetById(int id);
    Task<bool> Create(Ganado entity);
    Task<bool> Update(Ganado entity);
    Task<bool> Delete(int id);
}
