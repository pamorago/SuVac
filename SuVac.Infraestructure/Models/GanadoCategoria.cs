using System;

namespace SuVac.Infraestructure.Models;

public partial class GanadoCategoria
{
    public int GanadoId { get; set; }

    public int CategoriaId { get; set; }

    public virtual Ganado IdGanadoNavigation { get; set; } = null!;

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;
}
