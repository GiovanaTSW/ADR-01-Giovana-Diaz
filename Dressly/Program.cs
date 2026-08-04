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

    // --- SEED DE IDENTIDADES KIBBE (upsert: conserva lo existente, agrega lo faltante) ---
    var kibbeExistentes = await db.IdentidadesKibbe.ToListAsync();
    var nombresExistentes = kibbeExistentes.Select(k => k.Nombre).ToHashSet(StringComparer.OrdinalIgnoreCase);

    var kibbeCompletos = new List<IdentidadKibbeInfo>
    {
        new()
        {
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
            Nombre = "Soft Dramatic",
            TipoEnergia = "Yang con toques Yin",
            DescripcionFisica = "Silueta alta y angular con un toque de curvas suaves; estructura ósea marcada con algo de redondez en el rostro o el cuerpo.",
            LineasRecomendadas = "Líneas largas y estructuradas combinadas con curvas suaves, escotes y acentos que abracen la cintura.",
            LineasNoRecomendadas = "Siluetas demasiado rígidas sin ninguna curva, o ropa excesivamente holgada que borre la forma.",
            TelasSugeridas = "Telas con caída y estructura a la vez: seda pesada, crepé, jersey pesado y lana fina.",
            Evitar = "Prendas ultramasculinas cuadradas y looks completamente informales sin definición."
        },
        new()
        {
            Nombre = "Flamboyant Natural",
            TipoEnergia = "Yang rotundo",
            DescripcionFisica = "Estructura ósea muy ancha y angular, hombros fuertes, estatura alta y presencia física amplia y rotunda.",
            LineasRecomendadas = "Cortes amplios y rectos, líneas largas, prendas oversize con estructura y siluetas de gran escala.",
            LineasNoRecomendadas = "Prendas diminutas, ceñidas o delicadas; estampados pequeños y siluetas que reduzcan la escala.",
            TelasSugeridas = "Texturas gruesas y orgánicas: cuero, lino pesado, tweed, punto rústico y mezclilla.",
            Evitar = "Ropa rígida y ajustada al cuerpo, vestidos delicados y accesorios miniaturizados."
        },
        new()
        {
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
            Nombre = "Soft Natural",
            TipoEnergia = "Yang suave con Yin ligero",
            DescripcionFisica = "Estructura ósea natural y algo ancha con un toque de suavidad; hombros firmes y rasgos con curvas delicadas.",
            LineasRecomendadas = "Siluetas relajadas pero con suavidad, líneas rectas con caída, tejidos ligeros y detalles informales.",
            LineasNoRecomendadas = "Cortes rígidos y geométricos, ropa ajustada y estructurada, y exceso de formalidad.",
            TelasSugeridas = "Texturas suaves y naturales: algodón, lino, punto ligero, seda mate y gamuza.",
            Evitar = "Siluetas cuadradas duras, telas demasiado brillantes y accesorios pesados y rígidos."
        },
        new()
        {
            Nombre = "Dramatic Classic",
            TipoEnergia = "Equilibrio con acento Yang",
            DescripcionFisica = "Proporciones simétricas y moderadas con una pizca de angularidad; estructura balanceada pero con más presencia vertical.",
            LineasRecomendadas = "Cortes limpios y estructurados con un detalle angular, siluetas a medida y líneas moderadamente largas.",
            LineasNoRecomendadas = "Asimetrías extremas, siluetas muy suaves o muy rígidas, y estampados estridentes.",
            TelasSugeridas = "Telas de calidad media-alta con cuerpo: lana, cachemira, gabardina fina y algodón peinado.",
            Evitar = "Excesos en capas, prendas desestructuradas y looks casuales sin pulir."
        },
        new()
        {
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
            Nombre = "Soft Classic",
            TipoEnergia = "Equilibrio con acento Yin",
            DescripcionFisica = "Proporciones equilibradas y simétricas con un toque de suavidad; rasgos moderados y ligeramente redondeados.",
            LineasRecomendadas = "Siluetas clásicas y a medida con detalles suaves: drapeados delicados, colores empolvados y acabados pulidos.",
            LineasNoRecomendadas = "Cortes muy rígidos o masculinos, asimetrías fuertes y telas ásperas.",
            TelasSugeridas = "Telas suaves de buena calidad: cachemira, seda, crepé ligero y punto fino.",
            Evitar = "Prendas desestructuradas, estampados muy grandes y accesorios exagerados."
        },
        new()
        {
            Nombre = "Flamboyant Gamine",
            TipoEnergia = "Colisión Yin-Yang en escala",
            DescripcionFisica = "Presencia pequeña pero llamativa, con angularidad marcada y líneas atrevidas; apariencia juvenil con energía intensa.",
            LineasRecomendadas = "Cortes afilados y a medida, bloques de color fuertes, siluetas rectas con detalles audaces.",
            LineasNoRecomendadas = "Siluetas blandas y redondeadas, estampados pequeños y ropa demasiado holgada.",
            TelasSugeridas = "Telas firmes y estructuradas: sarga, cuero, gabardina, tweed y denim rígido.",
            Evitar = "Vestidos largos sin forma, telas pesadas y drapeadas, y looks demasiado apagados."
        },
        new()
        {
            Nombre = "Gamine",
            TipoEnergia = "Yin y Yang en colisión",
            DescripcionFisica = "Estatura baja, contextura delgada y ligeramente musculosa, rasgos faciales expresivos y apariencia juvenil eterna.",
            LineasRecomendadas = "Siluetas ajustadas, cortes sharp (afilados), bloques de color contrastantes y líneas cortas y definidas.",
            LineasNoRecomendadas = "Siluetas largas e ininterrumpidas, ropa holgada y vestidos excesivamente solemnes.",
            TelasSugeridas = "Telas firmes pero ligeras que mantengan su forma como gabardina ligera, cuero suave, punto ajustado y sarga.",
            Evitar = "Vestidos maxi sin forma, looks monocromáticos apagados de pies a cabeza y telas demasiado pesadas o drapeadas."
        },
        new()
        {
            Nombre = "Soft Gamine",
            TipoEnergia = "Colisión Yin-Yang suave",
            DescripcionFisica = "Estatura baja con rasgos algo redondeados y angularidad ligera; apariencia juvenil y dulce con chispa.",
            LineasRecomendadas = "Líneas cortas y a medida con detalles suaves, colores vivos, siluetas entalladas y acabados delicados.",
            LineasNoRecomendadas = "Siluetas largas y holgadas, cortes duros y masculinos, y telas pesadas.",
            TelasSugeridas = "Telas ligeras y con forma: algodón, punto fino, tul, seda ligera y cuero suave.",
            Evitar = "Prendas oversize, looks demasiado sobrios y texturas ásperas o rígidas."
        },
        new()
        {
            Nombre = "Theatrical Romantic",
            TipoEnergia = "Yin dominante con impacto",
            DescripcionFisica = "Curvas marcadas y redondeadas con presencia dramática; silueta de reloj de arena con energía y magnetismo.",
            LineasRecomendadas = "Siluetas que marquen la cintura y las curvas, líneas redondeadas, escotes elegantes y detalles llamativos.",
            LineasNoRecomendadas = "Líneas rectas y masculinas, siluetas cuadradas y ropa holgada sin forma.",
            TelasSugeridas = "Telas lujosas y fluidas: seda, satén, terciopelo, encaje y gasa con caída.",
            Evitar = "Cortes geométricos duros, estampados diminutos y prendas rígidas sin curva."
        },
        new()
        {
            Nombre = "Romantic",
            TipoEnergia = "Yin Extremo",
            DescripcionFisica = "Cuerpo con forma de reloj de arena muy marcado, hombros redondeados, extremidades cortas y rasgos suaves.",
            LineasRecomendadas = "Siluetas que enfaticen la cintura, líneas redondeadas, drapeados suaves y volantes delicados.",
            LineasNoRecomendadas = "Líneas rectas y rígidas, cortes geométricos y siluetas cuadradas o masculinas.",
            TelasSugeridas = "Telas fluidas y ligeras como seda, gasa, satén, encaje suave y tejidos finos.",
            Evitar = "Ropa rígida, sacos con hombreras estructuradas y looks monocromáticos planos sin curvas definidas."
        }
    };

    var kibbeFaltantes = kibbeCompletos
        .Where(k => !nombresExistentes.Contains(k.Nombre))
        .ToList();

    if (kibbeFaltantes.Count > 0)
    {
        await db.IdentidadesKibbe.AddRangeAsync(kibbeFaltantes);
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