using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuVac.Application.Services.Interfaces
{
    public interface IServiceLibro
    {

        Task<ICollection<LibroDTO>> ListAsync();

        Task<LibroDTO> FindByIdAsync(int id);
        Task<int> AddAsync(LibroDTO dto, string[] selectedCategorias);

        Task UpdateAsync(int id, LibroDTO dto, string[] selectedCategorias);

        Task<ICollection<LibroDTO>> GetLibroByCategoria(int IdCategoria);
        Task<ICollection<LibroDTO>> FindByNameAsync(string nombre);

        Task DeleteAsync(int id);
    }
}
