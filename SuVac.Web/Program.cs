
using SuVac.Application.Profiles;
using SuVac.Application.Services.Implementations;
using SuVac.Application.Services.Interfaces;
using SuVac.Infraestructure.Data;
using SuVac.Infraestructure.Repository.Implementations;
using SuVac.Infraestructure.Repository.Interfaces;
using SuVac.Web.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System.Text;

//***********
// =======================
// Configurar Serilog
// =======================
// Crear carpeta Logs autom�ticamente (evita errores si no existe)
Directory.CreateDirectory("Logs");

// Configuraci�n Serilog
var logger = new LoggerConfiguration()
    // Nivel m�nimo global (recomendado: Information)
    .MinimumLevel.Information()

    // Reducir ruido de logs internos de Microsoft
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    //Mostrar SQL ejecutado por EF Core
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)

    // Enriquecer logs con contexto (RequestId, etc.)
    .Enrich.FromLogContext()

    // Consola: �til para depurar en Visual Studio
    .WriteTo.Console()

    // Archivos separados por nivel (rolling diario)
    .WriteTo.Logger(l => l
        .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
        .WriteTo.File(@"Logs\Info-.log",
            shared: true,
            encoding: Encoding.UTF8,
            rollingInterval: RollingInterval.Day))

    .WriteTo.Logger(l => l
        .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
        .WriteTo.File(@"Logs\Warning-.log",
            shared: true,
            encoding: Encoding.UTF8,
            rollingInterval: RollingInterval.Day))

    .WriteTo.Logger(l => l
        .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
        .WriteTo.File(@"Logs\Error-.log",
            shared: true,
            encoding: Encoding.UTF8,
            rollingInterval: RollingInterval.Day))

    .WriteTo.Logger(l => l
        .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal)
        .WriteTo.File(@"Logs\Fatal-.log",
            shared: true,
            encoding: Encoding.UTF8,
            rollingInterval: RollingInterval.Day))

    .CreateLogger();

// Paso obligatorio ANTES de crear builder
Log.Logger = logger;

var builder = WebApplication.CreateBuilder(args);

// Integrar Serilog al host
builder.Host.UseSerilog(Log.Logger);

// Add services to the container.
builder.Services.AddControllersWithViews();
//***********
// =======================
// Configurar Dependency Injection
// =======================
//*** Repositories SuVac
builder.Services.AddTransient<IRepositoryGanado, RepositoryGanado>();
builder.Services.AddTransient<IRepositorySubasta, RepositorySubasta>();
builder.Services.AddTransient<IRepositoryPuja, RepositoryPuja>();
builder.Services.AddTransient<IRepositoryPago, RepositoryPago>();
builder.Services.AddTransient<IRepositoryUsuario, RepositoryUsuario>();
builder.Services.AddTransient<IRepositoryCategoria, RepositoryCategoria>();
builder.Services.AddTransient<IRepositoryRaza, RepositoryRaza>();
builder.Services.AddTransient<IRepositoryTipoGanado, RepositoryTipoGanado>();
builder.Services.AddTransient<IRepositoryRol, RepositoryRol>();
builder.Services.AddTransient<IRepositoryResultadoSubasta, RepositoryResultadoSubasta>();

//*** Services SuVac
builder.Services.AddTransient<IServiceGanado, ServiceGanado>();
builder.Services.AddTransient<IServiceSubasta, ServiceSubasta>();
builder.Services.AddTransient<IServicePuja, ServicePuja>();
builder.Services.AddTransient<IServicePago, ServicePago>();
builder.Services.AddTransient<IServiceUsuario, ServiceUsuario>();
builder.Services.AddTransient<IServiceCategoria, ServiceCategoria>();
builder.Services.AddTransient<IServiceRaza, ServiceRaza>();
builder.Services.AddTransient<IServiceTipoGanado, ServiceTipoGanado>();
builder.Services.AddTransient<IServiceRol, ServiceRol>();
builder.Services.AddTransient<IServiceResultadoSubasta, ServiceResultadoSubasta>();

// =======================
// Configurar AutoMapper
// =======================
builder.Services.AddAutoMapper(config =>
{
    //*** Profiles SuVac
    config.AddProfile<GanadoProfile>();
    config.AddProfile<ImagenGanadoProfile>();
    config.AddProfile<SubastaProfile>();
    config.AddProfile<PujaProfile>();
    config.AddProfile<PagoProfile>();
    config.AddProfile<RazaProfile>();
    config.AddProfile<TipoGanadoProfile>();
    config.AddProfile<ResultadoSubastaProfile>();
    config.AddProfile<RolProfile>();
    config.AddProfile<UsuarioProfile>();
    config.AddProfile<CategoriaProfile>();
});

// =======================
// Configurar SQL Server DbContext
// =======================
var connectionString = builder.Configuration.GetConnectionString("SqlServerDataBase");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "No se encontr� la cadena de conexi�n 'SqlServerDataBase' en appsettings.json / appsettings.Development.json.");
}

builder.Services.AddDbContext<SuVacContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        // Reintentos ante fallos transitorios (recomendado)
        sqlOptions.EnableRetryOnFailure();
    });

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Middleware personalizado
    app.UseMiddleware<ErrorHandlingMiddleware>();
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapStaticAssets();

app.UseAntiforgery();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
