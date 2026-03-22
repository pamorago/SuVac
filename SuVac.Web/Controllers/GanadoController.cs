using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;
using SuVac.Web.Util;

namespace SuVac.Web.Controllers;

public class GanadoController : Controller
{
    private readonly IServiceGanado _service;
    private readonly IServiceTipoGanado _serviceTipoGanado;
    private readonly IServiceRaza _serviceRaza;
    private readonly IServiceUsuario _serviceUsuario;
    private readonly IServiceCategoria _serviceCategoria;
    private readonly IWebHostEnvironment _env;

    public GanadoController(IServiceGanado service, IServiceTipoGanado serviceTipoGanado,
        IServiceRaza serviceRaza, IServiceUsuario serviceUsuario, IServiceCategoria serviceCategoria,
        IWebHostEnvironment env)
    {
        _service = service;
        _serviceTipoGanado = serviceTipoGanado;
        _serviceRaza = serviceRaza;
        _serviceUsuario = serviceUsuario;
        _serviceCategoria = serviceCategoria;
        _env = env;
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
        return View(new GanadoDTO
        {
            UsuarioVendedorId = UsuarioSimulado.UsuarioActualId
        });
    }

    // POST: Ganado/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GanadoDTO dto, List<IFormFile>? imagenesArchivos)
    {
        dto.EstadoGanadoId = 1; // Activo al crear
        dto.UsuarioVendedorId = UsuarioSimulado.UsuarioActualId;

        if (dto.CategoriasIds == null || dto.CategoriasIds.Count == 0)
            ModelState.AddModelError("CategoriasIds", "Debe seleccionar al menos una categoría.");

        var archivosValidos = imagenesArchivos?.Where(f => f != null && f.Length > 0).ToList() ?? [];
        if (archivosValidos.Count == 0)
            ModelState.AddModelError("imagenesArchivos", "Debe agregar al menos una imagen.");

        if (!ModelState.IsValid)
        {
            await CargarListas();
            return View(dto);
        }

        var rutas = await GuardarImagenes(archivosValidos);
        if (rutas.Count == 0)
        {
            ModelState.AddModelError("imagenesArchivos", "No se pudo guardar la imagen. Use JPG, PNG, GIF o WEBP (máx. 5 MB).");
            await CargarListas();
            return View(dto);
        }

        dto.ImagenesGanado = rutas.Select(r => new ImagenGanadoDTO { UrlImagen = r }).ToList();

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

        // Regla: no editar si participó en alguna subasta (activa o finalizada)
        if (ganado.SubastasParticipacion != null && ganado.SubastasParticipacion.Any())
        {
            var estadoActiva = ganado.SubastasParticipacion.Any(s => s.EstadoSubasta == "Activa");
            var mensaje = estadoActiva
                ? $"\"{ganado.Nombre}\" pertenece a una subasta activa y no puede ser editado."
                : $"\"{ganado.Nombre}\" ya participó en una subasta finalizada y no puede ser editado.";
            Notify("Edición no permitida", mensaje, "warning");
            return RedirectToAction(nameof(Index));
        }

        await CargarListas(ganado.CategoriasIds);
        return View(ganado);
    }

    // POST: Ganado/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GanadoDTO dto,
        List<string>? imagenesExistentes, List<IFormFile>? imagenesNuevas)
    {
        if (id != dto.GanadoId) return NotFound();

        // Protección POST: verificar regla de negocio también en el POST
        var ganadoActual = await _service.GetById(id);
        if (ganadoActual?.SubastasParticipacion != null && ganadoActual.SubastasParticipacion.Any())
        {
            Notify("Edición no permitida",
                $"\"{ganadoActual.Nombre}\" ya participó en una subasta y no puede ser editado.", "warning");
            return RedirectToAction(nameof(Index));
        }

        if (dto.CategoriasIds == null || dto.CategoriasIds.Count == 0)
            ModelState.AddModelError("CategoriasIds", "Debe seleccionar al menos una categoría.");

        var existentes = imagenesExistentes?.Where(u => !string.IsNullOrWhiteSpace(u)).ToList() ?? [];
        var nuevas = imagenesNuevas?.Where(f => f != null && f.Length > 0).ToList() ?? [];
        if (existentes.Count == 0 && nuevas.Count == 0)
            ModelState.AddModelError("imagenesNuevas", "Debe mantener o agregar al menos una imagen.");

        if (!ModelState.IsValid)
        {
            await CargarListas(dto.CategoriasIds);
            return View(dto);
        }

        var rutasNuevas = await GuardarImagenes(nuevas);
        dto.ImagenesGanado = existentes
            .Select(u => new ImagenGanadoDTO { UrlImagen = u })
            .Concat(rutasNuevas.Select(r => new ImagenGanadoDTO { UrlImagen = r }))
            .ToList();

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

        // Regla: si el ganado alguna vez participó en una subasta, su estado es inmutable
        if (ganado.SubastasParticipacion != null && ganado.SubastasParticipacion.Any())
        {
            Notify("Operación no permitida",
                $"\"{ganado.Nombre}\" ya participó en al menos una subasta. Su estado no puede modificarse.",
                "warning");
            return RedirectToAction(nameof(Index));
        }

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

    // ─── Guardar archivos subidos a wwwroot/images/ganado/ ─────────────────
    private async Task<List<string>> GuardarImagenes(List<IFormFile> archivos)
    {
        var rutas = new List<string>();
        var ext_permitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var carpeta = Path.Combine(_env.WebRootPath, "images", "ganado");
        Directory.CreateDirectory(carpeta);

        foreach (var archivo in archivos)
        {
            if (archivo.Length > 5 * 1024 * 1024) continue; // max 5 MB
            var ext = Path.GetExtension(archivo.FileName);
            if (!ext_permitidas.Contains(ext)) continue;

            var nombreArchivo = $"{Guid.NewGuid()}{ext}";
            var rutaFisica = Path.Combine(carpeta, nombreArchivo);
            await using var stream = new FileStream(rutaFisica, FileMode.Create);
            await archivo.CopyToAsync(stream);
            rutas.Add($"/images/ganado/{nombreArchivo}");
        }
        return rutas;
    }

    private async Task CargarListas(List<int>? categoriasSeleccionadas = null)
    {
        var tiposGanado = await _serviceTipoGanado.GetAll();
        var razas = await _serviceRaza.GetAll();
        var categorias = await _serviceCategoria.ListAsync();
        var usuarioActual = await _serviceUsuario.GetByIdConDetalle(UsuarioSimulado.UsuarioActualId);

        ViewBag.TiposGanado = new SelectList(tiposGanado, "TipoGanadoId", "Nombre");
        ViewBag.Razas = new SelectList(razas, "RazaId", "Nombre");
        ViewBag.Categorias = categorias; // ICollection<CategoriaDTO> para checkboxes
        ViewBag.UsuarioActualNombre = usuarioActual?.NombreCompleto ?? $"Usuario #{UsuarioSimulado.UsuarioActualId}";

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
