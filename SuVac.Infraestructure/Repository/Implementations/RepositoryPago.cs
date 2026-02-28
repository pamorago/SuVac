using SuVac.Infraestructure.Data;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SuVac.Infraestructure.Repository.Implementations;

public class RepositoryPago : IRepositoryPago
{
    private readonly SuVacContext _context;

    public RepositoryPago(SuVacContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Pago>> GetAll()
    {
        return await _context.Pagos.ToListAsync();
    }

    public async Task<Pago?> GetById(int id)
    {
        return await _context.Pagos.FindAsync(id);
    }

    public async Task<bool> Create(Pago entity)
    {
        try
        {
            _context.Pagos.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Update(Pago entity)
    {
        try
        {
            _context.Pagos.Update(entity);
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
            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null) return false;

            _context.Pagos.Remove(pago);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
