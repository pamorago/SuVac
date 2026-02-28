using System.ComponentModel;

namespace SuVac.Application.DTOs;

public class GanadoDTO
{
    [DisplayName("Identificador Ganado")]
    public int GanadoId { get; set; }

    [DisplayName("Nombre Ganado")]
    public string Nombre { get; set; } = null!;

    [DisplayName("Nombre Ganado")]
    public string Descripcion { get; set; } = null!;

    [DisplayName("Tipo Ganado")]
    public int TipoGanadoId { get; set; }

    [DisplayName("Nombre Tipo Ganado")]
    public string? NombreTipoGanado { get; set; }

    [DisplayName("Raza del Ganado")]
    public int RazaId { get; set; }

    [DisplayName("Nombre Raza")]
    public string? NombreRaza { get; set; }

    public int SexoId { get; set; }

    [DisplayName("Nombre Sexo")]
    public string? NombreSexo { get; set; }

    public DateTime FechaNacimiento { get; set; }
    public decimal PesoKg { get; set; }
    public string? CertificadoSalud { get; set; }
    public int EstadoGanadoId { get; set; }
    public DateTime FechaRegistro { get; set; }
    public int UsuarioVendedorId { get; set; }

    [DisplayName("Nombre Vendedor")]
    public string? NombreVendedor { get; set; }

    [DisplayName("Imágenes")]
    public ICollection<ImagenGanadoDTO>? ImagenesGanado { get; set; }
}
