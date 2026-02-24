using System;

namespace SuVac.Infraestructure.Models;

public partial class ResultadoSubasta
{
    public int ResultadoId { get; set; }

    public int SubastaId { get; set; }

    public int UsuarioGanadorId { get; set; }

    public decimal MontoFinal { get; set; }

    public DateTime FechaCierre { get; set; }

    public virtual Subasta IdSubastaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioGanadorNavigation { get; set; } = null!;
}
