namespace SuVac.Infraestructure.Modelos;

public class ImagenGanado
{
    public int ImagenId { get; set; }
    public int GanadoId { get; set; }
    public string UrlImagen { get; set; } = null!;

    public virtual Ganado GanadoNavigation { get; set; } = null!;
}
