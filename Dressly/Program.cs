// Dressly.Web/Program.cs
// ─────────────────────────────────────────────────────────────────────────────
// AQUÍ enchufas el Adapter que quieras usar para cada entidad.
// Domain y Application NO se tocan — solo cambia este archivo.
// ─────────────────────────────────────────────────────────────────────────────
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;
using Dressly.Application.UseCases;
using Dressly.Domain.DomainServices;
using Dressly.Domain.Entities;
using Dressly.Domain.Events;
using Dressly.Infrastructure.Data;
using Dressly.Infrastructure.Notifications;
using Dressly.Infrastructure.Repositories;
using Dressly.Infrastructure.Repositories.Decorators;
using Dressly.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Ruta SQLite ───────────────────────────────────────────────────────────
var sqlitePath = builder.Configuration.GetConnectionString("Sqlite") ?? "Data Source=dressly.db";

builder.Services.AddDbContext<SqliteDbContext>(options =>
    options.UseSqlite(sqlitePath));

// ── 2. Registro de Repositorios (¡Ahora todos son Scoped para SQLite!) ────────
var env = builder.Environment.EnvironmentName;

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

// Registrar el repositorio de Pacas que borramos de los JSON
builder.Services.AddScoped<INegocioPacaRepository>(sp =>
{
    return RepositoryFactory.CreateNegocioPacaRepository(env, sp);
});

builder.Services.AddScoped<IIdentidadKibbeRepository, SqliteIdentidadKibbeRepository>();
builder.Services.AddScoped<IEmpresaRepository, SqliteEmpresaRepository>();
builder.Services.AddScoped<IPatrocinioRepository, SqlitePatrocinioRepository>();
builder.Services.AddScoped<IIntercambioRepository, SqliteIntercambioRepository>();

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

builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IPatrocinioService, PatrocinioService>();
builder.Services.AddScoped<IIntercambioService, IntercambioService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ── 7. Seed y Creación automática de BD SQLite ──────────────────────────────
using (var scope = app.Services.CreateScope())
{
    // ¡Activado! Esto creará la base de datos sqlite en blanco automáticamente si no existe
    var db = scope.ServiceProvider.GetRequiredService<SqliteDbContext>();
    db.Database.EnsureCreated();

    // --- SEED DE IDENTIDADES KIBBE (¡Directo en base de datos!) ---
    if (!await db.IdentidadesKibbe.AnyAsync())
    {
        var kibbeEjemplos = new List<IdentidadKibbeInfo>
        {
            new()
            {
                Id = 1,
                Nombre = "Dramatic",
                TipoEnergia = "Yang Extremo",
                DescripcionFisica = "Estructura ósea muy angulosa, extremidades largas, hombros estrechos y figura recta o estilizada.",
                LineasRecomendadas = "Cortes rectos, líneas verticales largas, siluetas muy estructuradas y asimetrías marcadas.",
                LineasNoRecomendadas = "Líneas redondeadas, volantes excesivos, ropa holgada y sin forma definida.",
                TelasSugeridas = "Telas estructuradas y rígidas como lana pesada, brocado, cuero, gabardina rígida y lino grueso.",
                Evitar = "Prendas excesivamente sueltas, estampados florales pequeños y ropa con demasiada caída fluida."
            },
            new()
            {
                Id = 2,
                Nombre = "Romantic",
                TipoEnergia = "Yin Extremo",
                DescripcionFisica = "Cuerpo con forma de reloj de arena muy marcado, hombros redondeados, extremidades cortas y rasgos suaves.",
                LineasRecomendadas = "Siluetas que enfaticen la cintura, líneas redondeadas, drapeados suaves y volantes delicados.",
                LineasNoRecomendadas = "Líneas rectas y rígidas, cortes geométricos y siluetas cuadradas o masculinas.",
                TelasSugeridas = "Telas fluidas y ligeras como seda, gasa, satén, encaje suave y tejidos finos.",
                Evitar = "Ropa rígida, sacos con hombreras estructuradas y looks monocromáticos planos sin curvas definidas."
            },
            new()
            {
                Id = 3,
                Nombre = "Classic",
                TipoEnergia = "Equilibrio Yin-Yang",
                DescripcionFisica = "Rasgos y cuerpo simétricos, proporciones balanceadas, ni muy alto ni muy bajo, estructura ósea moderada.",
                LineasRecomendadas = "Cortes limpios, siluetas simétricas, líneas fluidas pero controladas y ropa a medida.",
                LineasNoRecomendadas = "Asimetrías exageradas, accesorios extremadamente llamativos y exceso de capas holgadas.",
                TelasSugeridas = "Telas de peso medio y alta calidad como algodón peinado, seda pesada, lana fina y cachemira.",
                Evitar = "Look grunge, prendas desestructuradas y estampados excesivamente coloridos u ornamentados."
            },
            new()
            {
                Id = 4,
                Nombre = "Natural",
                TipoEnergia = "Yang Suave",
                DescripcionFisica = "Estructura ósea ancha y ligeramente angulosa, hombros fuertes, complexión atlética natural y contextura recta.",
                LineasRecomendadas = "Cortes relajados, siluetas desestructuradas, líneas rectas informales y capas sueltas.",
                LineasNoRecomendadas = "Ropa extremadamente ceñida, corsés, siluetas ultra estructuradas o rígidas.",
                TelasSugeridas = "Fibras naturales y texturizadas como lino, gamuza, punto grueso, mezclilla y algodón rústico.",
                Evitar = "Prendas súper formales o rígidas, accesorios diminutos y telas ultra sintéticas brillantes."
            },
            new()
            {
                Id = 5,
                Nombre = "Gamine",
                TipoEnergia = "Yin y Yang en colisión",
                DescripcionFisica = "Estatura baja, contextura delgada y ligeramente musculosa, rasgos faciales expresivos y apariencia juvenil eterna.",
                LineasRecomendadas = "Siluetas ajustadas, cortes sharp (afilados), bloques de color contrastantes y líneas cortas y definidas.",
                LineasNoRecomendadas = "Siluetas largas e ininterrumpidas, ropa holgada y vestidos excesivamente solemnes.",
                TelasSugeridas = "Telas firmes pero ligeras que mantengan su forma como gabardina ligera, cuero suave, punto ajustado y sarga.",
                Evitar = "Vestidos maxi sin forma, looks monocromáticos apagados de pies a cabeza y telas demasiado pesadas o drapeadas."
            }
        };

        await db.IdentidadesKibbe.AddRangeAsync(kibbeEjemplos);
        await db.SaveChangesAsync();
    }

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