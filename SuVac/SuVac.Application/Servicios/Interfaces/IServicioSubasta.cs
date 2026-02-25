using SuVac.Application.DTOs;

namespace SuVac.Application.Servicios.Interfaces;

public interface IServicioSubasta
{
    /// <summary>Retorna el listado de subastas con estado Activa.</summary>
    Task<ICollection<SubastaListadoDTO>> ListarActivasAsync();

    /// <summary>Retorna el listado de subastas Finalizadas o Canceladas.</summary>
    Task<ICollection<SubastaListadoDTO>> ListarFinalizadasAsync();

    /// <summary>Retorna el detalle completo de una subasta por su Id.</summary>
    Task<SubastaDetalleDTO?> BuscarPorIdAsync(int id);
}
