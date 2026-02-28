using SuVac.Infraestructure.Data;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SuVac.Infraestructure.Repository.Implementations;

public class RepositoryGanado : IRepositoryGanado
{
    private readonly SuVacContext _context;

    public RepositoryGanado(SuVacContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ganado>> GetAll()
    {
        return await _context.Ganados
            .Include(g => g.IdTipoGanadoNavigation)
            .Include(g => g.IdRazaNavigation)
            .Include(g => g.IdSexoNavigation)
            .Include(g => g.IdEstadoGanadoNavigation)
            .Include(g => g.IdUsuarioVendedorNavigation)
            .Include(g => g.ImagenesGanado)
            .Include(g => g.GanadoCategorias)
                .ThenInclude(gc => gc.IdCategoriaNavigation)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Ganado?> GetById(int id)
    {
        return await _context.Ganados
            .Include(g => g.IdTipoGanadoNavigation)
            .Include(g => g.IdRazaNavigation)
            .Include(g => g.IdSexoNavigation)
            .Include(g => g.IdEstadoGanadoNavigation)
            .Include(g => g.IdUsuarioVendedorNavigation)
            .Include(g => g.ImagenesGanado)
            .Include(g => g.GanadoCategorias)
                .ThenInclude(gc => gc.IdCategoriaNavigation)
            .Include(g => g.Subastas)
                .ThenInclude(s => s.IdEstadoSubastaNavigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.GanadoId == id);
    }

    public async Task<bool> Create(Ganado entity)
    {
        try
        {
            _context.Ganados.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Update(Ganado entity)
    {
        try
        {
            _context.Ganados.Update(entity);
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
            var ganado = await _context.Ganados.FindAsync(id);
            if (ganado == null) return false;

            _context.Ganados.Remove(ganado);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
