using SuVac.Infraestructure.Data;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SuVac.Infraestructure.Repository.Implementations;

public class RepositorySubasta : IRepositorySubasta
{
    private readonly SuVacContext _context;

    public RepositorySubasta(SuVacContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Subasta>> GetAll()
    {
        return await _context.Subastas.ToListAsync();
    }

    public async Task<Subasta> GetById(int id)
    {
        return await _context.Subastas.FindAsync(id);
    }

    public async Task<bool> Create(Subasta entity)
    {
        try
        {
            _context.Subastas.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Update(Subasta entity)
    {
        try
        {
            _context.Subastas.Update(entity);
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
            var subasta = await _context.Subastas.FindAsync(id);
            if (subasta == null) return false;
            
            _context.Subastas.Remove(subasta);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
