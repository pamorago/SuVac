using Microsoft.AspNetCore.SignalR;

namespace SuVac.Web.Hubs;

/// <summary>
/// Hub de SignalR para el proceso de pujas en tiempo real.
/// Los clientes se unen al grupo "subasta-{id}" para recibir
/// actualizaciones de una subasta específica.
/// </summary>
public class PujaHub : Hub
{
    /// <summary>
    /// El cliente solicita unirse al canal de una subasta.
    /// Se llama al cargar la página de Sala.
    /// </summary>
    public async Task UnirseASubasta(int subastaId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"subasta-{subastaId}");
    }

    /// <summary>
    /// El cliente abandona el canal de una subasta.
    /// </summary>
    public async Task SalirDeSubasta(int subastaId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"subasta-{subastaId}");
    }
}
