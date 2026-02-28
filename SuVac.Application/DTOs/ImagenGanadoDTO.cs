using System.ComponentModel;

namespace SuVac.Application.DTOs;

public class ImagenGanadoDTO
{
    [DisplayName("Identificador Imagen")]
    public int ImagenId { get; set; }

    [DisplayName("Identificador Ganado")]
    public int GanadoId { get; set; }

    [DisplayName("URL Imagen")]
    public string UrlImagen { get; set; } = null!;
}
