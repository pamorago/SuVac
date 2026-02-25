namespace SuVac.Infraestructure.Modelos;

public class Raza
{
    public int RazaId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    public virtual ICollection<Ganado> Ganados { get; set; } = new List<Ganado>();
}
