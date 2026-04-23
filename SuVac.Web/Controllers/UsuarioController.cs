using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Web.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace SuVac.Web.Controllers;

[Authorize(Roles = "Admin")]
public class UsuarioController : Controller
{
    private readonly IServiceUsuario _service;
    private readonly IServiceRol _serviceRol;

    public UsuarioController(IServiceUsuario service, IServiceRol serviceRol)
    {
        _service = service;
        _serviceRol = serviceRol;
    }

    private async Task CargarRoles()
    {
        var roles = await _serviceRol.GetAll();
        ViewBag.Roles = new SelectList(roles, "RolId", "Nombre");
        ViewBag.EstadosUsuario = new SelectList(new[]
        {
            new { id = 1, nombre = "Activo" },
            new { id = 2, nombre = "Bloqueado" }
        }, "id", "nombre");
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

    // GET: Usuario/Create
    public async Task<IActionResult> Create()
    {
        await CargarRoles();
        return View();
    }

    // POST: Usuario/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioDTO dto)
    {
        // Validaciones manuales de campos no cubiertos por anotaciones
        if (string.IsNullOrWhiteSpace(dto.Contrasena))
            ModelState.AddModelError(nameof(dto.Contrasena), "La contraseña es obligatoria.");
        else if (dto.Contrasena.Length < 6)
            ModelState.AddModelError(nameof(dto.Contrasena), "La contraseña debe tener al menos 6 caracteres.");

        if (dto.RolId <= 0)
            ModelState.AddModelError(nameof(dto.RolId), "Debe seleccionar un rol.");

        if (dto.EstadoUsuarioId <= 0)
            ModelState.AddModelError(nameof(dto.EstadoUsuarioId), "Debe seleccionar un estado.");

        if (!ModelState.IsValid)
        {
            await CargarRoles();
            return View(dto);
        }

        dto.FechaRegistro = DateTime.Now;

        var result = await _service.Create(dto);
        if (result)
        {
            Notify("Usuario registrado", $"{dto.NombreCompleto} fue creado exitosamente.");
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "No se pudo registrar el usuario. Verifique que el correo no esté en uso.");
        await CargarRoles();
        return View(dto);
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

    // ─── Perfil del usuario autenticado ─────────────────────────────────────

    /// <summary>
    /// Muestra el perfil del usuario que inició sesión.
    /// </summary>
    // MiPerfil está en LoginController para evitar restricción de rol Admin
}
