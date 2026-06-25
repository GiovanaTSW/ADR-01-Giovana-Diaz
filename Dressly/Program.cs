using Microsoft.AspNetCore.Authentication.Cookies;
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;
using Dressly.Application.UseCases;
using Dressly.Domain.DomainServices;
using Dressly.Infrastructure.Services;
using Dressly_MVC.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";
    });

// Repositorios (JSON)
builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddSingleton<IPrendaRepository, PrendaRepository>();
builder.Services.AddSingleton<IOutfitRepository, OutfitRepository>();
builder.Services.AddSingleton<IDonacionRepository, DonacionRepository>();

// Infrastructure Services
builder.Services.AddSingleton<IAlmacenamientoImagenes, FileSystemFotoService>();

// Domain Services
builder.Services.AddScoped<IColorimetriaService, ColorimetriaService>();
builder.Services.AddScoped<IPerfilConocimientoService, PerfilConocimientoService>();

// Use Cases
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISeedService, SeedService>();
builder.Services.AddScoped<IPrendaService, PrendaService>();
builder.Services.AddScoped<IOutfitService, OutfitService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IDonacionService, DonacionService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed datos por defecto
using (var scope = app.Services.CreateScope())
{
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
