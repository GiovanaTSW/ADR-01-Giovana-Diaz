using Dressly_MVC.Repositories;
using Dressly_MVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";
    });

// Repositorios
builder.Services.AddSingleton<IPrendaRepository, PrendaRepository>();
builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddSingleton<IOutfitRepository, OutfitRepository>();
builder.Services.AddSingleton<IDonacionRepository, DonacionRepository>();

// Servicios
builder.Services.AddScoped<IPrendaService, PrendaService>();
builder.Services.AddScoped<IOutfitService, OutfitService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IDonacionService, DonacionService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IFotoService, FotoService>();
builder.Services.AddScoped<IColorimetriaService, ColorimetriaService>();
builder.Services.AddScoped<IPerfilConocimientoService, PerfilConocimientoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<SeedService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed datos por defecto
using (var scope = app.Services.CreateScope())
{
    var auth = scope.ServiceProvider.GetRequiredService<IAuthService>();
    await auth.SeedDefaultUserAsync();

    var seed = scope.ServiceProvider.GetRequiredService<SeedService>();
    await seed.SeedUserDataAsync(1);
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
