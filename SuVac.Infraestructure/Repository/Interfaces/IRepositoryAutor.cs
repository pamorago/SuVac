using SuVac.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuVac.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryAutor
    {
        Task<ICollection<Autor>> ListAsync();
        Task<Autor?> FindByIdAsync(int id);
    }
}
