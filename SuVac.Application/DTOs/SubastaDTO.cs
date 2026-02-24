namespace SuVac.Application.DTOs;

public class SubastaDTO
{
    public int SubastaId { get; set; }
    public int GanadoId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal PrecioBase { get; set; }
    public decimal IncrementoMinimo { get; set; }
    public int EstadoSubastaId { get; set; }
    public int UsuarioCreadorId { get; set; }
}
