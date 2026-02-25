namespace SuVac.Application.DTOs;

public class PujaDTO
{
    public int PujaId { get; set; }
    public int SubastaId { get; set; }
    public int UsuarioId { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaHora { get; set; }
}
