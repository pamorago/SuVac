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
        return await _context.Subastas
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Subasta?> GetById(int id)
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

    public async Task<IEnumerable<Subasta>> GetActivas()
    {
        return await _context.Subastas
            .Include(s => s.IdEstadoSubastaNavigation)
            .Include(s => s.IdGanadoNavigation)
                .ThenInclude(g => g.ImagenesGanado)
            .Include(s => s.IdGanadoNavigation)
                .ThenInclude(g => g.GanadoCategorias)
                    .ThenInclude(gc => gc.IdCategoriaNavigation)
            .Include(s => s.IdUsuarioCreadorNavigation)
            .Where(s => s.IdEstadoSubastaNavigation.Nombre == "Activa")
            .OrderBy(s => s.FechaFin)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Subasta>> GetFinalizadas()
    {
        return await _context.Subastas
            .Include(s => s.IdEstadoSubastaNavigation)
            .Include(s => s.IdGanadoNavigation)
                .ThenInclude(g => g.ImagenesGanado)
            .Include(s => s.IdGanadoNavigation)
                .ThenInclude(g => g.GanadoCategorias)
                    .ThenInclude(gc => gc.IdCategoriaNavigation)
            .Include(s => s.IdUsuarioCreadorNavigation)
            .Where(s => s.IdEstadoSubastaNavigation.Nombre == "Finalizada" ||
                        s.IdEstadoSubastaNavigation.Nombre == "Cancelada")
            .OrderByDescending(s => s.FechaFin)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Subasta?> GetByIdFull(int id)
    {
        return await _context.Subastas
            .Include(s => s.IdEstadoSubastaNavigation)
            .Include(s => s.IdGanadoNavigation)
                .ThenInclude(g => g.ImagenesGanado)
            .Include(s => s.IdGanadoNavigation)
                .ThenInclude(g => g.GanadoCategorias)
                    .ThenInclude(gc => gc.IdCategoriaNavigation)
            .Include(s => s.IdGanadoNavigation)
                .ThenInclude(g => g.IdTipoGanadoNavigation)
            .Include(s => s.IdGanadoNavigation)
                .ThenInclude(g => g.IdEstadoGanadoNavigation)
            .Include(s => s.IdUsuarioCreadorNavigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SubastaId == id);
    }

    /// <inheritdoc />
    public async Task<int> CountPujasAsync(int subastaId)
    {
        // LINQ sobre EF Core: genera SELECT COUNT(*) FROM Puja WHERE SubastaId = @p
        return await _context.Pujas
            .CountAsync(p => p.SubastaId == subastaId);
    }
}
