using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SuVac.Web.Controllers;

public class UsuarioController : Controller
{
    private readonly IServiceUsuario _service;
    private readonly IServiceRol _serviceRol;

    public UsuarioController(IServiceUsuario service, IServiceRol serviceRol)
    {
        _service = service;
        _serviceRol = serviceRol;
    }

    // GET: Usuario
    public async Task<IActionResult> Index()
    {
        var usuarios = await _service.GetAllConDetalle();
        return View(usuarios);
    }

    // GET: Usuario/Details/5
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0) return NotFound();
        var usuario = await _service.GetByIdConDetalle(id);
        if (usuario == null) return NotFound();
        return View(usuario);
    }

    // GET: Usuario/Create
    public async Task<IActionResult> Create()
    {
        await CargarListas();
        return View();
    }

    // POST: Usuario/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioDTO dto)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _service.Create(dto);
                if (result) return RedirectToAction(nameof(Index));
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

    // GET: Usuario/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0) return NotFound();
        var usuario = await _service.GetById(id);
        if (usuario == null) return NotFound();
        await CargarListas();
        return View(usuario);
    }

    // POST: Usuario/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UsuarioDTO dto)
    {
        if (id != dto.UsuarioId) return NotFound();
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _service.Update(dto);
                if (result) return RedirectToAction(nameof(Index));
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

    // GET: Usuario/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0) return NotFound();
        var usuario = await _service.GetByIdConDetalle(id);
        if (usuario == null) return NotFound();
        return View(usuario);
    }

    // POST: Usuario/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var result = await _service.Delete(id);
            if (result) return RedirectToAction(nameof(Index));
            return NotFound();
        }
        catch
        {
            return BadRequest();
        }
    }

    private async Task CargarListas()
    {
        var roles = await _serviceRol.GetAll();
        ViewBag.Roles = new SelectList(roles, "RolId", "Nombre");
        ViewBag.EstadosUsuario = new SelectList(new[]
        {
            new { id = 1, nombre = "Activo" },
            new { id = 2, nombre = "Bloqueado" }
        }, "id", "nombre");
    }
}
