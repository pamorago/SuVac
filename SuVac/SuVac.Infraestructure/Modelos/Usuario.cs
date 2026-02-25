namespace SuVac.Infraestructure.Modelos;

public class Usuario
{
    public int UsuarioId { get; set; }
    public string Correo { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string NombreCompleto { get; set; } = null!;
    public int RolId { get; set; }
    public int EstadoUsuarioId { get; set; }
    public DateTime FechaRegistro { get; set; }

    public virtual Rol RolNavigation { get; set; } = null!;
    public virtual EstadoUsuario EstadoUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Ganado> GanadosVendidos { get; set; } = new List<Ganado>();
    public virtual ICollection<Subasta> SubastasCreadas { get; set; } = new List<Subasta>();
    public virtual ICollection<Puja> Pujas { get; set; } = new List<Puja>();
    public virtual ICollection<ResultadoSubasta> ResultadosGanados { get; set; } = new List<ResultadoSubasta>();
    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
