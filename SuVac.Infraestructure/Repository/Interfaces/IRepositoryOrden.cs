using SuVac.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuVac.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryOrden
    {
        Task<ICollection<Orden>> ListAsync(); 
        Task<Orden> FindByIdAsync(int id);

        Task<Orden> FindByIdChangeAsync(int id);
        Task<int> AddAsync(Orden entity);
        Task<int> GetNextNumberOrden();
        Task<ICollection<Orden>> OrdenByClientId(string id);
    }
}
