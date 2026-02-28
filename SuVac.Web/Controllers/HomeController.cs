using SuVac.Web.Models;
using SuVac.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace SuVac.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServiceGanado _serviceGanado;
        private readonly IServiceSubasta _serviceSubasta;

        public HomeController(ILogger<HomeController> logger, IServiceGanado serviceGanado, IServiceSubasta serviceSubasta)
        {
            _logger = logger;
            _serviceGanado = serviceGanado;
            _serviceSubasta = serviceSubasta;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Entrando al metodo Index del HomeController");
            try
            {
                var ganados = await _serviceGanado.GetAll();
                var subastas = await _serviceSubasta.GetActivas();

                ViewBag.Ganados = ganados.Take(6);
                ViewBag.Subastas = subastas.Take(6);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al cargar datos en Index: {ex.Message}");
                ViewBag.Ganados = new List<object>();
                ViewBag.Subastas = new List<object>();
            }

            return View();
        }

        public IActionResult Privacy()
        {
            throw new Exception("Error probado desde Privacy()");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult ErrorHandler(string? messagesJson)
        {
            if (string.IsNullOrWhiteSpace(messagesJson))
            {
                ViewBag.ErrorMessages = new ErrorMiddlewareViewModel
                {
                    IdEvent = "SIN-DATO",
                    ListMessages = new List<string> { "No se recibio informacion de error." },
                    Path = "N/A"
                };

                return View("ErrorHandler");
            }

            ErrorMiddlewareViewModel? errorObject = null;

            try
            {
                errorObject = JsonSerializer.Deserialize<ErrorMiddlewareViewModel>(
                    messagesJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al deserializar mensaje del middleware: {ex.Message}");

                errorObject = new ErrorMiddlewareViewModel
                {
                    IdEvent = "JSON-INVALIDO",
                    ListMessages = new List<string>
                    {
                        "El mensaje recibido no tiene un formato valido."
                    }
                };
            }

            ViewBag.ErrorMessages = errorObject;
            return View("ErrorHandler");
        }
    }
}
