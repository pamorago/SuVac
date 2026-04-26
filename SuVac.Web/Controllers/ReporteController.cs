using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuVac.Application.Services.Interfaces;

namespace SuVac.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ReporteController : Controller
{
    private readonly IServiceReporte _service;

    public ReporteController(IServiceReporte service)
    {
        _service = service;
    }

    // ─── Reporte 1: Subastas por Período ─────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> SubastasPorPeriodo(
        DateTime? desde = null, DateTime? hasta = null, string? estado = null)
    {
        desde ??= DateTime.Today.AddMonths(-3);
        hasta ??= DateTime.Today;

        var subastas = (await _service.GetSubastasPorPeriodoAsync(desde.Value, hasta.Value, estado)).ToList();
        var estados = await _service.GetEstadosSubastaAsync();
        var montoTotal = await _service.GetMontoRecaudadoAsync(desde.Value, hasta.Value);

        // Agrupado por estado para gráfico doughnut
        var porEstado = subastas
            .GroupBy(s => s.EstadoSubasta)
            .Select(g => new { Estado = g.Key, Total = g.Count() })
            .OrderByDescending(x => x.Total)
            .ToList();

        // Agrupado por mes (cronológico) para gráfico de barras
        var porMes = subastas
            .GroupBy(s => new { s.FechaInicio.Year, s.FechaInicio.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new { mes = $"{g.Key.Month:D2}/{g.Key.Year}", total = g.Count() })
            .ToList();

        ViewBag.Desde = desde.Value.ToString("yyyy-MM-dd");
        ViewBag.Hasta = hasta.Value.ToString("yyyy-MM-dd");
        ViewBag.EstadoFiltro = estado ?? "";
        ViewBag.EstadosDisponibles = estados.ToList();
        ViewBag.Total = subastas.Count;
        ViewBag.MontoTotal = montoTotal;
        ViewBag.PorEstadoLabels = System.Text.Json.JsonSerializer.Serialize(porEstado.Select(x => x.Estado).ToArray());
        ViewBag.PorEstadoData = System.Text.Json.JsonSerializer.Serialize(porEstado.Select(x => x.Total).ToArray());
        ViewBag.PorMesJson = System.Text.Json.JsonSerializer.Serialize(porMes);

        return View(subastas);
    }

    // ─── Reporte 2: Top Compradores ───────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> TopCompradores(
        DateTime? desde = null, DateTime? hasta = null, int top = 10)
    {
        desde ??= DateTime.Today.AddMonths(-3);
        hasta ??= DateTime.Today;

        var resultado = (await _service.GetTopCompradoresAsync(desde.Value, hasta.Value, top)).ToList();

        ViewBag.Desde = desde.Value.ToString("yyyy-MM-dd");
        ViewBag.Hasta = hasta.Value.ToString("yyyy-MM-dd");
        ViewBag.Top = top;
        ViewBag.NombresLabels = System.Text.Json.JsonSerializer.Serialize(resultado.Select(x => x.Nombre).ToArray());
        ViewBag.PujasData = System.Text.Json.JsonSerializer.Serialize(resultado.Select(x => x.TotalPujas).ToArray());
        ViewBag.GanadasData = System.Text.Json.JsonSerializer.Serialize(resultado.Select(x => x.SubastasGanadas).ToArray());
        ViewBag.TablaJson = System.Text.Json.JsonSerializer.Serialize(resultado.Select(c => new
        {
            Nombre = c.Nombre,
            TotalPujas = c.TotalPujas,
            MontoMaximo = c.MontoMaximo,
            MontoPromedio = c.MontoPromedio,
            SubastasGanadas = c.SubastasGanadas
        }).ToArray());

        return View();
    }
}
