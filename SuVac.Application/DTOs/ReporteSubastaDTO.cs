namespace SuVac.Application.DTOs;

public class ReporteSubastaDTO
{
    public int SubastaId { get; set; }
    public string NombreGanado { get; set; } = default!;
    public string NombreCreador { get; set; } = default!;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal PrecioBase { get; set; }
    public string EstadoSubasta { get; set; } = default!;
}
