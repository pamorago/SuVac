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
            .Include(g => g.Subastas)
                .ThenInclude(s => s.IdEstadoSubastaNavigation)
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

    public async Task<bool> UpdateFull(Ganado entity, List<int> categoriasIds, List<string> imagenesUrls)
    {
        try
        {
            var tracked = await _context.Ganados.FindAsync(entity.GanadoId);
            if (tracked == null) return false;

            tracked.Nombre = entity.Nombre;
            tracked.Descripcion = entity.Descripcion;
            tracked.TipoGanadoId = entity.TipoGanadoId;
            tracked.RazaId = entity.RazaId;
            tracked.SexoId = entity.SexoId;
            tracked.FechaNacimiento = entity.FechaNacimiento;
            tracked.PesoKg = entity.PesoKg;
            tracked.CertificadoSalud = entity.CertificadoSalud;
            tracked.EstadoGanadoId = entity.EstadoGanadoId;
            // UsuarioVendedorId no se modifica (no editable)

            // Reemplazar imágenes
            var existingImages = await _context.ImagenesGanado
                .Where(i => i.GanadoId == entity.GanadoId).ToListAsync();
            _context.ImagenesGanado.RemoveRange(existingImages);
            foreach (var url in imagenesUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                _context.ImagenesGanado.Add(new ImagenGanado { GanadoId = entity.GanadoId, UrlImagen = url });

            // Reemplazar categorías
            var existingCats = await _context.GanadosCategorias
                .Where(gc => gc.GanadoId == entity.GanadoId).ToListAsync();
            _context.GanadosCategorias.RemoveRange(existingCats);
            foreach (var catId in categoriasIds)
                _context.GanadosCategorias.Add(new GanadoCategoria { GanadoId = entity.GanadoId, CategoriaId = catId });

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ToggleEstado(int id, int estadoId)
    {
        try
        {
            var ganado = await _context.Ganados.FindAsync(id);
            if (ganado == null) return false;
            ganado.EstadoGanadoId = estadoId;
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
