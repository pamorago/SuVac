namespace SuVac.Application.DTOs;

public class UsuarioDTO
{
    public int UsuarioId { get; set; }
    public string Correo { get; set; } = null!;
    public string NombreCompleto { get; set; } = null!;
    public int RolId { get; set; }
    public string? NombreRol { get; set; }
    public int EstadoUsuarioId { get; set; }
    public string? NombreEstado { get; set; }
    public DateTime FechaRegistro { get; set; }
    public int CantidadSubastasCreadas { get; set; }
    public int CantidadPujasRealizadas { get; set; }
}
