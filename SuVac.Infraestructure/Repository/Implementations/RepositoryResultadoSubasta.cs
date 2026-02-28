using SuVac.Infraestructure.Data;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SuVac.Infraestructure.Repository.Implementations;

public class RepositoryResultadoSubasta : IRepositoryResultadoSubasta
{
    private readonly SuVacContext _context;

    public RepositoryResultadoSubasta(SuVacContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ResultadoSubasta>> GetAll()
    {
        return await _context.ResultadosSubasta
            .Include(r => r.IdSubastaNavigation)
            .Include(r => r.IdUsuarioGanadorNavigation)
            .ToListAsync();
    }

    public async Task<ResultadoSubasta?> GetById(int id)
    {
        return await _context.ResultadosSubasta
            .Include(r => r.IdSubastaNavigation)
            .Include(r => r.IdUsuarioGanadorNavigation)
            .FirstOrDefaultAsync(r => r.ResultadoId == id);
    }

    public async Task<bool> Create(ResultadoSubasta entity)
    {
        try
        {
            _context.ResultadosSubasta.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Update(ResultadoSubasta entity)
    {
        try
        {
            _context.ResultadosSubasta.Update(entity);
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
            var resultado = await _context.ResultadosSubasta.FindAsync(id);
            if (resultado == null) return false;

            _context.ResultadosSubasta.Remove(resultado);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<ResultadoSubasta?> GetBySubastaId(int subastaId)
    {
        return await _context.ResultadosSubasta
            .Include(r => r.IdSubastaNavigation)
            .Include(r => r.IdUsuarioGanadorNavigation)
            .FirstOrDefaultAsync(r => r.SubastaId == subastaId);
    }
}
