using Microsoft.AspNetCore.Mvc;
using SuVac.Application.Servicios.Interfaces;

namespace SuVac.web.Controllers;

public class SubastaController : Controller
{
    private readonly IServicioSubasta _servicioSubasta;
    private readonly IServicioPuja _servicioPuja;

    public SubastaController(IServicioSubasta servicioSubasta, IServicioPuja servicioPuja)
    {
        _servicioSubasta = servicioSubasta;
        _servicioPuja = servicioPuja;
    }

    // GET: /Subasta/Activas
    [HttpGet]
    public async Task<IActionResult> Activas()
    {
        var lista = await _servicioSubasta.ListarActivasAsync();
        return View(lista);
    }

    // GET: /Subasta/Finalizadas
    [HttpGet]
    public async Task<IActionResult> Finalizadas()
    {
        var lista = await _servicioSubasta.ListarFinalizadasAsync();
        return View(lista);
    }

    // GET: /Subasta/Detalle/5
    [HttpGet]
    public async Task<IActionResult> Detalle(int? id)
    {
        if (id is null)
            return RedirectToAction(nameof(Activas));

        var subasta = await _servicioSubasta.BuscarPorIdAsync(id.Value);

        if (subasta is null)
        {
            TempData["MensajeError"] = $"No se encontró la subasta con Id {id}.";
            return RedirectToAction(nameof(Activas));
        }

        return View(subasta);
    }

    // GET: /Subasta/HistorialPujas/5
    [HttpGet]
    public async Task<IActionResult> HistorialPujas(int? id)
    {
        if (id is null)
            return RedirectToAction(nameof(Activas));

        // Verificar que la subasta existe
        var subasta = await _servicioSubasta.BuscarPorIdAsync(id.Value);
        if (subasta is null)
        {
            TempData["MensajeError"] = $"No se encontró la subasta con Id {id}.";
            return RedirectToAction(nameof(Activas));
        }

        // Obtener el historial de pujas - solo las que pertenecen a esta subasta
        var pujas = await _servicioPuja.ListarPorSubastaAsync(id.Value);

        ViewBag.SubastaId = id.Value;
        ViewBag.NombreGanado = subasta.NombreGanado;
        ViewBag.EstadoSubasta = subasta.EstadoSubasta;
        ViewBag.TotalPujas = pujas.Count;

        return View(pujas);
    }
}
