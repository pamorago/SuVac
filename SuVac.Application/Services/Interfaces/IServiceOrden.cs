using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuVac.Application.Services.Interfaces
{
    public interface IServiceOrden
    {
        Task<ICollection<OrdenDTO>> ListAsync();
        Task<OrdenDTO> FindByIdAsync(int id);

        Task<OrdenDTO> FindByIdChangeAsync(int id);
        Task<int> AddAsync(OrdenDTO dto);
        Task<int> GetNextNumberOrden();
    }
}
