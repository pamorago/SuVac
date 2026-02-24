using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Services.Interfaces;

public interface IServiceGanado
{
    Task<IEnumerable<GanadoDTO>> GetAll();
    Task<GanadoDTO> GetById(int id);
    Task<bool> Create(GanadoDTO dto);
    Task<bool> Update(GanadoDTO dto);
    Task<bool> Delete(int id);
}
