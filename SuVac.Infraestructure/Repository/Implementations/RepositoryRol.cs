using SuVac.Infraestructure.Data;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SuVac.Infraestructure.Repository.Implementations;

public class RepositoryRol : IRepositoryRol
{
    private readonly SuVacContext _context;

    public RepositoryRol(SuVacContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Rol>> GetAll()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<Rol> GetById(int id)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.RolId == id);
    }

    public async Task<bool> Create(Rol entity)
    {
        try
        {
            _context.Roles.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Update(Rol entity)
    {
        try
        {
            _context.Roles.Update(entity);
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
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return false;

            _context.Roles.Remove(rol);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
