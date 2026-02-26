using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SuVac.Web.Controllers;

public class GanadoController : Controller
{
    private readonly IServiceGanado _service;
    private readonly IServiceTipoGanado _serviceTipoGanado;
    private readonly IServiceRaza _serviceRaza;

    public GanadoController(IServiceGanado service, IServiceTipoGanado serviceTipoGanado, IServiceRaza serviceRaza)
    {
        _service = service;
        _serviceTipoGanado = serviceTipoGanado;
        _serviceRaza = serviceRaza;
    }

    // GET: GanadoController
    public async Task<IActionResult> Index()
    {
        var ganados = await _service.GetAll();
        return View(ganados);
    }

    // GET: GanadoController/Details/5
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
            return NotFound();

        var ganado = await _service.GetById(id);
        if (ganado == null)
            return NotFound();

        return View(ganado);
    }

    // GET: GanadoController/Create
    public async Task<IActionResult> Create()
    {
        await CargarListas();
        return View();
    }

    // POST: GanadoController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GanadoDTO dto)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _service.Create(dto);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            await CargarListas();
            return View(dto);
        }
        catch
        {
            await CargarListas();
            return View(dto);
        }
    }

    // GET: GanadoController/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
            return NotFound();

        var ganado = await _service.GetById(id);
        if (ganado == null)
            return NotFound();

        await CargarListas();
        return View(ganado);
    }

    // POST: GanadoController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GanadoDTO dto)
    {
        if (id != dto.GanadoId)
            return NotFound();

        try
        {
            if (ModelState.IsValid)
            {
                var result = await _service.Update(dto);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            await CargarListas();
            return View(dto);
        }
        catch
        {
            await CargarListas();
            return View(dto);
        }
    }

    // GET: GanadoController/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return NotFound();

        var ganado = await _service.GetById(id);
        if (ganado == null)
            return NotFound();

        return View(ganado);
    }

    // POST: GanadoController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var result = await _service.Delete(id);
            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
        catch
        {
            return BadRequest();
        }
    }

    private async Task CargarListas()
    {
        var tiposGanado = await _serviceTipoGanado.GetAll();
        var razas = await _serviceRaza.GetAll();

        ViewBag.TiposGanado = new SelectList(tiposGanado, "TipoGanadoId", "Nombre");
        ViewBag.Razas = new SelectList(razas, "RazaId", "Nombre");

        // Para Sexo, usamos una lista hardcodeada (Macho/Hembra)
        ViewBag.Sexos = new SelectList(new[]
        {
            new { id = 1, nombre = "Macho" },
            new { id = 2, nombre = "Hembra" }
        }, "id", "nombre");
    }
}
