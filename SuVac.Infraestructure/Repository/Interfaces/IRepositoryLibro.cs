using SuVac.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuVac.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryLibro
    {
        Task<ICollection<Libro>> ListAsync();
        
        Task<Libro> FindByIdAsync(int id);
        Task<int> AddAsync(Libro entity, string[] selectedCategorias);
        Task UpdateAsync(Libro entity, string[] selectedCategorias);
        Task <ICollection<Libro>> GetLibroByCategoria(int IdCategoria);
        Task<ICollection<Libro>> FindByNameAsync(string nombre);
        Task DeleteAsync(int id);
    }
}
