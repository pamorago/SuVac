namespace SuVac.Infraestructure.Modelos;

public class Categoria
{
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    public virtual ICollection<Ganado> Ganados { get; set; } = new List<Ganado>();
}
