using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuVac.Application.Services.Interfaces;
using SuVac.Web.Util;
using SuVac.Web.ViewModels;
using System.Security.Claims;
using System.Text.Json;

namespace SuVac.Web.Controllers;

public class LoginController : Controller
{
    private readonly IServiceUsuario _serviceUsuario;
    private readonly ILogger<LoginController> _logger;

    public LoginController(IServiceUsuario serviceUsuario, ILogger<LoginController> logger)
    {
        _serviceUsuario = serviceUsuario;
        _logger = logger;
    }

    // GET /Login/Index
    [HttpGet]
    public IActionResult Index()
    {
        // Si ya está autenticado, redirigir al inicio
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    // POST /Login/LogIn
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogIn(ViewModelLogin viewModelLogin)
    {
        if (!ModelState.IsValid)
        {
            TempData["LoginError"] = "Por favor corregí los errores del formulario.";
            return View("Index", viewModelLogin);
        }

        var usuario = await _serviceUsuario.LoginAsync(viewModelLogin.User, viewModelLogin.Password);

        if (usuario == null)
        {
            _logger.LogWarning("Intento de acceso fallido para {Correo}", viewModelLogin.User);
            TempData["LoginError"] = "Correo o contraseña incorrectos, o usuario bloqueado.";
            return View("Index", viewModelLogin);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.NombreCompleto),
            new Claim(ClaimTypes.Role, usuario.IdRolNavigation.Nombre),
            new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            IsPersistent = false
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        _logger.LogInformation("Sesión iniciada: {Nombre} [{Rol}]",
            usuario.NombreCompleto, usuario.IdRolNavigation.Nombre);

        TempData["Notificacion"] = JsonSerializer.Serialize(new
        {
            title = "Bienvenido",
            text = $"Hola, {usuario.NombreCompleto}.",
            icon = "success"
        });

        return RedirectToAction("Index", "Home");
    }

    // GET /Login/LogOff
    [Authorize]
    public async Task<IActionResult> LogOff()
    {
        _logger.LogInformation("Cierre de sesión: {Nombre}", User.Identity?.Name);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        TempData["Notificacion"] = JsonSerializer.Serialize(new
        {
            title = "Sesión finalizada",
            text = "Has cerrado sesión correctamente.",
            icon = "success"
        });

        return RedirectToAction("Index", "Login");
    }

    // GET /Login/MiPerfil
    [Authorize]
    public async Task<IActionResult> MiPerfil()
    {
        var id = UsuarioHelper.GetUsuarioId(User);
        if (id <= 0) return RedirectToAction("Index");

        var usuario = await _serviceUsuario.GetByIdConDetalle(id);
        if (usuario == null) return NotFound();
        return View(usuario);
    }

    // GET /Login/Historial
    [Authorize]
    public async Task<IActionResult> Historial()
    {
        var id = UsuarioHelper.GetUsuarioId(User);
        if (id <= 0) return RedirectToAction("Index");

        var historial = await _serviceUsuario.GetHistorialAsync(id);
        return View(historial);
    }

    // GET /Login/Forbidden
    public IActionResult Forbidden()
    {
        return View();
    }

    // GET /Login/Registro
    [HttpGet]
    public IActionResult Registro()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    // POST /Login/Registro
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(SuVac.Application.DTOs.UsuarioDTO dto)
    {
        // En registro público el rol siempre es Comprador (RolId = 3) y estado Activo (1)
        ModelState.Remove(nameof(dto.RolId));
        ModelState.Remove(nameof(dto.EstadoUsuarioId));

        if (string.IsNullOrWhiteSpace(dto.Contrasena))
            ModelState.AddModelError(nameof(dto.Contrasena), "La contraseña es obligatoria.");
        else if (dto.Contrasena.Length < 6)
            ModelState.AddModelError(nameof(dto.Contrasena), "La contraseña debe tener al menos 6 caracteres.");

        if (!ModelState.IsValid)
            return View(dto);

        dto.RolId = 3;           // Comprador
        dto.EstadoUsuarioId = 1; // Activo
        dto.FechaRegistro = DateTime.Now;

        var ok = await _serviceUsuario.Create(dto);
        if (!ok)
        {
            ModelState.AddModelError("", "No se pudo crear la cuenta. Verifique que el correo no esté en uso.");
            return View(dto);
        }

        _logger.LogInformation("Nuevo usuario registrado: {Correo}", dto.Correo);

        TempData["Notificacion"] = JsonSerializer.Serialize(new
        {
            title = "Cuenta creada",
            text = "Tu cuenta fue creada exitosamente. Podés iniciar sesión.",
            icon = "success"
        });

        return RedirectToAction("Index", "Login");
    }
}
