using SuVac.Infraestructure.Models;

namespace SuVac.Infraestructure.Repository.Interfaces;

public interface IRepositoryPago
{
    Task<IEnumerable<Pago>> GetAll();
    Task<Pago?> GetById(int id);
    Task<bool> Create(Pago entity);
    Task<bool> Update(Pago entity);
    Task<bool> Delete(int id);
}
