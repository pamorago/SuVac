using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuVac.Application.Services.Interfaces
{
    public interface IServiceCategoria
    {
        Task<ICollection<CategoriaDTO>> ListAsync();
        Task<CategoriaDTO?> FindByIdAsync(int id);
    }
}
