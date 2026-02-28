using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SuVac.Web.Controllers;

public class SubastaController : Controller
{
    private readonly IServiceSubasta _service;
    private readonly IServicePuja _servicePuja;

    public SubastaController(IServiceSubasta service, IServicePuja servicePuja)
    {
        _service = service;
        _servicePuja = servicePuja;
    }

    public async Task<IActionResult> Activas()
    {
        var subastas = await _service.GetActivas();
        return View(subastas);
    }

    public async Task<IActionResult> Finalizadas()
    {
        var subastas = await _service.GetFinalizadas();
        return View(subastas);
    }

    public async Task<IActionResult> Detalle(int? id)
    {
        if (id is null or <= 0) return NotFound();

        var subasta = await _service.GetDetalle(id.Value);
        if (subasta is null) return NotFound();

        return View(subasta);
    }

    public async Task<IActionResult> HistorialPujas(int? id)
    {
        if (id is null or <= 0) return NotFound();

        var subasta = await _service.GetDetalle(id.Value);
        if (subasta is null) return NotFound();

        var pujas = await _servicePuja.GetBySubasta(id.Value);

        ViewBag.SubastaId = id.Value;
        ViewBag.NombreGanado = subasta.NombreGanado;
        ViewBag.EstadoSubasta = subasta.EstadoSubasta;
        ViewBag.TotalPujas = subasta.TotalPujas;

        return View(pujas.ToList());
    }

    // GET: SubastaController/Create
    public IActionResult Create(string? from = null)
    {
        ViewBag.From = from ?? "activas";
        return View();
    }

    // POST: SubastaController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubastaDTO dto, string? from = null)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _service.Create(dto);
                if (result)
                {
                    return RedirectToAction(nameof(Activas));
                }
            }
            ViewBag.From = from ?? "activas";
            return View(dto);
        }
        catch
        {
            ViewBag.From = from ?? "activas";
            return View(dto);
        }
    }

    // GET: SubastaController/Edit/5
    public async Task<IActionResult> Edit(int id, string? from = null)
    {
        if (id <= 0)
            return NotFound();

        var subasta = await _service.GetById(id);
        if (subasta == null)
            return NotFound();

        ViewBag.From = from ?? "activas";
        return View(subasta);
    }

    // POST: SubastaController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SubastaDTO dto, string? from = null)
    {
        if (id != dto.SubastaId)
            return NotFound();

        try
        {
            if (ModelState.IsValid)
            {
                var result = await _service.Update(dto);
                if (result)
                {
                    return RedirectToAction(from == "finalizadas" ? nameof(Finalizadas) : nameof(Activas));
                }
            }
            ViewBag.From = from ?? "activas";
            return View(dto);
        }
        catch
        {
            ViewBag.From = from ?? "activas";
            return View(dto);
        }
    }

    // GET: SubastaController/Delete/5
    public async Task<IActionResult> Delete(int id, string? from = null)
    {
        if (id <= 0)
            return NotFound();

        var subasta = await _service.GetById(id);
        if (subasta == null)
            return NotFound();

        ViewBag.From = from ?? "activas";
        return View(subasta);
    }

    // POST: SubastaController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, string? from = null)
    {
        try
        {
            var result = await _service.Delete(id);
            if (result)
            {
                return RedirectToAction(from == "finalizadas" ? nameof(Finalizadas) : nameof(Activas));
            }
            return NotFound();
        }
        catch
        {
            return BadRequest();
        }
    }
}
