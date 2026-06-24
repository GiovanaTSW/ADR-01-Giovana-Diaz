using Dressly_MVC.Models;

namespace Dressly_MVC.Services;

public interface IAuthService
{
    Task<Usuario?> LoginAsync(string email, string password);
    Task<(bool Exito, string Error, int UsuarioId)> RegisterAsync(string nombre, string email, string password);
    Task SeedDefaultUserAsync();
}
