using Microsoft.EntityFrameworkCore;
using SuVac.Application.Perfiles;
using SuVac.Application.Servicios.Implementaciones;
using SuVac.Application.Servicios.Interfaces;
using SuVac.Infraestructure.Datos;
using SuVac.Infraestructure.Repositorio.Implementaciones;
using SuVac.Infraestructure.Repositorio.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

// ── Contexto de base de datos ─────────────────────────────────────────────────
builder.Services.AddDbContext<SuVacContexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SuVacConexion")));

// ── AutoMapper ────────────────────────────────────────────────────────────────
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<SubastaPerfiles>();
    config.AddProfile<PujaPerfiles>();
});

// ── Repositorios ──────────────────────────────────────────────────────────────
builder.Services.AddTransient<IRepositorioSubasta, RepositorioSubasta>();
builder.Services.AddTransient<IRepositorioPuja, RepositorioPuja>();

// ── Servicios de aplicación ───────────────────────────────────────────────────
builder.Services.AddTransient<IServicioSubasta, ServicioSubasta>();
builder.Services.AddTransient<IServicioPuja, ServicioPuja>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
