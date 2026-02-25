namespace SuVac.Infraestructure.Modelos;

public class ResultadoSubasta
{
    public int ResultadoId { get; set; }
    public int SubastaId { get; set; }
    public int UsuarioGanadorId { get; set; }
    public decimal MontoFinal { get; set; }
    public DateTime FechaCierre { get; set; }

    public virtual Subasta SubastaNavigation { get; set; } = null!;
    public virtual Usuario UsuarioGanadorNavigation { get; set; } = null!;
}
