using Dressly.Domain.Entities;

namespace Dressly.Application.Ports.Output;

public interface IUsuarioRepository
{
    Task<List<Usuario>> GetAllAsync();
    Task<Usuario?> GetByIdAsync(int id);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<int> GetNextIdAsync();
    Task AddAsync(Usuario usuario);
    Task UpdateAsync(Usuario usuario);
}
