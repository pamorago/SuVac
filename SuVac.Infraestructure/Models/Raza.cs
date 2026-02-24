using System;
using System.Collections.Generic;

namespace SuVac.Infraestructure.Models;

public partial class Raza
{
    public int RazaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Ganado> Ganados { get; set; } = new List<Ganado>();
}
