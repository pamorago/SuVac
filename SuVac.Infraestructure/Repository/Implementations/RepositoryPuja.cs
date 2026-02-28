using SuVac.Infraestructure.Data;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SuVac.Infraestructure.Repository.Implementations;

public class RepositoryPuja : IRepositoryPuja
{
    private readonly SuVacContext _context;

    public RepositoryPuja(SuVacContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Puja>> GetAll()
    {
        return await _context.Pujas
            .Include(p => p.IdUsuarioNavigation)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Puja?> GetById(int id)
    {
        return await _context.Pujas
            .Include(p => p.IdUsuarioNavigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PujaId == id);
    }

    public async Task<bool> Create(Puja entity)
    {
        try
        {
            _context.Pujas.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Update(Puja entity)
    {
        try
        {
            _context.Pujas.Update(entity);
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
            var puja = await _context.Pujas.FindAsync(id);
            if (puja == null) return false;

            _context.Pujas.Remove(puja);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<Puja>> GetBySubasta(int subastaId)
    {
        return await _context.Pujas
            .Include(p => p.IdUsuarioNavigation)
            .Where(p => p.SubastaId == subastaId)
            .OrderBy(p => p.FechaHora)
            .AsNoTracking()
            .ToListAsync();
    }
}
