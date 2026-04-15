using SuVac.Application.DTOs;

namespace SuVac.Web.Models;

/// <summary>
/// ViewModel para la sala de pujas en tiempo real.
/// Contiene toda la información necesaria para renderizar la página inicial.
/// Las actualizaciones posteriores llegan por SignalR.
/// </summary>
public class SalaSubastaViewModel
{
    /// <summary>Información completa de la subasta y el ganado.</summary>
    public SubastaDetalleDTO Subasta { get; set; } = null!;

    /// <summary>Puja con el monto más alto en este momento. Null si no hay pujas.</summary>
    public PujaDTO? PujaLider { get; set; }

    /// <summary>Historial de pujas ordenado por fecha descendente.</summary>
    public List<PujaDTO> Historial { get; set; } = new();

    /// <summary>ID del usuario que está viendo la sala (usuario simulado).</summary>
    public int UsuarioActualId { get; set; }

    /// <summary>Nombre completo del usuario que está viendo la sala.</summary>
    public string NombreUsuarioActual { get; set; } = string.Empty;

    /// <summary>True si el usuario actual es el vendedor de esta subasta.</summary>
    public bool EsVendedor { get; set; }
}
