using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;

namespace SuVac.Application.Services.Interfaces;

public interface IServiceResultadoSubasta
{
    Task<IEnumerable<ResultadoSubastaDTO>> GetAll();
    Task<ResultadoSubastaDTO> GetById(int id);
    Task<bool> Create(ResultadoSubastaDTO dto);
    Task<bool> Update(ResultadoSubastaDTO dto);
    Task<bool> Delete(int id);
    Task<ResultadoSubastaDTO> GetBySubastaId(int subastaId);
}
