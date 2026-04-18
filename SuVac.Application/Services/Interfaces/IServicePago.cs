using SuVac.Application.DTOs;

namespace SuVac.Application.Services.Interfaces;

public interface IServicePago
{
    Task<IEnumerable<PagoDTO>> GetAll();
    Task<PagoDTO?> GetById(int id);
    Task<bool> Create(PagoDTO dto);
    Task<bool> Update(PagoDTO dto);
    Task<bool> Delete(int id);

    /// <summary>Todos los pagos con datos de subasta, usuario y estado.</summary>
    Task<IEnumerable<PagoDTO>> GetAllConDetalle();

    /// <summary>Un pago con todos los datos para la vista de detalle.</summary>
    Task<PagoDTO?> GetByIdConDetalle(int id);

    /// <summary>Pago asociado a una subasta, si existe.</summary>
    Task<PagoDTO?> GetBySubastaId(int subastaId);

    /// <summary>Registra automáticamente el pago del ganador al cerrar una subasta.</summary>
    Task<(bool ok, string mensaje)> RegistrarPagoGanador(int subastaId, int usuarioGanadorId, decimal montoFinal);

    /// <summary>Confirma el pago (Pendiente → Confirmado).</summary>
    Task<(bool ok, string mensaje)> ConfirmarPago(int pagoId);
}
