using SuVac.Infraestructure.Models;

namespace SuVac.Infraestructure.Repository.Interfaces;

public interface IRepositoryPuja
{
    Task<IEnumerable<Puja>> GetAll();
    Task<Puja?> GetById(int id);
    Task<bool> Create(Puja entity);
    Task<bool> Update(Puja entity);
    Task<bool> Delete(int id);

    Task<IEnumerable<Puja>> GetBySubasta(int subastaId);

    /// <summary>Retorna la puja con el monto más alto para una subasta. Null si no hay pujas.</summary>
    Task<Puja?> GetPujaMasAlta(int subastaId);
}
