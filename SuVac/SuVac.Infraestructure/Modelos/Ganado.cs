namespace SuVac.Infraestructure.Modelos;

public class Ganado
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

    public virtual TipoGanado TipoGanadoNavigation { get; set; } = null!;
    public virtual Raza RazaNavigation { get; set; } = null!;
    public virtual Sexo SexoNavigation { get; set; } = null!;
    public virtual EstadoGanado EstadoGanadoNavigation { get; set; } = null!;
    public virtual Usuario UsuarioVendedorNavigation { get; set; } = null!;

    public virtual ICollection<ImagenGanado> Imagenes { get; set; } = new List<ImagenGanado>();
    public virtual ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
    public virtual ICollection<Subasta> Subastas { get; set; } = new List<Subasta>();
}
