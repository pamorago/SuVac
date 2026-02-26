using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SuVac.Web.Controllers;

public class RolController : Controller
{
    private readonly IServiceRol _service;

    public RolController(IServiceRol service)
    {
        _service = service;
    }

    // GET: RolController
    public async Task<IActionResult> Index()
    {
        var roles = await _service.GetAll();
        return View(roles);
    }

    // GET: RolController/Details/5
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
            return NotFound();

        var rol = await _service.GetById(id);
        if (rol == null)
            return NotFound();

        return View(rol);
    }

    // GET: RolController/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: RolController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RolDTO dto)
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

    // GET: RolController/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
            return NotFound();

        var rol = await _service.GetById(id);
        if (rol == null)
            return NotFound();

        return View(rol);
    }

    // POST: RolController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RolDTO dto)
    {
        if (id <= 0)
            return NotFound();

        try
        {
            dto.RolId = id;
            if (await _service.Update(dto))
                return RedirectToAction(nameof(Index));

            return View(dto);
        }
        catch
        {
            return View(dto);
        }
    }

    // GET: RolController/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return NotFound();

        var rol = await _service.GetById(id);
        if (rol == null)
            return NotFound();

        return View(rol);
    }

    // POST: RolController/Delete/5
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
