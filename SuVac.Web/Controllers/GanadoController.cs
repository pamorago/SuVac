using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SuVac.Web.Controllers;

public class GanadoController : Controller
{
    private readonly IServiceGanado _service;

    public GanadoController(IServiceGanado service)
    {
        _service = service;
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
    public IActionResult Create()
    {
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
            return View(dto);
        }
        catch
        {
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
            return View(dto);
        }
        catch
        {
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
}
