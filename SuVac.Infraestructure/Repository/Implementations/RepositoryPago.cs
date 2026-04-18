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

    public async Task<IEnumerable<Pago>> GetAllConDetalle()
    {
        return await _context.Pagos
            .Include(p => p.IdSubastaNavigation)
                .ThenInclude(s => s.IdGanadoNavigation)
            .Include(p => p.IdUsuarioNavigation)
            .Include(p => p.IdEstadoPagoNavigation)
            .OrderByDescending(p => p.FechaPago)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Pago?> GetByIdConDetalle(int id)
    {
        return await _context.Pagos
            .Include(p => p.IdSubastaNavigation)
                .ThenInclude(s => s.IdGanadoNavigation)
            .Include(p => p.IdUsuarioNavigation)
            .Include(p => p.IdEstadoPagoNavigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PagoId == id);
    }

    public async Task<Pago?> GetBySubastaId(int subastaId)
    {
        return await _context.Pagos
            .Include(p => p.IdSubastaNavigation)
                .ThenInclude(s => s.IdGanadoNavigation)
            .Include(p => p.IdUsuarioNavigation)
            .Include(p => p.IdEstadoPagoNavigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.SubastaId == subastaId);
    }

    public async Task<bool> ConfirmarPago(int pagoId)
    {
        var idConfirmado = await GetEstadoIdByNombre("Confirmado");
        if (idConfirmado == null) return false;

        var pago = await _context.Pagos.FindAsync(pagoId);
        if (pago == null) return false;

        pago.EstadoPagoId = idConfirmado.Value;
        pago.FechaPago = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistePagoParaSubasta(int subastaId)
    {
        return await _context.Pagos.AnyAsync(p => p.SubastaId == subastaId);
    }

    public async Task<int?> GetEstadoIdByNombre(string nombre)
    {
        var estado = await _context.EstadosPago
            .FirstOrDefaultAsync(e => e.Nombre == nombre);
        return estado?.EstadoPagoId;
    }
}
