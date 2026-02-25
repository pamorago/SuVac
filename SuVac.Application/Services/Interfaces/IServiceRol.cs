using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;

namespace SuVac.Application.Services.Interfaces;

public interface IServiceRol
{
    Task<IEnumerable<RolDTO>> GetAll();
    Task<RolDTO> GetById(int id);
    Task<bool> Create(RolDTO dto);
    Task<bool> Update(RolDTO dto);
    Task<bool> Delete(int id);
}
