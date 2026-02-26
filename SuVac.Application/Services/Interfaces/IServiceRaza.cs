using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;

namespace SuVac.Application.Services.Interfaces;

public interface IServiceRaza
{
    Task<IEnumerable<RazaDTO>> GetAll();
    Task<RazaDTO> GetById(int id);
    Task<bool> Create(RazaDTO dto);
    Task<bool> Update(RazaDTO dto);
    Task<bool> Delete(int id);
}
