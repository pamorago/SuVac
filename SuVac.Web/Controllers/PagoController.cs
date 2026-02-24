using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SuVac.Web.Controllers;

public class PagoController : Controller
{
    private readonly IServicePago _service;

    public PagoController(IServicePago service)
    {
        _service = service;
    }

    // GET: PagoController
    public async Task<IActionResult> Index()
    {
        var pagos = await _service.GetAll();
        return View(pagos);
    }

    // GET: PagoController/Details/5
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
            return NotFound();

        var pago = await _service.GetById(id);
        if (pago == null)
            return NotFound();

        return View(pago);
    }

    // GET: PagoController/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: PagoController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PagoDTO dto)
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

    // GET: PagoController/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
            return NotFound();

        var pago = await _service.GetById(id);
        if (pago == null)
            return NotFound();

        return View(pago);
    }

    // POST: PagoController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PagoDTO dto)
    {
        if (id != dto.PagoId)
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

    // GET: PagoController/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return NotFound();

        var pago = await _service.GetById(id);
        if (pago == null)
            return NotFound();

        return View(pago);
    }

    // POST: PagoController/Delete/5
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
