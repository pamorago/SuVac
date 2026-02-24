using System;
using System.Collections.Generic;

namespace SuVac.Infraestructure.Models;

public partial class Autor
{
    public int IdAutor { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Libro> Libro { get; set; } = new List<Libro>();
}
