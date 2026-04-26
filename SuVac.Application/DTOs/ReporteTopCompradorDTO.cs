namespace SuVac.Application.DTOs;

public class ReporteTopCompradorDTO
{
    public int UsuarioId { get; set; }
    public string Nombre { get; set; } = default!;
    public int TotalPujas { get; set; }
    public decimal MontoMaximo { get; set; }
    public decimal MontoPromedio { get; set; }
    public int SubastasGanadas { get; set; }
}
