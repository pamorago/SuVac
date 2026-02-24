using System.ComponentModel.DataAnnotations;

namespace SuVac.Application.DTOs;

public class PujaDTO
{
    [Display(Name = "Id Puja")]
    public int PujaId { get; set; }

    [Display(Name = "Subasta")]
    public int SubastaId { get; set; }

    [Display(Name = "Id Usuario")]
    public int UsuarioId { get; set; }

    /// <summary>Nombre completo del pujador. Calculado en el servicio.</summary>
    [Display(Name = "Pujador")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Display(Name = "Monto Ofertado")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    public decimal Monto { get; set; }

    [Display(Name = "Fecha y Hora")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
    public DateTime FechaHora { get; set; }
}
