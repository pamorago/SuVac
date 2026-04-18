namespace SuVac.Web.Util;

/// <summary>
/// Simula el usuario autenticado mientras no exista sistema de login.
/// Cambiar <see cref="UsuarioActualId"/> para probar con distintos usuarios.
/// </summary>
public static class UsuarioSimulado
{
    private static int _usuarioActualId = 2;

    /// <summary>
    /// Id del usuario que actúa como "usuario actual" del sistema.
    /// Se puede cambiar en tiempo de ejecución desde el botón de prueba de la barra de navegación.
    /// </summary>
    public static int UsuarioActualId
    {
        get => _usuarioActualId;
        set => _usuarioActualId = value;
    }
}
