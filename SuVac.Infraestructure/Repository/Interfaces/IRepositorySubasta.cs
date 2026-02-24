using SuVac.Infraestructure.Models;

namespace SuVac.Infraestructure.Repository.Interfaces;

public interface IRepositorySubasta
{
    Task<IEnumerable<Subasta>> GetAll();
    Task<Subasta> GetById(int id);
    Task<bool> Create(Subasta entity);
    Task<bool> Update(Subasta entity);
    Task<bool> Delete(int id);

    // Avance 2: consultas especializadas con includes
    Task<IEnumerable<Subasta>> GetActivas();
    Task<IEnumerable<Subasta>> GetFinalizadas();
    Task<Subasta?> GetByIdFull(int id);
}
