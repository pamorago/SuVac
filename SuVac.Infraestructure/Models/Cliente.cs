using System;
using System.Collections.Generic;

namespace SuVac.Infraestructure.Models;

public partial class Cliente
{
    public string IdCliente { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido1 { get; set; } = null!;

    public string? Apellido2 { get; set; }

    public string Sexo { get; set; } = null!;

    public DateTime FechaNacimiento { get; set; }

    public virtual ICollection<Orden> Orden { get; set; } = new List<Orden>();
}
