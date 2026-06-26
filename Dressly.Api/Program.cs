// Dressly.Api/Program.cs

using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;
using Dressly.Application.UseCases;
using Dressly.Domain.DomainServices;
using Dressly.Infrastructure.Services;
using Dressly.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ── 1. Carpeta de datos ───────────────────────────────────────────────────────
var dataFolder = Path.Combine(Directory.GetCurrentDirectory(), "data");
Directory.CreateDirectory(dataFolder);

// ── 2. Repositorios (JSON) ────────────────────────────────────────────────────
builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddSingleton<IPrendaRepository, PrendaRepository>();
builder.Services.AddSingleton<IOutfitRepository, OutfitRepository>();
builder.Services.AddSingleton<IDonacionRepository, DonacionRepository>();

// ── 3. Infrastructure Services ────────────────────────────────────────────────
builder.Services.AddSingleton<IAlmacenamientoImagenes, FileSystemFotoService>();

// ── 4. Domain Services ────────────────────────────────────────────────────────
builder.Services.AddScoped<IColorimetriaService, ColorimetriaService>();
builder.Services.AddScoped<IPerfilConocimientoService, PerfilConocimientoService>();

// ── 5. Use Cases ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISeedService, SeedService>();
builder.Services.AddScoped<IPrendaService, PrendaService>();
builder.Services.AddScoped<IOutfitService, OutfitService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IDonacionService, DonacionService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// ── 6. CORS ───────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var auth = scope.ServiceProvider.GetRequiredService<IAuthService>();
    await auth.SeedDefaultUserAsync();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();