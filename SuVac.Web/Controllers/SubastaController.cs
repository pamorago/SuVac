using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Web.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SuVac.Web.Controllers;

public class SubastaController : Controller
{
    private readonly IServiceSubasta _service;
    private readonly IServicePuja _servicePuja;
    private readonly IServiceUsuario _serviceUsuario;

    public SubastaController(IServiceSubasta service, IServicePuja servicePuja, IServiceUsuario serviceUsuario)
    {
        _service = service;
        _servicePuja = servicePuja;
        _serviceUsuario = serviceUsuario;
    }

    // ─── Helper: cargar dropdown de ganados y nombre del usuario simulado ────
    private async Task CargarDropdownsAsync()
    {
        var ganados = await _service.GetGanadosActivos();
        ViewBag.ListGanado = new SelectList(ganados, "GanadoId", "Nombre");

        var usuario = await _serviceUsuario.GetByIdConDetalle(UsuarioSimulado.UsuarioActualId);
        ViewBag.NombreUsuarioActual = usuario?.NombreCompleto ?? $"Usuario #{UsuarioSimulado.UsuarioActualId}";
    }

    // ─── LISTADO PÚBLICO ─────────────────────────────────────────────────────
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

    // ─── ADMIN: LISTADO COMPLETO ─────────────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        var subastas = await _service.GetAllAdmin();
        var total = subastas.Count();

        if (TempData["Notificacion"] == null)
        {
            ViewBag.Notificacion = SweetAlertHelper.CrearNotificacion(
                "Gestión de Subastas",
                $"Total de subastas registradas: {total}.",
                SweetAlertMessageType.info
            );
        }

        return View(subastas);
    }

    // ─── DETALLE ─────────────────────────────────────────────────────────────
    public async Task<IActionResult> Detalle(int? id, string? from = null)
    {
        if (id is null or <= 0)
        {
            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "Error", "ID de subasta inválido.", SweetAlertMessageType.error);
            return RedirectToAction(nameof(Index));
        }

        var subasta = await _service.GetDetalle(id.Value);
        if (subasta is null)
        {
            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "No encontrada", $"No existe una subasta con ID {id}.", SweetAlertMessageType.error);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Notificacion = SweetAlertHelper.CrearNotificacion(
            "Detalle de la Subasta",
            $"Mostrando información de la subasta #{subasta.SubastaId}: {subasta.NombreGanado}",
            SweetAlertMessageType.info
        );
        ViewBag.From = from;

        return View(subasta);
    }

    // ─── CREAR ───────────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CargarDropdownsAsync();

        var dto = new SubastaDTO
        {
            UsuarioCreadorId = UsuarioSimulado.UsuarioActualId,
            NombreCreador = ViewBag.NombreUsuarioActual,
            FechaInicio = DateTime.Now.AddDays(1),
            FechaFin = DateTime.Now.AddDays(8)
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubastaDTO dto, string? accion = null)
    {
        // Siempre forzar el usuario simulado — nunca tomar del form binding
        dto.UsuarioCreadorId = UsuarioSimulado.UsuarioActualId;

        // Los campos no editables no deben generar errores de validación
        ModelState.Remove(nameof(SubastaDTO.EstadoSubastaId));
        ModelState.Remove(nameof(SubastaDTO.NombreCreador));
        ModelState.Remove(nameof(SubastaDTO.NombreGanado));
        ModelState.Remove(nameof(SubastaDTO.NombreEstadoSubasta));

        if (!ModelState.IsValid)
        {
            await CargarDropdownsAsync();
            dto.NombreCreador = ViewBag.NombreUsuarioActual;
            return View(dto);
        }

        try
        {
            var (ok, mensaje, subastaId) = await _service.CreateValidado(dto);

            if (ok)
            {
                if (accion == "publicar")
                {
                    var (pubOk, pubMensaje) = await _service.Publicar(subastaId);
                    TempData["Notificacion"] = pubOk
                        ? SweetAlertHelper.CrearNotificacion("Subasta publicada", "La subasta fue creada y publicada como Programada.", SweetAlertMessageType.success)
                        : SweetAlertHelper.CrearNotificacion("Creada pero no publicada", pubMensaje, SweetAlertMessageType.warning);
                }
                else
                {
                    TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                        "Subasta creada",
                        "La subasta fue registrada como borrador. Publíquela cuando esté lista.",
                        SweetAlertMessageType.success
                    );
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Notificacion = SweetAlertHelper.CrearNotificacion(
                "No se pudo crear", mensaje, SweetAlertMessageType.error);
            await CargarDropdownsAsync();
            dto.NombreCreador = ViewBag.NombreUsuarioActual;
            return View(dto);
        }
        catch (Exception ex)
        {
            ViewBag.Notificacion = SweetAlertHelper.CrearNotificacion(
                "Error inesperado", ex.Message, SweetAlertMessageType.error);
            await CargarDropdownsAsync();
            dto.NombreCreador = ViewBag.NombreUsuarioActual;
            return View(dto);
        }
    }

    // ─── EDITAR ──────────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "Error", "ID de subasta inválido.", SweetAlertMessageType.error);
            return RedirectToAction(nameof(Index));
        }

        var dto = await _service.GetById(id);
        if (dto == null)
        {
            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "No encontrada", $"No existe subasta con ID {id}.", SweetAlertMessageType.error);
            return RedirectToAction(nameof(Index));
        }

        // Restricción: no editable si tiene pujas o está Finalizada/Cancelada
        var detalle = await _service.GetDetalle(id);
        var tienePujas = detalle?.TotalPujas > 0;
        if (tienePujas || dto.NombreEstadoSubasta == "Finalizada" || dto.NombreEstadoSubasta == "Cancelada")
        {
            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "No se puede editar",
                tienePujas
                    ? "No se puede editar: la subasta ya tiene pujas registradas."
                    : $"No se puede editar una subasta en estado {dto.NombreEstadoSubasta}.",
                SweetAlertMessageType.warning
            );
            return RedirectToAction(nameof(Detalle), new { id });
        }

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SubastaDTO dto)
    {
        if (id != dto.SubastaId)
        {
            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "Error", "El identificador de la subasta no coincide.", SweetAlertMessageType.error);
            return RedirectToAction(nameof(Index));
        }

        // Quitar validaciones de campos que no se editan en este formulario
        ModelState.Remove(nameof(SubastaDTO.GanadoId));
        ModelState.Remove(nameof(SubastaDTO.UsuarioCreadorId));
        ModelState.Remove(nameof(SubastaDTO.EstadoSubastaId));
        ModelState.Remove(nameof(SubastaDTO.NombreCreador));
        ModelState.Remove(nameof(SubastaDTO.NombreGanado));
        ModelState.Remove(nameof(SubastaDTO.NombreEstadoSubasta));

        if (!ModelState.IsValid)
        {
            var errores = string.Join("<br>",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

            ViewBag.Notificacion = SweetAlertHelper.CrearNotificacion(
                "Errores de validación",
                $"Corrija los siguientes errores:<br>{errores}",
                SweetAlertMessageType.warning
            );
            return View(dto);
        }

        try
        {
            var (ok, mensaje) = await _service.UpdateValidado(dto);

            if (ok)
            {
                TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                    "Subasta actualizada",
                    $"La subasta #{id} fue actualizada correctamente.",
                    SweetAlertMessageType.success
                );
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Notificacion = SweetAlertHelper.CrearNotificacion(
                "No se pudo actualizar", mensaje, SweetAlertMessageType.error);
            return View(dto);
        }
        catch (Exception ex)
        {
            ViewBag.Notificacion = SweetAlertHelper.CrearNotificacion(
                "Error inesperado", ex.Message, SweetAlertMessageType.error);
            return View(dto);
        }
    }

    // ─── PUBLICAR ────────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publicar(int id)
    {
        try
        {
            var (ok, mensaje) = await _service.Publicar(id);
            TempData["Notificacion"] = ok
                ? SweetAlertHelper.CrearNotificacion("Subasta publicada", mensaje, SweetAlertMessageType.success)
                : SweetAlertHelper.CrearNotificacion("No se pudo publicar", mensaje, SweetAlertMessageType.error);
        }
        catch (Exception ex)
        {
            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "Error inesperado", ex.Message, SweetAlertMessageType.error);
        }

        return RedirectToAction(nameof(Index));
    }

    // ─── CANCELAR ────────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Cancelar(int id)
    {
        if (id <= 0)
        {
            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "Error", "ID de subasta inválido.", SweetAlertMessageType.error);
            return RedirectToAction(nameof(Index));
        }

        var detalle = await _service.GetDetalle(id);
        if (detalle == null)
        {
            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "No encontrada", $"No existe subasta con ID {id}.", SweetAlertMessageType.error);
            return RedirectToAction(nameof(Index));
        }

        if (detalle.EstadoSubasta == "Finalizada" || detalle.EstadoSubasta == "Cancelada")
        {
            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "No se puede cancelar",
                $"Una subasta <strong>{detalle.EstadoSubasta}</strong> no puede cancelarse.",
                SweetAlertMessageType.warning);
            return RedirectToAction(nameof(Detalle), new { id });
        }

        if (detalle.TotalPujas > 0)
        {
            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "No se puede cancelar",
                "La subasta ya tiene pujas registradas y no puede cancelarse.",
                SweetAlertMessageType.warning);
            return RedirectToAction(nameof(Detalle), new { id });
        }

        return View(detalle);
    }

    [HttpPost, ActionName("CancelarConfirmado")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelarConfirmado(int id)
    {
        try
        {
            var (ok, mensaje) = await _service.Cancelar(id);
            TempData["Notificacion"] = ok
                ? SweetAlertHelper.CrearNotificacion("Subasta cancelada", mensaje, SweetAlertMessageType.success)
                : SweetAlertHelper.CrearNotificacion("No se pudo cancelar", mensaje, SweetAlertMessageType.error);
        }
        catch (Exception ex)
        {
            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "Error inesperado", ex.Message, SweetAlertMessageType.error);
        }

        return RedirectToAction(nameof(Index));
    }
}

