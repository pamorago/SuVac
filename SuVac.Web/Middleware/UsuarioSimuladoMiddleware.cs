using SuVac.Web.Util;

namespace SuVac.Web.Middleware;

/// <summary>
/// Middleware de demo: lee el usuario simulado de la cabecera HTTP <c>X-Suvac-Uid</c>
/// (peticiones AJAX) o de la cookie <c>suvac-uid</c> (navegación normal y form-POSTs),
/// y lo asigna en el contexto de ejecución asíncrono actual para que sea accesible
/// vía <see cref="UsuarioSimulado.UsuarioActualId"/> durante toda la petición.
/// 
/// Cada pestaña del navegador gestiona su propio valor a través de <c>sessionStorage</c>
/// en el cliente, lo que garantiza el aislamiento entre pestañas.
/// </summary>
public class UsuarioSimuladoMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Suvac-Uid";
    private const string CookieName = "suvac-uid";

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Cabecera tiene prioridad (fetch/AJAX desde el cliente)
        if (context.Request.Headers.TryGetValue(HeaderName, out var headerVal)
            && int.TryParse(headerVal, out int hid) && hid > 0)
        {
            UsuarioSimulado.UsuarioActualId = hid;
        }
        // 2. Cookie (navegación full-page y form POSTs — sincronizada por JS desde sessionStorage)
        else if (context.Request.Cookies.TryGetValue(CookieName, out var cookieVal)
                 && int.TryParse(cookieVal, out int cid) && cid > 0)
        {
            UsuarioSimulado.UsuarioActualId = cid;
        }
        // 3. Valor por defecto (primera carga sin cookie aún)
        else
        {
            UsuarioSimulado.UsuarioActualId = 2;
        }

        await next(context);
    }
}
