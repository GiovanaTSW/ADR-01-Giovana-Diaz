using Dressly_MVC.Models;

namespace Dressly_MVC.Repositories;

public interface IUsuarioRepository
{
    Task<List<Usuario>> GetAllAsync();
    Task<Usuario?> GetByIdAsync(int id);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<int> GetNextIdAsync();
    Task AddAsync(Usuario usuario);
    Task UpdateAsync(Usuario usuario);
}
