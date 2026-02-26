using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SuVac.Web.Controllers;

public class RazaController : Controller
{
    private readonly IServiceRaza _service;

    public RazaController(IServiceRaza service)
    {
        _service = service;
    }

    // GET: RazaController
    public async Task<IActionResult> Index()
    {
        var razas = await _service.GetAll();
        return View(razas);
    }

    // GET: RazaController/Details/5
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
            return NotFound();

        var raza = await _service.GetById(id);
        if (raza == null)
            return NotFound();

        return View(raza);
    }

    // GET: RazaController/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: RazaController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RazaDTO dto)
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

    // GET: RazaController/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
            return NotFound();

        var raza = await _service.GetById(id);
        if (raza == null)
            return NotFound();

        return View(raza);
    }

    // POST: RazaController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RazaDTO dto)
    {
        if (id <= 0)
            return NotFound();

        try
        {
            dto.RazaId = id;
            if (await _service.Update(dto))
                return RedirectToAction(nameof(Index));

            return View(dto);
        }
        catch
        {
            return View(dto);
        }
    }

    // GET: RazaController/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return NotFound();

        var raza = await _service.GetById(id);
        if (raza == null)
            return NotFound();

        return View(raza);
    }

    // POST: RazaController/Delete/5
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
