using System.Security.Claims;

namespace SuVac.Web.Util;

/// <summary>
/// Helpers para obtener datos del usuario autenticado desde el <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class UsuarioHelper
{
    /// <summary>Retorna el UsuarioId del usuario autenticado (claim NameIdentifier). Retorna 0 si no hay sesión.</summary>
    public static int GetUsuarioId(ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var id) ? id : 0;
    }

    /// <summary>Retorna el nombre completo del usuario autenticado (claim Name). Retorna string vacío si no hay sesión.</summary>
    public static string GetNombreCompleto(ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

    /// <summary>Retorna el rol del usuario autenticado (claim Role). Retorna string vacío si no hay sesión.</summary>
    public static string GetRol(ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
}
