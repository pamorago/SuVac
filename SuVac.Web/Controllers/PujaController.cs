using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SuVac.Web.Controllers;

public class PujaController : Controller
{
    private readonly IServicePuja _service;

    public PujaController(IServicePuja service)
    {
        _service = service;
    }

    // GET: PujaController
    public async Task<IActionResult> Index()
    {
        var pujas = await _service.GetAll();
        return View(pujas);
    }

    // GET: PujaController/Details/5
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
            return NotFound();

        var puja = await _service.GetById(id);
        if (puja == null)
            return NotFound();

        return View(puja);
    }

    // GET: PujaController/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: PujaController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PujaDTO dto)
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

    // GET: PujaController/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
            return NotFound();

        var puja = await _service.GetById(id);
        if (puja == null)
            return NotFound();

        return View(puja);
    }

    // POST: PujaController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PujaDTO dto)
    {
        if (id != dto.PujaId)
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

    // GET: PujaController/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return NotFound();

        var puja = await _service.GetById(id);
        if (puja == null)
            return NotFound();

        return View(puja);
    }

    // POST: PujaController/Delete/5
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
