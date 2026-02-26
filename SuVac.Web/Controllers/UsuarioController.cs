using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SuVac.Web.Controllers;

public class UsuarioController : Controller
{
    private readonly IServiceUsuario _service;

    public UsuarioController(IServiceUsuario service)
    {
        _service = service;
    }

    // GET: UsuarioController
    public async Task<IActionResult> Index()
    {
        var usuarios = await _service.GetAllConDetalle();
        return View(usuarios);
    }

    // GET: UsuarioController/Details/5
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
            return NotFound();

        var usuario = await _service.GetByIdConDetalle(id);
        if (usuario == null)
            return NotFound();

        return View(usuario);
    }
}
