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

    // â”€â”€ Avance 2: vistas de solo lectura â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

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

    // â”€â”€ CRUD â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    // GET: SubastaController
    public async Task<IActionResult> Index()
    {
        var subastas = await _service.GetAll();
        return View(subastas);
    }

    // GET: SubastaController/Details/5
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
            return NotFound();

        var subasta = await _service.GetById(id);
        if (subasta == null)
            return NotFound();

        return View(subasta);
    }

    // GET: SubastaController/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: SubastaController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubastaDTO dto)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _service.Create(dto);
                if (result)
                    return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }
        catch
        {
            return View(dto);
        }
    }

    // GET: SubastaController/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
            return NotFound();

        var subasta = await _service.GetById(id);
        if (subasta == null)
            return NotFound();

        return View(subasta);
    }

    // POST: SubastaController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SubastaDTO dto)
    {
        if (id != dto.SubastaId)
            return NotFound();

        try
        {
            if (ModelState.IsValid)
            {
                var result = await _service.Update(dto);
                if (result)
                    return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }
        catch
        {
            return View(dto);
        }
    }

    // GET: SubastaController/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return NotFound();

        var subasta = await _service.GetById(id);
        if (subasta == null)
            return NotFound();

        return View(subasta);
    }

    // POST: SubastaController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var result = await _service.Delete(id);
            if (result)
                return RedirectToAction(nameof(Index));

            return NotFound();
        }
        catch
        {
            return BadRequest();
        }
    }
}
