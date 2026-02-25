namespace SuVac.Infraestructure.Modelos;

public class EstadoSubasta
{
    public int EstadoSubastaId { get; set; }
    public string Nombre { get; set; } = null!;

    public virtual ICollection<Subasta> Subastas { get; set; } = new List<Subasta>();
}
