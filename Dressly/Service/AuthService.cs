using System.Security.Cryptography;
using Dressly_MVC.Models;
using Dressly_MVC.Repositories;

namespace Dressly_MVC.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarios;

    public AuthService(IUsuarioRepository usuarios)
    {
        _usuarios = usuarios;
    }

    public async Task<Usuario?> LoginAsync(string email, string password)
    {
        var usuario = await _usuarios.GetByEmailAsync(email);
        if (usuario == null) return null;

        var hash = HashPassword(password);
        return usuario.PasswordHash == hash ? usuario : null;
    }

    public async Task<(bool Exito, string Error, int UsuarioId)> RegisterAsync(string nombre, string email, string password)
    {
        var existente = await _usuarios.GetByEmailAsync(email);
        if (existente != null)
            return (false, "El email ya está registrado", 0);

        var usuario = new Usuario
        {
            Id = await _usuarios.GetNextIdAsync(),
            Nombre = nombre,
            Email = email,
            PasswordHash = HashPassword(password)
        };

        await _usuarios.AddAsync(usuario);
        return (true, string.Empty, usuario.Id);
    }

    public async Task SeedDefaultUserAsync()
    {
        var usuarios = await _usuarios.GetAllAsync();
        if (usuarios.Any()) return;

        await _usuarios.AddAsync(new Usuario
        {
            Id = 1,
            Nombre = "Giovana",
            Email = "giovana@dressly.com",
            PasswordHash = HashPassword("123456")
        });
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
