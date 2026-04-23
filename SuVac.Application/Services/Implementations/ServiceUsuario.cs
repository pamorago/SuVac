using AutoMapper;
using Microsoft.Extensions.Options;
using SuVac.Application.Config;
using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Application.Utils;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;

namespace SuVac.Application.Services.Implementations;

public class ServiceUsuario : IServiceUsuario
{
    private readonly IRepositoryUsuario _repository;
    private readonly IMapper _mapper;
    private readonly IOptions<AppConfig> _options;

    public ServiceUsuario(IRepositoryUsuario repository, IMapper mapper, IOptions<AppConfig> options)
    {
        _repository = repository;
        _mapper = mapper;
        _options = options;
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
        var listado = new List<UsuarioDTO>();

        foreach (var u in usuarios)
        {
            // Campos calculados mediante LINQ CountAsync (no almacenados en BD)
            var cantSubastas = await _repository.CountSubastasAsync(u.UsuarioId);
            var cantPujas = await _repository.CountPujasAsync(u.UsuarioId);
            listado.Add(MapearUsuario(u, cantSubastas, cantPujas));
        }

        return listado;
    }

    public async Task<UsuarioDTO?> GetByIdConDetalle(int id)
    {
        var usuario = await _repository.GetByIdFull(id);
        if (usuario is null) return null;

        // Campos calculados mediante LINQ CountAsync (no almacenados en BD)
        var cantSubastas = await _repository.CountSubastasAsync(id);
        var cantPujas = await _repository.CountPujasAsync(id);

        return MapearUsuario(usuario, cantSubastas, cantPujas);
    }

    public async Task<bool> Create(UsuarioDTO dto)
    {
        // Cifrar contraseña con AES antes de persistir
        if (!string.IsNullOrWhiteSpace(dto.Contrasena))
            dto.Contrasena = Cryptography.Encrypt(dto.Contrasena, _options.Value.Crypto.Secret);

        var usuario = _mapper.Map<Usuario>(dto);
        return await _repository.Create(usuario);
    }

    public async Task<bool> Update(UsuarioDTO dto)
    {
        var usuario = _mapper.Map<Usuario>(dto);

        // Preserve existing password if the form left it blank
        if (string.IsNullOrWhiteSpace(dto.Contrasena))
        {
            var existing = await _repository.GetById(dto.UsuarioId);
            if (existing != null)
                usuario.PasswordHash = existing.PasswordHash;
        }

        return await _repository.Update(usuario);
    }

    public async Task<bool> UpdatePerfil(int id, string nombreCompleto, string correo)
        => await _repository.UpdatePerfil(id, nombreCompleto, correo);

    public async Task<bool> ToggleEstado(int id)
        => await _repository.ToggleEstado(id);

    public async Task<bool> Delete(int id)
    {
        return await _repository.Delete(id);
    }

    public async Task<Usuario?> LoginAsync(string correo, string password)
    {
        var encrypted = Cryptography.Encrypt(password, _options.Value.Crypto.Secret);
        return await _repository.GetByCorreoYPassword(correo, encrypted);
    }

    public async Task<IEnumerable<HistorialUsuarioDTO>> GetHistorialAsync(int usuarioId)
    {
        var items = await _repository.GetHistorialAsync(usuarioId);
        return items.Select(x => new HistorialUsuarioDTO
        {
            SubastaId = x.subasta.SubastaId,
            NombreGanado = x.subasta.IdGanadoNavigation?.Nombre ?? $"Ganado #{x.subasta.GanadoId}",
            FechaInicio = x.subasta.FechaInicio,
            FechaFin = x.subasta.FechaFin,
            EstadoSubasta = x.subasta.IdEstadoSubastaNavigation?.Nombre ?? "—",
            PrecioBase = x.subasta.PrecioBase,
            RolEnSubasta = x.rolEnSubasta,
            MejorPuja = x.mejorPuja,
            EsGanador = x.esGanador
        });
    }

    /// <summary>
    /// Mapea un Usuario a UsuarioDTO con campos calculados recibidos como parámetros.
    /// Los conteos provienen de consultas LINQ CountAsync ejecutadas contra la BD.
    /// </summary>
    private static UsuarioDTO MapearUsuario(Usuario usuario, int cantSubastas, int cantPujas)
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
            CantidadSubastasCreadas = cantSubastas,
            CantidadPujasRealizadas = cantPujas
        };
    }
}
