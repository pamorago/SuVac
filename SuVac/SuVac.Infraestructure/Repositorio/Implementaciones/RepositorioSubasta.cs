using Microsoft.EntityFrameworkCore;
using SuVac.Infraestructure.Datos;
using SuVac.Infraestructure.Modelos;
using SuVac.Infraestructure.Repositorio.Interfaces;

namespace SuVac.Infraestructure.Repositorio.Implementaciones;

public class RepositorioSubasta : IRepositorioSubasta
{
    private readonly SuVacContexto _contexto;

    public RepositorioSubasta(SuVacContexto contexto)
    {
        _contexto = contexto;
    }

    public async Task<ICollection<Subasta>> ListarActivasAsync()
    {
        return await _contexto.Subasta
            .Include(s => s.EstadoSubastaNavigation)
            .Include(s => s.GanadoNavigation)
                .ThenInclude(g => g.Imagenes)
            .Include(s => s.Pujas)
            .Where(s => s.EstadoSubastaNavigation.Nombre == "Activa")
            .OrderBy(s => s.FechaFin)
            .ToListAsync();
    }

    public async Task<ICollection<Subasta>> ListarFinalizadasAsync()
    {
        return await _contexto.Subasta
            .Include(s => s.EstadoSubastaNavigation)
            .Include(s => s.GanadoNavigation)
                .ThenInclude(g => g.Imagenes)
            .Include(s => s.Pujas)
            .Where(s => s.EstadoSubastaNavigation.Nombre == "Finalizada"
                     || s.EstadoSubastaNavigation.Nombre == "Cancelada")
            .OrderByDescending(s => s.FechaFin)
            .ToListAsync();
    }

    public async Task<Subasta?> BuscarPorIdAsync(int id)
    {
        return await _contexto.Subasta
            .Include(s => s.EstadoSubastaNavigation)
            .Include(s => s.UsuarioCreadorNavigation)
                .ThenInclude(u => u.RolNavigation)
            .Include(s => s.GanadoNavigation)
                .ThenInclude(g => g.Imagenes)
            .Include(s => s.GanadoNavigation)
                .ThenInclude(g => g.Categorias)
            .Include(s => s.GanadoNavigation)
                .ThenInclude(g => g.TipoGanadoNavigation)
            .Include(s => s.GanadoNavigation)
                .ThenInclude(g => g.EstadoGanadoNavigation)
            .Include(s => s.Pujas)
            .FirstOrDefaultAsync(s => s.SubastaId == id);
    }
}
