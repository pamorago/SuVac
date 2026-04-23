using System.ComponentModel.DataAnnotations;

namespace SuVac.Web.ViewModels;

public record ViewModelLogin
{
    [Display(Name = "Correo electrónico")]
    [Required(ErrorMessage = "{0} es requerido.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
    public string User { get; set; } = default!;

    [Display(Name = "Contraseña")]
    [Required(ErrorMessage = "{0} es requerida.")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 50 caracteres.")]
    public string Password { get; set; } = default!;
}
