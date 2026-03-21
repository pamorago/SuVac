using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SuVac.Application.DTOs;

public class GanadoDTO
{
    [DisplayName("Identificador Ganado")]
    public int GanadoId { get; set; }

    [DisplayName("Nombre")]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [DisplayName("Descripción")]
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [MinLength(20, ErrorMessage = "La descripción debe tener al menos 20 caracteres.")]
    [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres.")]
    public string Descripcion { get; set; } = null!;

    [DisplayName("Tipo Ganado")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de ganado.")]
    public int TipoGanadoId { get; set; }

    [DisplayName("Nombre Tipo Ganado")]
    public string? NombreTipoGanado { get; set; }

    [DisplayName("Raza del Ganado")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una raza.")]
    public int RazaId { get; set; }

    [DisplayName("Nombre Raza")]
    public string? NombreRaza { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar el sexo.")]
    public int SexoId { get; set; }

    [DisplayName("Nombre Sexo")]
    public string? NombreSexo { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateTime FechaNacimiento { get; set; }

    [Range(0.01, 9999.99, ErrorMessage = "El peso debe estar entre 0.01 y 9999.99 kg.")]
    public decimal PesoKg { get; set; }
    public string? CertificadoSalud { get; set; }
    public int EstadoGanadoId { get; set; }
    public DateTime FechaRegistro { get; set; }
    public int UsuarioVendedorId { get; set; }

    [DisplayName("Nombre Vendedor")]
    public string? NombreVendedor { get; set; }

    [DisplayName("Estado Ganado")]
    public string? NombreEstadoGanado { get; set; }

    [DisplayName("Categorías")]
    public ICollection<string>? Categorias { get; set; }

    /// <summary>IDs de categorías seleccionadas — usado solo en formularios Create/Edit.</summary>
    public List<int>? CategoriasIds { get; set; }

    [DisplayName("Imágenes")]
    public ICollection<ImagenGanadoDTO>? ImagenesGanado { get; set; }

    /// <summary>Historial de subastas donde ha participado este ganado (calculado via LINQ).</summary>
    [DisplayName("Subastas")]
    public ICollection<SubastaResumenDTO>? SubastasParticipacion { get; set; }
}
