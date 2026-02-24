using System;

namespace SuVac.Infraestructure.Models;

public partial class ImagenGanado
{
    public int ImagenId { get; set; }

    public int GanadoId { get; set; }

    public string UrlImagen { get; set; } = null!;

    public virtual Ganado IdGanadoNavigation { get; set; } = null!;
}
