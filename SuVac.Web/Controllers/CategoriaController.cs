using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SuVac.Web.Controllers;

public class CategoriaController : Controller
{
    private readonly IServiceCategoria _service;

    public CategoriaController(IServiceCategoria service)
    {
        _service = service;
    }

    // GET: CategoriaController
    public async Task<IActionResult> Index()
    {
        var categorias = await _service.ListAsync();
        return View(categorias);
    }

    // GET: CategoriaController/Details/5
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
            return NotFound();

        var categoria = await _service.FindByIdAsync(id);
        if (categoria == null)
            return NotFound();

        return View(categoria);
    }

    // GET: CategoriaController/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CategoriaController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoriaDTO dto)
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

    // GET: CategoriaController/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
            return NotFound();

        var categoria = await _service.FindByIdAsync(id);
        if (categoria == null)
            return NotFound();

        return View(categoria);
    }

    // POST: CategoriaController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoriaDTO dto)
    {
        if (id <= 0)
            return NotFound();

        try
        {
            dto.CategoriaId = id;
            if (await _service.Update(dto))
                return RedirectToAction(nameof(Index));

            return View(dto);
        }
        catch
        {
            return View(dto);
        }
    }

    // GET: CategoriaController/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return NotFound();

        var categoria = await _service.FindByIdAsync(id);
        if (categoria == null)
            return NotFound();

        return View(categoria);
    }

    // POST: CategoriaController/Delete/5
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
