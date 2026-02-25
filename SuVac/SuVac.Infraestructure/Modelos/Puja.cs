namespace SuVac.Infraestructure.Modelos;

public class Puja
{
    public int PujaId { get; set; }
    public int SubastaId { get; set; }
    public int UsuarioId { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaHora { get; set; }

    public virtual Subasta SubastaNavigation { get; set; } = null!;
    public virtual Usuario UsuarioNavigation { get; set; } = null!;
}
