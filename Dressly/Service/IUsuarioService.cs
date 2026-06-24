using Dressly_MVC.Models;

namespace Dressly_MVC.Services;

public interface IUsuarioService
{
    Task<Usuario?> GetByIdAsync(int id);
}
