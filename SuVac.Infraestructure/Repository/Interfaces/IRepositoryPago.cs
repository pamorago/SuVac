using SuVac.Infraestructure.Models;

namespace SuVac.Infraestructure.Repository.Interfaces;

public interface IRepositoryPago
{
    Task<IEnumerable<Pago>> GetAll();
    Task<Pago?> GetById(int id);
    Task<bool> Create(Pago entity);
    Task<bool> Update(Pago entity);
    Task<bool> Delete(int id);

    /// <summary>Todos los pagos con navegaciones para listado.</summary>
    Task<IEnumerable<Pago>> GetAllConDetalle();

    /// <summary>Un pago con todas las navegaciones para detalle.</summary>
    Task<Pago?> GetByIdConDetalle(int id);

    /// <summary>Pago asociado a una subasta, si existe.</summary>
    Task<Pago?> GetBySubastaId(int subastaId);

    /// <summary>Cambia el estado del pago a Confirmado.</summary>
    Task<bool> ConfirmarPago(int pagoId);

    /// <summary>Indica si ya existe un pago para la subasta dada.</summary>
    Task<bool> ExistePagoParaSubasta(int subastaId);

    /// <summary>Obtiene el EstadoPagoId por nombre.</summary>
    Task<int?> GetEstadoIdByNombre(string nombre);
}
