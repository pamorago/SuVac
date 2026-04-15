using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Models;
using SuVac.Infraestructure.Repository.Interfaces;
using SuVac.Web.Hubs;

namespace SuVac.Web.Services;

/// <summary>
/// Servicio en segundo plano completamente reactivo (sin polling de cierre):
///
///  1. Al iniciar: carga todas las subastas Activas en la BD y programa un
///     Task.Delay exacto hasta su FechaFin por cada una.
///
///  2. Loop liviano cada 5 s: detecta subastas Programadas cuya FechaInicio
///     ya llegó → las activa en la BD → programa su timer exacto de cierre.
///
///  3. Cuando el timer de una subasta expira (exactamente en FechaFin):
///     • Cambia estado a Finalizada en la BD.
///     • Registra ResultadoSubasta (ganador + monto).
///     • Emite SignalR "SubastaFinalizada" — el modal aparece en el navegador
///       en menos de 1 segundo, sin recargar la página.
/// </summary>
public class SubastaTransicionService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SubastaTransicionService> _logger;
    private readonly IHubContext<PujaHub> _hubContext;

    // Un CancellationTokenSource por SubastaId para poder cancelar si la subasta se cancela manualmente
    private readonly ConcurrentDictionary<int, CancellationTokenSource> _timers = new();

    // Solo para detectar Programada → Activa (no para cerrar)
    private static readonly TimeSpan IntervaloActivacion = TimeSpan.FromSeconds(5);

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
        _logger.LogInformation("SubastaTransicionService (reactivo) iniciado.");

        // Al arrancar: registrar timers exactos para subastas ya activas en la BD
        await ProgramarTimersActivasAsync(stoppingToken);

        // Loop liviano: solo activa subastas Programadas cuya FechaInicio llegó
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ActivarProgramadasAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ciclo de activación de subastas programadas.");
            }

            await Task.Delay(IntervaloActivacion, stoppingToken);
        }
    }

    // ── Startup: timers para subastas ya activas ─────────────────────────────

    private async Task ProgramarTimersActivasAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRepositorySubasta>();
            var activas = (await repo.GetActivas()).ToList();

            foreach (var s in activas)
                ProgramarCierreExacto(s.SubastaId, s.FechaFin, ct);

            _logger.LogInformation("{N} subasta(s) activa(s): timers exactos registrados.", activas.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar subastas activas al inicio.");
        }
    }

    // ── Loop liviano: Programada → Activa ────────────────────────────────────

    private async Task ActivarProgramadasAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IRepositorySubasta>();

        var aProgramar = (await repo.GetProgramadasParaActivarAsync()).ToList();
        if (!aProgramar.Any()) return;

        var idActiva = await repo.GetEstadoIdByNombre("Activa");
        if (idActiva == null) return;

        foreach (var s in aProgramar)
        {
            await repo.CambiarEstado(s.SubastaId, idActiva.Value);
            ProgramarCierreExacto(s.SubastaId, s.FechaFin, ct);
            _logger.LogInformation("Subasta #{Id} → Activa. Cierre exacto programado: {Fecha}.", s.SubastaId, s.FechaFin);
        }
    }

    // ── Timer exacto por subasta ──────────────────────────────────────────────

    private void ProgramarCierreExacto(int subastaId, DateTime fechaFin, CancellationToken serviceCt)
    {
        // Evitar duplicados
        var cts = CancellationTokenSource.CreateLinkedTokenSource(serviceCt);
        if (!_timers.TryAdd(subastaId, cts))
        {
            cts.Dispose();
            return;
        }

        var demora = fechaFin - DateTime.Now;
        if (demora < TimeSpan.Zero) demora = TimeSpan.Zero;

        _logger.LogInformation("Timer exacto subasta #{Id}: {D:mm\\:ss} restantes (cierre: {F}).",
            subastaId, demora, fechaFin);

        // Fire-and-forget: espera exacto hasta FechaFin, luego cierra
        _ = Task.Run(async () =>
        {
            try
            {
                if (demora > TimeSpan.Zero)
                    await Task.Delay(demora, cts.Token);

                await CerrarSubastaAsync(subastaId, cts.Token);
            }
            catch (OperationCanceledException) { /* Subasta cancelada manualmente */ }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar subasta #{Id} por timer exacto.", subastaId);
            }
            finally
            {
                _timers.TryRemove(subastaId, out _);
                cts.Dispose();
            }
        }, CancellationToken.None);
    }

    // ── Cierre efectivo: BD + ResultadoSubasta + SignalR ─────────────────────

    private async Task CerrarSubastaAsync(int subastaId, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IRepositorySubasta>();
        var servicePuja = scope.ServiceProvider.GetRequiredService<IServicePuja>();
        var repoResultado = scope.ServiceProvider.GetRequiredService<IRepositoryResultadoSubasta>();

        // Verificar que sigue Activa (pudo cancelarse manualmente antes de que expirara el timer)
        var subasta = await repo.GetById(subastaId);
        var idActiva = await repo.GetEstadoIdByNombre("Activa");
        if (subasta == null || subasta.EstadoSubastaId != idActiva)
        {
            _logger.LogWarning("Subasta #{Id}: ya no está Activa al disparar el timer (se omite cierre).", subastaId);
            return;
        }

        // 1. Cambiar a Finalizada + marcar ganado Inactivo
        await repo.FinalizarSubastaAsync(subastaId);

        // 2. Registrar ResultadoSubasta (idempotente)
        var ganador = await servicePuja.GetPujaMasAlta(subastaId);
        var nombreGanador = ganador?.NombreUsuario ?? "Sin ganador";
        var montoFinal = ganador?.Monto ?? 0m;

        if (ganador is not null && await repoResultado.GetBySubastaId(subastaId) is null)
        {
            await repoResultado.Create(new ResultadoSubasta
            {
                SubastaId = subastaId,
                UsuarioGanadorId = ganador.UsuarioId,
                MontoFinal = ganador.Monto,
                FechaCierre = DateTime.Now
            });
        }

        // 3. Notificar navegadores — inmediato, sin delay adicional
        await _hubContext.Clients
            .Group($"subasta-{subastaId}")
            .SendAsync("SubastaFinalizada", subastaId, nombreGanador, montoFinal, cancellationToken: ct);

        _logger.LogInformation(
            "Subasta #{Id} cerrada exactamente. Ganador: '{G}', ₡{M:N2}.",
            subastaId, nombreGanador, montoFinal);
    }
}
