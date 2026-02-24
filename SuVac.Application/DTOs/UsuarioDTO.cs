namespace SuVac.Application.DTOs;

public class UsuarioDTO
{
    public int UsuarioId { get; set; }
    public string Correo { get; set; } = null!;
    public string NombreCompleto { get; set; } = null!;
    public int RolId { get; set; }
    public int EstadoUsuarioId { get; set; }
    public DateTime FechaRegistro { get; set; }
}
