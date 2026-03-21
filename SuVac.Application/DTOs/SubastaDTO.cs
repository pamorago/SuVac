using System.ComponentModel.DataAnnotations;

namespace SuVac.Application.DTOs;

public class SubastaDTO
{
    public int SubastaId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un ganado.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un ganado válido.")]
    [Display(Name = "Ganado Subastable")]
    public int GanadoId { get; set; }

    /// <summary>Solo visualización (no viene del form binding).</summary>
    [Display(Name = "Ganado")]
    public string? NombreGanado { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es requerida.")]
    [Display(Name = "Fecha de Inicio")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de cierre es requerida.")]
    [Display(Name = "Fecha de Cierre")]
    public DateTime FechaFin { get; set; }

    [Required(ErrorMessage = "El precio base es requerido.")]
    [Range(0.01, 99999999, ErrorMessage = "El precio base debe ser mayor a ₡0.")]
    [Display(Name = "Precio Base (₡)")]
    public decimal PrecioBase { get; set; }

    [Required(ErrorMessage = "El incremento mínimo es requerido.")]
    [Range(0.01, 99999999, ErrorMessage = "El incremento mínimo debe ser mayor a ₡0.")]
    [Display(Name = "Incremento Mínimo (₡)")]
    public decimal IncrementoMinimo { get; set; }

    public int EstadoSubastaId { get; set; }

    /// <summary>Solo visualización.</summary>
    [Display(Name = "Estado")]
    public string? NombreEstadoSubasta { get; set; }

    public int UsuarioCreadorId { get; set; }

    /// <summary>Solo visualización.</summary>
    [Display(Name = "Vendedor / Creador")]
    public string? NombreCreador { get; set; }
}
