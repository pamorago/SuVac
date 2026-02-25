namespace SuVac.Infraestructure.Modelos;

public class Pago
{
    public int PagoId { get; set; }
    public int SubastaId { get; set; }
    public int UsuarioId { get; set; }
    public decimal Monto { get; set; }
    public int EstadoPagoId { get; set; }
    public DateTime? FechaPago { get; set; }

    public virtual Subasta SubastaNavigation { get; set; } = null!;
    public virtual Usuario UsuarioNavigation { get; set; } = null!;
    public virtual EstadoPago EstadoPagoNavigation { get; set; } = null!;
}
