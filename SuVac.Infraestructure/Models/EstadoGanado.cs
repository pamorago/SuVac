using System;
using System.Collections.Generic;

namespace SuVac.Infraestructure.Models;

public partial class EstadoGanado
{
    public int EstadoGanadoId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Ganado> Ganados { get; set; } = new List<Ganado>();
}
