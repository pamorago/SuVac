namespace SuVac.Application.DTOs;

/// <summary>
/// Representa una entrada en el historial de actividad de un usuario:
/// ya sea una subasta que creó (como vendedor) o en la que pujó (como comprador).
/// </summary>
public class HistorialUsuarioDTO
{
    public int SubastaId { get; set; }
    public string NombreGanado { get; set; } = default!;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string EstadoSubasta { get; set; } = default!;
    public decimal PrecioBase { get; set; }

    /// <summary>"Vendedor" si creó la subasta, "Comprador" si pujó en ella.</summary>
    public string RolEnSubasta { get; set; } = default!;

    /// <summary>Monto de la puja más alta del usuario en esta subasta (null si es vendedor).</summary>
    public decimal? MejorPuja { get; set; }

    /// <summary>Indica si el usuario ganó la subasta (solo aplica para comprador).</summary>
    public bool EsGanador { get; set; }
}
