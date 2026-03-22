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

    // ── Métodos administrativos ──────────────────────────────────────────────

    public async Task<IEnumerable<Subasta>> GetAllAdmin()
    {
        return await _context.Subastas
            .Include(s => s.IdEstadoSubastaNavigation)
            .Include(s => s.IdGanadoNavigation)
                .ThenInclude(g => g.ImagenesGanado)
            .Include(s => s.IdUsuarioCreadorNavigation)
            .OrderByDescending(s => s.SubastaId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Ganado>> GetGanadosActivos()
    {
        // Ganados "ocupados": con subasta en estado Activa, Programada o Borrador
        var ganadosOcupados = await _context.Subastas
            .Where(s => s.IdEstadoSubastaNavigation.Nombre == "Activa"
                     || s.IdEstadoSubastaNavigation.Nombre == "Programada"
                     || s.IdEstadoSubastaNavigation.Nombre == "Borrador")
            .Select(s => s.GanadoId)
            .Distinct()
            .ToListAsync();

        return await _context.Ganados
            .Include(g => g.IdEstadoGanadoNavigation)
            .Where(g => g.IdEstadoGanadoNavigation.Nombre == "Activo"
                     && !ganadosOcupados.Contains(g.GanadoId))
            .OrderBy(g => g.Nombre)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> CambiarEstado(int subastaId, int nuevoEstadoId)
    {
        try
        {
            var subasta = await _context.Subastas.FindAsync(subastaId);
            if (subasta == null) return false;

            subasta.EstadoSubastaId = nuevoEstadoId;
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> TienePujas(int subastaId)
    {
        return await _context.Pujas.AnyAsync(p => p.SubastaId == subastaId);
    }

    public async Task<bool> GanadoTieneSubastaActiva(int ganadoId, int? excluirSubastaId = null)
    {
        return await _context.Subastas
            .Where(s => s.GanadoId == ganadoId
                     && (s.IdEstadoSubastaNavigation.Nombre == "Activa"
                      || s.IdEstadoSubastaNavigation.Nombre == "Programada"
                      || s.IdEstadoSubastaNavigation.Nombre == "Borrador")
                     && (excluirSubastaId == null || s.SubastaId != excluirSubastaId.Value))
            .AnyAsync();
    }

    public async Task<bool> GanadoEstaActivo(int ganadoId)
    {
        return await _context.Ganados
            .Where(g => g.GanadoId == ganadoId
                     && g.IdEstadoGanadoNavigation.Nombre == "Activo")
            .AnyAsync();
    }

    public async Task<int?> GetEstadoIdByNombre(string nombre)
    {
        var estado = await _context.EstadosSubasta
            .FirstOrDefaultAsync(e => e.Nombre == nombre);
        return estado?.EstadoSubastaId;
    }

    public async Task ActualizarEstadosAsync()
    {
        var ahora = DateTime.Now;

        var idProgramada = await GetEstadoIdByNombre("Programada");
        var idActiva     = await GetEstadoIdByNombre("Activa");
        var idFinalizada = await GetEstadoIdByNombre("Finalizada");

        if (idProgramada == null || idActiva == null || idFinalizada == null) return;

        // Inactivo en EstadoGanado
        var idInactivoGanado = await _context.EstadosGanado
            .Where(e => e.Nombre == "Inactivo")
            .Select(e => e.EstadoGanadoId)
            .FirstOrDefaultAsync();

        // Programada → Activa
        var aActivar = await _context.Subastas
            .Where(s => s.EstadoSubastaId == idProgramada && s.FechaInicio <= ahora)
            .ToListAsync();
        foreach (var s in aActivar)
            s.EstadoSubastaId = idActiva.Value;

        // Activa → Finalizada + desactivar ganado
        var aFinalizar = await _context.Subastas
            .Include(s => s.IdGanadoNavigation)
            .Where(s => s.EstadoSubastaId == idActiva && s.FechaFin <= ahora)
            .ToListAsync();
        foreach (var s in aFinalizar)
        {
            s.EstadoSubastaId = idFinalizada.Value;
            if (s.IdGanadoNavigation != null)
                s.IdGanadoNavigation.EstadoGanadoId = idInactivoGanado;
        }

        if (aActivar.Any() || aFinalizar.Any())
            await _context.SaveChangesAsync();
    }
}
