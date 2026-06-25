using System.Security.Cryptography;
using System.Text;
using Dressly.Domain.Entities;
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;

namespace Dressly.Application.UseCases;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly ISeedService _seed;

    public AuthService(IUsuarioRepository usuarios, ISeedService seed)
    {
        _usuarios = usuarios;
        _seed = seed;
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
        var existente = await _usuarios.GetByEmailAsync("giovana@dressly.com");
        if (existente != null) return;

        var usuario = new Usuario
        {
            Id = await _usuarios.GetNextIdAsync(),
            Nombre = "Giovana Díaz",
            Email = "giovana@dressly.com",
            PasswordHash = HashPassword("123456")
        };

        await _usuarios.AddAsync(usuario);
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
