using SuVac.Infraestructure.Models;

namespace SuVac.Infraestructure.Repository.Interfaces;

public interface IRepositoryTipoGanado
{
    Task<IEnumerable<TipoGanado>> GetAll();
    Task<TipoGanado> GetById(int id);
    Task<bool> Create(TipoGanado entity);
    Task<bool> Update(TipoGanado entity);
    Task<bool> Delete(int id);
}
