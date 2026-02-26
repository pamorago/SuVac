using SuVac.Infraestructure.Data;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SuVac.Infraestructure.Repository.Implementations;

public class RepositoryRaza : IRepositoryRaza
{
    private readonly SuVacContext _context;

    public RepositoryRaza(SuVacContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Raza>> GetAll()
    {
        return await _context.Razas.ToListAsync();
    }

    public async Task<Raza> GetById(int id)
    {
        return await _context.Razas.FirstOrDefaultAsync(r => r.RazaId == id);
    }

    public async Task<bool> Create(Raza entity)
    {
        try
        {
            _context.Razas.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Update(Raza entity)
    {
        try
        {
            _context.Razas.Update(entity);
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
            var raza = await _context.Razas.FindAsync(id);
            if (raza == null) return false;

            _context.Razas.Remove(raza);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
