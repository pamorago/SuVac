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
}
