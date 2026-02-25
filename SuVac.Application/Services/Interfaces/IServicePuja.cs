using SuVac.Application.DTOs;

namespace SuVac.Application.Services.Interfaces;

public interface IServicePuja
{
    Task<IEnumerable<PujaDTO>> GetAll();
    Task<PujaDTO> GetById(int id);
    Task<bool> Create(PujaDTO dto);
    Task<bool> Update(PujaDTO dto);
    Task<bool> Delete(int id);

    Task<IEnumerable<PujaDTO>> GetBySubasta(int subastaId);
}
