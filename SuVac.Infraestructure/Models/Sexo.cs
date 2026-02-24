using System;
using System.Collections.Generic;

namespace SuVac.Infraestructure.Models;

public partial class Sexo
{
    public int SexoId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Ganado> Ganados { get; set; } = new List<Ganado>();
}
