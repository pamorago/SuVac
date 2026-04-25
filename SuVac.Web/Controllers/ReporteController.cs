using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuVac.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SuVac.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ReporteController : Controller
{
    private readonly SuVacContext _context;

    public ReporteController(SuVacContext context)
    {
        _context = context;
    }

    // ─── Reporte 1: Subastas por Período ─────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> SubastasPorPeriodo(
        DateTime? desde = null, DateTime? hasta = null, string? estado = null)
    {
        desde ??= DateTime.Today.AddMonths(-3);
        hasta ??= DateTime.Today;

        var query = _context.Subastas
            .Include(s => s.IdEstadoSubastaNavigation)
            .Include(s => s.IdGanadoNavigation)
            .Include(s => s.IdUsuarioCreadorNavigation)
            .Where(s => s.FechaInicio >= desde && s.FechaInicio <= hasta.Value.AddDays(1));

        if (!string.IsNullOrEmpty(estado))
            query = query.Where(s => s.IdEstadoSubastaNavigation.Nombre == estado);

        var subastas = await query.AsNoTracking().ToListAsync();

        // Lista de estados disponibles para el filtro
        var estados = await _context.EstadosSubasta
            .AsNoTracking()
            .Select(e => e.Nombre)
            .ToListAsync();

        // Agrupado por estado para gráfico doughnut
        var porEstado = subastas
            .GroupBy(s => s.IdEstadoSubastaNavigation?.Nombre ?? "Sin estado")
            .Select(g => new { Estado = g.Key, Total = g.Count() })
            .OrderByDescending(x => x.Total)
            .ToList();

        // Agrupado por mes (cronológico) para gráfico de barras
        var porMes = subastas
            .GroupBy(s => new { s.FechaInicio.Year, s.FechaInicio.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new
            {
                mes = $"{g.Key.Month:D2}/{g.Key.Year}",
                total = g.Count()
            })
            .ToList();

        // Monto total recaudado (subastas finalizadas en el período)
        var montoTotal = await _context.ResultadosSubasta
            .Where(r => r.IdSubastaNavigation.FechaInicio >= desde
                     && r.IdSubastaNavigation.FechaInicio <= hasta.Value.AddDays(1))
            .SumAsync(r => (decimal?)r.MontoFinal) ?? 0;

        ViewBag.Desde = desde.Value.ToString("yyyy-MM-dd");
        ViewBag.Hasta = hasta.Value.ToString("yyyy-MM-dd");
        ViewBag.EstadoFiltro = estado ?? "";
        ViewBag.EstadosDisponibles = estados;
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

        // Compradores con más pujas en el período — GroupBy solo por UsuarioId (scalar)
        var pujasPorUsuario = await _context.Pujas
            .Where(p => p.FechaHora >= desde && p.FechaHora <= hasta.Value.AddDays(1))
            .GroupBy(p => p.UsuarioId)
            .Select(g => new
            {
                UsuarioId = g.Key,
                TotalPujas = g.Count(),
                MontoMaximo = g.Max(p => p.Monto),
                MontoPromedio = g.Average(p => p.Monto)
            })
            .OrderByDescending(x => x.TotalPujas)
            .Take(top)
            .ToListAsync();

        // Obtener nombres de usuario por separado
        var usuarioIds = pujasPorUsuario.Select(x => x.UsuarioId).ToList();
        var usuarios = await _context.Usuarios
            .Where(u => usuarioIds.Contains(u.UsuarioId))
            .Select(u => new { u.UsuarioId, u.NombreCompleto })
            .ToListAsync();

        // Ganadores en el período
        var ganadores = await _context.ResultadosSubasta
            .Where(r => r.FechaCierre >= desde && r.FechaCierre <= hasta.Value.AddDays(1))
            .GroupBy(r => r.UsuarioGanadorId)
            .Select(g => new { UsuarioId = g.Key, SubastasGanadas = g.Count() })
            .ToListAsync();

        // Unir en memoria
        var resultado = pujasPorUsuario.Select(c => new
        {
            c.UsuarioId,
            Nombre = usuarios.FirstOrDefault(u => u.UsuarioId == c.UsuarioId)?.NombreCompleto
                     ?? $"Usuario #{c.UsuarioId}",
            c.TotalPujas,
            c.MontoMaximo,
            c.MontoPromedio,
            SubastasGanadas = ganadores.FirstOrDefault(g => g.UsuarioId == c.UsuarioId)?.SubastasGanadas ?? 0
        }).ToList();

        ViewBag.Desde = desde.Value.ToString("yyyy-MM-dd");
        ViewBag.Hasta = hasta.Value.ToString("yyyy-MM-dd");
        ViewBag.Top = top;
        ViewBag.NombresLabels = System.Text.Json.JsonSerializer.Serialize(resultado.Select(x => x.Nombre).ToArray());
        ViewBag.PujasData = System.Text.Json.JsonSerializer.Serialize(resultado.Select(x => x.TotalPujas).ToArray());
        ViewBag.GanadasData = System.Text.Json.JsonSerializer.Serialize(resultado.Select(x => x.SubastasGanadas).ToArray());

        // Serializar la tabla completa para la vista (evita tipo anónimo en Razor)
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
