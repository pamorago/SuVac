namespace SuVac.Infraestructure.Modelos;

public class EstadoPago
{
    public int EstadoPagoId { get; set; }
    public string Nombre { get; set; } = null!;

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
