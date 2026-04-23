using SuVac.Application.DTOs;
using SuVac.Infraestructure.Models;

namespace SuVac.Application.Services.Interfaces;

public interface IServiceUsuario
{
    Task<IEnumerable<UsuarioDTO>> GetAll();
    Task<UsuarioDTO> GetById(int id);
    Task<IEnumerable<UsuarioDTO>> GetAllConDetalle();
    Task<UsuarioDTO?> GetByIdConDetalle(int id);
    Task<bool> Create(UsuarioDTO dto);
    Task<bool> Update(UsuarioDTO dto);
    /// <summary>Actualiza únicamente NombreCompleto y Correo. No toca Rol, PasswordHash ni Estado.</summary>
    Task<bool> UpdatePerfil(int id, string nombreCompleto, string correo);
    /// <summary>Alterna el estado lógico: Activo (1) ↔ Bloqueado (2).</summary>
    Task<bool> ToggleEstado(int id);
    Task<bool> Delete(int id);
    /// <summary>Valida credenciales y retorna la entidad Usuario con su rol. Null si inválido o bloqueado.</summary>
    Task<Usuario?> LoginAsync(string correo, string password);
    /// <summary>Retorna el historial de actividad del usuario (subastas creadas y en las que pujó).</summary>
    Task<IEnumerable<HistorialUsuarioDTO>> GetHistorialAsync(int usuarioId);
}
