// Dressly.Api/Program.cs

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ── 1. Carpeta de datos ───────────────────────────────────────────────────────
var dataFolder = Path.Combine(Directory.GetCurrentDirectory(), "data");
Directory.CreateDirectory(dataFolder);

// ── 2. Factory + Decorator ───────────────────────────────────────────────────
var env = builder.Environment.EnvironmentName;
var sqlitePath = builder.Configuration.GetConnectionString("Sqlite") ?? "Data Source=dressly.db";

builder.Services.AddDbContext<SqliteDbContext>(options =>
    options.UseSqlite(sqlitePath));

builder.Services.AddScoped<IPrendaRepository>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<LoggingPrendaRepository>>();
    var realRepo = RepositoryFactory.CreatePrendaRepository(env, sp);
    return new LoggingPrendaRepository(realRepo, logger);
});

builder.Services.AddScoped<IUsuarioRepository>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<LoggingUsuarioRepository>>();
    var realRepo = RepositoryFactory.CreateUsuarioRepository(env, sp);
    return new LoggingUsuarioRepository(realRepo, logger);
});

builder.Services.AddScoped<IOutfitRepository>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<LoggingOutfitRepository>>();
    var realRepo = RepositoryFactory.CreateOutfitRepository(env, sp);
    return new LoggingOutfitRepository(realRepo, logger);
});

builder.Services.AddScoped<IDonacionRepository>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<LoggingDonacionRepository>>();
    var realRepo = RepositoryFactory.CreateDonacionRepository(env, sp);
    return new LoggingDonacionRepository(realRepo, logger);
});

builder.Services.AddScoped<IIdentidadKibbeRepository>(sp =>
    RepositoryFactory.CreateIdentidadKibbeRepository(env, sp));

// ── 3. Infrastructure Services ────────────────────────────────────────────────
builder.Services.AddSingleton<IAlmacenamientoImagenes, FileSystemFotoService>();

// ── 4. Domain Services ────────────────────────────────────────────────────────
builder.Services.AddScoped<IColorimetriaService, ColorimetriaService>();
builder.Services.AddScoped<IPerfilConocimientoService, PerfilConocimientoService>();

// ── 5. Use Cases ──────────────────────────────────────────────────────────────
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