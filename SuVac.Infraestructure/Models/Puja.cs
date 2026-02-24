using System;

namespace SuVac.Infraestructure.Models;

public partial class Puja
{
    public int PujaId { get; set; }

    public int SubastaId { get; set; }

    public int UsuarioId { get; set; }

    public decimal Monto { get; set; }

    public DateTime FechaHora { get; set; }

    public virtual Subasta IdSubastaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
