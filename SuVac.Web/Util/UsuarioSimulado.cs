namespace SuVac.Web.Util;

/// <summary>
/// Simula el usuario autenticado mientras no exista sistema de login.
/// Cambiar <see cref="UsuarioActualId"/> para probar con distintos usuarios.
/// </summary>
public static class UsuarioSimulado
{
    // AsyncLocal: el valor es independiente por cada contexto de ejecución asíncrono
    // (es decir, por cada request HTTP), por lo que cada pestaña obtiene su propio usuario.
    private static readonly AsyncLocal<int> _current = new();

    /// <summary>
    /// Id del usuario que actúa como "usuario actual" del sistema para esta petición.
    /// Es asignado por <c>UsuarioSimuladoMiddleware</c> al inicio de cada request
    /// leyendo la cabecera HTTP <c>X-Suvac-Uid</c> o la cookie <c>suvac-uid</c>.
    /// </summary>
    public static int UsuarioActualId
    {
        get => _current.Value > 0 ? _current.Value : 2;
        internal set => _current.Value = value;
    }
}
