using SuVac.Application.DTOs;

namespace SuVac.Application.Services.Interfaces;

public interface IServiceSubasta
{
    Task<IEnumerable<SubastaDTO>> GetAll();
    Task<SubastaDTO?> GetById(int id);
    Task<bool> Create(SubastaDTO dto);
    Task<bool> Update(SubastaDTO dto);
    Task<bool> Delete(int id);

    Task<IEnumerable<SubastaListadoDTO>> GetActivas();
    Task<IEnumerable<SubastaListadoDTO>> GetFinalizadas();
    Task<SubastaDetalleDTO?> GetDetalle(int id);

    // ── Métodos administrativos ───────────────────────────────────────────

    /// <summary>Listado admin con todos los estados.</summary>
    Task<IEnumerable<SubastaListadoDTO>> GetAllAdmin();

    /// <summary>Ganados activos sin subasta activa para el dropdown de crear.</summary>
    Task<IEnumerable<GanadoDTO>> GetGanadosActivos();

    /// <summary>Crea una subasta con validaciones de negocio. Estado inicial: Borrador.</summary>
    Task<(bool ok, string mensaje, int subastaId)> CreateValidado(SubastaDTO dto);

    /// <summary>Edita fechas y precios si no ha iniciado y no tiene pujas.</summary>
    Task<(bool ok, string mensaje)> UpdateValidado(SubastaDTO dto);

    /// <summary>Cambia estado Borrador → Programada.</summary>
    Task<(bool ok, string mensaje)> Publicar(int subastaId);

    /// <summary>Cambia estado a Cancelada si no ha iniciado o no tiene pujas.</summary>
    Task<(bool ok, string mensaje)> Cancelar(int subastaId);

    /// <summary>Actualiza automáticamente estados: Programada→Activa y Activa→Finalizada según fechas.</summary>
    Task ActualizarEstadosAsync();
}
