using SuVac.Application.DTOs;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Repository.Interfaces;
using SuVac.Web.Hubs;
using SuVac.Web.Models;
using SuVac.Web.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;

namespace SuVac.Web.Controllers;

public class SubastaController : Controller
{
    private readonly IServiceSubasta _service;
    private readonly IServicePuja _servicePuja;
    private readonly IServiceUsuario _serviceUsuario;
    private readonly IHubContext<PujaHub> _hubContext;
    private readonly IRepositoryResultadoSubasta _repoResultado;

    public SubastaController(IServiceSubasta service, IServicePuja servicePuja,
        IServiceUsuario serviceUsuario, IHubContext<PujaHub> hubContext,
        IRepositoryResultadoSubasta repoResultado)
    {
        _service = service;
        _servicePuja = servicePuja;
        _serviceUsuario = serviceUsuario;
        _hubContext = hubContext;
        _repoResultado = repoResultado;
    }

    // ─── Helper: cargar dropdown de ganados y nombre del usuario actual ────
    private async Task CargarDropdownsAsync()
    {
        var ganados = await _service.GetGanadosActivos();
        ViewBag.ListGanado = new SelectList(ganados, "GanadoId", "Nombre");

        var uid = UsuarioHelper.GetUsuarioId(User);
        var usuario = await _serviceUsuario.GetByIdConDetalle(uid);
        ViewBag.NombreUsuarioActual = usuario?.NombreCompleto ?? $"Usuario #{uid}";
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
    [Authorize(Roles = "Admin,Vendedor")]
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

        // Si está finalizada, cargar el ganador
        if (subasta.EstadoSubasta == "Finalizada")
        {
            var resultado = await _repoResultado.GetBySubastaId(subasta.SubastaId);
            if (resultado != null)
            {
                subasta = subasta with
                {
                    NombreGanador = resultado.IdUsuarioGanadorNavigation?.NombreCompleto ?? "Sin ganador",
                    MontoFinal = resultado.MontoFinal
                };
            }
        }

        return View(subasta);
    }

    // ─── CREAR ───────────────────────────────────────────────────────────────
    [Authorize(Roles = "Vendedor")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CargarDropdownsAsync();

        var dto = new SubastaDTO
        {
            UsuarioCreadorId = UsuarioHelper.GetUsuarioId(User),
            NombreCreador = ViewBag.NombreUsuarioActual,
            FechaInicio = DateTime.Now,
            FechaFin = DateTime.Now.AddDays(7)
        };

        return View(dto);
    }

    [Authorize(Roles = "Vendedor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubastaDTO dto, string? accion = null)
    {
        // Siempre forzar el usuario autenticado — nunca tomar del form binding
        dto.UsuarioCreadorId = UsuarioHelper.GetUsuarioId(User);

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
                        ? SweetAlertHelper.CrearNotificacion("Subasta publicada", pubMensaje, SweetAlertMessageType.success)
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
    [Authorize(Roles = "Vendedor")]
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

        // Restricción: no editable si está Finalizada/Cancelada, o si tiene pujas
        var estado = dto.NombreEstadoSubasta;
        if (estado == "Finalizada" || estado == "Cancelada")
        {
            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "No se puede editar",
                $"No se puede editar una subasta {estado.ToLower()}.",
                SweetAlertMessageType.warning);
            return RedirectToAction(nameof(Detalle), new { id });
        }

        var detalle = await _service.GetDetalle(id);
        if (detalle?.TotalPujas > 0)
        {
            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "No se puede editar",
                "No se puede editar: la subasta ya tiene pujas registradas.",
                SweetAlertMessageType.warning);
            return RedirectToAction(nameof(Detalle), new { id });
        }

        return View(dto);
    }

    [Authorize(Roles = "Vendedor")]
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
    [Authorize(Roles = "Vendedor")]
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
    [Authorize(Roles = "Vendedor")]
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

    [Authorize(Roles = "Vendedor")]
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

    // ─── SALA DE PUJAS EN TIEMPO REAL ─────────────────────────────────────────
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Sala(int? id)
    {
        if (id is null or <= 0) return NotFound();

        var subasta = await _service.GetDetalle(id.Value);
        if (subasta is null) return NotFound();

        if (subasta.EstadoSubasta != "Activa")
        {
            TempData["Notificacion"] = SweetAlertHelper.CrearNotificacion(
                "Subasta no activa",
                $"La subasta #{id} no está activa. Estado actual: {subasta.EstadoSubasta}.",
                SweetAlertMessageType.warning);
            return RedirectToAction(nameof(Activas));
        }

        var uid = UsuarioHelper.GetUsuarioId(User);
        var usuarioActual = await _serviceUsuario.GetByIdConDetalle(uid);
        var pujaMasAlta = await _servicePuja.GetPujaMasAlta(id.Value);
        var historial = (await _servicePuja.GetBySubasta(id.Value))
                              .OrderByDescending(p => p.FechaHora)
                              .ToList();

        var vm = new SalaSubastaViewModel
        {
            Subasta = subasta,
            PujaLider = pujaMasAlta,
            Historial = historial,
            UsuarioActualId = uid,
            NombreUsuarioActual = usuarioActual?.NombreCompleto ?? $"Usuario #{uid}",
            EsVendedor = subasta.UsuarioCreadorId == uid
        };

        return View(vm);
    }

    /// <summary>
    /// Registra una nueva puja. Retorna JSON. El broadcast SignalR se realiza aquí.
    /// </summary>
    [Authorize(Roles = "Comprador")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarPuja([FromForm] int subastaId, [FromForm] decimal monto)
    {
        // El usuario SIEMPRE se obtiene de los claims de autenticación, nunca del formulario.
        var uid = UsuarioHelper.GetUsuarioId(User);

        var (ok, mensaje, puja) = await _servicePuja.RegistrarPujaValidada(subastaId, uid, monto);

        if (!ok)
            return Json(new { ok = false, mensaje });

        await _hubContext.Clients.Group($"subasta-{subastaId}")
            .SendAsync("NuevaPuja", new
            {
                pujaId = puja!.PujaId,
                subastaId = puja.SubastaId,
                usuarioId = puja.UsuarioId,
                nombreUsuario = puja.NombreUsuario,
                monto = puja.Monto,
                fechaHora = puja.FechaHora.ToString("dd/MM/yyyy HH:mm:ss")
            });

        return Json(new { ok = true, mensaje });
    }

    /// <summary>
    /// Retorna el estado actual de la sala: si está activa o finalizada, y el ganador.
    /// Usado como fallback cuando SignalR no entregó la notificación.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> EstadoSala(int id)
    {
        var subasta = await _service.GetDetalle(id);
        if (subasta is null)
            return Json(new { ok = false });

        if (subasta.EstadoSubasta != "Finalizada")
            return Json(new { ok = true, finalizada = false });

        var ganador = await _servicePuja.GetPujaMasAlta(id);
        return Json(new
        {
            ok = true,
            finalizada = true,
            ganador = ganador?.NombreUsuario ?? "Sin ganador",
            monto = ganador?.Monto ?? 0m,
            ganadorId = ganador?.UsuarioId ?? 0
        });
    }
}

