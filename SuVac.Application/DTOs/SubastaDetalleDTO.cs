using System.ComponentModel.DataAnnotations;

namespace SuVac.Application.DTOs;

public record SubastaDetalleDTO
{
    [Display(Name = "Id Subasta")]
    public int SubastaId { get; set; }

    // ── Datos del ganado ──────────────────────────────────────────────
    [Display(Name = "Nombre del Ganado")]
    public string NombreGanado { get; set; } = string.Empty;

    [Display(Name = "Tipo de Ganado")]
    public string TipoGanado { get; set; } = string.Empty;

    [Display(Name = "Estado del Ganado")]
    public string EstadoGanado { get; set; } = string.Empty;

    [Display(Name = "Condición (Certificado de Salud)")]
    public string? CertificadoSalud { get; set; }

    [Display(Name = "Categorías")]
    public List<string> Categorias { get; set; } = new();

    [Display(Name = "Imágenes")]
    public List<string> ImagenesGanado { get; set; } = new();

    // ── Datos de la subasta ──────────────────────────────────────────
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

    [Display(Name = "Estado Actual")]
    public string EstadoSubasta { get; set; } = string.Empty;

    /// <summary>Campo calculado mediante LINQ: total de pujas en la subasta.</summary>
    [Display(Name = "Total de Pujas")]
    public int TotalPujas { get; set; }

    [Display(Name = "Creador")]
    public string NombreCreador { get; set; } = string.Empty;
}
