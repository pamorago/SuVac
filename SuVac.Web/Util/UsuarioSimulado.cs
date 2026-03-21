namespace SuVac.Web.Util;

/// <summary>
/// Simula el usuario autenticado mientras no exista sistema de login.
/// Cambiar <see cref="UsuarioActualId"/> para probar con distintos usuarios.
/// </summary>
public static class UsuarioSimulado
{
    /// <summary>
    /// Id del usuario que actúa como "usuario actual" del sistema.
    /// Cambiar este valor para simular distintos usuarios (vendedores / compradores).
    /// </summary>
    public const int UsuarioActualId = 2; // Carlos Vendedor — usuarioId=2 en la BD
}
