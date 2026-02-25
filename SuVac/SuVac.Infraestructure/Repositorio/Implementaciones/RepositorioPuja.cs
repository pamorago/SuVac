using Microsoft.EntityFrameworkCore;
using SuVac.Infraestructure.Datos;
using SuVac.Infraestructure.Modelos;
using SuVac.Infraestructure.Repositorio.Interfaces;

namespace SuVac.Infraestructure.Repositorio.Implementaciones;

public class RepositorioPuja : IRepositorioPuja
{
    private readonly SuVacContexto _contexto;

    public RepositorioPuja(SuVacContexto contexto)
    {
        _contexto = contexto;
    }

    public async Task<ICollection<Puja>> ListarPorSubastaAsync(int subastaId)
    {
        return await _contexto.Puja
            .Include(p => p.UsuarioNavigation)
            .Where(p => p.SubastaId == subastaId)
            .OrderBy(p => p.FechaHora)   // Orden cronológico ascendente
            .ToListAsync();
    }
}
