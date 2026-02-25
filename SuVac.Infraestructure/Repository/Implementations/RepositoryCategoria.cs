using SuVac.Infraestructure.Data;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SuVac.Infraestructure.Repository.Implementations;

public class RepositoryCategoria : IRepositoryCategoria
{
    private readonly SuVacContext _context;

    public RepositoryCategoria(SuVacContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Categoria>> GetAll()
    {
        return await _context.Categorias.ToListAsync();
    }

    public async Task<Categoria> GetById(int id)
    {
        return await _context.Categorias.FirstOrDefaultAsync(c => c.CategoriaId == id);
    }

    public async Task<bool> Create(Categoria entity)
    {
        try
        {
            _context.Categorias.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Update(Categoria entity)
    {
        try
        {
            _context.Categorias.Update(entity);
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
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return false;

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
