using SuVac.Infraestructure.Data;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SuVac.Infraestructure.Repository.Implementations;

public class RepositoryTipoGanado : IRepositoryTipoGanado
{
    private readonly SuVacContext _context;

    public RepositoryTipoGanado(SuVacContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TipoGanado>> GetAll()
    {
        return await _context.TiposGanado.ToListAsync();
    }

    public async Task<TipoGanado?> GetById(int id)
    {
        return await _context.TiposGanado.FirstOrDefaultAsync(t => t.TipoGanadoId == id);
    }

    public async Task<bool> Create(TipoGanado entity)
    {
        try
        {
            _context.TiposGanado.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Update(TipoGanado entity)
    {
        try
        {
            _context.TiposGanado.Update(entity);
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
            var tipoGanado = await _context.TiposGanado.FindAsync(id);
            if (tipoGanado == null) return false;

            _context.TiposGanado.Remove(tipoGanado);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
