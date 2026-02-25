using System.ComponentModel.DataAnnotations;

namespace SuVac.Application.DTOs;

/// <summary>DTO para mostrar en el listado de subastas activas y finalizadas.</summary>
public record SubastaListadoDTO
{
    [Display(Name = "Id Subasta")]
    public int SubastaId { get; set; }

    [Display(Name = "Nombre del Ganado")]
    public string NombreGanado { get; set; } = string.Empty;

    [Display(Name = "Imagen")]
    public string? ImagenGanado { get; set; }

    [Display(Name = "Fecha de Inicio")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime FechaInicio { get; set; }

    [Display(Name = "Fecha de Cierre")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime FechaFin { get; set; }

    [Display(Name = "Precio Base")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal PrecioBase { get; set; }

    [Display(Name = "Incremento Mínimo")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal IncrementoMinimo { get; set; }

    [Display(Name = "Estado")]
    public string EstadoSubasta { get; set; } = string.Empty;

    /// <summary>Campo calculado mediante LINQ: cantidad de pujas registradas para la subasta.</summary>
    [Display(Name = "Cantidad de Pujas")]
    public int CantidadPujas { get; set; }
}
