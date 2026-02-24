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
        return await _context.Usuarios.ToListAsync();
    }

    public async Task<Usuario> GetById(int id)
    {
        return await _context.Usuarios.FindAsync(id);
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
