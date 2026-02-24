using System;
using System.Collections.Generic;

namespace SuVac.Infraestructure.Models;

public partial class Orden
{
    public int IdOrden { get; set; }

    public string IdCliente { get; set; } = null!;

    public int IdUsuario { get; set; }

    public DateTime FechaOrden { get; set; }

    public decimal Total { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<OrdenDetalle> OrdenDetalle { get; set; } = new List<OrdenDetalle>();
}
