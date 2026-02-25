using System.ComponentModel.DataAnnotations;

namespace SuVac.Application.DTOs;

public class PujaDTO
{
    public int PujaId { get; set; }
    public int SubastaId { get; set; }
    public int UsuarioId { get; set; }

    [Display(Name = "Usuario")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Display(Name = "Monto"), DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Monto { get; set; }

    [Display(Name = "Fecha y hora"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime FechaHora { get; set; }
    public string? NombreUsuario { get; set; }
}
