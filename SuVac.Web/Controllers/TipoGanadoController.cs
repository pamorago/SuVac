using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SuVac.Web.Controllers;

public class TipoGanadoController : Controller
{
    private readonly IServiceTipoGanado _service;

    public TipoGanadoController(IServiceTipoGanado service)
    {
        _service = service;
    }

    // GET: TipoGanadoController
    public async Task<IActionResult> Index()
    {
        var tiposGanado = await _service.GetAll();
        return View(tiposGanado);
    }

    // GET: TipoGanadoController/Details/5
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
            return NotFound();

        var tipoGanado = await _service.GetById(id);
        if (tipoGanado == null)
            return NotFound();

        return View(tipoGanado);
    }

    // GET: TipoGanadoController/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: TipoGanadoController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TipoGanadoDTO dto)
    {
        try
        {
            if (await _service.Create(dto))
                return RedirectToAction(nameof(Index));

            return View(dto);
        }
        catch
        {
            return View(dto);
        }
    }

    // GET: TipoGanadoController/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
            return NotFound();

        var tipoGanado = await _service.GetById(id);
        if (tipoGanado == null)
            return NotFound();

        return View(tipoGanado);
    }

    // POST: TipoGanadoController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TipoGanadoDTO dto)
    {
        if (id <= 0)
            return NotFound();

        try
        {
            dto.TipoGanadoId = id;
            if (await _service.Update(dto))
                return RedirectToAction(nameof(Index));

            return View(dto);
        }
        catch
        {
            return View(dto);
        }
    }

    // GET: TipoGanadoController/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return NotFound();

        var tipoGanado = await _service.GetById(id);
        if (tipoGanado == null)
            return NotFound();

        return View(tipoGanado);
    }

    // POST: TipoGanadoController/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            if (await _service.Delete(id))
                return RedirectToAction(nameof(Index));

            return NotFound();
        }
        catch
        {
            return View();
        }
    }
}
