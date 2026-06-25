using Dressly.Domain.Entities;

namespace Dressly.Application.Ports.Input;

public interface IAuthService
{
    Task<(bool Exitoso, Usuario? Usuario)> LoginAsync(string email, string password);
    Task<(bool Exitoso, string Error)> RegisterAsync(string nombre, string email, string password);
    Task SeedDefaultUserAsync();
}
