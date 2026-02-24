using System;
using System.Collections.Generic;

namespace SuVac.Infraestructure.Models;

public partial class EstadoUsuario
{
    public int EstadoUsuarioId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
