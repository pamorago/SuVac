using System;
using System.Collections.Generic;

namespace SuVac.Infraestructure.Models;

public partial class OrdenDetalle
{
    public int IdOrden { get; set; }

    public int IdLibro { get; set; }

    public int Cantidad { get; set; }

    public decimal Precio { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Libro IdLibroNavigation { get; set; } = null!;

    public virtual Orden IdOrdenNavigation { get; set; } = null!;
}
