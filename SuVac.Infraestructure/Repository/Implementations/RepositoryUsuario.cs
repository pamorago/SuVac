using SuVac.Infraestructure.Data;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SuVac.Infraestructure.Repository.Implementations;

public class RepositoryUsuario : IRepositoryUsuario
{
    private readonly SuVacContext _context;

    public RepositoryUsuario(SuVacContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Usuario>> GetAll()
    {
        return await _context.Usuarios
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Usuario?> GetById(int id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<IEnumerable<Usuario>> GetAllFull()
    {
        // Solo Include de navegaciones necesarias para mostrar (Rol, Estado).
        // Los conteos de subastas/pujas se calculan aparte con LINQ CountAsync.
        return await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .Include(u => u.IdEstadoNavigation)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Usuario?> GetByIdFull(int id)
    {
        // Solo Include de navegaciones necesarias para mostrar (Rol, Estado).
        // Los conteos de subastas/pujas se calculan aparte con LINQ CountAsync.
        return await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .Include(u => u.IdEstadoNavigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UsuarioId == id);
    }

    /// <inheritdoc />
    public async Task<int> CountSubastasAsync(int usuarioId)
    {
        // LINQ sobre EF Core: genera SELECT COUNT(*) FROM Subasta WHERE UsuarioCreadorId = @p
        return await _context.Subastas
            .CountAsync(s => s.UsuarioCreadorId == usuarioId);
    }

    /// <inheritdoc />
    public async Task<int> CountPujasAsync(int usuarioId)
    {
        // LINQ sobre EF Core: genera SELECT COUNT(*) FROM Puja WHERE UsuarioId = @p
        return await _context.Pujas
            .CountAsync(p => p.UsuarioId == usuarioId);
    }

    public async Task<bool> Create(Usuario entity)
    {
        try
        {
            _context.Usuarios.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Update(Usuario entity)
    {
        try
        {
            _context.Usuarios.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Delete(int id)
    {
        try
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdatePerfil(int id, string nombreCompleto, string correo)
    {
        try
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;
            usuario.NombreCompleto = nombreCompleto;
            usuario.Correo = correo;
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ToggleEstado(int id)
    {
        try
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;
            usuario.EstadoUsuarioId = usuario.EstadoUsuarioId == 1 ? 2 : 1;
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Usuario?> GetByCorreoYPassword(string correo, string passwordEncriptado)
    {
        return await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .Include(u => u.IdEstadoNavigation)
            .Where(u => u.Correo == correo
                     && u.PasswordHash == passwordEncriptado
                     && u.IdEstadoNavigation.Nombre == "Activo")
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<(Subasta subasta, string rolEnSubasta, decimal? mejorPuja, bool esGanador)>> GetHistorialAsync(int usuarioId)
    {
        var resultado = new List<(Subasta, string, decimal?, bool)>();

        // Subastas creadas por el usuario (rol vendedor)
        var creadas = await _context.Subastas
            .AsNoTracking()
            .Include(s => s.IdGanadoNavigation)
            .Include(s => s.IdEstadoSubastaNavigation)
            .Where(s => s.UsuarioCreadorId == usuarioId)
            .ToListAsync();

        foreach (var s in creadas)
            resultado.Add((s, "Vendedor", null, false));

        // Subastas donde pujó el usuario (rol comprador)
        var subastasPujadas = await _context.Pujas
            .AsNoTracking()
            .Where(p => p.UsuarioId == usuarioId)
            .Select(p => p.SubastaId)
            .Distinct()
            .ToListAsync();

        foreach (var sid in subastasPujadas)
        {
            // Evitar duplicar si también es creador
            if (creadas.Any(s => s.SubastaId == sid)) continue;

            var subasta = await _context.Subastas
                .AsNoTracking()
                .Include(s => s.IdGanadoNavigation)
                .Include(s => s.IdEstadoSubastaNavigation)
                .Include(s => s.ResultadoSubasta)
                .FirstOrDefaultAsync(s => s.SubastaId == sid);

            if (subasta is null) continue;

            var mejorPuja = await _context.Pujas
                .AsNoTracking()
                .Where(p => p.SubastaId == sid && p.UsuarioId == usuarioId)
                .MaxAsync(p => (decimal?)p.Monto);

            var esGanador = subasta.ResultadoSubasta
                .Any(r => r.UsuarioGanadorId == usuarioId);

            resultado.Add((subasta, "Comprador", mejorPuja, esGanador));
        }

        return resultado.OrderByDescending(x => x.Item1.FechaInicio);
    }
}
