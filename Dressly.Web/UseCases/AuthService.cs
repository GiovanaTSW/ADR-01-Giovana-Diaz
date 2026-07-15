using System.Security.Cryptography;
using System.Text;
using Dressly.Domain.Entities;
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;
using Microsoft.Extensions.Configuration;

namespace Dressly.Application.UseCases;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly ISeedService _seed;
    private readonly IConfiguration _config;

    public AuthService(IUsuarioRepository usuarios, ISeedService seed, IConfiguration config)
    {
        _usuarios = usuarios;
        _seed = seed;
        _config = config;
    }

    public async Task<(bool Exitoso, Usuario? Usuario)> LoginAsync(string email, string password)
    {
        var usuario = await _usuarios.GetByEmailAsync(email);
        if (usuario == null) return (false, null);

        var hash = HashPassword(password);
        if (usuario.PasswordHash != hash) return (false, null);

        return (true, usuario);
    }

    public async Task<(bool Exitoso, string Error)> RegisterAsync(string nombre, string email, string password)
    {
        var existente = await _usuarios.GetByEmailAsync(email);
        if (existente != null) return (false, "El email ya está registrado");

        var usuario = new Usuario
        {
            Id = await _usuarios.GetNextIdAsync(),
            Nombre = nombre,
            Email = email,
            PasswordHash = HashPassword(password)
        };

        await _usuarios.AddAsync(usuario);
        await _seed.SeedUserDataAsync(usuario.Id);
        return (true, "");
    }

    public async Task SeedDefaultUserAsync()
    {
        var nombre = _config["SeedUser:Nombre"] ?? "Giovana Díaz";
        var email = _config["SeedUser:Email"] ?? "giovana@dressly.com";
        var password = _config["SeedUser:Password"] ?? "123456";

        var existente = await _usuarios.GetByEmailAsync(email);
        if (existente != null) return;

        var usuario = new Usuario
        {
            Id = await _usuarios.GetNextIdAsync(),
            Nombre = nombre,
            Email = email,
            PasswordHash = HashPassword(password)
        };

        await _usuarios.AddAsync(usuario);
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
