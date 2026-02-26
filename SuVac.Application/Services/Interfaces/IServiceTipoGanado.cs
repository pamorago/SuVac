using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;

namespace SuVac.Application.Services.Interfaces;

public interface IServiceTipoGanado
{
    Task<IEnumerable<TipoGanadoDTO>> GetAll();
    Task<TipoGanadoDTO> GetById(int id);
    Task<bool> Create(TipoGanadoDTO dto);
    Task<bool> Update(TipoGanadoDTO dto);
    Task<bool> Delete(int id);
}
