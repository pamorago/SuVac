namespace SuVac.Application.DTOs;

/// <summary>Resumen de una subasta para mostrar en el historial de participación de un ganado.</summary>
public class SubastaResumenDTO
{
    public int SubastaId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string EstadoSubasta { get; set; } = string.Empty;
}
