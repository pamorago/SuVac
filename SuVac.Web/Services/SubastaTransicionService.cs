using SuVac.Application.Services.Interfaces;

namespace SuVac.Web.Services;

/// <summary>
/// Servicio en segundo plano que actualiza automáticamente los estados de las subastas
/// cada 60 segundos: Programada → Activa (cuando FechaInicio ≤ ahora)
///                   Activa     → Finalizada (cuando FechaFin ≤ ahora)
/// </summary>
public class SubastaTransicionService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SubastaTransicionService> _logger;
    private static readonly TimeSpan Intervalo = TimeSpan.FromSeconds(60);

    public SubastaTransicionService(IServiceScopeFactory scopeFactory,
        ILogger<SubastaTransicionService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SubastaTransicionService iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IServiceSubasta>();
                await service.ActualizarEstadosAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estados de subastas.");
            }

            await Task.Delay(Intervalo, stoppingToken);
        }
    }
}
