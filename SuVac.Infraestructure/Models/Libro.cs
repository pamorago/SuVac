using System;
using System.Collections.Generic;

namespace SuVac.Infraestructure.Models;

public partial class Libro
{
    public int IdLibro { get; set; }

    public string Isbn { get; set; } = null!;

    public int IdAutor { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Precio { get; set; }

    public int Cantidad { get; set; }

    public byte[] Imagen { get; set; } = null!;

    public virtual Autor IdAutorNavigation { get; set; } = null!;

    public virtual ICollection<OrdenDetalle> OrdenDetalle { get; set; } = new List<OrdenDetalle>();

    public virtual ICollection<Categoria> IdCategoria { get; set; } = new List<Categoria>();
}
