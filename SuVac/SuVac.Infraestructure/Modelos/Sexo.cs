namespace SuVac.Infraestructure.Modelos;

public class Sexo
{
    public int SexoId { get; set; }
    public string Nombre { get; set; } = null!;

    public virtual ICollection<Ganado> Ganados { get; set; } = new List<Ganado>();
}
