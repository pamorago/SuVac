using AutoMapper;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;

namespace SuVac.Application.Services.Implementations;

public class ServiceUsuario : IServiceUsuario
{
    private readonly IRepositoryUsuario _repository;
    private readonly IMapper _mapper;

    public ServiceUsuario(IRepositoryUsuario repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UsuarioDTO>> GetAll()
    {
        var usuarios = await _repository.GetAll();
        return _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);
    }

    public async Task<UsuarioDTO> GetById(int id)
    {
        var usuario = await _repository.GetById(id);
        return _mapper.Map<UsuarioDTO>(usuario);
    }

    public async Task<IEnumerable<UsuarioDTO>> GetAllConDetalle()
    {
        var usuarios = await _repository.GetAllFull();
        return usuarios.Select(u => MapearUsuarioConDetalle(u));
    }

    public async Task<UsuarioDTO?> GetByIdConDetalle(int id)
    {
        var usuario = await _repository.GetByIdFull(id);
        if (usuario is null) return null;
        return MapearUsuarioConDetalle(usuario);
    }

    public async Task<bool> Create(UsuarioDTO dto)
    {
        var usuario = _mapper.Map<Usuario>(dto);
        return await _repository.Create(usuario);
    }

    public async Task<bool> Update(UsuarioDTO dto)
    {
        var usuario = _mapper.Map<Usuario>(dto);
        return await _repository.Update(usuario);
    }

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }

    private UsuarioDTO MapearUsuarioConDetalle(Usuario usuario)
    {
        return new UsuarioDTO
        {
            UsuarioId = usuario.UsuarioId,
            Correo = usuario.Correo,
            NombreCompleto = usuario.NombreCompleto,
            RolId = usuario.RolId,
            NombreRol = usuario.IdRolNavigation?.Nombre,
            EstadoUsuarioId = usuario.EstadoUsuarioId,
            NombreEstado = usuario.IdEstadoNavigation?.Nombre,
            FechaRegistro = usuario.FechaRegistro,
            CantidadSubastasCreadas = usuario.Subastas?.Count ?? 0,
            CantidadPujasRealizadas = usuario.Pujas?.Count ?? 0
        };
    }
}
