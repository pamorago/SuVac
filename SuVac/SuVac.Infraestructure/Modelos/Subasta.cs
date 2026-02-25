namespace SuVac.Infraestructure.Modelos;

public class Subasta
{
    public int SubastaId { get; set; }
    public int GanadoId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal PrecioBase { get; set; }
    public decimal IncrementoMinimo { get; set; }
    public int EstadoSubastaId { get; set; }
    public int UsuarioCreadorId { get; set; }

    public virtual Ganado GanadoNavigation { get; set; } = null!;
    public virtual EstadoSubasta EstadoSubastaNavigation { get; set; } = null!;
    public virtual Usuario UsuarioCreadorNavigation { get; set; } = null!;

    public virtual ICollection<Puja> Pujas { get; set; } = new List<Puja>();
    public virtual ResultadoSubasta? ResultadoSubasta { get; set; }
    public virtual Pago? Pago { get; set; }
}
