using SuVac.Application.DTOs;

namespace SuVac.Application.Services.Interfaces;

public interface IServiceSubasta
{
    Task<IEnumerable<SubastaDTO>> GetAll();
    Task<SubastaDTO> GetById(int id);
    Task<bool> Create(SubastaDTO dto);
    Task<bool> Update(SubastaDTO dto);
    Task<bool> Delete(int id);
}
