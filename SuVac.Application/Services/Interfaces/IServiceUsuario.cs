using SuVac.Application.DTOs;

namespace SuVac.Application.Services.Interfaces;

public interface IServiceUsuario
{
    Task<IEnumerable<UsuarioDTO>> GetAll();
    Task<UsuarioDTO> GetById(int id);
    Task<bool> Create(UsuarioDTO dto);
    Task<bool> Update(UsuarioDTO dto);
    Task<bool> Delete(int id);
}
