using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace SuVac.Web.Controllers;

public class GanadoController : Controller
{
    private readonly IServiceGanado _service;
    private readonly IServiceTipoGanado _serviceTipoGanado;
    private readonly IServiceRaza _serviceRaza;
    private readonly IServiceUsuario _serviceUsuario;
    private readonly IServiceCategoria _serviceCategoria;

    public GanadoController(IServiceGanado service, IServiceTipoGanado serviceTipoGanado,
        IServiceRaza serviceRaza, IServiceUsuario serviceUsuario, IServiceCategoria serviceCategoria)
    {
        _service = service;
        _serviceTipoGanado = serviceTipoGanado;
        _serviceRaza = serviceRaza;
        _serviceUsuario = serviceUsuario;
        _serviceCategoria = serviceCategoria;
    }

    // ─── Utilidad de notificaciones via TempData + SweetAlert ───────────────
    private void Notify(string title, string text, string icon = "success") =>
        TempData["Notificacion"] = JsonSerializer.Serialize(new { title, text, icon });

    // GET: Ganado
    public async Task<IActionResult> Index()
    {
        var ganados = await _service.GetAll();
        return View(ganados);
    }

    // GET: Ganado/Details/5
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0) return NotFound();
        var ganado = await _service.GetById(id);
        if (ganado == null) return NotFound();
        return View(ganado);
    }

    // GET: Ganado/Create
    public async Task<IActionResult> Create()
    {
        await CargarListas();
        return View();
    }

    // POST: Ganado/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GanadoDTO dto)
    {
        dto.EstadoGanadoId = 1; // Activo al crear

        // Validar vendedor seleccionado
        if (dto.UsuarioVendedorId <= 0)
            ModelState.AddModelError("UsuarioVendedorId", "Debe seleccionar un usuario vendedor.");

        // Validar categorías (mínimo 1)
        if (dto.CategoriasIds == null || dto.CategoriasIds.Count == 0)
            ModelState.AddModelError("CategoriasIds", "Debe seleccionar al menos una categoría.");

        // Validar imágenes (mínimo 1 URL no vacía)
        var urlsValidas = dto.ImagenesGanado?
            .Where(i => !string.IsNullOrWhiteSpace(i.UrlImagen))
            .ToList();
        if (urlsValidas == null || urlsValidas.Count == 0)
            ModelState.AddModelError("ImagenesGanado", "Debe agregar al menos una imagen.");
        else
            dto.ImagenesGanado = urlsValidas;

        if (!ModelState.IsValid)
        {
            await CargarListas();
            return View(dto);
        }

        var result = await _service.Create(dto);
        if (result)
        {
            Notify("Ganado registrado", $"El ganado \"{dto.Nombre}\" fue creado exitosamente.");
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "No se pudo guardar el ganado. Intente nuevamente.");
        await CargarListas();
        return View(dto);
    }

    // GET: Ganado/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0) return NotFound();

        var ganado = await _service.GetById(id);
        if (ganado == null) return NotFound();

        // Regla: no editar si está en subasta activa (EstadoSubastaId = 2)
        if (ganado.SubastasParticipacion != null &&
            ganado.SubastasParticipacion.Any(s => s.EstadoSubasta == "Activa"))
        {
            Notify("Edición no permitida",
                $"\"{ganado.Nombre}\" pertenece a una subasta activa y no puede ser editado.",
                "warning");
            return RedirectToAction(nameof(Index));
        }

        await CargarListas(ganado.CategoriasIds);
        return View(ganado);
    }

    // POST: Ganado/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GanadoDTO dto)
    {
        if (id != dto.GanadoId) return NotFound();

        // Validar categorías (mínimo 1)
        if (dto.CategoriasIds == null || dto.CategoriasIds.Count == 0)
            ModelState.AddModelError("CategoriasIds", "Debe seleccionar al menos una categoría.");

        // Validar imágenes (mínimo 1 URL válida)
        var urlsValidas = dto.ImagenesGanado?
            .Where(i => !string.IsNullOrWhiteSpace(i.UrlImagen))
            .ToList();
        if (urlsValidas == null || urlsValidas.Count == 0)
            ModelState.AddModelError("ImagenesGanado", "Debe agregar al menos una imagen.");
        else
            dto.ImagenesGanado = urlsValidas;

        if (!ModelState.IsValid)
        {
            await CargarListas(dto.CategoriasIds);
            return View(dto);
        }

        // Preservar UsuarioVendedorId y FechaRegistro del registro original (no editables)
        var original = await _service.GetById(id);
        if (original == null) return NotFound();
        dto.UsuarioVendedorId = original.UsuarioVendedorId;
        dto.FechaRegistro = original.FechaRegistro;

        var result = await _service.Update(dto);
        if (result)
        {
            Notify("Ganado actualizado", $"\"{dto.Nombre}\" fue actualizado exitosamente.");
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "No se pudo actualizar el ganado. Intente nuevamente.");
        await CargarListas(dto.CategoriasIds);
        return View(dto);
    }

    // GET: Ganado/Delete/5  —  Confirmación de eliminación lógica
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0) return NotFound();

        var ganado = await _service.GetById(id);
        if (ganado == null) return NotFound();

        // Verificar reglas de negocio para eliminación lógica
        bool enSubastaActiva = ganado.SubastasParticipacion?
            .Any(s => s.EstadoSubasta == "Activa") == true;
        bool fueSubastado = ganado.SubastasParticipacion?.Count > 0;

        ViewBag.PuedeEliminar = !enSubastaActiva && !fueSubastado;
        ViewBag.EnSubastaActiva = enSubastaActiva;
        ViewBag.FueSubastado = fueSubastado;

        return View(ganado);
    }

    // POST: Ganado/Delete/5  —  Eliminación lógica (pone en Inactivo)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ganado = await _service.GetById(id);
        if (ganado == null) return NotFound();

        // Re-validar condiciones
        bool enSubastaActiva = ganado.SubastasParticipacion?
            .Any(s => s.EstadoSubasta == "Activa") == true;
        bool fueSubastado = ganado.SubastasParticipacion?.Count > 0;

        if (enSubastaActiva || fueSubastado)
        {
            Notify("Operación no permitida",
                "Este ganado no puede desactivarse porque ha participado en una subasta o está en una activa.",
                "error");
            return RedirectToAction(nameof(Index));
        }

        var result = await _service.Delete(id); // Lógico: pone EstadoGanadoId = 2
        if (result)
            Notify("Ganado desactivado", $"\"{ganado.Nombre}\" fue desactivado del sistema.");
        else
            Notify("Error", "No se pudo desactivar el ganado.", "error");

        return RedirectToAction(nameof(Index));
    }

    // POST: Ganado/ToggleEstado/5  —  Activar / Desactivar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleEstado(int id)
    {
        var ganado = await _service.GetById(id);
        if (ganado == null) return NotFound();

        int nuevoEstadoId = ganado.NombreEstadoGanado == "Activo" ? 2 : 1;
        var result = await _service.ToggleEstado(id, nuevoEstadoId);

        if (result)
        {
            var mensaje = nuevoEstadoId == 2 ? "desactivado" : "activado";
            Notify($"Ganado {mensaje}", $"\"{ganado.Nombre}\" fue {mensaje} exitosamente.",
                nuevoEstadoId == 2 ? "warning" : "success");
        }
        else
        {
            Notify("Error", "No se pudo cambiar el estado del ganado.", "error");
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListas(List<int>? categoriasSeleccionadas = null)
    {
        var tiposGanado = await _serviceTipoGanado.GetAll();
        var razas = await _serviceRaza.GetAll();
        // Solo mostrar usuarios con rol Vendedor (RolId = 2)
        var todosUsuarios = await _serviceUsuario.GetAllConDetalle();
        var vendedores = todosUsuarios.Where(u => u.NombreRol == "Vendedor");
        var categorias = await _serviceCategoria.ListAsync();

        ViewBag.TiposGanado = new SelectList(tiposGanado, "TipoGanadoId", "Nombre");
        ViewBag.Razas = new SelectList(razas, "RazaId", "Nombre");
        ViewBag.Vendedores = new SelectList(vendedores, "UsuarioId", "NombreCompleto");
        ViewBag.Categorias = categorias; // ICollection<CategoriaDTO> para checkboxes

        ViewBag.Sexos = new SelectList(new[]
        {
            new { id = 1, nombre = "Macho" },
            new { id = 2, nombre = "Hembra" }
        }, "id", "nombre");

        ViewBag.EstadosGanado = new SelectList(new[]
        {
            new { id = 1, nombre = "Activo" },
            new { id = 2, nombre = "Inactivo" }
        }, "id", "nombre");

        ViewBag.CategoriasSeleccionadas = categoriasSeleccionadas ?? new List<int>();
    }
}
