using SuVac.Infraestructure.Models;

namespace SuVac.Infraestructure.Repository.Interfaces;

public interface IRepositoryResultadoSubasta
{
    Task<IEnumerable<ResultadoSubasta>> GetAll();
    Task<ResultadoSubasta> GetById(int id);
    Task<bool> Create(ResultadoSubasta entity);
    Task<bool> Update(ResultadoSubasta entity);
    Task<bool> Delete(int id);
    Task<ResultadoSubasta> GetBySubastaId(int subastaId);
}
