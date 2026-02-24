using System;
using System.Collections.Generic;

namespace SuVac.Infraestructure.Models;

public partial class Categoria
{
    public int CategoriaId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<GanadoCategoria> GanadoCategorias { get; set; } = new List<GanadoCategoria>();
}
