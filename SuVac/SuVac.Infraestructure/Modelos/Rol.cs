namespace SuVac.Infraestructure.Modelos;

public class Rol
{
    public int RolId { get; set; }
    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
