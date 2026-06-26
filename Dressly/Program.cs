// Dressly.Web/Program.cs
// ─────────────────────────────────────────────────────────────────────────────
// AQUÍ enchufas el Adapter que quieras usar para cada entidad.
// Domain y Application NO se tocan — solo cambia este archivo.
// ─────────────────────────────────────────────────────────────────────────────

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;
using Dressly.Application.UseCases;
using Dressly.Domain.DomainServices;
using Dressly.Domain.Events;
using Dressly.Infrastructure.Data;
using Dressly.Infrastructure.Notifications;
using Dressly.Infrastructure.Repositories;
using Dressly.Infrastructure.Repositories.Decorators;
using Dressly.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Carpeta de datos ───────────────────────────────────────────────────────
var dataFolder = Path.Combine(builder.Environment.WebRootPath, "data");
Directory.CreateDirectory(dataFolder);

// Ruta SQLite
var sqlitePath = builder.Configuration.GetConnectionString("Sqlite") ?? "Data Source=dressly.db";


// ── 2. Factory + Decorator ───────────────────────────────────────────────────
// El Factory decide el backend (JSON para Development, SQLite para Production)
// y el Decorator envuelve cada repositorio con logging.

var env = builder.Environment.EnvironmentName;

builder.Services.AddDbContext<SqliteDbContext>(options =>
    options.UseSqlite(sqlitePath));

builder.Services.AddSingleton<IPrendaRepository>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<LoggingPrendaRepository>>();
    var realRepo = RepositoryFactory.CreatePrendaRepository(env, sp);
    return new LoggingPrendaRepository(realRepo, logger);
});

builder.Services.AddSingleton<IUsuarioRepository>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<LoggingUsuarioRepository>>();
    var realRepo = RepositoryFactory.CreateUsuarioRepository(env, sp);
    return new LoggingUsuarioRepository(realRepo, logger);
});

builder.Services.AddSingleton<IOutfitRepository>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<LoggingOutfitRepository>>();
    var realRepo = RepositoryFactory.CreateOutfitRepository(env, sp);
    return new LoggingOutfitRepository(realRepo, logger);
});

builder.Services.AddSingleton<IDonacionRepository>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<LoggingDonacionRepository>>();
    var realRepo = RepositoryFactory.CreateDonacionRepository(env, sp);
    return new LoggingDonacionRepository(realRepo, logger);
});


// ── 3. Autenticación ──────────────────────────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";
    });

// ── 4. Infrastructure Services ────────────────────────────────────────────────
builder.Services.AddSingleton<IAlmacenamientoImagenes, FileSystemFotoService>();

// ── 5. Domain Services ────────────────────────────────────────────────────────
builder.Services.AddScoped<IColorimetriaService, ColorimetriaService>();
builder.Services.AddScoped<IPerfilConocimientoService, PerfilConocimientoService>();

// ── 6. Use Cases ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISeedService, SeedService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IPrendaService>(sp =>
{
    var prendaRepo = sp.GetRequiredService<IPrendaRepository>();
    var fotos = sp.GetRequiredService<IAlmacenamientoImagenes>();
    var service = new PrendaService(prendaRepo, fotos);
    var logger = sp.GetRequiredService<ILogger<ConsoleNotifier<PrendaCreadaEvent>>>();
    service.SubscribePrendaCreada(new ConsoleNotifier<PrendaCreadaEvent>(logger));
    return service;
});

builder.Services.AddScoped<IOutfitService>(sp =>
{
    var outfits = sp.GetRequiredService<IOutfitRepository>();
    var prendas = sp.GetRequiredService<IPrendaRepository>();
    var colorimetria = sp.GetRequiredService<IColorimetriaService>();
    var usuarios = sp.GetRequiredService<IUsuarioRepository>();
    var perfil = sp.GetRequiredService<IPerfilService>();
    var conocimiento = sp.GetRequiredService<IPerfilConocimientoService>();
    var service = new OutfitService(outfits, prendas, colorimetria, usuarios, perfil, conocimiento);
    var logger = sp.GetRequiredService<ILogger<ConsoleNotifier<OutfitGeneradoEvent>>>();
    service.SubscribeOutfitGenerado(new ConsoleNotifier<OutfitGeneradoEvent>(logger));
    return service;
});

builder.Services.AddScoped<IDonacionService>(sp =>
{
    var donaciones = sp.GetRequiredService<IDonacionRepository>();
    var prendas = sp.GetRequiredService<IPrendaService>();
    var service = new DonacionService(donaciones, prendas);
    var logger = sp.GetRequiredService<ILogger<ConsoleNotifier<DonacionRegistradaEvent>>>();
    service.SubscribeDonacionRegistrada(new ConsoleNotifier<DonacionRegistradaEvent>(logger));
    return service;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ── 7. Seed ───────────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    // Descomenta solo si el Bloque C (SQLite) está activo
    /*
    var db = scope.ServiceProvider.GetRequiredService<SqliteDbContext>();
    db.Database.EnsureCreated();
    */

    var auth = scope.ServiceProvider.GetRequiredService<IAuthService>();
    await auth.SeedDefaultUserAsync();

    var usuarios = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();
    var admin = await usuarios.GetByEmailAsync("giovana@dressly.com");
    if (admin != null)
    {
        var seed = scope.ServiceProvider.GetRequiredService<ISeedService>();
        await seed.SeedUserDataAsync(admin.Id);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();