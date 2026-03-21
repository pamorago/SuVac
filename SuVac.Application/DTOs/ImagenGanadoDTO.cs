using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SuVac.Application.DTOs;

public class ImagenGanadoDTO
{
    [DisplayName("Identificador Imagen")]
    public int ImagenId { get; set; }

    [DisplayName("Identificador Ganado")]
    public int GanadoId { get; set; }

    [DisplayName("URL Imagen")]
    [StringLength(2000, ErrorMessage = "La URL de la imagen no puede superar 2000 caracteres.")]
    public string UrlImagen { get; set; } = null!;
}
