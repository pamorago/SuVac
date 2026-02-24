using System;

namespace SuVac.Infraestructure.Models;

public partial class Pago
{
    public int PagoId { get; set; }

    public int SubastaId { get; set; }

    public int UsuarioId { get; set; }

    public decimal Monto { get; set; }

    public int EstadoPagoId { get; set; }

    public DateTime? FechaPago { get; set; }

    public virtual Subasta IdSubastaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual EstadoPago IdEstadoPagoNavigation { get; set; } = null!;
}
