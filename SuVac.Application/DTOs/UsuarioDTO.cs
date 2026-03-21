using System.ComponentModel.DataAnnotations;

namespace SuVac.Application.DTOs;

public class UsuarioDTO
{
    public int UsuarioId { get; set; }

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
    [StringLength(150, ErrorMessage = "El correo no puede exceder 150 caracteres.")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres.")]
    public string NombreCompleto { get; set; } = null!;

    public string? Contrasena { get; set; }
    public int RolId { get; set; }
    public string? NombreRol { get; set; }
    public int EstadoUsuarioId { get; set; }
    public string? NombreEstado { get; set; }
    public DateTime FechaRegistro { get; set; }
    public int CantidadSubastasCreadas { get; set; }
    public int CantidadPujasRealizadas { get; set; }
}
