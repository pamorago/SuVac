using SuVac.Application.DTOs;

namespace SuVac.Application.Services.Interfaces;

public interface IServicePago
{
    Task<IEnumerable<PagoDTO>> GetAll();
    Task<PagoDTO> GetById(int id);
    Task<bool> Create(PagoDTO dto);
    Task<bool> Update(PagoDTO dto);
    Task<bool> Delete(int id);
}
