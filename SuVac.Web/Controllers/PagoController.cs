using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Web.Util;
using Microsoft.AspNetCore.Mvc;

namespace SuVac.Web.Controllers;

public class PagoController : Controller
{
    private readonly IServicePago _service;

    public PagoController(IServicePago service)
    {
        _service = service;
    }

    // GET: /Pago
    public async Task<IActionResult> Index()
    {
        var pagos = await _service.GetAllConDetalle();
        return View(pagos);
    }

    // GET: /Pago/Detalle/5
    public async Task<IActionResult> Detalle(int id)
    {
        if (id <= 0) return NotFound();
        var pago = await _service.GetByIdConDetalle(id);
        if (pago is null) return NotFound();
        ViewBag.EsComprador = pago.UsuarioId == UsuarioSimulado.UsuarioActualId;
        return View(pago);
    }

    // POST: /Pago/Confirmar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirmar(int id)
    {
        if (id <= 0) return NotFound();

        // Solo el comprador ganador puede confirmar su propio pago
        var pago = await _service.GetByIdConDetalle(id);
        if (pago is null) return NotFound();

        if (pago.UsuarioId != UsuarioSimulado.UsuarioActualId)
        {
            TempData["Notificacion_Tipo"] = "danger";
            TempData["Notificacion_Mensaje"] = "Solo el comprador ganador puede confirmar este pago.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        var (ok, mensaje) = await _service.ConfirmarPago(id);

        TempData["Notificacion_Tipo"] = ok ? "success" : "warning";
        TempData["Notificacion_Mensaje"] = mensaje;

        return RedirectToAction(nameof(Detalle), new { id });
    }

    // GET: /Pago/PorSubasta?subastaId=5  — redirige al detalle del pago de la subasta dada
    [HttpGet]
    public async Task<IActionResult> PorSubasta(int subastaId)
    {
        if (subastaId <= 0) return NotFound();

        var pago = await _service.GetBySubastaId(subastaId);
        if (pago is null)
        {
            TempData["Notificacion_Tipo"] = "warning";
            TempData["Notificacion_Mensaje"] = "No se encontró el pago para esta subasta. Puede que aún esté procesándose.";
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Detalle), new { id = pago.PagoId });
    }

    // POST: /Pago/RegistrarManual   (fallback: desde la vista de subastas finalizadas)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarManual(int subastaId, int usuarioGanadorId, decimal montoFinal)
    {
        if (subastaId <= 0 || usuarioGanadorId <= 0 || montoFinal <= 0)
        {
            TempData["Notificacion_Tipo"] = "danger";
            TempData["Notificacion_Mensaje"] = "Datos inválidos para registrar el pago.";
            return RedirectToAction(nameof(Index));
        }

        var (ok, mensaje) = await _service.RegistrarPagoGanador(subastaId, usuarioGanadorId, montoFinal);

        TempData["Notificacion_Tipo"] = ok ? "success" : "warning";
        TempData["Notificacion_Mensaje"] = mensaje;

        // Si se creó, redirigir directo al detalle del pago recién creado
        if (ok)
        {
            var pago = await _service.GetBySubastaId(subastaId);
            if (pago is not null)
                return RedirectToAction(nameof(Detalle), new { id = pago.PagoId });
        }

        return RedirectToAction(nameof(Index));
    }
}
