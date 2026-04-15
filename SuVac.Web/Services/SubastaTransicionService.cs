using Microsoft.AspNetCore.SignalR;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;
using SuVac.Web.Hubs;

namespace SuVac.Web.Services;

/// <summary>
/// Servicio en segundo plano que actualiza automáticamente los estados de las subastas
/// cada 30 segundos: Programada → Activa (cuando FechaInicio ≤ ahora)
///                   Activa → Finalizada (cuando FechaFin ≤ ahora)
/// Al finalizar una subasta:
///   - Cambia estado a Finalizada en la BD
///   - Registra el ResultadoSubasta (ganador + monto final)
///   - Notifica a todos los clientes en la sala vía SignalR
/// </summary>
public class SubastaTransicionService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SubastaTransicionService> _logger;
    private readonly IHubContext<PujaHub> _hubContext;
    private static readonly TimeSpan Intervalo = TimeSpan.FromSeconds(30);

    public SubastaTransicionService(IServiceScopeFactory scopeFactory,
        ILogger<SubastaTransicionService> logger,
        IHubContext<PujaHub> hubContext)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SubastaTransicionService iniciado. Intervalo: {Seg}s.", Intervalo.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcesarCicloAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ciclo de transición de subastas.");
            }

            await Task.Delay(Intervalo, stoppingToken);
        }
    }

    private async Task ProcesarCicloAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var serviceSubasta = scope.ServiceProvider.GetRequiredService<IServiceSubasta>();
        var servicePuja = scope.ServiceProvider.GetRequiredService<IServicePuja>();
        var repoResultado = scope.ServiceProvider.GetRequiredService<IRepositoryResultadoSubasta>();

        // 1. Capturar los IDs activos que ya pasaron su FechaFin ANTES de cambiar estados
        var idsAFinalizar = (await serviceSubasta.GetIdsActivasParaFinalizarAsync()).ToList();

        // 2. Ejecutar transición de estados en la BD (Programada→Activa, Activa→Finalizada)
        await serviceSubasta.ActualizarEstadosAsync();

        // 3. Por cada subasta finalizada en este ciclo: crear ResultadoSubasta y notificar
        foreach (var subastaId in idsAFinalizar)
        {
            // Obtener la puja más alta (ganador)
            var ganador = await servicePuja.GetPujaMasAlta(subastaId);
            var nombreGanador = ganador?.NombreUsuario ?? "Sin ganador";
            var montoFinal = ganador?.Monto ?? 0m;

            // Registrar ResultadoSubasta en la BD (solo si tiene ganador y aún no existe)
            if (ganador is not null)
            {
                var yaExiste = await repoResultado.GetBySubastaId(subastaId);
                if (yaExiste is null)
                {
                    var resultado = new ResultadoSubasta
                    {
                        SubastaId = subastaId,
                        UsuarioGanadorId = ganador.UsuarioId,
                        MontoFinal = ganador.Monto,
                        FechaCierre = DateTime.Now
                    };
                    await repoResultado.Create(resultado);

                    _logger.LogInformation(
                        "ResultadoSubasta creado — Subasta #{Id}, Ganador: {Ganador}, Monto: ₡{Monto:N2}.",
                        subastaId, nombreGanador, montoFinal);
                }
            }
            else
            {
                _logger.LogWarning("Subasta #{Id} finalizada sin pujas — no se registra ganador.", subastaId);
            }

            // Notificar a todos los clientes conectados en la sala via SignalR
            await _hubContext.Clients
                .Group($"subasta-{subastaId}")
                .SendAsync("SubastaFinalizada", subastaId, nombreGanador, montoFinal, cancellationToken: ct);

            _logger.LogInformation(
                "Subasta #{Id} cerrada automáticamente. Ganador: '{Ganador}' con ₡{Monto:N2}.",
                subastaId, nombreGanador, montoFinal);
        }
    }
}
