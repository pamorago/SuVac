namespace SuVac.Application.DTOs;

public class PagoDTO
{
    public int PagoId { get; set; }
    public int SubastaId { get; set; }
    public int UsuarioId { get; set; }
    public decimal Monto { get; set; }
    public int EstadoPagoId { get; set; }
    public DateTime? FechaPago { get; set; }
}
