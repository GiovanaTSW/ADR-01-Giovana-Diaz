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
using Dressly.Infrastructure.Data;
using Dressly.Infrastructure.Repositories;
using Dressly.Infrastructure.Services;
using Dressly_MVC.Repositories; // ← necesario para Bloque A (JSON)

var builder = WebApplication.CreateBuilder(args);

// ── 1. Carpeta de datos ───────────────────────────────────────────────────────
var dataFolder = Path.Combine(builder.Environment.WebRootPath, "data");
Directory.CreateDirectory(dataFolder);

// Ruta SQLite
var sqlitePath = builder.Configuration.GetConnectionString("Sqlite") ?? "Data Source=dressly.db";


// ── 2. Elige tu Adapter ───────────────────────────────────────────────────────
// Descomenta el bloque que quieras y comenta los otros dos.
// ¡Las interfaces (Ports) no cambian!

// ▶ Bloque A — JSON  ← activo ahora
builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddSingleton<IPrendaRepository, PrendaRepository>();
builder.Services.AddSingleton<IOutfitRepository, OutfitRepository>();
builder.Services.AddSingleton<IDonacionRepository, DonacionRepository>();

// ▶ Bloque B — CSV
/*
builder.Services.AddSingleton<IUsuarioRepository, CsvUsuarioRepository>();
builder.Services.AddSingleton<IPrendaRepository, CsvPrendaRepository>();
builder.Services.AddSingleton<IOutfitRepository, CsvOutfitRepository>();
builder.Services.AddSingleton<IDonacionRepository, CsvDonacionRepository>();
*/

// ▶ Bloque C — SQLite
/*
builder.Services.AddDbContext<SqliteDbContext>(options =>
    options.UseSqlite(sqlitePath));
builder.Services.AddScoped<IUsuarioRepository, SqliteUsuarioRepository>();
builder.Services.AddScoped<IPrendaRepository, SqlitePrendaRepository>();
builder.Services.AddScoped<IOutfitRepository, SqliteOutfitRepository>();
builder.Services.AddScoped<IDonacionRepository, SqliteDonacionRepository>();
*/


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
builder.Services.AddScoped<IPrendaService, PrendaService>();
builder.Services.AddScoped<IOutfitService, OutfitService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IDonacionService, DonacionService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

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