namespace SuVac.Application.DTOs;

public class GanadoDTO
{
    public int GanadoId { get; set; }
    public string Nombre { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public int TipoGanadoId { get; set; }
    public int RazaId { get; set; }
    public int SexoId { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public decimal PesoKg { get; set; }
    public string? CertificadoSalud { get; set; }
    public int EstadoGanadoId { get; set; }
    public DateTime FechaRegistro { get; set; }
    public int UsuarioVendedorId { get; set; }
}
