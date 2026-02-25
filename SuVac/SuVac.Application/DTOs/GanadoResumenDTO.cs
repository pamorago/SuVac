using System.ComponentModel.DataAnnotations;

namespace SuVac.Application.DTOs;

public record GanadoResumenDTO
{
    [Display(Name = "Ganado")]
    public int GanadoId { get; set; }

    [Display(Name = "Nombre del Ganado")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Tipo")]
    public string TipoGanado { get; set; } = string.Empty;

    [Display(Name = "Estado")]
    public string EstadoGanado { get; set; } = string.Empty;

    [Display(Name = "Categorías")]
    public List<string> Categorias { get; set; } = new();

    [Display(Name = "Imagen Principal")]
    public string? ImagenPrincipal { get; set; }
}
