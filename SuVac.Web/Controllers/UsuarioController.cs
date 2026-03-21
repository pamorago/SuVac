using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SuVac.Web.Controllers;

public class UsuarioController : Controller
{
    private readonly IServiceUsuario _service;

    public UsuarioController(IServiceUsuario service)
    {
        _service = service;
    }

    // ─── Utilidad de notificaciones via TempData + SweetAlert ───────────────
    private void Notify(string title, string text, string icon = "success") =>
        TempData["Notificacion"] = JsonSerializer.Serialize(new { title, text, icon });

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

    // GET: Usuario/Edit/5  —  solo permite editar Nombre y Correo
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0) return NotFound();
        var usuario = await _service.GetByIdConDetalle(id);
        if (usuario == null) return NotFound();
        return View(usuario);
    }

    // POST: Usuario/Edit/5  —  solo persiste NombreCompleto y Correo
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UsuarioDTO dto)
    {
        if (id != dto.UsuarioId) return NotFound();

        // Solo validar los campos editables; ignorar el resto
        ModelState.Remove(nameof(dto.Contrasena));
        ModelState.Remove(nameof(dto.RolId));
        ModelState.Remove(nameof(dto.EstadoUsuarioId));

        if (!ModelState.IsValid)
        {
            // Reload full data for read-only fields in view
            var full = await _service.GetByIdConDetalle(id);
            if (full != null)
            {
                dto.NombreRol = full.NombreRol;
                dto.NombreEstado = full.NombreEstado;
                dto.FechaRegistro = full.FechaRegistro;
                dto.CantidadSubastasCreadas = full.CantidadSubastasCreadas;
                dto.CantidadPujasRealizadas = full.CantidadPujasRealizadas;
            }
            return View(dto);
        }

        var result = await _service.UpdatePerfil(id, dto.NombreCompleto, dto.Correo);
        if (result)
        {
            Notify("Perfil actualizado", $"Los datos de {dto.NombreCompleto} fueron guardados exitosamente.");
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "No se pudo actualizar el perfil. Verifique que el correo no esté en uso.");
        return View(dto);
    }

    // POST: Usuario/ToggleEstado/5  —  Bloquear ↔ Activar (cambio lógico de estado)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleEstado(int id)
    {
        var usuario = await _service.GetByIdConDetalle(id);
        if (usuario == null) return NotFound();

        var esActivo = usuario.NombreEstado == "Activo";
        var result = await _service.ToggleEstado(id);

        if (result)
        {
            var nuevoEstado = esActivo ? "bloqueado" : "activado";
            Notify(
                esActivo ? "Usuario bloqueado" : "Usuario activado",
                $"{usuario.NombreCompleto} fue {nuevoEstado} exitosamente.",
                esActivo ? "warning" : "success");
        }
        else
        {
            Notify("Error", "No se pudo cambiar el estado del usuario.", "error");
        }

        return RedirectToAction(nameof(Index));
    }
}
